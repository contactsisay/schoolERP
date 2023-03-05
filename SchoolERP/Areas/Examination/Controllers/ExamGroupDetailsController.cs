using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Examination;
using SchoolERP.Data;

namespace SchoolERP.Areas.Examination.Controllers
{
    [Area("Examination")]
    public class ExamGroupDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamGroupDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Examination/ExamGroupDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ExamGroupDetails.Include(e => e.Exam).Include(e => e.ExamGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Examination/ExamGroupDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ExamGroupDetails == null)
            {
                return NotFound();
            }

            var examGroupDetail = await _context.ExamGroupDetails
                .Include(e => e.Exam)
                .Include(e => e.ExamGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examGroupDetail == null)
            {
                return NotFound();
            }

            return View(examGroupDetail);
        }

        // GET: Examination/ExamGroupDetails/Create
        public IActionResult Create()
        {
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name");
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name");
            return View();
        }

        // POST: Examination/ExamGroupDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExamGroupId,ExamId,PassingMark")] ExamGroupDetail examGroupDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(examGroupDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name", examGroupDetail.ExamId);
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name", examGroupDetail.ExamGroupId);
            return View(examGroupDetail);
        }

        // GET: Examination/ExamGroupDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ExamGroupDetails == null)
            {
                return NotFound();
            }

            var examGroupDetail = await _context.ExamGroupDetails.FindAsync(id);
            if (examGroupDetail == null)
            {
                return NotFound();
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name", examGroupDetail.ExamId);
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name", examGroupDetail.ExamGroupId);
            return View(examGroupDetail);
        }

        // POST: Examination/ExamGroupDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamGroupId,ExamId,PassingMark")] ExamGroupDetail examGroupDetail)
        {
            if (id != examGroupDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examGroupDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamGroupDetailExists(examGroupDetail.Id))
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
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name", examGroupDetail.ExamId);
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name", examGroupDetail.ExamGroupId);
            return View(examGroupDetail);
        }

        // GET: Examination/ExamGroupDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ExamGroupDetails == null)
            {
                return NotFound();
            }

            var examGroupDetail = await _context.ExamGroupDetails
                .Include(e => e.Exam)
                .Include(e => e.ExamGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examGroupDetail == null)
            {
                return NotFound();
            }

            return View(examGroupDetail);
        }

        // POST: Examination/ExamGroupDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ExamGroupDetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ExamGroupDetails'  is null.");
            }
            var examGroupDetail = await _context.ExamGroupDetails.FindAsync(id);
            if (examGroupDetail != null)
            {
                _context.ExamGroupDetails.Remove(examGroupDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamGroupDetailExists(int id)
        {
          return _context.ExamGroupDetails.Any(e => e.Id == id);
        }
    }
}
