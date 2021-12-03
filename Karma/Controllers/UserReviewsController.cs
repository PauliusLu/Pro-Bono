using Karma.Areas.Identity.Pages.Account.Manage;
using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManage _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserReviewsController(KarmaContext context, UserManage userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(m => m.UserName == id);
            if (user == null)
            {
                return NotFound();
            }

            IndexModel modelView = new IndexModel(_userManager, _signInManager, _context);
            modelView.currentUser = user;
            var ratings = await _context.UserReview.Where(m => m.ReceiverId == user.UserName).ToListAsync();
            modelView.RatingAverage = UserReview.CountRatingAverage(ratings);

            ViewBag.UserModel = modelView;

            var userReviews = await _context.UserReview.Where(m => m.ReceiverId == id).ToListAsync();
            var users = await _context.User.ToListAsync();
            var reviewsWithCreators = userReviews.Join(
                                users,
                                userReview => userReview.CreatorId,
                                user => user.UserName,
                                (userReview, user) => { userReview.Creator = user; return userReview; });

            return View(reviewsWithCreators);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,CreatorId,ReceiverId,Rating,ReviewText,Date")] UserReview userReview)
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
                return RedirectToAction("Index", new { id = userReview.ReceiverId });
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

            return RedirectToAction("Index", new { id = userReview.ReceiverId });
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
            userReview.Date = DateTime.UtcNow;

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
