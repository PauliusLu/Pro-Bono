using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Controllers
{
    public class UserReviewsController : Controller
    {
        private readonly KarmaContext _context;

        public UserReviewsController(KarmaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            return View(await _context.UserReview.ToListAsync());
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReview = await _context.UserReview.FindAsync(id);
            if (userReview == null)
            {
                return NotFound();
            }
            return View(userReview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,CreatorId,ReceiverId,Rating,ReviewText")] UserReview userReview)
        {
            if (id != userReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserReviewExists(userReview.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Messages");
            }
            return View(userReview);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReview = await _context.UserReview
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userReview == null)
            {
                return NotFound();
            }

            return View(userReview);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userReview = await _context.UserReview.FindAsync(id);
            _context.UserReview.Remove(userReview);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "UserReviews");
        }

        private bool UserReviewExists(int id)
        {
            return _context.UserReview.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostReview([FromBody] PostReviewModel reviewModel)
        {
            var userReview = new UserReview();

            userReview.CreatorId = reviewModel.CreatorId;
            userReview.ReceiverId = reviewModel.ReceiverId;
            userReview.PostId = reviewModel.PostId;
            userReview.ReviewText = reviewModel.ReviewText;
            userReview.Rating = reviewModel.Rating;

            _context.Add(userReview);
            await _context.SaveChangesAsync();

            return new EmptyResult();
        }

        public class PostReviewModel
        {
            public int PostId { get; set; }
            public string CreatorId { get; set; }
            public string ReceiverId { get; set; }
            public string ReviewText { get; set; }
            public int Rating { get; set; }

        }
    }
}
