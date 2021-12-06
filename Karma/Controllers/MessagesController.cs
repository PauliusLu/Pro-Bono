using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models.Messaging;
using Karma.Models;
using Karma.Services;

namespace Karma.Controllers
{
    public class MessagesController : Controller
    {
        delegate List<T> GenerateList<K, T>(Dictionary<K, T> objs);
        delegate K GetKey<K, T>(T obj);
        delegate void Modify<T>(T obj);

        private readonly KarmaContext _context;
        private readonly IMessageService _messageService;

        public MessagesController(KarmaContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        // GET: Messages
        public async Task<IActionResult> Index(int chatId = -1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            User user = _context.User.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            List<Chat> userChats = await GetUserChats(_context, user)
                .ToListAsync();

            List<Message> allMessages = await GetUserMessages(_context, userChats)
                .ToListAsync();

            bool isValidChatId = false;

            GetKey<int, Message> MessageChatKey = delegate (Message m)
            {
                return m.Chat.Id;
            };

            var messages = ConvertDictionary(allMessages, MessageChatKey, delegate (Dictionary<int, List<Message>> objs)
            {
                var finalList = new List<List<Message>>();

                foreach (var list in objs.Values)
                {
                    Message m = list.First();
                    if (m.Chat.Id == chatId)
                    {
                        ViewBag.History = list;
                        isValidChatId = true;
                    }
                    finalList.Add(list);
                }
                finalList.Sort((m1, m2) => m2.Last().Date.CompareTo(m1.Last().Date));

                return finalList;
            });

            int? newChatId = isValidChatId ? chatId : null;
            if (!isValidChatId)
            {
                ViewBag.History = messages?.FirstOrDefault();
                newChatId = messages?.FirstOrDefault()?.LastOrDefault()?.Chat.Id;
                ViewBag.ChatId = newChatId;
            }
            else
            {
                ViewBag.ChatId = chatId;
            }
            Chat chat = _context.Chat.Find(newChatId);
            if (chat != null)
            {
                MarkChatAsSeen(chat, user);
            }

            var userReviews = await _context.UserReview.Where(m => m.CreatorId == user.UserName).ToListAsync();
            ViewBag.UserReviews = userReviews;

            return View(messages);
        }

        private IQueryable<Message> GetUserMessages(KarmaContext context, List<Chat> userChats)
        {
            return context.Message
                 .Include(m => m.Sender)
                 .Where(m => userChats.Contains(m.Chat))
                 .OrderBy(m => m.Date).Include(m => m.Chat.AttachedPost);
        }
        private IQueryable<Chat> GetUserChats(KarmaContext context, User user)
        {
            return context.Message
             .Include(m => m.Chat.Creator)
             .Include(m => m.Chat.PostUser)
             .Where(m => m.Sender == user || m.Chat.AttachedPost.UserId == user.UserName)
             .Select(m => m.Chat);
        }

        private List<List<T>> ConvertDictionary<K, T>(List<T> list, GetKey<K, T> getKey, GenerateList<K, List<T>> genFunc)
        {
            Dictionary<K, List<T>> dict = new();
            foreach (T m in list)
            {
                K key = getKey(m);
                if (!dict.TryGetValue(key, out List<T> dictList))
                {
                    dictList = new List<T>();
                    dict[key] = dictList;
                }
                dictList.Add(m);
            }

            return genFunc(dict);
        }

        public IActionResult Create(int chatId = -1)
        {
            if (!User.Identity.IsAuthenticated || chatId == -1)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var message = new CreateMessageModel() { ChatId = chatId };
            return PartialView(message);
        }


        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMessageModel m)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (ModelState.IsValid)
            {
                Message message = _messageService.CreateMessage(_context, m, User.Identity.Name);
                MarkChatAsNotSeen(message.Chat, message.Sender);

                _context.Add(message);
                await _context.SaveChangesAsync();

                return StatusCode(204);
            }
            return PartialView(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePostState([FromBody] PostModel pm)
        {
            if (!User.Identity.IsAuthenticated || User.Identity.Name != pm.Post.UserId)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            pm.Post.ChangeState((Models.Post.PostState)pm.State, pm.Receiver);

            if (pm.Post.State == (int)Models.Post.PostState.Traded)
            {
                int karmaPoints = 3;
                var user = _context.User.Where(u => u.UserName == pm.Post.UserId).FirstOrDefault();
                user.KarmaPoints += karmaPoints;
                _context.Update(user);
            }

             _context.Update(pm.Post);
            await _context.SaveChangesAsync();

            return new EmptyResult();
        }

        public class PostModel
        {
            public Models.Post Post { get; set; }
            public int State { get; set; }
            public string Receiver { get; set; }
        }

        private void MarkChatAsNotSeen(Chat chat, User user)
        {
            UpdateChat(chat, delegate (Chat c)
            {
                if (c.Creator == user)
                {
                    c.IsSeenByPostUser = false;
                }
                else
                {
                    c.IsSeenByCreator = false;
                }
            });
        }

        private void MarkChatAsSeen(Chat chat, User user)
        {
            UpdateChat(chat, delegate (Chat c)
            {
                if (c.Creator == user)
                {
                    c.IsSeenByCreator = true;
                }
                else
                {
                    c.IsSeenByPostUser = true;
                }
            });
        }

        private async void UpdateChat(Chat chat, Modify<Chat> modify) {
            modify(chat);

            _context.Update(chat);
            await _context.SaveChangesAsync();
        }

    }
}