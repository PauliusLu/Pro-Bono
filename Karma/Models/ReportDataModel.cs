using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class ReportDataModel
    {
        public List<Report> Reports { get; set; }
        public int? page { get; set; }
        public string sorting { get; set; }
        public string? aggr { get; set; }

        public static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }
    }

}
