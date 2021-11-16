using System;

namespace Karma.Models
{
    public class ErrorViewModel
    {

        int id { get; set; }
        string userString { get; set; }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
