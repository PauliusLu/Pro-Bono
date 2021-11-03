using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Karma.Data;
using Microsoft.EntityFrameworkCore;

namespace Karma.Models
{

    public class Post : IComparable<Post>, IEquatable<Post>
    {
        public enum PostState
        {
            NotSet = 0,
            Reserved,
            Open,
            Traded, //Not specified whether Donated or Received
            Donated,
            Received,
            Hidden, //Settable by user
            Deleted,
            Reported
        }

        public static readonly string ImagesDirName = Path.Combine("data", "PostImages");
        public static readonly string DefaultImagesDirName = Path.Combine("data", "DefaultPostImages");

        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool IsDonation { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a category")]
        public int ItemType { get; set; }
        [Required]
        [MaxLength(120)]
        public string Description { get; set; }
        // Should be combined with ImagesDirName
        public string ImagePath { get; set; }
        [Required]
        public bool IsVisible { get; set; }
        public int State { get; set; }

        public Post()
        {

        }

        protected Post(int id, string userId, bool isDonation, DateTime date, string title, 
            int itemType, string description, string imagePath, bool isVisible)
        {
            Id = id;
            UserId = userId;
            IsDonation = isDonation;
            Date = date;
            Title = title;
            ItemType = ItemType;
            Description = description;
            ImagePath = imagePath;
            IsVisible = isVisible;
        }

        public static string GetPostTypeName(bool isDonation)
        {
            //Name for which kind of post it is.
            return isDonation ? "Offer" : "Request";
        }

        public string GetFullImagePath()
        {
            if (ImagePath == null)
            {
                return GetDefaultImage();
            }
            else
            {
                return Path.Combine(Post.ImagesDirName, ImagePath);
            }
        }

        public string GetDefaultImage()
        {
            if (ItemTypes.Types.ContainsKey(ItemType))
            {
                ItemType itemType = new ItemTypes()[ItemType];
                return Path.Combine(Post.DefaultImagesDirName, itemType.ImagePath);
            }
            return null;
        }

        // By visibility and by date (descending order).
        public int CompareTo(Post other)
        {
            if (other.IsVisible == false && this.IsVisible == true)
                return -1;
            else if (other.IsVisible == true && this.IsVisible == false)
                return 1;
            else
                return other.Date.CompareTo(this.Date);
        }

        public bool Equals(Post other)
        {
            if (Id == other.Id &&
                UserId == other.UserId &&
                IsDonation == other.IsDonation &&
                Date == other.Date &&
                Title == other.Title &&
                ItemType == other.ItemType &&
                Description == other.Description &&
                //ImagePath == other.ImagePath &&
                IsVisible == other.IsVisible)
            {
                return true;
            }
            return false;
        }
        static List<Post> posts;
        static List<User> users;

        public static void GetLists(List<Post> p, List<User> u)
        {
            posts = p;
            users = u;
        }
        public static string CompareQuerie(Post rPost)
        {
            string text = "User";
            var query = from user in users
                        join post in posts on user.UserName equals post.UserId into gj
                        select new { PostOwnerName = user.Name, PostOwnerSurname = user.Surname, Posts = gj };
            foreach (var v in query)
            {
                foreach (Post po in v.Posts)
                    if (po.Id == rPost.Id)
                    {
                        text = v.PostOwnerName + " " + v.PostOwnerSurname;
                    }
            }
            return text;
        }

        public static async Task<List<Post>> getUserPosts(KarmaContext context, string userId)
        {
            List<Post> posts = await context.Post.ToListAsync();

            return posts.Where((p) => p.UserId == userId).ToList();
        }
    }
}
