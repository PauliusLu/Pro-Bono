using Karma.Data;
using Karma.Models;
using Karma.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Services
{
    public interface IMessageService
    {
        Message CreateMessage(KarmaContext context, CreateMessageModel m, string userName);

        Chat CreateChat(KarmaContext context, CreateChatModel chatData, string userName, Chat chat = null);
    }
}
