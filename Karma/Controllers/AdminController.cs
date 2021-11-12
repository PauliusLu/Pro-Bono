﻿using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Karma.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly KarmaContext _context;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManage _userManager;

        private readonly IWebHostEnvironment _iWebHostEnv;

        public AdminController(KarmaContext context, RoleManager<IdentityRole> roleManager, UserManage userManager, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _iWebHostEnv = webHostEnv;
        }

        public IActionResult Index(AdminTabViewModel tabViewModel)
        {
            tabViewModel ??= new AdminTabViewModel(Enums.AdminTab.Users);
            
            return View(tabViewModel);
        }

        public IActionResult SwitchToTabs(Karma.Enums.AdminTab adminTab)
        {
            var tabViewModel = new AdminTabViewModel();

            switch(adminTab)
        {
                case Karma.Enums.AdminTab.Users:
                    tabViewModel.ActiveTab = Enums.AdminTab.Users;
                    break;
                case Karma.Enums.AdminTab.CharityReview:
                    tabViewModel.ActiveTab = Enums.AdminTab.CharityReview;
                    break;
                default:
                    tabViewModel.ActiveTab = Enums.AdminTab.Users;
                    break;
            }

            return RedirectToAction(nameof(AdminController.Index), tabViewModel);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role.Name);

            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role.Name));
            }

            return View();
        }

        public async Task<IActionResult> UserList()
        {
            return View(await _context.User.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.errorMessage = $"User with Id = {userId} cannot be found";
                return NotFound();
            }

            ViewBag.username = user.UserName;
            var model = new List<UserRolesViewModel>();

            foreach(var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    CharityId = 0
                };
                
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            var charities = await _context.Charity.ToListAsync();
            ViewBag.charities = charities;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.errorMessage = $"User with Id = {userId} cannot be found";
                return NotFound();
            }

            ViewBag.username = user.UserName;
            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove existing user roles");
                return View(model);
            }

            var charityManagerRole = await _roleManager.FindByNameAsync("Charity manager");
            var selectedRoles = model.Where(r => r.IsSelected);

            result = await _userManager.AddToRolesAsync(user,
                selectedRoles.Where(r => r.RoleId != charityManagerRole.Id)
                    .Select(r => r.RoleName));

            result = await _userManager.AddUserRoleWithCharityId(user, charityManagerRole.Name, 
                selectedRoles.Where(r => r.RoleId == charityManagerRole.Id)
                    .Select(r => r.CharityId).FirstOrDefault());

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to the user");
                return View(model);
            }

            var charities = await _context.Charity.ToListAsync();
            ViewBag.charities = charities;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CharityReview(int? charityId)
        {
            if (charityId == null)
            {
                return View();
            }

            var charity = await _context.Charity.FirstOrDefaultAsync(c => c.Id == charityId);

            var user = _userManager.GetUserByCharityId("Charity manager", (int)charityId);
            ViewBag.User = user;

            if (charity == null)
            {
                ViewBag.errorMessage = $"Charity with Id = {charityId} cannot be found";
                return NotFound();
            }

            return View(charity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CharityReview(int id, [Bind("Id,ReviewState")] Charity charity)
        {
            var dbCharity = await _context.Charity.FirstOrDefaultAsync(c => c.Id == id);
            dbCharity.CharityStateChanged += CharityStateChangedEventHandler;

            dbCharity.ReviewState = charity.ReviewState;

            _context.Update(dbCharity);
            await _context.SaveChangesAsync();

            return View(dbCharity);
        }

        public async void CharityStateChangedEventHandler(object sender, CharityStateChangedEventArgs e)
        {
            var user = _userManager.GetUserByCharityId("Charity manager", e.CharityId);
            var charity = await _context.Charity.FindAsync(e.CharityId);

            if (user != null)
            {
                var emailModel = new EmailModel.EmailCharityState(user.UserName, charity.Name, e.ReviewState, e.TimeChanged);
                await emailModel.SendEmail(_iWebHostEnv, user);
            }
        }
    }
}
