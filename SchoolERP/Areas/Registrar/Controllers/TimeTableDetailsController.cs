using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class TimeTableDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeTableDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/TimeTableDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TimeTableDetails.Include(t => t.Period).Include(t => t.TimeTable);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Registrar/TimeTableDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TimeTableDetails == null)
            {
                return NotFound();
            }

            var timeTableDetail = await _context.TimeTableDetails
                .Include(t => t.Period)
                .Include(t => t.TimeTable)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeTableDetail == null)
            {
                return NotFound();
            }

            return View(timeTableDetail);
        }

        // GET: Registrar/TimeTableDetails/Create
        public IActionResult Create()
        {
            ViewData["PeriodId"] = new SelectList(_context.Periods, "Id", "FromTime");
            return View();
        }

        // POST: Registrar/TimeTableDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TimeTableId,EmployeeId,PeriodId,SubjectId,WeekDay")] TimeTableDetail timeTableDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeTableDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PeriodId"] = new SelectList(_context.Periods, "Id", "FromTime", timeTableDetail.PeriodId);
            return View(timeTableDetail);
        }

        // GET: Registrar/TimeTableDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TimeTableDetails == null)
            {
                return NotFound();
            }

            var timeTableDetail = await _context.TimeTableDetails.FindAsync(id);
            if (timeTableDetail == null)
            {
                return NotFound();
            }
            ViewData["PeriodId"] = new SelectList(_context.Periods, "Id", "FromTime", timeTableDetail.PeriodId);
            return View(timeTableDetail);
        }

        // POST: Registrar/TimeTableDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TimeTableId,EmployeeId,PeriodId,SubjectId,WeekDay")] TimeTableDetail timeTableDetail)
        {
            if (id != timeTableDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeTableDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeTableDetailExists(timeTableDetail.Id))
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
            ViewData["PeriodId"] = new SelectList(_context.Periods, "Id", "FromTime", timeTableDetail.PeriodId);
            return View(timeTableDetail);
        }

        // GET: Registrar/TimeTableDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TimeTableDetails == null)
            {
                return NotFound();
            }

            var timeTableDetail = await _context.TimeTableDetails
                .Include(t => t.Period)
                .Include(t => t.TimeTable)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeTableDetail == null)
            {
                return NotFound();
            }

            return View(timeTableDetail);
        }

        // POST: Registrar/TimeTableDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TimeTableDetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TimeTableDetails'  is null.");
            }
            var timeTableDetail = await _context.TimeTableDetails.FindAsync(id);
            if (timeTableDetail != null)
            {
                _context.TimeTableDetails.Remove(timeTableDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeTableDetailExists(int id)
        {
          return _context.TimeTableDetails.Any(e => e.Id == id);
        }
    }
}
