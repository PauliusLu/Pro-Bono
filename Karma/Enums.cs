using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma
{
    public class Enums
    {
        public enum ActionType
        {
            Donate = 0,
            Request
        }

        public enum ReviewState
        {
            [Display(Name = "Waiting")]
            Waiting = 0,

            [Display(Name = "In review")]
            InReview,

            [Display(Name = "Approved")]
            Approved,

            [Display(Name = "Declined")]
            Declined
        }

        public enum AdminTab
        {
            Users,
            CharityReview
        }
    }

}
