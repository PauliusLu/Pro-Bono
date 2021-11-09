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
    public class CharityItemTypesController : Controller
    {
        private readonly KarmaContext _context;

        public CharityItemTypesController(KarmaContext context)
        {
            _context = context;
        }

        // GET: CharityItemTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CharityItemType.ToListAsync());
        }

        // GET: CharityItemTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityItemType = await _context.CharityItemType
                .FirstOrDefaultAsync(m => m.CharityId == id);
            if (charityItemType == null)
            {
                return NotFound();
            }

            return View(charityItemType);
        }

        // GET: CharityItemTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CharityItemTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CharityId,ItemTypeId")] CharityItemType charityItemType)
        {
            var charityItemTypeExists = await _context.CharityItemType
                .Where(m => m.CharityId == charityItemType.CharityId 
                    && m.ItemTypeId == charityItemType.ItemType.Id)
                .FirstOrDefaultAsync();

            if (charityItemTypeExists != null)
            {
                ModelState.AddModelError("charityItemType.ItemTypeId", "This category has already been added");
                return View(charityItemType);
            }

            if (ModelState.IsValid)
            {
                _context.Add(charityItemType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "CharityManager");
            }
            return View(charityItemType);
        }

        // GET: CharityItemTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityItemType = await _context.CharityItemType.FindAsync(id);
            if (charityItemType == null)
            {
                return NotFound();
            }
            return View(charityItemType);
        }

        // POST: CharityItemTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CharityId,ItemTypeId")] CharityItemType charityItemType)
        {
            if (id != charityItemType.CharityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charityItemType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharityItemTypeExists(charityItemType.CharityId))
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
            return View(charityItemType);
        }

        // GET: CharityItemTypes/Delete/5
        public async Task<IActionResult> Delete(int? charityId, int? itemTypeId)
        {
            if (charityId == null || itemTypeId == null)
            {
                return NotFound();
            }

            var charityItemType = await _context.CharityItemType
                .FirstOrDefaultAsync(m => m.CharityId == charityId && m.ItemTypeId == itemTypeId);
            if (charityItemType == null)
            {
                return NotFound();
            }

            return View(charityItemType);
        }

        // POST: CharityItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int charityId, int itemTypeId)
        {
            var charityItemType = await _context.CharityItemType
                .FirstOrDefaultAsync(m => m.CharityId == charityId && m.ItemTypeId == itemTypeId);
            _context.CharityItemType.Remove(charityItemType);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "CharityManager");
        }

        private bool CharityItemTypeExists(int id)
        {
            return _context.CharityItemType.Any(e => e.CharityId == id);
        }


    }
}
