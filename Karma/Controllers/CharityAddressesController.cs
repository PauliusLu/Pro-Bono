using Karma.Data;
using Karma.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Controllers
{
    public class CharityAddressesController : Controller
    {
        private readonly KarmaContext _context;

        public CharityAddressesController(KarmaContext context)
        {
            _context = context;
        }

        // GET: CharityAddressesController
        public async Task<IActionResult> Index()
        {
            return View(await _context.CharityAddress.ToListAsync());
        }

        // GET: CharityAddressesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CharityAddressesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CharityAddressesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CharityId,Country,City,Street,HouseNumber,PostCode")] CharityAddress charityAddress, IFormCollection collection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(charityAddress);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(charityAddress);
        }

        // GET: CharityAddressesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CharityAddressesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CharityAddressesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CharityAddressesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
