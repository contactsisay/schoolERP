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
    public class StudentAbsenteesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentAbsenteesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teacher/StudentAbsentees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentAbsentees.Include(s => s.SectionTeacherSubject).Include(s => s.StudentPromotion);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Teacher/StudentAbsentees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentAbsentees == null)
            {
                return NotFound();
            }

            var studentAbsentee = await _context.StudentAbsentees
                .Include(s => s.SectionTeacherSubject)
                .Include(s => s.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAbsentee == null)
            {
                return NotFound();
            }

            return View(studentAbsentee);
        }

        // GET: Teacher/StudentAbsentees/Create
        public IActionResult Create()
        {
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id");
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id");
            return View();
        }

        // POST: Teacher/StudentAbsentees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentPromotionId,SectionTeacherSubjectId,AbsentDate,FilePath,Reason")] StudentAbsentee studentAbsentee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentAbsentee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentAbsentee.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentAbsentee.StudentPromotionId);
            return View(studentAbsentee);
        }

        // GET: Teacher/StudentAbsentees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentAbsentees == null)
            {
                return NotFound();
            }

            var studentAbsentee = await _context.StudentAbsentees.FindAsync(id);
            if (studentAbsentee == null)
            {
                return NotFound();
            }
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentAbsentee.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentAbsentee.StudentPromotionId);
            return View(studentAbsentee);
        }

        // POST: Teacher/StudentAbsentees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentPromotionId,SectionTeacherSubjectId,AbsentDate,FilePath,Reason")] StudentAbsentee studentAbsentee)
        {
            if (id != studentAbsentee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentAbsentee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentAbsenteeExists(studentAbsentee.Id))
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
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentAbsentee.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentAbsentee.StudentPromotionId);
            return View(studentAbsentee);
        }

        // GET: Teacher/StudentAbsentees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentAbsentees == null)
            {
                return NotFound();
            }

            var studentAbsentee = await _context.StudentAbsentees
                .Include(s => s.SectionTeacherSubject)
                .Include(s => s.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAbsentee == null)
            {
                return NotFound();
            }

            return View(studentAbsentee);
        }

        // POST: Teacher/StudentAbsentees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentAbsentees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentAbsentees'  is null.");
            }
            var studentAbsentee = await _context.StudentAbsentees.FindAsync(id);
            if (studentAbsentee != null)
            {
                _context.StudentAbsentees.Remove(studentAbsentee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentAbsenteeExists(int id)
        {
          return _context.StudentAbsentees.Any(e => e.Id == id);
        }
    }
}
