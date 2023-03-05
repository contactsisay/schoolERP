using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.HR;
using SchoolERP.Data;

namespace SchoolERP.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmploymentTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmploymentTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HR/EmploymentTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.EmploymentTypes.ToListAsync());
        }

        // GET: HR/EmploymentTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmploymentTypes == null)
            {
                return NotFound();
            }

            var employmentType = await _context.EmploymentTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employmentType == null)
            {
                return NotFound();
            }

            return View(employmentType);
        }

        // GET: HR/EmploymentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HR/EmploymentTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EmploymentType employmentType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employmentType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employmentType);
        }

        // GET: HR/EmploymentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmploymentTypes == null)
            {
                return NotFound();
            }

            var employmentType = await _context.EmploymentTypes.FindAsync(id);
            if (employmentType == null)
            {
                return NotFound();
            }
            return View(employmentType);
        }

        // POST: HR/EmploymentTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] EmploymentType employmentType)
        {
            if (id != employmentType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employmentType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmploymentTypeExists(employmentType.Id))
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
            return View(employmentType);
        }

        // GET: HR/EmploymentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EmploymentTypes == null)
            {
                return NotFound();
            }

            var employmentType = await _context.EmploymentTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employmentType == null)
            {
                return NotFound();
            }

            return View(employmentType);
        }

        // POST: HR/EmploymentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmploymentTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EmploymentTypes'  is null.");
            }
            var employmentType = await _context.EmploymentTypes.FindAsync(id);
            if (employmentType != null)
            {
                _context.EmploymentTypes.Remove(employmentType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmploymentTypeExists(int id)
        {
          return _context.EmploymentTypes.Any(e => e.Id == id);
        }
    }
}
