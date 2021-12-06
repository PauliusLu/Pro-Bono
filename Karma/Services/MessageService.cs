using Karma.Data;
using Karma.Models;
using Karma.Models.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Services
{
    public class MessageService : IMessageService
    {
        public Message CreateMessage(KarmaContext context, CreateMessageModel m, string userName)
        {
            User user = context.User.Where(u => u.UserName == userName).FirstOrDefault();
            Message message = new()
            {
                Chat = context.Chat.Find(m.ChatId),
                Date = DateTime.UtcNow,
                Text = m.Text,
                Sender = user
            };

            return message;
        }

        public Chat CreateChat(KarmaContext context, CreateChatModel chatData, string userName, Chat chat = null)
        {
            User creator = context.User.Where(u => u.UserName == userName).FirstOrDefault();
            if (chat == null)
            {
                Post post = context.Post.Find(chatData.PostId);
                User postUser = context.User.Where(u => u.UserName == post.UserId).FirstOrDefault();

                chat = new(0, post, Chat.ChatState.Open, creator, postUser, true, false);
                context.Add(chat);
            }
            else
            {
                chat.IsSeenByPostUser = false;
            }
            Message message = CreateChatMessage(chat, chatData.Text, creator);

            context.Add(message);
            context.SaveChangesAsync();
            return chat;
        }

        private Message CreateChatMessage(Chat chat, string text, User creator)
        {
            Message message = new()
            {
                Chat = chat,
                Date = DateTime.UtcNow,
                Text = text,
                Sender = creator
            };
            return message;
        }
    }
}
