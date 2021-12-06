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
using Geocoding;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;

namespace Karma.Controllers
{
    public class CharitiesController : Controller
    {
        private readonly KarmaContext _context;

        private readonly IWebHostEnvironment _iWebHostEnv;

        private readonly GoogleMaps _googleMaps;

        private List<Charity> _charities;

        // Passes an object of type IWebHostEnvironment that carries information about our host environment.
        public CharitiesController(KarmaContext context, IWebHostEnvironment webHostEnvironment, GoogleMaps googleMaps)
        {
            _context = context;
            _iWebHostEnv = webHostEnvironment;
            _googleMaps = googleMaps;

            _charities = GetCharityList().Result;
        }

        // GET: Charities
        public async Task<IActionResult> Index()
        {
            return View(_charities);
        }

        [HttpGet]
        public IActionResult FilteredCharities(int itemTypeId)
        {
            List<Charity> filtered;
            if (itemTypeId < 0)
            {
                ViewBag.ItemType = null;
                filtered = Charity.GetApprovedCharities(_charities);
            }
            else
            {
                ItemType itemType = new ItemTypes()[itemTypeId];
                ViewBag.ItemType = itemType.Name;
                filtered = Charity.FilteredCharities(_charities, itemType);
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

            var charityAddresses = GetCharityLocales(charity).Result;
            ViewBag.CharityAddresses = charityAddresses;

            return View(charity);
        }

        public async Task<List<Address>> GetCharityLocales(Charity charity)
        {
            List<Address> addresses = new List<Address>();
            List<CharityAddress> charityAddresses = GetCharityAddresses(charity).Result;
            string serviceApiKey = _googleMaps.ServiceApiKey;

            if (serviceApiKey != null)
            {
                ViewBag.ApiKey = serviceApiKey; 
                GoogleGeocoder geocoder = new GoogleGeocoder(serviceApiKey);

                foreach (CharityAddress address in charityAddresses)
                {
                    string fullAddress = address.GetFullAddress();
                    IEnumerable<GoogleAddress> googleAddress = await geocoder.GeocodeAsync(fullAddress);
                    GoogleAddress first = googleAddress.FirstOrDefault();

                    if(first != null)
                        addresses.Add(first);
                }
            }

            return addresses;
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
            if (ModelState.IsValid)
            {
                Charity charity = new();

                if (CopyFileToRoot(charity, file) == false)
                {
                    return View(charity);
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
            charity.DateCreated = DateTime.UtcNow;
            charity.ReviewState = Enums.ReviewState.Waiting;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImagePath,Description,DateCreated,ReviewState")] Charity charity, IFormFile file)
        {
            if (id != charity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // If the edited charity is identical to the old one
                var dbCharity =  _context.Charity.Find(charity.Id);

                if(file == null)
                    charity.ImagePath = dbCharity.ImagePath;
                
                _context.Entry(dbCharity).State = EntityState.Detached;

                if (CopyFileToRoot(charity, file) == false)
                {
                    return View(charity);
                }

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
                return RedirectToAction(nameof(Index), "CharityManager");
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

        public async Task<List<Charity>> GetCharityList()
        {
            List<Charity> charities = await _context.Charity.ToListAsync();

            foreach (var charity in charities)
            {
                charity.CharityItemTypes = GetCharityItemTypes(charity).Result;
                charity.CharityAddresses = GetCharityAddresses(charity).Result;
            }

            return charities;
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
                .OrderBy(m => m.Country)
                .ThenBy(m => m.City)
                .ToListAsync();

            return charityAddressesList;
        }

        // Returns null if there is no file, false if extension is invalid and true otherwise
        private bool? IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            else if (!Path.GetExtension(file.FileName).IsValidExtension())
            {
                return false;
            }
            return true;
        }

        private bool CopyFileToRoot(Charity charity, IFormFile file, string ext = null)
        {
            bool? isValidFile = IsValidFile(file);
            if (isValidFile == false)
            {
                ViewBag.Message = "Invalid file type.";
                return false;
            }
            else if (isValidFile == true)
            {
                ext = ext ?? Path.GetExtension(file.FileName);

                string fileName = "x" + DateTime.Now.Ticks.ToString() + ext;

                string path = Path.Combine(_iWebHostEnv.WebRootPath, Charity.ImagesDirName, fileName);

                // Copying file to wwwroot/CharityImages
                FileStream stream = new FileStream(path, FileMode.Create);
                _ = file.CopyToAsync(stream);

                charity.ImagePath = fileName;
            }
            return true;
        }
    }
}
