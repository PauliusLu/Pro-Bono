using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models.Messaging
{
    public class CreateChatModel
    {
        public int? PostId { get; set; }
        public string Text { get; set; }
    }
}
