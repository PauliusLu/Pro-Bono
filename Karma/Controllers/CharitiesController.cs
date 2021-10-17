using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Karma.Data;
using Karma.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Karma.Controllers
{
    public class CharitiesController : Controller
    {
        private readonly KarmaContext _context;

        private readonly IWebHostEnvironment _iWebHostEnv;

        // Passes an object of type IWebHostEnvironment that carries information about our host environment.
        public CharitiesController(KarmaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _iWebHostEnv = webHostEnvironment;
        }

        // GET: Charities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Charity.ToListAsync());
        }
        public async Task<IActionResult> FilteredCharities(int itemTypeId)
        {
            List<Charity> charities = await _context.Charity.ToListAsync();

            List<Charity> filtered = new List<Charity>();
            ItemType itemType = ItemType.GetItemType(itemTypeId);

            ViewBag.ItemType = itemType.Name;

            if (charities != null)
            {
                foreach (Charity c in charities)
                {
                    c.LoadItemTypes();
                    if (c.ItemTypes.Contains(itemType))
                    {
                        filtered.Add(c);
                    }
                }
            }   
            return View(filtered);
        }

        // GET: Charities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charity = await _context.Charity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charity == null)
            {
                return NotFound();
            }

            return View(charity);
        }

        // GET: Charities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Charities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,ItemTypes,ImagePath,Description")] CreateCharityViewModel charityForm, IFormFile file)
        {
            Console.WriteLine("Was here");
            if (ModelState.IsValid)
            {
                Console.WriteLine("Here tii");
                Charity charity = new();

                if (file != null && file.Length != 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    if (!ext.IsValidExtension())
                    {
                        ViewBag.Message = "Invalid file type.";
                        return View(charityForm);
                    }

                    // Generating a file name.
                    string fileName = "x" + DateTime.Now.Ticks.ToString() + ext;

                    string path = Path.Combine(_iWebHostEnv.WebRootPath, Charity.ImagesDirName, fileName);
                    FileStream stream = new FileStream(path, FileMode.Create);
                    _ = file.CopyToAsync(stream);

                    charity.ImagePath = fileName;               
                }

                FillCharityDetails(charity, charityForm);
                _context.Add(charity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(charityForm);
        }

        private void FillCharityDetails(Charity charity, CreateCharityViewModel charityForm)
        {
            charity.Description = charityForm.Description;
            charity.Name = charityForm.Name;
            charity.ItemTypePath = charityForm.CreateFilePath(_iWebHostEnv, charity.Name, Charity.ItemTypesDirName);
            charity.AddressesPath = charityForm.CreateFilePath(_iWebHostEnv, charity.Name, Charity.AdressDirName);
        }

        // GET: Charities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charity = await _context.Charity.FindAsync(id);
            if (charity == null)
            {
                return NotFound();
            }
            return View(charity);
        }

        // POST: Charities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AddressesPath,ItemTypePath,ImagePath,Description")] Charity charity)
        {
            if (id != charity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharityExists(charity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(charity);
        }

        // GET: Charities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charity = await _context.Charity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charity == null)
            {
                return NotFound();
            }

            return View(charity);
        }

        // POST: Charities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charity = await _context.Charity.FindAsync(id);
            _context.Charity.Remove(charity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharityExists(int id)
        {
            return _context.Charity.Any(e => e.Id == id);
        }
    }
}
