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
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Karma.Controllers
{
    public class PostsController : Controller
    {
        private readonly KarmaContext _context;

        private readonly IWebHostEnvironment _iWebHostEnv;

        private readonly ILogger<PostsController> _logger;

        // Passes an object of type IWebHostEnvironment that carries information about our host environment.
        public PostsController(KarmaContext context, IWebHostEnvironment webHostEnvironment, ILogger<PostsController> logger)
        {
            _context = context;
            _iWebHostEnv = webHostEnvironment;
            _logger = logger;
        }

        // GET: Posts
        public async Task<IActionResult> Index(bool? isDonation, string searchString, int category=-1)
        {
            List<Post> posts;
            List<User> users;
            ViewBag.Header = "All posts";
            var visiblePosts = _context.Post.Where(p => p.IsVisible);
            users = await _context.User.ToListAsync();
            if (isDonation != null)
            {
                ViewBag.Header = (bool)isDonation ? "All donations" : "All requests";
                visiblePosts = _context.Post.Where(p => p.IsVisible && p.IsDonation == isDonation);
            }
                if (!String.IsNullOrEmpty(searchString) && category!= -1)
                    {
                        posts = await visiblePosts.
                        Where(p => p.Title.ToLower().Contains(searchString.ToLower())).
                        Where(p => p.ItemType == category).ToListAsync();
                    }
                    else if(category!= -1)
                    {
                        posts = await visiblePosts.
                        Where(p => p.ItemType == category).ToListAsync();
                    }
                    else if (!String.IsNullOrEmpty(searchString))
                    {
                        posts = await visiblePosts.
                        Where(p => p.Title.ToLower().Contains(searchString.ToLower())).ToListAsync();
                    }
                    else
                    {
                        posts = await visiblePosts.ToListAsync();
                    }
                    PostsData pd = new PostsData();


            // Sets default image for post by itemtype if there's no image given
            foreach (Post post in posts)
            {
                post.ImagePath = post.GetFullImagePath();
            }

            if (isDonation == null && posts.Count>=3)
            {
                foreach (Advert ad in Advert.Samples)
                    posts.Add(ad);
            }
            if (posts.Count == 0)
            {
                ViewBag.Message = "No posts";
            }

            posts.Sort();
            var postVM = new CollectionDataModel
            {
                Posts = posts,
                isDonation = isDonation,
                SearchString = searchString,
            };
            Post.GetLists(posts, users);
            return View(postVM);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(LogEvents.GetPost, "Post NOT FOUND, Post.Id == null");
                Response.StatusCode = 404;
                return View("ErrorPages/404Post");
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                _logger.LogWarning(LogEvents.GetPost, "Post {PostId} NOT FOUND", id);
                Response.StatusCode = 404;
                return View("ErrorPages/404Post");
            }

            // Sets default image for post by itemtype if there's no image given
            post.ImagePath = post.GetFullImagePath();

            return View(post);
        }

        // GET: Posts/Donate
        public IActionResult Donate()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogInformation(LogEvents.UserNotAuthenticated, "User is not authenticated");
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

                _logger.LogInformation(LogEvents.CreatePost, "Post created by User {UserId}", post.UserId);
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
                Thread thread = new Thread(() =>
                {
                    FileStream stream = new FileStream(path, FileMode.Create);
                    file.CopyTo(stream);
                });
                thread.Start();

                post.ImagePath = fileName;
                _logger.LogInformation(LogEvents.AddImage, "Image {ImagePath} copied to {PathToImagesDir}", post.ImagePath, path);
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

                _logger.LogInformation(LogEvents.CreatePost, "Post created by User {UserId}", post.UserId);
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
                _logger.LogWarning(LogEvents.GetPost, "Post NOT FOUND, Post.Id == null");
                Response.StatusCode = 404;
                return View("ErrorPages/404Post");
            }
            var post = await _context.Post.FindAsync(id);

            if (IsUserHavePermission(out IActionResult act, post: post) != null)
                return act;

            if (post == null || !post.IsVisible)
            {
                try 
                { 
                    _logger.LogWarning(LogEvents.GetPost, "Post {PostId} NOT FOUND", post.Id);
                }
                catch (NullReferenceException)
                {
                    _logger.LogError(LogEvents.GetPost, "Post {PostId} DOES NOT EXIST", id);
                    Response.StatusCode = 404;
                    return View("ErrorPages/404Post");
                }
               
            }
            post.ImagePath = post.GetFullImagePath();

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
                _logger.LogWarning(LogEvents.GetPost, "Post {PostId} NOT FOUND", post.Id);
                Response.StatusCode = 404;
                return View("ErrorPages/404Post");
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
                        Response.StatusCode = 404;
                        return View("ErrorPages/404Post");
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.LogInformation(LogEvents.EditPost, "Post {PostId} edited", post.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(LogEvents.GetPost, "Post NOT FOUND, Post.Id == null");
                return NotFound();
            }

            try
            {
                if (IsUserHavePermission(out IActionResult act, postId: id) != null)
                    return act;
            }
            catch (NullReferenceException)
            {
                Response.StatusCode = 404;
                return View("ErrorPages/404Post");
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null || !post.IsVisible)
            {
                try 
                { 
                    _logger.LogWarning(LogEvents.GetPost, "Post {PostId} NOT FOUND", post.Id);
                }
                catch (NullReferenceException) 
                {
                    _logger.LogError(LogEvents.GetPost, "Post {PostId} DOES NOT EXIST", id);
                    Response.StatusCode = 404;
                    return View("ErrorPages/404Post");
                }
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

            _logger.LogInformation(LogEvents.DeletePost, "Post {PostId} deleted", post.Id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Posts/ReserveItem
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Posts/ReserveItem/{postId:int}")]
        public async Task<IActionResult> ReserveItem(int postId)
        {
            if (IsUserHavePermission(out IActionResult act, postId:postId) != null)
                return act;


            return await findPostAsync(postId, async (post) => {
                if (post != null)
                {
                    if (post.State == (int)Post.PostState.Reserved)
                        post.State = (int)Post.PostState.Open;
                    else if (post.State == (int)Post.PostState.NotSet || post.State == (int)Post.PostState.Open)
                        post.State = (int)Post.PostState.Reserved;

                    _context.Update(post);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(LogEvents.ChangeState, "Post {PostId} state changed to {PostState}", post.Id, post.State);
                }

                return RedirectToAction("Details", new { id = postId });
            });
            
        }

        [HttpPost]
        [Route("Posts/CompleteItem/{postId:int}")]
        public async Task<IActionResult> CompleteItem(int postId)
        {
            if (IsUserHavePermission(out IActionResult act, postId: postId) != null)
                return act;


            return await findPostAsync(postId, async (post) =>
            {
                if (post != null)
                {
                    if (post.State == (int)Post.PostState.Reserved)
                    {
                        post.State = (int)Post.PostState.Traded;
                        post.IsVisible = false;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(LogEvents.ChangeState, "Post {PostId} state changed to {PostState}", post.Id, post.State);
                }

                return RedirectToAction("Index");
            });
        }

        // Returns the YesNoDialog view
        // Is called from Details.cshtml button Reserve This Item.
        [Route("Posts/YesNoDialog/{postId:int}/{questionText}/{action3}/{buttonAffirmText}")]
        public IActionResult YesNoDialog(int postId, string questionText, string action3, string buttonAffirmText)
        {
            var post = _context.Post
                .FirstOrDefault(m => m.Id == postId);
            if (post == null)
                return null;
            ViewData["Message"] = questionText;
            ViewData["MessageAction"] = action3;
            ViewData["buttonAffirmText"] = buttonAffirmText;
            return View(post);
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
                    act = View("ErrorPages/404Post");
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
                    act = View("ErrorPages/404Post");
                    return act;
                }
            }
            //if check is done by post
            if (post != null)
            {
                if (User.Identity.Name != post.UserId)
                {
                    //return NoAccess();
                    act = View("ErrorPages/404Post");
                    return act;
                }
            }
            //if check is done by postUserId
            if (postUserId != null)
            {
                if (User.Identity.Name != postUserId)
                {
                    //return NoAccess();
                    act = View("ErrorPages/404Post");
                    return act;
                }
            }

            //If user has Permission
            act = null;
            return null;
        }

        private delegate Task<IActionResult> findPostCallback(Post post);
        private async Task<IActionResult> findPostAsync(int postId, findPostCallback cb)
        {
            var post = await _context.Post.FindAsync(postId);
            return await cb(post);
        }

    }
}
