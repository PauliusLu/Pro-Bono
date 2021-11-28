using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Collections;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Karma.Areas.Identity.Pages.Account.Manage;
using Microsoft.AspNetCore.Identity;

namespace Karma.Controllers
{
    public class ProfileController : Controller
    {
        private readonly KarmaContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IWebHostEnvironment _iWebHostEnv;
        public ProfileController(KarmaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _iWebHostEnv = webHostEnvironment;
        }
        public async Task<IActionResult> Index(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(m => m.UserName == userId);
            if (user == null)
            {
                return NotFound();
            }
            IndexModel modelView = new IndexModel(_userManager, _signInManager, _context);
            modelView.currentUser = user;
            modelView.UserPosts = new(() => Post.getUserPosts(_context, user.UserName));

            var ratings = await _context.UserReview.Where(m => m.ReceiverId == user.UserName).ToListAsync();
            modelView.RatingAverage = UserReview.CountRatingAverage(ratings);

            return View(modelView);
        }
    }



}
