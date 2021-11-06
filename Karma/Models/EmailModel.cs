using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class EmailModel
    {
        public const string SendingEmail = "info@karma.com";

        public class EmailCharityState
        {
            public const string EmailSubject = "Charity state changed";
            public string UserName { get; set; }
            public string CharityName { get; set; }
            public Enums.ReviewState ReviewState { get; set; }
            public DateTime TimeChanged { get; set; }

            public EmailCharityState(string userName, string charityName, Enums.ReviewState reviewState, DateTime timeChanged)
            {
                UserName = userName;
                CharityName = charityName;
                ReviewState = reviewState;
                TimeChanged = timeChanged;
            }
        }
    }
}
