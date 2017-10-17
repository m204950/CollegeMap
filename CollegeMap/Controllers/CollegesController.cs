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
using CsvHelper;
using X.PagedList;

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
        public async Task<IActionResult> Index(int? page, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            int pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            IEnumerable<College> colleges = await _context.Colleges.
                Include(c => c.Type).Include(c => c.HighestDegreeOffered).
                OrderBy(c => c.Name).
                ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                colleges = colleges.Where(c => c.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            var onePageOfColleges = colleges.ToPagedList(pageNumber, 25);
            return View(onePageOfColleges);
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
                    CollegeScorecardID = addCollegeViewModel.CollegeScorecardID,
                    Enrollment = addCollegeViewModel.Enrollment,
                    AnnualTuition = addCollegeViewModel.AnnualTuition,
                    AnnualTuitionOut = addCollegeViewModel.AnnualTuitionOut,
                    AcceptanceRate = addCollegeViewModel.AcceptanceRate,
                    AvgNetPrice = addCollegeViewModel.AvgNetPrice,
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
            addCollegeViewModel.CollegeScorecardID = college.CollegeScorecardID;
            addCollegeViewModel.Address = college.Address;
            addCollegeViewModel.State = college.State;
            addCollegeViewModel.AnnualTuition = college.AnnualTuition;
            addCollegeViewModel.AnnualTuitionOut = college.AnnualTuitionOut;
            addCollegeViewModel.AcceptanceRate = college.AcceptanceRate;
            addCollegeViewModel.AvgNetPrice = college.AvgNetPrice;
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
                                college.State = addCollegeViewModel.State;
                                college.Address = addCollegeViewModel.Address;

                                Location location = await GetLocationFromAddress(addCollegeViewModel.Address);
                                college.Latitude = location.Lat;
                                college.Longitude = location.Lon;

                                college.CollegeScorecardID = addCollegeViewModel.CollegeScorecardID;

                                college.AnnualTuition = addCollegeViewModel.AnnualTuition;
                                college.AnnualTuitionOut = addCollegeViewModel.AnnualTuitionOut;
                                college.AcceptanceRate = addCollegeViewModel.AcceptanceRate;
                                college.AvgNetPrice = addCollegeViewModel.AvgNetPrice;
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

        // GET: Colleges/Import
        public async Task<IActionResult> Import(string source, string version)
        {
            ViewData["ImportMessage"] = "Import Fail";
            if (source == null)
            {
                source = "No source specified";
            }
            if (version == null)
            {
                version = "No version specified";
            }
            ImportSource importSource = new ImportSource
            {
                Source = source,
                Version = version,
                ImportTime = DateTime.Now
            };
            _context.Add(importSource);
            await _context.SaveChangesAsync();

            string path = @"C:\Users\m204950\Downloads\EssentialCollegeData.csv";
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                {

                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        var reader = new CsvReader(sr);

                        //CSVReader will now read the whole file into an enumerable
                        IEnumerable<Csv_CollegeImport> records = reader.GetRecords<Csv_CollegeImport>();
                        //int count = records.Count();
                        foreach (Csv_CollegeImport entry in records)
                        {
                            // only use if some cost information is available
                            if (entry.TUITIONFEE_IN != -1 || entry.TUITIONFEE_OUT != -1 || entry.NetPrice != -1)
                            {
                                DegreeType highDeg = ConvertImportedDegreeType(entry.HIGHDEG);
                                CollegeType collegeType = ConvertImportedCollegeType(entry.CONTROL);
                                College college = new College
                                {
                                    Name = entry.INSTNM,
                                    CollegeScorecardID = entry.UNITID,
                                    Enrollment = entry.UGDS,
                                    AnnualTuition = entry.TUITIONFEE_IN,
                                    AnnualTuitionOut = entry.TUITIONFEE_OUT,
                                    AvgNetPrice = entry.NetPrice,
                                    AcceptanceRate = entry.ADM_RATE,
                                    Website = "http://" + entry.INSTURL,
                                    Address = entry.CITY + ", " + entry.STABBR + " " + entry.ZIP,
                                    Latitude = entry.LATITUDE,
                                    Longitude = entry.LONGITUDE,
                                    State = entry.STABBR,

                                    Type = collegeType,
                                    HighestDegreeOffered = highDeg
                                };
                                _context.Add(college);
                            }

                        }
                        await _context.SaveChangesAsync();
                        ViewData["ImportMessage"] = "Import Success";

                    }
                }
            //           return RedirectToAction("Maintenance",);
            return View();
        }

        private DegreeType ConvertImportedDegreeType(int highDeg)
        {
            DegreeType degreeType;
            switch (highDeg)
            {
                case 0:
                    degreeType = _context.DegreeTypes.Single(c => c.Name == "Non-degree-granting");
                    break;
                case 1:
                    degreeType = _context.DegreeTypes.Single(c => c.Name == "Certificate");
                    break;
                case 2:
                    degreeType = _context.DegreeTypes.Single(c => c.Name == "Associate");
                    break;
                case 3:
                    degreeType = _context.DegreeTypes.Single(c => c.Name == "Bachelors");
                    break;
                case 4:
                    degreeType = _context.DegreeTypes.Single(c => c.Name == "Graduate");
                    break;
                default:
                    degreeType = _context.DegreeTypes.Single(c => c.Name == "Non-degree-granting");
                    break;
            }
            return degreeType;

        }

        private CollegeType ConvertImportedCollegeType(int importedCollegeType)
        {
            CollegeType collegeType;
            switch (importedCollegeType)
            {
                case 1:
                    collegeType = _context.CollegeTypes.Single(c => c.Name == "Public");
                    break;
                case 2:
                    collegeType = _context.CollegeTypes.Single(c => c.Name == "Private nonprofit");
                    break;
                case 3:
                    collegeType = _context.CollegeTypes.Single(c => c.Name == "Private for-profit");
                    break;
                default:
                    collegeType = _context.CollegeTypes.Single(c => c.Name == "Unknown");
                    break;
            }
            return collegeType;

        }


        private bool CollegeExists(int id)
        {
            return _context.Colleges.Any(e => e.ID == id);
        }
    }
}
