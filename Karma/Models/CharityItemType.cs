using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CharityItemType
    {
        [Key]
        public int CharityId { get; set; }
        [Key]
        public int ItemTypeId { get; set; }
        [NotMapped]
        public ItemType ItemType 
        { 
            get { return ItemTypes.GetItemType(ItemTypeId); }
            set { ItemType = ItemTypes.GetItemType(ItemTypeId); } 
        }

        public CharityItemType()
        {

        }
    }
}
