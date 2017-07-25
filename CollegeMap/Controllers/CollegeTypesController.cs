using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollegeMap.Data;
using CollegeMap.Models.CollegeMapModels;

namespace CollegeMap.Controllers
{
    public class CollegeTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollegeTypesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: CollegeTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CollegeTypes.ToListAsync());
        }

        // GET: CollegeTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeType = await _context.CollegeTypes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (collegeType == null)
            {
                return NotFound();
            }

            return View(collegeType);
        }

        // GET: CollegeTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CollegeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] CollegeType collegeType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collegeType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(collegeType);
        }

        // GET: CollegeTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeType = await _context.CollegeTypes.SingleOrDefaultAsync(m => m.ID == id);
            if (collegeType == null)
            {
                return NotFound();
            }
            return View(collegeType);
        }

        // POST: CollegeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] CollegeType collegeType)
        {
            if (id != collegeType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collegeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollegeTypeExists(collegeType.ID))
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
            return View(collegeType);
        }

        // GET: CollegeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeType = await _context.CollegeTypes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (collegeType == null)
            {
                return NotFound();
            }

            return View(collegeType);
        }

        // POST: CollegeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collegeType = await _context.CollegeTypes.SingleOrDefaultAsync(m => m.ID == id);
            _context.CollegeTypes.Remove(collegeType);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CollegeTypeExists(int id)
        {
            return _context.CollegeTypes.Any(e => e.ID == id);
        }
    }
}
