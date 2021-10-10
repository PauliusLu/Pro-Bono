using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

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

            if (isDonation == null)
            {
                ViewBag.Header = "All posts";
                posts = await _context.Post.ToListAsync();
            }
            else
            {
                posts = await _context.Post.Where(p => p.IsDonation == isDonation).ToListAsync();
                ViewBag.Header = (bool) isDonation ? "All donations" : "All requests";
            }

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
            return View();
        }

        // POST: Posts/Donate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate([Bind("Id,UserId,IsDonation,Date,Title,ItemType,Description,ImagePath,IsVisible")] Post post, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length != 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    if (!Utils.IsValidExtension(ext))
                    {
                        ViewBag.Message = "Invalid file type.";
                        return View(post);
                    }

                    
                    // post.UserId is always 0, should be configured in the future
                    string fileName = post.UserId.ToString() + "x" + DateTime.Now.Ticks.ToString() + ext;

                    string path = Path.Combine(_iWebHostEnv.WebRootPath, Post.ImagesDirName, fileName);

                    // Copying file to wwwroot/PostImages
                    FileStream stream = new FileStream(path, FileMode.Create);
                    _ = file.CopyToAsync(stream);

                    post.ImagePath = fileName;
                }

                FillPostFields(post, true);
                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/CreateRequest
        public IActionResult CreateRequest()
        {
            return View();
        }

        // POST: Posts/CreateRequest
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest([Bind("Id,UserId,IsDonation,Date,Title,ItemType,Description,ImagePath,IsVisible")] Post post)
        {
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
            if (post == null)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,IsDonation,Date,Title,ItemType,Description,ImagePath,IsVisible")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
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
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

    }
}
