using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class SelectCategoryViewModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please select a category")]
        public int? ItemType { get; set; }
        public Enums.ActionType ActionType { get; set; }

    }
}
