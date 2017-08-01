using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CollegeMap.Data;
using Microsoft.EntityFrameworkCore;
using CollegeMap.Models.CollegeMapViewModels;
using CollegeMap.Models.CollegeMapModels;

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
                int minEnrollment = queryCollegeViewModel.MinimumEnrollment;
                int maxEnrollment = queryCollegeViewModel.MaximumEnrollment;
                int maxTotalCost = queryCollegeViewModel.MaxTotalCost;
                int desiredDegreeLevelID = queryCollegeViewModel.DegreeTypeID;
                int collegeTypeID = queryCollegeViewModel.CollegeTypeID;
                string nameContains;
                if (queryCollegeViewModel.NameContains == null)
                {
                    nameContains = "";
                }
                else
                {
                    nameContains = queryCollegeViewModel.NameContains;
                }
                if (collegeTypeID == 500)  // ID for selecting all college types
                {
                    queryCollegeViewModel.Colleges = await _context.Colleges.
                        Where(c => c.Enrollment <= maxEnrollment).Where(c => c.Enrollment >= minEnrollment).
                        Where(c => c.AnnualTuition + c.AnnualRoomAndBoard <= maxTotalCost).
                        Where(c => c.Name.ToUpper().Contains(nameContains.ToUpper())).
                        // following relies on degree types having ascending IDs corresponding to their level
                        Where(c => c.HighestDegreeOffered.ID >= desiredDegreeLevelID).
                        Include(c => c.Type).Include(c => c.HighestDegreeOffered).ToListAsync();
                }
                else
                {
                    queryCollegeViewModel.Colleges = await _context.Colleges.
                        Where(c => c.Enrollment <= maxEnrollment).Where(c => c.Enrollment >= minEnrollment).
                        Where(c => c.AnnualTuition + c.AnnualRoomAndBoard <= maxTotalCost).
                        Where(c => c.Name.ToUpper().Contains(nameContains.ToUpper())).
                        // following relies on degree types having ascending IDs corresponding to their level
                        Where(c => c.HighestDegreeOffered.ID >= desiredDegreeLevelID).
                        Where(c => c.Type.ID == collegeTypeID).
                        Include(c => c.Type).Include(c => c.HighestDegreeOffered).ToListAsync();
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
