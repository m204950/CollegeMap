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

namespace CollegeMap.Controllers
{
    public class CollegesController : Controller
    {
        private readonly ApplicationDbContext _context;

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
                College college = new College
                {
                    Name = addCollegeViewModel.Name,
                    Description = addCollegeViewModel.Description,
                    Enrollment = addCollegeViewModel.Enrollment,
                    AnnualTuition = addCollegeViewModel.AnnualTuition,
                    AnnualRoomAndBoard = addCollegeViewModel.AnnualRoomAndBoard,
                    Website = addCollegeViewModel.Website,
                    Address = addCollegeViewModel.Address,

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
            return View(college);

        }

        // POST: Colleges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,ID,Enrollment,AnnualTuition,AnnualRoomAndBoard,Website,Address")] College college)
        {
            if (id != college.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(college);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollegeExists(college.ID))
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
            return View(college);
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
