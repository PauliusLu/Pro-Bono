using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models.Messaging;

namespace Karma.Controllers
{
    public class MessagesController : Controller
    {
        delegate List<T> GenerateList<T>(Dictionary<int, T> objs);

        private readonly KarmaContext _context;

        public MessagesController(KarmaContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index(int chatId = -1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string user = User.Identity.Name;

            List<Chat> userChats = await _context.Message.Where(m => m.Username == user || m.Chat.AttachedPost.UserId == user)
                .Select(m => m.Chat).ToListAsync();
            List<Message> allMessages = await _context.Message.Where(m => userChats.Contains(m.Chat))
                .OrderBy(m => m.Date).Include(m => m.Chat.AttachedPost).ToListAsync();

            bool isValidChatId = false;

            var messages = ConvertDictionary(user, allMessages, delegate (Dictionary<int, List<Message>> objs)
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
            if (!isValidChatId)
            {
                ViewBag.History = messages?.FirstOrDefault();
                ViewBag.ChatId = messages?.FirstOrDefault()?.LastOrDefault()?.Chat.Id;
            }
            else
            {
                ViewBag.ChatId = chatId;
            }

            return View(messages);
        }

        private List<List<T>> ConvertDictionary<T>(string user, List<Message> allMessages, GenerateList<List<T>> genFunc)
            where T : Message
        {

            Dictionary<int, List<T>> messages = new();
            foreach (T m in allMessages)
            {
                if (!messages.TryGetValue(m.Chat.Id, out List<T> list))
                {
                    list = new List<T>();
                    messages[m.Chat.Id] = list;
                }
                list.Add(m);
            }

            return genFunc(messages);

        }

        public async Task<IActionResult> Create(int chatId = -1)
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
                AddMessage(m);
                var model = new CreateMessageModel() { ChatId = m.ChatId };
                return new EmptyResult();
            }
            return PartialView(m);
        }

        private void AddMessage(CreateMessageModel m)
        {
            Message message = new()
            {
                Chat = _context.Chat.Find(m.ChatId),
                Date = DateTime.UtcNow,
                Text = m.Text,
                Username = User.Identity.Name
            };
            _context.Add(message);
            _context.SaveChangesAsync();
        }

    }
}