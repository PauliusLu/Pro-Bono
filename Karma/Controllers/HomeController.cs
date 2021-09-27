using Karma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Redirecting the primary page to 'All posts'
            return RedirectToAction("Index", "Posts");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Returns the DonateDialog view
        // Is called from _Layout.cshtml button #donateDialog
        public IActionResult DonateDialog()
        {
            return View();
        }

        // Returns the RequestDialog view
        // Is called from _Layout.cshtml button #requestDialog
        public IActionResult RequestDialog()
        {
            return View();
        }
    }
}
