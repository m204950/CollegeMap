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
            IEnumerable<ImportSource> importSources = _context.ImportSources.ToList();
            IEnumerable<String> states = _context.Colleges.OrderBy(c => c.State).Select(c => c.State).Distinct().ToList();

            ImportSource latestImport = importSources.OrderByDescending(i => i.ImportTime).FirstOrDefault();
 
            QueryCollegeViewModel queryCollegeViewModel = new QueryCollegeViewModel(collegeTypes, degreeTypes, states);
            queryCollegeViewModel.CollegeDataProvider = latestImport.Source;
            queryCollegeViewModel.CollegeDataVersion = latestImport.Version;

            // set default minimum enrollment to minimum value in DB
            queryCollegeViewModel.MinimumEnrollment = _context.Colleges.Where(c => c.Enrollment > 0).Min(c => c.Enrollment);
            // set default maximum enrollment to minimum value in DB
            queryCollegeViewModel.MaximumEnrollment = _context.Colleges.Max(c => c.Enrollment);
            queryCollegeViewModel.MaxTotalCost = _context.Colleges.Max(c => c.AvgNetPrice);
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
                int maxTravel = 100000; // set default max travel big in case not specified
                if (queryCollegeViewModel.MaxTravel != null)
                {
                    maxTravel = (int) queryCollegeViewModel.MaxTravel;
                }
                string homeAddress = queryCollegeViewModel.HomeAddress;
                Location homeLocation = await CollegesController.GetLocationFromAddress(homeAddress);
                queryCollegeViewModel.HomeLatitude = homeLocation.Lat;
                queryCollegeViewModel.HomeLongitude = homeLocation.Lon;

                // determine lat and lon limits for a distance box around "home"
                double milesPerLatDeg = 68.7;
                double maxLat = homeLocation.Lat + maxTravel / milesPerLatDeg;
                double minLat = homeLocation.Lat - maxTravel / milesPerLatDeg;
                // assuming northern hemisphere, use northern-most lat to compute miles per lon deg 
                double milesPerLonDeg = 69.172 * Math.Cos(maxLat * Math.PI / 180);
 
                double maxLon = homeLocation.Lon + maxTravel / milesPerLonDeg;
                double minLon = homeLocation.Lon - maxTravel / milesPerLonDeg;

                int minEnrollment = queryCollegeViewModel.MinimumEnrollment;
                int maxEnrollment = queryCollegeViewModel.MaximumEnrollment;
                int maxTotalCost = queryCollegeViewModel.MaxTotalCost;
                int desiredDegreeLevelID = queryCollegeViewModel.DegreeTypeID;
                IEnumerable<int> collegeTypeIDs = queryCollegeViewModel.CollegeTypeIDs;
                IEnumerable<string> selectedStates = queryCollegeViewModel.StateIDs;

                string nameContains;
                if (queryCollegeViewModel.NameContains == null)
                {
                    nameContains = "";
                }
                else
                {
                    nameContains = queryCollegeViewModel.NameContains;
                }

                string addressContains;
                if (queryCollegeViewModel.AddressContains == null)
                {
                    addressContains = "";
                }
                else
                {
                    addressContains = queryCollegeViewModel.AddressContains;
                }

                IEnumerable<ImportSource> importSources = _context.ImportSources.ToList();
                ImportSource latestImport = importSources.OrderByDescending(i => i.ImportTime).FirstOrDefault();
                queryCollegeViewModel.CollegeDataProvider = latestImport.Source;
                queryCollegeViewModel.CollegeDataVersion = latestImport.Version;

                queryCollegeViewModel.Colleges = await _context.Colleges.
                    Where(c => c.Latitude <= maxLat).Where(c => c.Latitude >= minLat).
                    Where(c => c.Longitude <= maxLon).Where(c => c.Longitude >= minLon).
                    Where(c => c.Enrollment <= maxEnrollment).Where(c => c.Enrollment >= minEnrollment).
                    Where(c => c.AvgNetPrice  <= maxTotalCost).
                    Where(c => c.Name.ToUpper().Contains(nameContains.ToUpper())).
                    Where(c => c.Address.ToUpper().Contains(addressContains.ToUpper())).
                    // following relies on degree types having ascending IDs corresponding to their level
                    Where(c => c.HighestDegreeOffered.ID >= desiredDegreeLevelID).
                    Where(c => collegeTypeIDs.Contains(c.Type.ID)).
                    Where(c => selectedStates.Contains(c.State)).
                    OrderBy(c => c.Name).
                    Include(c => c.Type).Include(c => c.HighestDegreeOffered).
 //                   OrderBy(c => c.Distance).
                    ToListAsync();

                // loop in reverse since deletion of entries affects indices
                for (int i = queryCollegeViewModel.Colleges.Count - 1; i >= 0 ; i--)
                {
                    College thisCollege = queryCollegeViewModel.Colleges[i];
                    int collegeDistance = (int)Math.Round(Distance(thisCollege.Latitude, thisCollege.Longitude, homeLocation.Lat, homeLocation.Lon, 'M'), 0);
                    if (collegeDistance > maxTravel)
                    {
                        queryCollegeViewModel.Colleges.RemoveAt(i);
                    }
                    else
                    {
                        queryCollegeViewModel.Colleges[i].Distance = collegeDistance;
                    }

                }
                queryCollegeViewModel.DistanceMessage = "* Distance shown is Great Circle distance.";
                // refine distance to driving distance if small enough

                if (queryCollegeViewModel.Colleges.Count < College.MAX_DRIVE_DIST_ENTRIES)
                {
                    queryCollegeViewModel.DistanceMessage = "* Distance shown is driving distance.";
                    queryCollegeViewModel = await DetermineDrivingDistance(homeAddress, queryCollegeViewModel, maxTravel);

                }
            }

            IEnumerable<CollegeType> collegeTypes = _context.CollegeTypes.ToList();
            IEnumerable<DegreeType> degreeTypes = _context.DegreeTypes.ToList();
            IEnumerable<String> states = _context.Colleges.OrderBy(c => c.State).Select(c => c.State).Distinct().ToList();
            queryCollegeViewModel.CreateCollegeTypeSelects(collegeTypes);
            queryCollegeViewModel.CreateDegreeTypeSelects(degreeTypes);
            queryCollegeViewModel.CreateStateSelects(states);

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

        private async Task<QueryCollegeViewModel> DetermineDrivingDistance(string homeAddress, QueryCollegeViewModel queryCollegeViewModel, int maxTravel)
        {
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
                    if (rootData.status == "OK")
                    {
                        // loop in reverse since deletion of entries affects indices
                        for (int i = queryCollegeViewModel.Colleges.Count - 1; i >= 0; i--)
                        {
                            if (rootData.rows[0].elements[i].status == "OK")
                            {
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
                            else
                            {
                                queryCollegeViewModel.DistanceMessage = "* Distance shown is Great Circle distance.";
                            }
                        }
                    }
                    else
                    {
                        queryCollegeViewModel.DistanceMessage = "* Distance shown is Great Circle distance.";
                    }

                }
            }
            return queryCollegeViewModel;
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::                                                                         :::
        //:::  This routine calculates the distance between two points (given the     :::
        //:::  latitude/longitude of those points). It is being used to calculate     :::
        //:::  the distance between two locations using GeoDataSource(TM) products    :::
        //:::                                                                         :::
        //:::  Definitions:                                                           :::
        //:::    South latitudes are negative, east longitudes are positive           :::
        //:::                                                                         :::
        //:::  Passed to function:                                                    :::
        //:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
        //:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
        //:::    unit = the unit you desire for results                               :::
        //:::           where: 'M' is statute miles (default)                         :::
        //:::                  'K' is kilometers                                      :::
        //:::                  'N' is nautical miles                                  :::
        //:::                                                                         :::
        //:::  Worldwide cities and other features databases with latitude longitude  :::
        //:::  are available at http://www.geodatasource.com                          :::
        //:::                                                                         :::
        //:::  For enquiries, please contact sales@geodatasource.com                  :::
        //:::                                                                         :::
        //:::  Official Web site: http://www.geodatasource.com                        :::
        //:::                                                                         :::
        //:::           GeoDataSource.com (C) All Rights Reserved 2017                :::
        //:::                                                                         :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //::  This function converts decimal degrees to radians             :::
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            private double deg2rad(double deg)
            {
                return (deg * Math.PI / 180.0);
            }

            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            //::  This function converts radians to decimal degrees             :::
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            private double rad2deg(double rad)
            {
                return (rad / Math.PI * 180.0);
            }
            
    }
}
