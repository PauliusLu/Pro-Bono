using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CollectionDataModel
    {
        public List<Post> Posts { get; set; }
        public SelectCategoryViewModel Categories { get; set; }

    }
}
