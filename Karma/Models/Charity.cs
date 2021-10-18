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
        public List<ItemType> ItemTypes { get; set; }

        public Charity()
        {
            ItemTypes = new List<ItemType>();
        }

        public static List<Charity> FilteredCharities(List<Charity> charities, ItemType itemType) 
        {
            List<Charity> filtered = new List<Charity>();
            if (charities == null)
                return filtered;


            foreach (Charity c in charities)
            {
                if (c.HasItemTypesFile())
                {
                    c.LoadItemTypes();
                }

                if (c.ItemTypes.Contains(itemType))
                {
                    filtered.Add(c);
                }
            }

            return filtered;
        }

        public bool HasItemTypesFile()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ItemTypePath);
            return File.Exists(filePath);
        }

        public void LoadItemTypes()
        {
            StreamReader sr = new StreamReader(ItemTypePath);
            ItemTypes = ItemTypes ?? new();
            string line;
            
            while ((line = sr.ReadLine()) != null)
            {
                ItemTypes.Add(Karma.Models.ItemTypes.GetItemType(line));
            }
        }
    }
}
