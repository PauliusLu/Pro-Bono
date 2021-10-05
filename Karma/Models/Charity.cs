using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class Charity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string AddressesPath { get; set; }
        [Required]
        public string ItemTypePath { get; set; }
        // Path to charity logo.
        public string ImagePath { get; set; }
        [Required]
        [MaxLength(120)]
        public string Description { get; set; }
        [NotMapped]
        public List<Enums.Category> ItemTypes { get; set; }

        public void LoadItemTypes()
        {
           if (File.Exists(ItemTypePath))
            {
                StreamReader sr = new StreamReader(ItemTypePath);
                ItemTypes = ItemTypes ?? new();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ItemTypes.Add(Enum.Parse<Enums.Category>(line));
                }
            }
        }
        public string GetImageName()
        {
            return Path.GetFileName(ImagePath);
        }
    }

}
