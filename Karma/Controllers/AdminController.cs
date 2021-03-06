using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static Karma.Models.Report;

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

        public IActionResult Index(AdminTabViewModel tabViewModel, int? reportPage = null)
        {
            tabViewModel ??= new AdminTabViewModel(Enums.AdminTab.Users);
            ViewData["reportPage"] = reportPage;
            return View(tabViewModel);
        }

        public IActionResult SwitchToTabs(Karma.Enums.AdminTab adminTab, int? reportPage = null)
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
                case Karma.Enums.AdminTab.ReportReview:
                    tabViewModel.ActiveTab = Enums.AdminTab.ReportReview;
                    break;
                default:
                    tabViewModel.ActiveTab = Enums.AdminTab.Users;
                    break;
            }

            return RedirectToAction(nameof(AdminController.Index), new { ActiveTab = tabViewModel.ActiveTab, reportPage });
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
            var charities = await _context.Charity.ToListAsync();
            ViewBag.charities = charities;

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

            var charityId = selectedRoles.Where(r => r.RoleId == charityManagerRole.Id)
                    .Select(r => r.CharityId).FirstOrDefault();

            if (charityId == -1)
            {
                ModelState.AddModelError("CharityNotSelected", "Charity should be selected to add this user role");
                return View(model);
            }

            if (charityId != 0)
                result = await _userManager.AddUserRoleWithCharityId(user, charityManagerRole.Name, charityId);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to the user");
                return View(model);
            }

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

        [HttpGet]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateReport(int? postId)
        {
            if (postId == null)
            {
                return View();
            }

            var post = await _context.Post.FirstOrDefaultAsync(c => c.Id == postId);

            if (post == null)
            {
                ViewBag.errorMessage = $"Post with Id = {postId} cannot be found";
                return NotFound();
            }

            post.ImagePath = post.GetFullImagePath();

            ViewData["post"] = post;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateReport(int id, [Bind("PostId,ReportMessage")] Report report)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var post = await _context.Post.FirstOrDefaultAsync(c => c.Id == report.PostId);

            report.PostOwnerId = post.UserId;
            report.ReporterId = User.Identity.Name;
            report.ReportState = Report.ReportStates.Open;

            _context.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ThankYou));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ThankYou()
        {
            return View();
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportReview(int? reportId)
        {
            if (reportId == null)
            {
                return View();
            }

            var report = await _context.Report.FirstOrDefaultAsync(c => c.Id == reportId);
            var post = await _context.Post.FirstOrDefaultAsync(c => c.Id == report.PostId);

            if (report == null)
            {
                ViewBag.errorMessage = $"Report with Id = {report} cannot be found";
                return NotFound();
            }

            post.ImagePath = post.GetFullImagePath();

            ViewData["post"] = post;
            return View(report);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportReview(int reportId, string confirm, string reject)
        {
            if (string.IsNullOrEmpty(confirm) && string.IsNullOrEmpty(reject)) return NotFound();
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            bool confirmed = !string.IsNullOrEmpty(confirm);
            
            var oldReport = await _context.Report.FirstOrDefaultAsync(c => c.Id == reportId);

            oldReport.ReportState = confirmed ? ReportStates.Approved : ReportStates.Declined;
            if (confirmed)
                hideReportedPost(oldReport.PostId);

            _context.Update(oldReport);
            await _context.SaveChangesAsync();

            return SwitchToTabs(Enums.AdminTab.ReportReview);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportReview(int reportId)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
            var oldReport = await _context.Report.FirstOrDefaultAsync(c => c.Id == reportId);

            _context.Remove(oldReport);
            await _context.SaveChangesAsync();

            return SwitchToTabs(Enums.AdminTab.ReportReview);
        }


        private void hideReportedPost(int id)
        {
            var post = _context.Post.FirstOrDefault(c => c.Id == id);
            post.State = (int)Post.PostState.Hidden;
            post.IsVisible = false;

            var allSimilarReports = _context.Report.Where(a => a.PostId == id && a.ReportState == ReportStates.Open).ToListAsync();
            allSimilarReports.Result.ForEach(a => a.ReportState = ReportStates.Approved);
            _context.Report.UpdateRange(allSimilarReports.Result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewReports(int? page, string sorting, string? aggr)
        {
            const int ReportsPerPage = 6;
            ReportDataModel reportsDM = GetFilteredReports(page, sorting, aggr, ReportsPerPage, ViewData);
            return View(reportsDM);
        }

        private ReportDataModel GetFilteredReports(int? page, string sorting, string aggr, int reportsPerPage, ViewDataDictionary viewData)
        {
            var reports = _context.Report.ToList();
            var posts = _context.Post.ToList();
            bool filter = false;

            ReportStates rptState = ReportStates.Open;

            if (sorting != null)
            {
                filter = true;
                viewData["filter"] = true;
                rptState = (ReportStates)Enum.Parse(typeof(ReportStates), sorting);
            }

            var reportsDM = new ReportDataModel
            {
                page = page,
                sorting = sorting,
                aggr = aggr
            };

            if (aggr == "on")
            {
                reportsDM.Reports = GetAggregatedReports(reports, posts, reportsPerPage, rptState, filter, page, viewData);
            }
            else
            {
                reportsDM.Reports = GetPagedReports(reports, posts, reportsPerPage, rptState, filter, page, viewData);
            }

            return reportsDM;
        }

        private List<Report> GetPagedReports(List<Report> reports, List<Post> posts, int reportsPerPage, ReportStates rptState, bool filter, int? page, ViewDataDictionary viewData)
        {
            //Paged reports
            var pgdReports = (from report in reports
                              join pst in posts
                              on report.PostId equals pst.Id
                              where filter == false || report.ReportState == rptState
                              select new
                              {
                                  post = pst,
                                  report = report
                              })
                              .Skip(reportsPerPage * ((page ?? 1) - 1))
                              .Take(reportsPerPage);

            ViewData["reportsAndPosts"] = pgdReports.Select(m => m).ToList();
            return pgdReports.Select(m => m.report).ToList();
        }

        private List<Report> GetAggregatedReports(List<Report> reports, List<Post> posts, int reportsPerPage, ReportStates rptState, bool filter, int? page, ViewDataDictionary viewData)
        {
            //Grouped reports
            var grpReports = (from report in reports
                              where filter == false || report.ReportState == rptState
                              group report by report.PostId)
                                .Skip(reportsPerPage * ((page ?? 1) - 1))
                                .Take(reportsPerPage);

            //Aggregated reports
            grpReports.ForEach((group) =>
            {
                group.First().ReportMessage = group.Select(a => a.ReportMessage)
                     .Aggregate((old, next) => { return old + "\n" + next; });
            });

            ViewData["grpReports"] = grpReports.Select(m => m).ToList();
            ViewData["aggr"] = true;
            var reportsAndPosts = grpReports.Select(m => m.First())
                                            .ToList()
                                            .Join(posts,
                                                  m => m.PostId,
                                                  p => p.Id,
                                                  (m, p) => { 
                                                      return new { post = p, report = m }; 
                                                  });
            ViewData["reportsAndPosts"] = reportsAndPosts.Select(m => m)
                                                         .ToList();
            return reportsAndPosts.Select(m => m.report).ToList();
        }

    }
}
