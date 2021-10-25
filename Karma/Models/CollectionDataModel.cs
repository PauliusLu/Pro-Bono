using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CollectionDataModel
    {
        public List<Post> Posts { get; set; }
        public int? Category { get; set; }
        public string SearchString { get; set; }
        public bool? State { get; set; }
    }
}
