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

        public event EventHandler<CharityStateChangedEventArgs> CharityStateChanged;

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Image")]
        public string ImagePath { get; set; }
        [Required]
        [MaxLength(120)]
        public string Description { get; set; }
        [NotMapped]
        [Display(Name = "Item types")]
        public List<CharityAddress> CharityAddresses { get; set; }
        [NotMapped]
        [Display(Name = "Item types")]
        public List<CharityItemType> CharityItemTypes { get; set; }
        [Required]
        [Display(Name = "Request date")]
        public DateTime DateCreated { get; set; }
        [Required]
        [Display(Name = "Review state")]
        private Enums.ReviewState _reviewState;
        [Required]
        [Display(Name = "Review state")]
        public Enums.ReviewState ReviewState
        {
            get => _reviewState;
            set
            {
                if (value != _reviewState)
                {
                    _reviewState = value;
                    
                    var args = new CharityStateChangedEventArgs();
                    args.CharityId = this.Id;
                    args.ReviewState = this.ReviewState;
                    args.TimeChanged = DateTime.Now;

                    OnCharityStateChanged(args);
                }
            }
        }

        public Charity()
        {
            CharityAddresses = new List<CharityAddress>();
            CharityItemTypes = new List<CharityItemType>();
        }

        public static List<Charity> FilteredCharities(List<Charity> charities, ItemType itemType) 
        {
            List<Charity> filtered = new List<Charity>();
            if (charities == null)
                return filtered;

            foreach (Charity c in charities)
            {
                var itemTypes = c.GetItemTypes();

                if (c.ReviewState == Enums.ReviewState.Approved
                    && itemTypes.Contains(itemType))
                {
                    filtered.Add(c);
                }
            }

            return filtered;
        }

        public List<ItemType> GetItemTypes()
        {
            var itemTypes = new List<ItemType>();
            itemTypes = CharityItemTypes.Select(x => x.ItemType).ToList();

            return itemTypes;
        }

        protected virtual void OnCharityStateChanged(CharityStateChangedEventArgs e)
        {
            EventHandler<CharityStateChangedEventArgs> handler = CharityStateChanged;
            handler?.Invoke(this, e);
        }

    }

    public class CharityStateChangedEventArgs : EventArgs
    {
        public int CharityId { get; set; }
        public Enums.ReviewState ReviewState { get; set; }
        public DateTime TimeChanged { get; set; }
    }
}
