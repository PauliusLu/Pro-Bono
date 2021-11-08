using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models.Messaging
{
    public class Chat
    {
        public enum ChatState { Open, Closed }

        [Key]
        public int Id { get; set; }
        [Required]
        public Post AttachedPost { get; set; }
        [Required]
        public ChatState State { get; set; }
        [Required]
        public string CreatorId { get; set; }
    }
}
