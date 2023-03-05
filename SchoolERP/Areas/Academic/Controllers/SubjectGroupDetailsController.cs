using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Academic;
using SchoolERP.Data;

namespace SchoolERP.Areas.Academic.Controllers
{
    [Area("Academic")]
    public class SubjectGroupDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectGroupDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/SubjectGroupDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SubjectGroupDetails.Include(s => s.Subject).Include(s => s.SubjectGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Academic/SubjectGroupDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SubjectGroupDetails == null)
            {
                return NotFound();
            }

            var subjectGroupDetail = await _context.SubjectGroupDetails
                .Include(s => s.Subject)
                .Include(s => s.SubjectGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectGroupDetail == null)
            {
                return NotFound();
            }

            return View(subjectGroupDetail);
        }

        // GET: Academic/SubjectGroupDetails/Create
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Code");
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Description");
            return View();
        }

        // POST: Academic/SubjectGroupDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectGroupId,SubjectId,Description")] SubjectGroupDetail subjectGroupDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subjectGroupDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Code", subjectGroupDetail.SubjectId);
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Description", subjectGroupDetail.SubjectGroupId);
            return View(subjectGroupDetail);
        }

        // GET: Academic/SubjectGroupDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubjectGroupDetails == null)
            {
                return NotFound();
            }

            var subjectGroupDetail = await _context.SubjectGroupDetails.FindAsync(id);
            if (subjectGroupDetail == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Code", subjectGroupDetail.SubjectId);
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Description", subjectGroupDetail.SubjectGroupId);
            return View(subjectGroupDetail);
        }

        // POST: Academic/SubjectGroupDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectGroupId,SubjectId,Description")] SubjectGroupDetail subjectGroupDetail)
        {
            if (id != subjectGroupDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectGroupDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectGroupDetailExists(subjectGroupDetail.Id))
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Code", subjectGroupDetail.SubjectId);
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Description", subjectGroupDetail.SubjectGroupId);
            return View(subjectGroupDetail);
        }

        // GET: Academic/SubjectGroupDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubjectGroupDetails == null)
            {
                return NotFound();
            }

            var subjectGroupDetail = await _context.SubjectGroupDetails
                .Include(s => s.Subject)
                .Include(s => s.SubjectGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectGroupDetail == null)
            {
                return NotFound();
            }

            return View(subjectGroupDetail);
        }

        // POST: Academic/SubjectGroupDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubjectGroupDetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubjectGroupDetail'  is null.");
            }
            var subjectGroupDetail = await _context.SubjectGroupDetails.FindAsync(id);
            if (subjectGroupDetail != null)
            {
                _context.SubjectGroupDetails.Remove(subjectGroupDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectGroupDetailExists(int id)
        {
          return _context.SubjectGroupDetails.Any(e => e.Id == id);
        }
    }
}
