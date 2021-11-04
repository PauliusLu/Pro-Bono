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
    public class CharityManagerController : Controller
    {
        private readonly KarmaContext _context;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManage _userManager;

        public CharityManagerController(KarmaContext context, RoleManager<IdentityRole> roleManager, UserManage userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
