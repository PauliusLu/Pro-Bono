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
        [Required]
        public string PostUserId { get; set; }
        [Required]
        public bool IsSeenByCreator { get; set; }
        [Required]
        public bool IsSeenByPostUser { get; set; }


        public Chat()
        {

        }

        public Chat(int id, Post post, ChatState state, string creatorId, string postUserId, bool isSeenByCreator, bool isSeenByPostUser)
        {
            Id = id;
            AttachedPost = post;
            State = state;
            CreatorId = creatorId;
            PostUserId = postUserId;
            IsSeenByCreator = isSeenByCreator;
            IsSeenByPostUser = isSeenByPostUser;
        }
    }
}
