using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models;

namespace Karma
{
    public class CharityAddressesController : Controller
    {
        private readonly KarmaContext _context;

        public CharityAddressesController(KarmaContext context)
        {
            _context = context;
        }

        // GET: CharityAddresses
        public async Task<IActionResult> Index()
        {
            return View(await _context.CharityAddress.ToListAsync());
        }

        // GET: CharityAddresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityAddress = await _context.CharityAddress
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charityAddress == null)
            {
                return NotFound();
            }

            return View(charityAddress);
        }

        // GET: CharityAddresses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CharityAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CharityId,Country,City,Street,HouseNumber,PostCode")] CharityAddress charityAddress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(charityAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","CharityManager");
            }
            return View(charityAddress);
        }

        // GET: CharityAddresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityAddress = await _context.CharityAddress.FindAsync(id);
            if (charityAddress == null)
            {
                return NotFound();
            }
            return View(charityAddress);
        }

        // POST: CharityAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CharityId,Country,City,Street,HouseNumber,PostCode")] CharityAddress charityAddress)
        {
            if (id != charityAddress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charityAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharityAddressExists(charityAddress.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "CharityManager");
            }
            return View(charityAddress);
        }

        // GET: CharityAddresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityAddress = await _context.CharityAddress
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charityAddress == null)
            {
                return NotFound();
            }

            return View(charityAddress);
        }

        // POST: CharityAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charityAddress = await _context.CharityAddress.FindAsync(id);
            _context.CharityAddress.Remove(charityAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "CharityManager");
        }

        private bool CharityAddressExists(int id)
        {
            return _context.CharityAddress.Any(e => e.Id == id);
        }
    }
}
