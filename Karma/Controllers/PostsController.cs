﻿using System;
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

namespace Karma.Controllers
{
    public class PostsController : Controller
    {
        private readonly KarmaContext _context;

        private readonly IWebHostEnvironment _iWebHostEnv;

        // Passes an object of type IWebHostEnvironment that carries information about our host environment.
        public PostsController(KarmaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _iWebHostEnv = webHostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index(bool? isDonation)
        {
            List<Post> posts;
            List<User> users;
            users = await _context.User.ToListAsync();
            if (isDonation == null)
            {
                ViewBag.Header = "All posts";

                posts = await _context.Post.
                    Where(p => p.IsVisible).ToListAsync();

                PostsData pd = new PostsData();
                pd.PostAverage(posts.Count(), posts[0].Date);
            }
            else
            {
                posts = await _context.Post
                    .Where(p => p.IsVisible && p.IsDonation == isDonation).ToListAsync();
                ViewBag.Header = (bool) isDonation ? "All donations" : "All requests";
            }
            // Sets default image for post by itemtype if there's no image given
            foreach (Post post in posts)
            {
                if (post.ImagePath == null)
                {
                    int itemTypeId = post.ItemType;
                    if (ItemTypes.Types.ContainsKey(itemTypeId))
                    {
                        ItemType itemType = new ItemTypes()[itemTypeId];
                        post.ImagePath = Path.Combine(Post.DefaultImagesDirName, itemType.ImagePath);
                    }
                }
                else
                {
                    post.ImagePath = Path.Combine(Post.ImagesDirName, post.ImagePath);
                }
            }


            if (isDonation == null)
            {
                foreach (Advert ad in Advert.Samples)
                    posts.Add(ad);
            }

            Post.GetLists(posts, users);
            posts.Sort();
            return View(posts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Donate
        public IActionResult Donate()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            return View();
        }

        // POST: Posts/Donate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate([Bind("Id,UserId,IsDonation,Date,Title,ItemType,Description,ImagePath,IsVisible")] Post post, IFormFile file)
        {
            if (IsUserHavePermission(out IActionResult act) != null)
                return act;

            post.UserId = User.Identity.Name;
            

            if (ModelState.IsValid)
            {
                if (CopyFileToRoot(post, file) == false)
                {
                    return View(post);
                }

                FillPostFields(post, true);
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // Returns null if there is no file, false if extension is invalid and true otherwise
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

        // Returns true on success, false on failure
        private bool CopyFileToRoot(Post post, IFormFile file, string ext = null)
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

                string fileName = post.UserId.ToString() + "x" + DateTime.Now.Ticks.ToString() + ext;

                string path = Path.Combine(_iWebHostEnv.WebRootPath, Post.ImagesDirName, fileName);

                // Copying file to wwwroot/PostImages
                FileStream stream = new FileStream(path, FileMode.Create);
                _ = file.CopyToAsync(stream);

                post.ImagePath = fileName;
            }
            return true;
        }


        // GET: Posts/CreateRequest
        public IActionResult CreateRequest()
        {
            if (IsUserHavePermission(out IActionResult act) != null)
                return act;
            return View();
        }

        // POST: Posts/CreateRequest
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest([Bind("Id,UserId,IsDonation,Date,Title,ItemType,Description,ImagePath,IsVisible")] Post post)
        {
            if (IsUserHavePermission(out IActionResult act) != null)
                return act;


            post.UserId = User.Identity.Name;
            if (ModelState.IsValid)
            {
                FillPostFields(post, false);
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }



        private void FillPostFields(Post post, bool isDonation)
        {
            post.IsDonation = isDonation;
            post.Date = DateTime.UtcNow;
            post.IsVisible = true;
        }


        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = await _context.Post.FindAsync(id);

            if (IsUserHavePermission(out IActionResult act, post: post) != null)
                return act;

            if (post == null || !post.IsVisible)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,IsDonation,Date,Title,ItemType,Description,ImagePath,IsVisible")] Post post, IFormFile file)
        {
            if (id != post.Id || !post.IsVisible)
            {
                return NotFound();
            }

            if (IsUserHavePermission(out IActionResult act, postUserId: post.UserId) != null)
                return act;

            if (ModelState.IsValid)
            {
                // If the edited post is identical to the old one, return to the index page.
                Post real_post = _context.Post.Find(id);
                if (post.Equals(real_post) && file == null)
                    return RedirectToAction(nameof(Index));
                post.ImagePath = real_post.ImagePath;
                _context.Entry(real_post).State = EntityState.Detached;


                if (CopyFileToRoot(post, file) == false)
                {
                    return View(post);
                }

                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (IsUserHavePermission(out IActionResult act, postId: id) != null)
                return act;

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null || !post.IsVisible)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (IsUserHavePermission(out IActionResult act, postId: id) != null)
                return act;

            var post = await _context.Post.FindAsync(id);
            if (!post.IsVisible)
                return NotFound();
            post.IsVisible = false;

            _context.Post.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        private object IsUserHavePermission(out IActionResult act, int? postId = null, string userId = null, Post post = null, string postUserId = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                act = RedirectToPage("/Account/Login", new { area = "Identity" });
                return act;
            }
            //If check is done by userID
            if (userId != null)
            {
                if (User.Identity.Name != userId)
                {
                    //return NoAccess();
                    act = NotFound();
                    return act;
                }
            }
            //if check is done by postID
            if (postId != null)
            {
                var real_post = _context.Post.Find(postId);
                if (User.Identity.Name != real_post.UserId)
                {
                    //return NoAccess();
                    act = NotFound();
                    return act;
                }
            }
            //if check is done by post
            if (post != null)
            {
                if (User.Identity.Name != post.UserId)
                {
                    //return NoAccess();
                    act = NotFound();
                    return act;
                }
            }
            //if check is done by postUserId
            if (postUserId != null)
            {
                if (User.Identity.Name != postUserId)
                {
                    //return NoAccess();
                    act = NotFound();
                    return act;
                }
            }

            //If user has Permission
            act = null;
            return null;
        }
    }
}
