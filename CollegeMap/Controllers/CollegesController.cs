using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollegeMap.Data;
using CollegeMap.Models.CollegeMapModels;
using CollegeMap.Models.CollegeMapViewModels;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace CollegeMap.Controllers
{
    public class CollegesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public struct Location
        {
            public float Lat;
            public float Lon;
        }

        public static async Task<Location> GetLocationFromAddress(string address)
        {
            string addressNoWhitespace = Regex.Replace(address, "\\s+", "+");

            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create("https://maps.googleapis.com/maps/api/geocode/json?address="
                + addressNoWhitespace
                + "&key=AIzaSyDZlFiNuQsfssb97q19gLwKWvpdb4ptC-U");

            float lat = 0.0F;
            float lon = 0.0F;

            WebResponse response = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                if (!string.IsNullOrEmpty(result))
                {
                    JSON_Position.Rootobject rootData = JsonConvert.DeserializeObject<JSON_Position.Rootobject>(result);
                    // loop in reverse since deletion of entries affects indices
                    if (rootData.status == "OK")
                    {
                        lat = rootData.results[0].geometry.location.lat;
                        lon = rootData.results[0].geometry.location.lng;
                    }
                }

            }
            Location location = new Location
            {
                Lat = lat,
                Lon = lon
            };
            return location;

        }

        public CollegesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Colleges
        public async Task<IActionResult> Index()
        {
            return View(await _context.Colleges.Include(c => c.Type).Include(c => c.HighestDegreeOffered).ToListAsync());
        }

        // GET: Colleges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var college = await _context.Colleges.Include(c => c.Type).Include(c => c.HighestDegreeOffered)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (college == null)
            {
                return NotFound();
            }

            return View(college);
        }

        // GET: Colleges/Create
        public IActionResult Create()
        {
            IEnumerable<CollegeType> collegeTypes = _context.CollegeTypes.ToList();
            IEnumerable<DegreeType> degreeTypes = _context.DegreeTypes.ToList();
            AddCollegeViewModel addCollegeViewModel = new AddCollegeViewModel(collegeTypes, degreeTypes);
            return View(addCollegeViewModel);
        }

        // POST: Colleges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCollegeViewModel addCollegeViewModel)
        {
            if (ModelState.IsValid)
            {
                CollegeType newCollegeType =
                    _context.CollegeTypes.Single(c => c.ID == addCollegeViewModel.CollegeTypeID);
                DegreeType newDegreeType =
                    _context.DegreeTypes.Single(c => c.ID == addCollegeViewModel.DegreeTypeID);

                Location location = await GetLocationFromAddress(addCollegeViewModel.Address);

                College college = new College
                {
                    Name = addCollegeViewModel.Name,
                    Description = addCollegeViewModel.Description,
                    Enrollment = addCollegeViewModel.Enrollment,
                    AnnualTuition = addCollegeViewModel.AnnualTuition,
                    AnnualRoomAndBoard = addCollegeViewModel.AnnualRoomAndBoard,
                    Website = addCollegeViewModel.Website,
                    Address = addCollegeViewModel.Address,
                    Latitude = location.Lat,
                    Longitude = location.Lon,

                    Type = newCollegeType,
                    HighestDegreeOffered = newDegreeType
                };
                _context.Add(college);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(addCollegeViewModel);
        }

        // GET: Colleges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var college = await _context.Colleges.Include(c => c.Type).Include(c => c.HighestDegreeOffered).SingleOrDefaultAsync(m => m.ID == id);
            if (college == null)
            {
                return NotFound();
            }
            IEnumerable<CollegeType> collegeTypes = _context.CollegeTypes.ToList();
            IEnumerable<DegreeType> degreeTypes = _context.DegreeTypes.ToList();
            AddCollegeViewModel addCollegeViewModel = new AddCollegeViewModel(collegeTypes, degreeTypes);
            addCollegeViewModel.ID = (int)id;
            addCollegeViewModel.Address = college.Address;
            addCollegeViewModel.AnnualRoomAndBoard = college.AnnualRoomAndBoard;
            addCollegeViewModel.AnnualTuition = college.AnnualTuition;
            addCollegeViewModel.CollegeTypeID = college.Type.ID;
            addCollegeViewModel.DegreeTypeID = college.HighestDegreeOffered.ID;
            addCollegeViewModel.Description = college.Description;
            addCollegeViewModel.Enrollment = college.Enrollment;
            addCollegeViewModel.Name = college.Name;
            addCollegeViewModel.Website = college.Website;
            return View(addCollegeViewModel);

        }

        // POST: Colleges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddCollegeViewModel addCollegeViewModel)
        {

                        if (ModelState.IsValid)
                        {
                            try
                            {
                                College college = await _context.Colleges.Include(c => c.Type).Include(c => c.HighestDegreeOffered).SingleOrDefaultAsync(m => m.ID == addCollegeViewModel.ID);
                                if (college == null)
                                {
                                    return NotFound();
                                }
                                CollegeType newCollegeType =
                                    _context.CollegeTypes.Single(c => c.ID == addCollegeViewModel.CollegeTypeID);
                                DegreeType newDegreeType =
                                    _context.DegreeTypes.Single(c => c.ID == addCollegeViewModel.DegreeTypeID);
                                college.Address = addCollegeViewModel.Address;

                                Location location = await GetLocationFromAddress(addCollegeViewModel.Address);
                                college.Latitude = location.Lat;
                                college.Longitude = location.Lon;

                                college.AnnualRoomAndBoard = addCollegeViewModel.AnnualRoomAndBoard;
                                college.AnnualTuition = addCollegeViewModel.AnnualTuition;
                                college.Type = newCollegeType;
                                college.HighestDegreeOffered = newDegreeType;
                                college.Description = addCollegeViewModel.Description;
                                college.Enrollment = addCollegeViewModel.Enrollment;
                                college.Name = addCollegeViewModel.Name;
                                college.Website = addCollegeViewModel.Website;
                                _context.Update(college);
                                await _context.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!CollegeExists(addCollegeViewModel.ID))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            return RedirectToAction("Index");
                        }
                        return View(addCollegeViewModel);
                        
        }

        // GET: Colleges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var college = await _context.Colleges
                .SingleOrDefaultAsync(m => m.ID == id);
            if (college == null)
            {
                return NotFound();
            }

            return View(college);
        }

        // POST: Colleges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var college = await _context.Colleges.SingleOrDefaultAsync(m => m.ID == id);
            _context.Colleges.Remove(college);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Colleges/Maintenance
        public async Task<IActionResult> Maintenance()
        {
            return View();
        }

        private bool CollegeExists(int id)
        {
            return _context.Colleges.Any(e => e.ID == id);
        }
    }
}
