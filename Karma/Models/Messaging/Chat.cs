using System.ComponentModel.DataAnnotations;

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
        public User Creator { get; set; }
        [Required]
        public User PostUser { get; set; }
        [Required]
        public bool IsSeenByCreator { get; set; }
        [Required]
        public bool IsSeenByPostUser { get; set; }


        public Chat()
        {

        }

        public Chat(int id, Post post, ChatState state, User creatorId, User postUserId, bool isSeenByCreator, bool isSeenByPostUser)
        {
            Id = id;
            AttachedPost = post;
            State = state;
            Creator = creatorId;
            PostUser = postUserId;
            IsSeenByCreator = isSeenByCreator;
            IsSeenByPostUser = isSeenByPostUser;
        }
    }
}
