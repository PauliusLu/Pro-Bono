using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{

    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostType { get; set; }
        public DateTime PostingDate { get; set; }
        public string Title { get; set; }
        public int ItemType { get; set; }
        public string PostText { get; set; }
        public string Image { get; set; }
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
