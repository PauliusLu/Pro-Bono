using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class AdminIndexTabViewModel
    {
        public Enums.AdminTab ActiveTab { get; set; }

        public AdminIndexTabViewModel()
        {

        }

        public AdminIndexTabViewModel(Enums.AdminTab activeTab)
        {
            ActiveTab = activeTab;
        }
    }
}
