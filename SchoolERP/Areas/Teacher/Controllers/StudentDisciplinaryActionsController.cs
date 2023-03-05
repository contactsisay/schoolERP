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
    public class StudentDisciplinaryActionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentDisciplinaryActionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teacher/StudentDisciplinaryActions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentDisciplinaryActions.Include(s => s.ActionType).Include(s => s.SectionTeacherSubject).Include(s => s.StudentPromotion);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Teacher/StudentDisciplinaryActions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentDisciplinaryActions == null)
            {
                return NotFound();
            }

            var studentDisciplinaryAction = await _context.StudentDisciplinaryActions
                .Include(s => s.ActionType)
                .Include(s => s.SectionTeacherSubject)
                .Include(s => s.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentDisciplinaryAction == null)
            {
                return NotFound();
            }

            return View(studentDisciplinaryAction);
        }

        // GET: Teacher/StudentDisciplinaryActions/Create
        public IActionResult Create()
        {
            ViewData["ActionTypeId"] = new SelectList(_context.ActionTypes, "Id", "Description");
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id");
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id");
            return View();
        }

        // POST: Teacher/StudentDisciplinaryActions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentPromotionId,SectionTeacherSubjectId,TeacherId,ActionTypeId,FilePath,Remark")] StudentDisciplinaryAction studentDisciplinaryAction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentDisciplinaryAction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActionTypeId"] = new SelectList(_context.ActionTypes, "Id", "Description", studentDisciplinaryAction.ActionTypeId);
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentDisciplinaryAction.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentDisciplinaryAction.StudentPromotionId);
            return View(studentDisciplinaryAction);
        }

        // GET: Teacher/StudentDisciplinaryActions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentDisciplinaryActions == null)
            {
                return NotFound();
            }

            var studentDisciplinaryAction = await _context.StudentDisciplinaryActions.FindAsync(id);
            if (studentDisciplinaryAction == null)
            {
                return NotFound();
            }
            ViewData["ActionTypeId"] = new SelectList(_context.ActionTypes, "Id", "Description", studentDisciplinaryAction.ActionTypeId);
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentDisciplinaryAction.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentDisciplinaryAction.StudentPromotionId);
            return View(studentDisciplinaryAction);
        }

        // POST: Teacher/StudentDisciplinaryActions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentPromotionId,SectionTeacherSubjectId,TeacherId,ActionTypeId,FilePath,Remark")] StudentDisciplinaryAction studentDisciplinaryAction)
        {
            if (id != studentDisciplinaryAction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentDisciplinaryAction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentDisciplinaryActionExists(studentDisciplinaryAction.Id))
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
            ViewData["ActionTypeId"] = new SelectList(_context.ActionTypes, "Id", "Description", studentDisciplinaryAction.ActionTypeId);
            ViewData["SectionTeacherSubjectId"] = new SelectList(_context.SectionTeacherSubjects, "Id", "Id", studentDisciplinaryAction.SectionTeacherSubjectId);
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", studentDisciplinaryAction.StudentPromotionId);
            return View(studentDisciplinaryAction);
        }

        // GET: Teacher/StudentDisciplinaryActions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentDisciplinaryActions == null)
            {
                return NotFound();
            }

            var studentDisciplinaryAction = await _context.StudentDisciplinaryActions
                .Include(s => s.ActionType)
                .Include(s => s.SectionTeacherSubject)
                .Include(s => s.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentDisciplinaryAction == null)
            {
                return NotFound();
            }

            return View(studentDisciplinaryAction);
        }

        // POST: Teacher/StudentDisciplinaryActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentDisciplinaryActions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentDisciplinaryActions'  is null.");
            }
            var studentDisciplinaryAction = await _context.StudentDisciplinaryActions.FindAsync(id);
            if (studentDisciplinaryAction != null)
            {
                _context.StudentDisciplinaryActions.Remove(studentDisciplinaryAction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentDisciplinaryActionExists(int id)
        {
          return _context.StudentDisciplinaryActions.Any(e => e.Id == id);
        }
    }
}
