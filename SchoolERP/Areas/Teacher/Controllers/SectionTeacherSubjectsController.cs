using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;

namespace SchoolERP.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class SectionTeacherSubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionTeacherSubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teacher/SectionTeacherSubjects
        public async Task<IActionResult> Index()
        {
              return View(await _context.SectionTeacherSubjects.ToListAsync());
        }

        // GET: Teacher/SectionTeacherSubjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SectionTeacherSubjects == null)
            {
                return NotFound();
            }

            var sectionTeacherSubject = await _context.SectionTeacherSubjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sectionTeacherSubject == null)
            {
                return NotFound();
            }

            return View(sectionTeacherSubject);
        }

        // GET: Teacher/SectionTeacherSubjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teacher/SectionTeacherSubjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AcademicYearSectionId,SubjectGroupDetailId,TeacherId,AssignedDate")] SectionTeacherSubject sectionTeacherSubject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sectionTeacherSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sectionTeacherSubject);
        }

        // GET: Teacher/SectionTeacherSubjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SectionTeacherSubjects == null)
            {
                return NotFound();
            }

            var sectionTeacherSubject = await _context.SectionTeacherSubjects.FindAsync(id);
            if (sectionTeacherSubject == null)
            {
                return NotFound();
            }
            return View(sectionTeacherSubject);
        }

        // POST: Teacher/SectionTeacherSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AcademicYearSectionId,SubjectGroupDetailId,TeacherId,AssignedDate")] SectionTeacherSubject sectionTeacherSubject)
        {
            if (id != sectionTeacherSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sectionTeacherSubject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionTeacherSubjectExists(sectionTeacherSubject.Id))
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
            return View(sectionTeacherSubject);
        }

        // GET: Teacher/SectionTeacherSubjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SectionTeacherSubjects == null)
            {
                return NotFound();
            }

            var sectionTeacherSubject = await _context.SectionTeacherSubjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sectionTeacherSubject == null)
            {
                return NotFound();
            }

            return View(sectionTeacherSubject);
        }

        // POST: Teacher/SectionTeacherSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SectionTeacherSubjects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SectionTeacherSubjects'  is null.");
            }
            var sectionTeacherSubject = await _context.SectionTeacherSubjects.FindAsync(id);
            if (sectionTeacherSubject != null)
            {
                _context.SectionTeacherSubjects.Remove(sectionTeacherSubject);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectionTeacherSubjectExists(int id)
        {
          return _context.SectionTeacherSubjects.Any(e => e.Id == id);
        }
    }
}
