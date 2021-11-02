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
            [Required]
            public List<int> ItemTypes { get; set; }
            [Required]
            public string Address { get; set; }
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

        public async Task<IActionResult> OnPostAsync()
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
            
            FillCharityDetails(charity);
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
            charity.ItemTypePath = CreateFilePath(_iWebHostEnv, charity.Name, Charity.ItemTypesDirName);
            charity.AddressesPath = CreateFilePath(_iWebHostEnv, charity.Name, Charity.AdressDirName);
            charity.DateCreated = DateTime.UtcNow;
            charity.ReviewState = Enums.ReviewState.Waiting;
        }

        public string CreateFilePath(IWebHostEnvironment webHostEnvironment, string fileName, string dir)
        {
            StringBuilder content = new();

            if (dir == Charity.ItemTypesDirName)
            {
                foreach (int e in Input.ItemTypes)
                {
                    content.AppendLine(Karma.Models.ItemTypes.GetItemType(e).Name);
                }
            }
            else if (dir == Charity.AdressDirName)
            {
                content.AppendLine(Input.Address);
            }
            else
            {
                return "";
            }


            string relativePath = Path.Combine(dir, fileName + ".txt");
            string path = Path.Combine(webHostEnvironment.ContentRootPath, relativePath);

            System.IO.File.WriteAllText(path, content.ToString());

            return relativePath;
        }
    }
}
