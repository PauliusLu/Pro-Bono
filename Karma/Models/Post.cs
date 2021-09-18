using System;
using System.Collections.Generic;
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
    }
}
