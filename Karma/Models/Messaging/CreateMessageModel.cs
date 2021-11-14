using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models.Messaging
{
    public class CreateMessageModel
    {
        public int ChatId { get; set; }
        public string Text { get; set; }
    }
}
