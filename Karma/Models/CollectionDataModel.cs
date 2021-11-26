using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CollectionDataModel
    {
        public List<Post> Posts { get; set; }
        public int? Category { get; set; }
        [StringLength(60)]
        public string SearchString { get; set; }
        public bool? isDonation { get; set; }
    }
}
