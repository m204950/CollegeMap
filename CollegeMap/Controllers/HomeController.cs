using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CollegeMap.Data;
using Microsoft.EntityFrameworkCore;
using CollegeMap.Models.CollegeMapViewModels;
using CollegeMap.Models.CollegeMapModels;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static CollegeMap.Controllers.CollegesController;

namespace CollegeMap.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            /* query components
             * text that is contained in College Name
             * minimum degree offered - so must be <= highest degree offered
             * equals selected type
             * minimum enrollment context.Persons.Max(p => p.Age)
             * maximum enrollment
             * maximum tuition + R&B
            */
            IEnumerable<CollegeType> collegeTypes = _context.CollegeTypes.ToList();
            IEnumerable<DegreeType> degreeTypes = _context.DegreeTypes.ToList();
            QueryCollegeViewModel queryCollegeViewModel = new QueryCollegeViewModel(collegeTypes, degreeTypes);
            // set default minimum enrollment to minimum value in DB
            queryCollegeViewModel.MinimumEnrollment = _context.Colleges.Min(c => c.Enrollment);
            // set default maximum enrollment to minimum value in DB
            queryCollegeViewModel.MaximumEnrollment = _context.Colleges.Max(c => c.Enrollment);
            queryCollegeViewModel.MaxTotalCost = _context.Colleges.Max(c => c.AnnualTuition + c.AnnualRoomAndBoard);
            return View(queryCollegeViewModel);
        }

        // POST: /Query
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Query(QueryCollegeViewModel queryCollegeViewModel)
        {
            if (ModelState.IsValid)
            {
                string homeAddress = queryCollegeViewModel.HomeAddress;
                Location location = await CollegesController.GetLocationFromAddress(homeAddress);
                queryCollegeViewModel.HomeLatitude = location.Lat;
                queryCollegeViewModel.HomeLongitude = location.Lon;

                int maxTravel = queryCollegeViewModel.MaxTravel;  
                int minEnrollment = queryCollegeViewModel.MinimumEnrollment;
                int maxEnrollment = queryCollegeViewModel.MaximumEnrollment;
                int maxTotalCost = queryCollegeViewModel.MaxTotalCost;
                int desiredDegreeLevelID = queryCollegeViewModel.DegreeTypeID;
                IEnumerable<int> collegeTypeIDs = queryCollegeViewModel.CollegeTypeIDs;
                string nameContains;
                if (queryCollegeViewModel.NameContains == null)
                {
                    nameContains = "";
                }
                else
                {
                    nameContains = queryCollegeViewModel.NameContains;
                }
                queryCollegeViewModel.Colleges = await _context.Colleges.
                    Where(c => c.Enrollment <= maxEnrollment).Where(c => c.Enrollment >= minEnrollment).
                    Where(c => c.AnnualTuition + c.AnnualRoomAndBoard <= maxTotalCost).
                    Where(c => c.Name.ToUpper().Contains(nameContains.ToUpper())).
                    // following relies on degree types having ascending IDs corresponding to their level
                    Where(c => c.HighestDegreeOffered.ID >= desiredDegreeLevelID).
                    Where(c => collegeTypeIDs.Contains(c.Type.ID)).OrderBy(c => c.Name).
                    Include(c => c.Type).Include(c => c.HighestDegreeOffered).ToListAsync();

                //Pass request to google api with orgin and destination details
                string origins = "&origins=" + Regex.Replace(homeAddress, "\\s+", "+");
                string destinations = "&destinations=";

                foreach (College college in queryCollegeViewModel.Colleges)
                {
                    destinations += college.Address + "|";
                }
                destinations = Regex.Replace(destinations, "\\s+", "+");

                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create("https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial"
                    + origins + destinations
                    + "&mode=Car&language=us-en&sensor=false&key=AIzaSyDZlFiNuQsfssb97q19gLwKWvpdb4ptC-U");


                WebResponse response = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    if (!string.IsNullOrEmpty(result))
                    {
                        JSON_Distance.Rootobject rootData = JsonConvert.DeserializeObject<JSON_Distance.Rootobject>(result);
                        // loop in reverse since deletion of entries affects indices
                        for (int i = queryCollegeViewModel.Colleges.Count - 1; i >= 0 ; i--)
                        {
                            //int collegeDistance = rootData.rows[0].elements[i].distance.value;
                            string collegeDistanceStr = rootData.rows[0].elements[i].distance.text.Replace("mi", "");
                            collegeDistanceStr = collegeDistanceStr.Replace("ft", "");
                            int collegeDistance = (int)Math.Round(float.Parse(collegeDistanceStr), 0);
                            if (collegeDistance > maxTravel)
                            {
                                queryCollegeViewModel.Colleges.RemoveAt(i);
                            }
                            else
                            {
                                queryCollegeViewModel.Colleges[i].Distance = collegeDistance;
                            }

                        }
                    }

                }
            }

            IEnumerable<CollegeType> collegeTypes = _context.CollegeTypes.ToList();
            IEnumerable<DegreeType> degreeTypes = _context.DegreeTypes.ToList();
            queryCollegeViewModel.CreateCollegeTypeSelects(collegeTypes);
            queryCollegeViewModel.CreateDegreeTypeSelects(degreeTypes);
            return View("Index", queryCollegeViewModel);
        }
        public IActionResult About()
        {
            if (User.Identity.IsAuthenticated)
            {

               ViewData["Message"] = "Your application description page.";
            }
            else
            {
                ViewData["Message"] = "You are not logged in.";
            }
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
