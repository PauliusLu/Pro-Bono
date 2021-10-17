﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Karma.Models
{

    public class Post : IComparable<Post>
    {
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

        public Post()
        {

        }

        public static string GetPostTypeName(bool isDonation)
        {
            //Name for which kind of post it is.
            return isDonation ? "Offer" : "Request";
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
    }
}
