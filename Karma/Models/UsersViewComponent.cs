using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class UsersViewComponent : ViewComponent
    {
        private readonly UserManager<Karma.Models.User> _userManager;

        public UsersViewComponent(UserManager<Karma.Models.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }
    }
}
