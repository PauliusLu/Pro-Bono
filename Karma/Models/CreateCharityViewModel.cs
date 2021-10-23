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


        public string CreateFilePath(IWebHostEnvironment webHostEnvironment, string fileName, string dir)
        {
            StringBuilder content = new();

            if (dir == Charity.ItemTypesDirName)
            {
                foreach (int e in ItemTypes)
                {
                    content.AppendLine(Karma.Models.ItemTypes.GetItemType(e).Name);
                }
            }
            else if (dir == Charity.AdressDirName)
            {
                content.AppendLine(Address);
            }
            else
            {
                return "";
            }


            string relativePath = Path.Combine(dir, fileName + ".txt");
            string path = Path.Combine(webHostEnvironment.ContentRootPath, relativePath);

            File.WriteAllText(path, content.ToString());

            return relativePath;
        }
    }

}
