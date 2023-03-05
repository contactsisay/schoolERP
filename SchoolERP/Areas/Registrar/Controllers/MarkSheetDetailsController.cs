using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;
using BALibrary.Academic;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class MarkSheetDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarkSheetDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/MarkSheetDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MarksSheetDetails.Include(m => m.MarkSheet);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Registrar/MarkSheetDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MarksSheetDetails == null)
            {
                return NotFound();
            }

            var markSheetDetail = await _context.MarksSheetDetails
                .Include(m => m.MarkSheet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markSheetDetail == null)
            {
                return NotFound();
            }

            return View(markSheetDetail);
        }

        // GET: Registrar/MarkSheetDetails/Create
        public IActionResult Create()
        {
            ViewData["MarkSheetId"] = new SelectList(_context.MarksSheets, "Id", "Id");
            return View();
        }

        // POST: Registrar/MarkSheetDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MarkSheetId,SubjectId,ExamId,ExamMark,IsPassed,IsHigher,IsExcellent,IsTop")] MarkSheetDetail markSheetDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(markSheetDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarkSheetId"] = new SelectList(_context.MarksSheets, "Id", "Id", markSheetDetail.MarkSheetId);
            return View(markSheetDetail);
        }

        // GET: Registrar/MarkSheetDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MarksSheetDetails == null)
            {
                return NotFound();
            }

            var markSheetDetail = await _context.MarksSheetDetails.FindAsync(id);
            if (markSheetDetail == null)
            {
                return NotFound();
            }
            ViewData["MarkSheetId"] = new SelectList(_context.MarksSheets, "Id", "Id", markSheetDetail.MarkSheetId);
            return View(markSheetDetail);
        }

        // POST: Registrar/MarkSheetDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MarkSheetId,SubjectId,ExamId,ExamMark,IsPassed,IsHigher,IsExcellent,IsTop")] MarkSheetDetail markSheetDetail)
        {
            if (id != markSheetDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(markSheetDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkSheetDetailExists(markSheetDetail.Id))
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
            ViewData["MarkSheetId"] = new SelectList(_context.MarksSheets, "Id", "Id", markSheetDetail.MarkSheetId);
            return View(markSheetDetail);
        }

        // GET: Registrar/MarkSheetDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MarksSheetDetails == null)
            {
                return NotFound();
            }

            var markSheetDetail = await _context.MarksSheetDetails
                .Include(m => m.MarkSheet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markSheetDetail == null)
            {
                return NotFound();
            }

            return View(markSheetDetail);
        }

        // POST: Registrar/MarkSheetDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MarksSheetDetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MarksSheetDetails'  is null.");
            }
            var markSheetDetail = await _context.MarksSheetDetails.FindAsync(id);
            if (markSheetDetail != null)
            {
                _context.MarksSheetDetails.Remove(markSheetDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkSheetDetailExists(int id)
        {
          return _context.MarksSheetDetails.Any(e => e.Id == id);
        }
    }
}
