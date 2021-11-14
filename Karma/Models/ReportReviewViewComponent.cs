using Karma.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class ReportReviewViewComponent : ViewComponent
    {
        private readonly KarmaContext _context;

        public ReportReviewViewComponent(KarmaContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dbReportList = await _context.Report.ToListAsync();

            dbReportList.Sort((Report a, Report b) =>
            {
                return a.ReportState.CompareTo(b.ReportState);
            });

            return View(dbReportList);
        }
    }
}
