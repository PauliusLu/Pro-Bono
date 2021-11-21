﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class UserReview
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        public Post Post { get; set; }
        [Required]
        public string CreatorId { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        [MaxLength(160)]
        [Display(Name = "Review text")]
        public string ReviewText { get; set; }

        public UserReview()
        {

        }
    }
}