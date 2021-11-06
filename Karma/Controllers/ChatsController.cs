﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models.Messaging;
using Karma.Models;

namespace Karma.Controllers
{
    public class ChatsController : Controller
    {
        private readonly KarmaContext _context;

        public ChatsController(KarmaContext context)
        {
            _context = context;
        }

        // GET: Chats/Create
        public IActionResult Create(Post post)
        {
            if (User.Identity.IsAuthenticated && post != null && User.Identity.Name != post.UserId)
            {
                string user = User.Identity.Name;

                List<Chat> chats = _context.Chat.Where(c => c.AttachedPost.Id == post.Id && c.CreatorId == user).ToList();
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
        public async Task<IActionResult> Create(CreateChatModel chatData)
        {

            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                string user = User.Identity.Name;
                if (chatData.PostId == null || _context.Post.Find(chatData.PostId)?.UserId == user)
                {
                    return RedirectToAction("Index", "Messages");
                }


                List<Chat> chats = await _context.Chat.Where(c => c.AttachedPost.Id == chatData.PostId && c.CreatorId == user).ToListAsync();
                Chat chat = chats?.FirstOrDefault();

                chat = CreateChat(chatData, chat);
                if (chat != null)
                {
                    return RedirectToAction("Index", "Messages", new { chatId = chat.Id });
                }
                return RedirectToAction("Index", "Messages");
            }
            return RedirectToAction("Index", "Posts");
        }

        private Chat CreateChat(CreateChatModel chatData, Chat chat = null)
        {
            if (chat == null)
            {
                Post post = _context.Post.Find(chatData.PostId);
                chat = new() { AttachedPost = post, State = Chat.ChatState.Open, CreatorId = User.Identity.Name};
                _context.Add(chat);
            }



            Message message = new()
            {
                Chat = chat,
                Date = DateTime.UtcNow,
                Text = chatData.Text,
                Username = User.Identity.Name
            };

            _context.Add(message);
            _context.SaveChangesAsync();
            return chat;
        }

        private bool ChatExists(int id)
        {
            return _context.Chat.Any(e => e.Id == id);
        }
    }
}