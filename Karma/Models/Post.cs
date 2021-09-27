using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Karma.Models
{

    public class Post
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsDonation { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }
        public Enums.Category ItemType { get; set; }
        [Required]
        [MaxLength(120)]
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsVisible { get; set; }

        public Post()
        {

        }

        public static string GetPostTypeName(int postType)
        {
            //Name for which kind of post it is.
            return postType == 0 ? "Request" : "Offer";
        }

        public static string GetItemTypeName(int itemType)
        {
            //Name for which kind of item it is.
            return itemType == 0 ? "Clothes" : "Other";
        }
    }
}
