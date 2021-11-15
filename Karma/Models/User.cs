using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Karma.Models
{
    public class User : IdentityUser
    {
        public static readonly string ImagesDirName = Path.Combine("data", "UserImages");
        public static readonly string DefaultImagesDirName = Path.Combine("data", "DefaultUserImages");

        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        [Display(Name = "Is active")]
        public bool IsActive { get; set; }
    }
}
