using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karma.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Karma.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Karma.Areas.Identity.Pages.Account.Manage
{
    public class CreateCharity : PageModel
    {
        private readonly UserManage _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly KarmaContext _context;
        private readonly IWebHostEnvironment _iWebHostEnv;

        public string Username { get; set; }

        public CreateCharity(UserManage userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager, 
            KarmaContext context, 
            IWebHostEnvironment iWebHostEnv)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _iWebHostEnv = iWebHostEnv;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            public string ImagePath { get; set; }
    }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            Input = new InputModel();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            var charity = new Charity();
            charity.Name = Input.Name;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var dbCharity = await _context.Charity.FirstOrDefaultAsync(m => m.Name == charity.Name);
            if (dbCharity != null)
            {
                StatusMessage = "Error: Charity with this name has already been created.";
                return RedirectToPage();
            }

            if (await _userManager.IsInRoleAsync(user, "Charity manager"))
            {
                StatusMessage = "Error: You cannot create a charity. Your user is already a manager of some charity.";
                return RedirectToPage();
            }

            FillCharityDetails(charity);

            if (CopyFileToRoot(charity, file) == false)
            {
                return RedirectToPage();
            }

            _context.Add(charity);
            await _context.SaveChangesAsync();

            var newCharity = await _context.Charity.FirstOrDefaultAsync(c => c.Name == charity.Name);
            var charityId = newCharity.Id;

            await _userManager.AddUserRoleWithCharityId(user, "Charity manager", charityId);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "You have succesfully created a charity. The charity will now be in review.";
            return RedirectToPage();
        }

        private void FillCharityDetails(Charity charity)
        {
            charity.Description = Input.Description;
            charity.Name = Input.Name;
            charity.DateCreated = DateTime.UtcNow;
            charity.ReviewState = Enums.ReviewState.Waiting;
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

        private bool CopyFileToRoot(Charity charity, IFormFile file, string ext = null)
        {
            bool? isValidFile = IsValidFile(file);
            if (isValidFile == false)
            {
                StatusMessage = "Invalid file type";
                return false;
            }
            else if (isValidFile == true)
            {
                ext = ext ?? Path.GetExtension(file.FileName);

                string fileName = "x" + DateTime.Now.Ticks.ToString() + ext;

                string path = Path.Combine(_iWebHostEnv.WebRootPath, Charity.ImagesDirName, fileName);

                // Copying file to wwwroot/CharityImages
                FileStream stream = new FileStream(path, FileMode.Create);
                _ = file.CopyToAsync(stream);

                charity.ImagePath = fileName;
            }
            return true;
        }
    }
}
