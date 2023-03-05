using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Teacher;
using SchoolERP.Data;

namespace SchoolERP.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class StudentLeavesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentLeavesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teacher/StudentLeaves
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentLeaves.Include(s => s.SectionTeacherSubject).Include(s => s.StudentPromotion);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Teacher/StudentLeaves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentLeaves == null)
            {
                return NotFound();
            }

            var studentLeave = await _context.StudentLeaves
                .Include(s => s.SectionTeacherSubject)
                .Include(s => s.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentLeave == null)
            {
                return NotFound();
            }

            return View(studentLeave);
        }

        // GET: Teacher/StudentLeaves/Create
        public IActionResult Create()
        {
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id");
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id");
            return View();
        }

        // POST: Teacher/StudentLeaves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentPromotionId,SectionTeacherSubjectId,LeaveDate,FilePath,Reason")] StudentLeave studentLeave)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentLeave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentLeave.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentLeave.StudentPromotionId);
            return View(studentLeave);
        }

        // GET: Teacher/StudentLeaves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentLeaves == null)
            {
                return NotFound();
            }

            var studentLeave = await _context.StudentLeaves.FindAsync(id);
            if (studentLeave == null)
            {
                return NotFound();
            }
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentLeave.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentLeave.StudentPromotionId);
            return View(studentLeave);
        }

        // POST: Teacher/StudentLeaves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentPromotionId,SectionTeacherSubjectId,LeaveDate,FilePath,Reason")] StudentLeave studentLeave)
        {
            if (id != studentLeave.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentLeave);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentLeaveExists(studentLeave.Id))
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
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentLeave.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentLeave.StudentPromotionId);
            return View(studentLeave);
        }

        // GET: Teacher/StudentLeaves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentLeaves == null)
            {
                return NotFound();
            }

            var studentLeave = await _context.StudentLeaves
                .Include(s => s.SectionTeacherSubject)
                .Include(s => s.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentLeave == null)
            {
                return NotFound();
            }

            return View(studentLeave);
        }

        // POST: Teacher/StudentLeaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentLeaves == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentLeaves'  is null.");
            }
            var studentLeave = await _context.StudentLeaves.FindAsync(id);
            if (studentLeave != null)
            {
                _context.StudentLeaves.Remove(studentLeave);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentLeaveExists(int id)
        {
          return _context.StudentLeaves.Any(e => e.Id == id);
        }
    }
}
