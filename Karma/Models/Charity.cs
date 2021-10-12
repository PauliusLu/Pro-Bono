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
        public static readonly string ImagesDirName = Path.Combine("data", "CharityImages");
        public static readonly string AdressDirName = Path.Combine("Data", "Charities", "Address");
        public static readonly string ItemTypesDirName = Path.Combine("Data", "Charities", "ItemTypes");


        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string AddressesPath { get; set; }
        [Required]
        public string ItemTypePath { get; set; }
        public string ImagePath { get; set; }
        [Required]
        [MaxLength(120)]
        public string Description { get; set; }
        [NotMapped]
        public List<Enums.Category> ItemTypes { get; set; }

        public Charity()
        {
            ItemTypes = new List<Enums.Category>();
        }

        public void LoadItemTypes()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ItemTypePath);
            if (File.Exists(filePath))
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
    }

}
