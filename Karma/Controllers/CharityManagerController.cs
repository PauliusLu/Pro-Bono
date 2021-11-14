using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Controllers
{
    [Authorize(Roles = "Charity manager")]
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
            var userId = _userManager.GetUserId(User);
            var charity = _userManager.GetCharityByUserRole("Charity manager", userId);

            var charityAddress = new CharityAddress();
            charityAddress.CharityId = charity.Id;
            ViewData["NewCharityAddress"] = charityAddress;

            var charityItemType = new CharityItemType();
            charityItemType.CharityId = charity.Id;
            ViewData["NewCharityItemType"] = charityItemType;

            return View(GetCharity(charity));
        }

        public Charity GetCharity(Charity charity)
        {
            charity.CharityItemTypes = GetCharityItemTypes(charity).Result;
            charity.CharityAddresses = GetCharityAddresses(charity).Result;

            return charity;
        }

        public async Task<List<CharityItemType>> GetCharityItemTypes(Charity charity)
        {
            var charityItemTypesList = await _context.CharityItemType
                .Where(x => x.CharityId == charity.Id)
                .ToListAsync();

            return charityItemTypesList;
        }

        public async Task<List<CharityAddress>> GetCharityAddresses(Charity charity)
        {
            var charityAddressesList = await _context.CharityAddress
                .Where(x => x.CharityId == charity.Id)
                .ToListAsync();

            return charityAddressesList;
        }
    }
}
