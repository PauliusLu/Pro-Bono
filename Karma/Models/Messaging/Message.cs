using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models.Messaging
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public User Sender { get; set; }
        [Required]
        public Chat Chat { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public bool IsSender(string username)
        {
            return Sender.UserName == username;
        }
    }
}
