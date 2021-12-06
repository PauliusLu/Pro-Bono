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
    public class ChatsController : Controller
    {
        private readonly KarmaContext _context;
        private readonly IMessageService _messageService;

        public ChatsController(KarmaContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        // GET: Chats/Create
        public IActionResult Create(Post post)
        {
            if (User.Identity.IsAuthenticated && post != null && User.Identity.Name != post.UserId)
            {
                string user = User.Identity.Name;

                List<Chat> chats = _context.Chat
                    .Include(c => c.Creator)
                    .Include(c => c.PostUser)
                    .Where(c => c.AttachedPost.Id == post.Id && c.Creator.UserName == user).ToList();
                Chat chat = chats?.FirstOrDefault();
                if (chat != null)
                {
                    return RedirectToAction("Index", "Messages", new { chatId = chat.Id });
                }

                CreateChatModel chatModel = new() { PostId = post.Id };

                return View(chatModel);
            }
            return RedirectToAction("Index", "Posts");
        }

        // POST: Chats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateChatModel chatData)
        {

            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                string user = User.Identity.Name;
                if (chatData.PostId == null || _context.Post.Find(chatData.PostId)?.UserId == user)
                {
                    return RedirectToAction("Index", "Messages");
                }


                List<Chat> chats = await _context.Chat
                    .Include(c => c.Creator)
                    .Include(c => c.PostUser)
                    .Where(c => c.AttachedPost.Id == chatData.PostId && c.Creator.UserName == user).ToListAsync();
                Chat chat = chats?.FirstOrDefault();

                chat = _messageService.CreateChat(_context, chatData, user, chat);
                if (chat != null)
                {
                    return RedirectToAction("Index", "Messages", new { chatId = chat.Id });
                }
                return RedirectToAction("Index", "Messages");
            }
            return RedirectToAction("Index", "Posts");
        }

        public IActionResult UnseenMessages()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string count = UnseenMessagesNumber(User.Identity.Name);
            ViewBag.Count = count;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnseenMessages(string userId)
        {
            if (!User.Identity.IsAuthenticated || userId != User.Identity.Name)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string count = UnseenMessagesNumber(userId);
            ViewBag.Count = count;
            return View();
        }

        private int UnseenMessagesCount(string userId)
        {
            return _context.Chat
               .Include(c => c.Creator )
               .Include(c => c.PostUser)
               .Where(c => (c.PostUser.UserName == userId && c.IsSeenByPostUser == false) || (c.Creator.UserName == userId && c.IsSeenByCreator == false))
               .Count();
        }
        private string UnseenMessagesNumber(string userId)
        {
            int num = UnseenMessagesCount(userId);
            return (num < 100) ? num.ToString() : "99+";
        }

        private bool ChatExists(int id)
        {
            return _context.Chat.Any(e => e.Id == id);
        }
    }
}
