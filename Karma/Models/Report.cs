using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class Report
    {
        public enum ReportStates
        {
            NotSet = 0,
            Open,
            Approved,
            Declined,
            Closed
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public string PostOwnerId { get; set; }
        [Required]
        public string ReporterId { get; set; }
        public ReportStates ReportState { get; set; }
        public string ReportMessage { get; set; }
    }
}
