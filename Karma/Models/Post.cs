using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Karma.Models
{
    public class Post
    {
        [Key]
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
    }
}
