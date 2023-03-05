using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;
using SchoolERP.Models;
using BALibrary.HR;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class SectionTeacherSubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionTeacherSubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/SectionTeacherSubjects
        public async Task<IActionResult> Index()
        {
            var queryResult = (from sTS in _context.SectionTeacherSubjects
                              join aYS in _context.AcademicYearSections.Include(a=>a.Section).Include(a=>a.AcademicYear).Include(a=>a.AcademicYear.Class) on sTS.AcademicYearSectionId equals aYS.Id
                              join e in _context.Employees on sTS.TeacherId equals e.Id
                               join sGD in _context.SubjectGroupDetails.Include(a=>a.Subject) on sTS.SubjectGroupDetailId equals sGD.Id
                               select new {
                                  sTS.Id,
                                  SessionName = aYS.AcademicYear.Session.Year.ToString() + "-"+aYS.AcademicYear.Session.SchoolType.ToString()+"-"+aYS.AcademicYear.Semester.Name,
                                  SectionName = aYS.AcademicYear.Class.Name+" ("+aYS.Section.Name+")",
                                  SubjectName = sGD.Subject.Name +'('+sGD.Subject.Code+')',
                                  TeacherName = e.FirstName +' '+e.MiddleName+'-'+e.LastName,
                                  TeacherPhone = e.PhoneNo,
                                  TeacherEmailAddress= e.EmailAddress,
                              }).ToList();

            ViewData["queryResult"] = queryResult;
            return View();
        }

        // GET: Registrar/SectionTeacherSubjects/Details/5
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

        // GET: Registrar/SectionTeacherSubjects/Create
        public IActionResult Create()
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year.ToString() + " " + aY.Session.SchoolType.ToString() + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            var employees = (from e in _context.Employees
                                 select new
                                 {
                                     e.Id,
                                     FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                 }).ToList();

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            ViewData["Employees"] = new SelectList(employees, "Id", "FullName");
            return View();
        }

        // POST: Registrar/SectionTeacherSubjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AcademicYearSectionId,SubjectGroupDetailId,TeacherId,AssignedDate,TotalClassPerWeek")] SectionTeacherSubject sectionTeacherSubject)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if (ModelState.IsValid)
            {
                _context.Add(sectionTeacherSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            var employees = (from e in _context.Employees
                             select new
                             {
                                 e.Id,
                                 FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                             }).ToList();

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            ViewData["Employees"] = new SelectList(employees, "Id", "FullName");
            return View(sectionTeacherSubject);
        }

        // GET: Registrar/SectionTeacherSubjects/Edit/5
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

        // POST: Registrar/SectionTeacherSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AcademicYearSectionId,SubjectGroupDetailId,TeacherId,AssignedDate,TotalClassPerWeek")] SectionTeacherSubject sectionTeacherSubject)
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

        // GET: Registrar/SectionTeacherSubjects/Delete/5
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

        // POST: Registrar/SectionTeacherSubjects/Delete/5
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
