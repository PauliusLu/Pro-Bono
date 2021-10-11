using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Karma.Models
{

    public class Post
    {
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
        public Enums.Category ItemType { get; set; }
        [Required]
        [MaxLength(120)]
        public string Description { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public bool IsVisible { get; set; }

        public Post()
        {

        }

        public static string GetPostTypeName(bool isDonation)
        {
            //Name for which kind of post it is.
            return isDonation ? "Offer" : "Request";
        }

        public string GetImageName()
        {
            return Path.GetFileName(ImagePath);
        }
    }
}
