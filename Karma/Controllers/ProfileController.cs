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
using System.Threading;

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
            modelView.UserPosts = new(() => Post.getUserPosts(_context, user.UserName));

            var ratings = await _context.UserReview.Where(m => m.ReceiverId == user.UserName).ToListAsync();
            int sum = ratings.Sum(m => m.Rating);
            modelView.RatingAverage = sum == 0 ? 0 : (float) sum / ratings.Count();

            return View(modelView);
        }
        public async Task<IActionResult> Edit(string id)
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
            modelView.UserPosts = new(() => Post.getUserPosts(_context, user.UserName));
            return View(modelView);
        }
        // POST: Profile/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind(include:"Name,Surname,IpAddress,UserName,Description,ImagePath,IsActive")] User user, IFormFile file)
        {
            
            if (ModelState.IsValid)
            {
                var real_user = await _context.User.FirstOrDefaultAsync(m => m.UserName == id);
                string oldPath = real_user.ImagePath;
                if (real_user == null)
                {
                    return NotFound();
                }
                real_user.Description = user.Description;
                if (String.IsNullOrEmpty(user.UserName) || String.IsNullOrEmpty(user.Name))
                {
                    real_user.Surname = user.Surname;
                    real_user.Name = user.Name;
                }
                else
                {
                    return Redirect("~/Identity/Account/Manage");
                }

                

                _context.Entry(real_user).State = EntityState.Detached;


                if (CopyFileToRoot(real_user, file) == false)
                {
                    return View(real_user);
                }
                try
                {
                    _context.Update(real_user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine(user.Name);
                    if (!UserExists(user.Surname))
                    {
                        Response.StatusCode = 404;
                        return View("ErrorPages/404Post");
                    }
                    else
                    {
                        throw;
                    }
                }
//                if (oldPath != null && real_user.ImagePath != null)
//                {
//                    System.IO.File.Delete(Path.Combine(_iWebHostEnv.WebRootPath, Karma.Models.User.ImagesDirName, oldPath));
//                }
                return Redirect("~/Identity/Account/Manage");
            }
            IndexModel modelView = new IndexModel(_userManager, _signInManager, _context);
            modelView.currentUser = user;
            modelView.UserPosts = new(() => Post.getUserPosts(_context, user.UserName));
           // return Redirect("~/Identity/Account/Manage");
            return View(modelView);
        }
        private bool CopyFileToRoot(User user, IFormFile file, string ext = null)
        {
            bool? isValidFile = IsValidFile(file);
            if (isValidFile == false)
            {
                ViewBag.Message = "Invalid file type.";
                return false;
            }
            else if (isValidFile == true)
            {
                ext = ext ?? Path.GetExtension(file.FileName);

                string fileName = user.Id.ToString() + "x" + DateTime.Now.Ticks.ToString() + ext;

                string path = Path.Combine(_iWebHostEnv.WebRootPath, Karma.Models.User.ImagesDirName, fileName);

                Thread thread = new Thread(() =>
                {
                    FileStream stream = new FileStream(path, FileMode.Create);
                    
                    file.CopyTo(stream);
                });
                thread.Start();
                user.ImagePath = fileName;
            }
            return true;
        }
        private bool? IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            else if (!Path.GetExtension(file.FileName).IsValidExtension())
            {
                return false;
            }
            return true;
        }
        private bool UserExists(string surname)
        {
            return true;
        }
    }



}
