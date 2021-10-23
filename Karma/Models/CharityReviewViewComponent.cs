using Karma.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class CharityReviewViewComponent : ViewComponent
    {
        private readonly KarmaContext _context;

        public CharityReviewViewComponent(KarmaContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dbCharitiesList = await _context.Charity.ToListAsync();

            IEnumerable<Charity> charityQuery =
                from charity in dbCharitiesList
                orderby charity.ReviewState
                select charity;

            var charitiesList = new List<Charity>();

            foreach (Charity c in charityQuery)
            {
                charitiesList.Add(c);
            }

            return View(charitiesList);
        }

    }
}
