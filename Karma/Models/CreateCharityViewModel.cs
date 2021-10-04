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
        public List<Enums.Category> ItemTypes { get; set; }
        [Required]
        public string Address { get; set; }


        public string CreateFilePath(IWebHostEnvironment webHostEnvironment, string fileName, string prop)
        {
            StringBuilder content = new();
            switch (prop)
            {
                case "ItemTypes":
                    foreach (var e in ItemTypes)
                    {
                        content.AppendLine(e.ToString());
                    }
                    break;
                case "Address":
                    content.AppendLine(Address);
                    break;
                default:
                    break;
            }
            string path = Path.Combine(webHostEnvironment.ContentRootPath,
                "Charities", prop, fileName + ".txt");

            File.WriteAllText(path, content.ToString());

            return path;
        }
    }

}
