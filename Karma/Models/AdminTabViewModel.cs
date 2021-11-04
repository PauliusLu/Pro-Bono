using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class AdminTabViewModel
    {
        public Enums.AdminTab ActiveTab { get; set; }

        public AdminTabViewModel()
        {

        }

        public AdminTabViewModel(Enums.AdminTab activeTab)
        {
            ActiveTab = activeTab;
        }
    }
}
