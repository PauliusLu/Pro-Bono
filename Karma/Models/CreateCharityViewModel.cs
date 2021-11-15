using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CreateCharityViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public List<int> ItemTypes { get; set; }
        [Required]
        public string Address { get; set; }

    }

}
