using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CharityAddress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CharityId { get; set; }
        [Required]
        [MaxLength(30)]
        public string Country { get; set; }
        [Required]
        [MaxLength(30)]
        public string City { get; set; }
        [Required]
        [MaxLength(50)]
        public string Street { get; set; }
        [Required]
        [MaxLength(10)]
        public string HouseNumber { get; set; }
        [MaxLength(10)]
        public string PostCode { get; set; }

        public CharityAddress()
        {

        }
    }
}
