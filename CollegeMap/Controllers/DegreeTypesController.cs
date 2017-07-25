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
    public class DegreeTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DegreeTypesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: DegreeTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DegreeTypes.ToListAsync());
        }

        // GET: DegreeTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var degreeType = await _context.DegreeTypes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (degreeType == null)
            {
                return NotFound();
            }

            return View(degreeType);
        }

        // GET: DegreeTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DegreeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] DegreeType degreeType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(degreeType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(degreeType);
        }

        // GET: DegreeTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var degreeType = await _context.DegreeTypes.SingleOrDefaultAsync(m => m.ID == id);
            if (degreeType == null)
            {
                return NotFound();
            }
            return View(degreeType);
        }

        // POST: DegreeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] DegreeType degreeType)
        {
            if (id != degreeType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(degreeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DegreeTypeExists(degreeType.ID))
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
            return View(degreeType);
        }

        // GET: DegreeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var degreeType = await _context.DegreeTypes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (degreeType == null)
            {
                return NotFound();
            }

            return View(degreeType);
        }

        // POST: DegreeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var degreeType = await _context.DegreeTypes.SingleOrDefaultAsync(m => m.ID == id);
            _context.DegreeTypes.Remove(degreeType);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool DegreeTypeExists(int id)
        {
            return _context.DegreeTypes.Any(e => e.ID == id);
        }
    }
}
