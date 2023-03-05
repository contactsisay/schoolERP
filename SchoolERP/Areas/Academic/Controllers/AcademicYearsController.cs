using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Academic;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Academic.Controllers
{
    [Area("Academic")]
    public class AcademicYearsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicYearsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/AcademicYears
        public async Task<IActionResult> Index(string? aYId)
        {
            var applicationDbContext = _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup).Include(a => a.Session).Include(a => a.SubjectGroup).Include(a => a.Semester);

            if (!string.IsNullOrEmpty(aYId)) {
                int aId = Convert.ToInt32(aYId);
                applicationDbContext.Where(a => a.Id == aId).ToList();
            }

            return View(await applicationDbContext.ToListAsync());
        }

        #region LOADING DATA        
        [HttpPost]
        public JsonResult GetSessionClasses(string sid)
        {
            var classes = from aY in _context.AcademicYears.Include(a=>a.Class)
                          where aY.SessionId == Convert.ToInt32(sid)
                          select new
                          {
                              AYId = aY.Id,
                              aY.ClassId,
                              ClassName = "Grade " + aY.Class.Name,
                          };

            return Json(new SelectList(classes, "AYId", "ClassName"));
        }

        [HttpPost]
        public JsonResult GetYearClasses(string year)
        {
            var classes = from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Semester)
                                       where aY.Session.Year == Convert.ToInt32(year)
                                       select new
                                       {
                                           AYId = aY.Id,
                                           aY.ClassId,
                                           ClassName = aY.Session.Year +"-"+aY.Session.SchoolType.ToUpper() + "(" + aY.Class.Name+")",
                                       };

            return Json(new SelectList(classes, "AYId", "ClassName"));
        }

        [HttpPost]
        public JsonResult GetClassSections(string cid)
        {
            int classId = Convert.ToInt32(cid);
            var academicYearSections = from aYS in _context.AcademicYearSections.Include(a => a.Section)
                                       join aY in _context.AcademicYears.Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                       where aY.Id == Convert.ToInt32(cid)
                                       select new
                                       {
                                           AYId = aY.Id,
                                           AYSId = aYS.Id,
                                           aY.ClassId,
                                           ClassName = aY.Class.Name,
                                           aYS.SectionId,
                                           SectionName2 = aYS.Section.Name,
                                           SectionName = aY.Class.Name + "(" + aYS.Section.Name + ")",
                                       };

            return Json(new SelectList(academicYearSections, "AYSId", "SectionName"));
        }

        [HttpPost]
        public JsonResult GetClassSections2(string cid)
        {
            int classId = Convert.ToInt32(cid);
            var academicYearSections = from aYS in _context.AcademicYearSections.Include(a => a.Section)
                                       join aY in _context.AcademicYears.Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                       where aY.ClassId == classId
                                       select new
                                       {
                                           AYId = aY.Id,
                                           AYSId = aYS.Id,
                                           aY.ClassId,
                                           ClassName = aY.Class.Name,
                                           aYS.SectionId,
                                           SectionName2 = aYS.Section.Name,
                                           SectionName = aY.Class.Name + "(" + aYS.Section.Name + ")",
                                       };

            return Json(new SelectList(academicYearSections, "AYSId", "SectionName"));
        }

        [HttpPost]
        public JsonResult GetClassSubjects(string cid)
        {
            string options = string.Empty;
            int classId = Convert.ToInt32(cid);
            var academicYears = from aY in _context.AcademicYears.Include(a => a.Class)
                                join sGD in _context.SubjectGroupDetails on aY.SubjectGroupId equals sGD.SubjectGroupId
                                join s in _context.Subjects on sGD.SubjectId equals s.Id
                                where aY.Id == Convert.ToInt32(cid)
                                select new
                                {
                                    SGDId = sGD.Id,
                                    AYId = aY.Id,
                                    aY.ClassId,
                                    ClassName = aY.Class.Name,
                                    SubjectId = s.Id,
                                    SubjectName = s.Name + "(" + s.Code + ")",
                                };

            return Json(new SelectList(academicYears, "SGDId", "SubjectName"));
        }

        [HttpPost]
        public JsonResult GetClassExams(string cid)
        {
            int classId = Convert.ToInt32(cid);
            var academicYears = from aY in _context.AcademicYears.Include(a => a.Class).Include(a=>a.ExamGroup)
                                join eGD in _context.ExamGroupDetails on aY.ExamGroupId equals eGD.ExamGroupId
                                join e in _context.Exams on eGD.ExamId equals e.Id
                                where aY.Id == Convert.ToInt32(cid)
                                select new
                                {
                                    EGDId = eGD.Id,
                                    AYId = aY.Id,
                                    aY.ClassId,
                                    ClassName = aY.Class.Name,
                                    ExamId = e.Id,
                                    ExamName = e.Name,
                                };

            return Json(new SelectList(academicYears, "EGDId", "ExamName"));
        }

        #endregion

        // GET: Academic/AcademicYears
        public async Task<IActionResult> AssignHomeRoomTeacher(string? id)
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     HomeRoomTeacherId = aY.EmployeeId,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            int empId = 0;
            int aId = 0;
            if (id != null) {
                aId = Convert.ToInt32(id);
                academicYears = academicYears.Where(a => a.AcademicYearId == aId).ToList();
                if (academicYears.Count > 0)
                    empId = Convert.ToInt32(academicYears[0].HomeRoomTeacherId);
            }

            var employees = (from e in _context.Employees
                             select new
                             {
                                 e.Id,
                                 FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                             }).ToList();

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName", aId);
            ViewData["Employees"] = new SelectList(employees, "Id", "FullName", empId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignHomeRoomTeacher(string AcademicYearId, string TeacherId)
        {
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

            if ((!string.IsNullOrEmpty(AcademicYearId) && Convert.ToInt32(AcademicYearId) > 0) && (!string.IsNullOrEmpty(TeacherId) && Convert.ToInt32(TeacherId) > 0))
            {
                //saving
                int aYId = Convert.ToInt32(AcademicYearId);
                AcademicYear aY = _context.AcademicYears.Find(aYId);
                aY.EmployeeId = Convert.ToInt32(TeacherId);
                _context.AcademicYears.Update(aY);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { AcademicYearId = aYId });
            }

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName", AcademicYearId);
            ViewData["Employees"] = new SelectList(employees, "Id", "FullName", TeacherId);
            return View();
        }

        // GET: Academic/AcademicYears/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AcademicYears == null)
            {
                return NotFound();
            }

            var academicYear = await _context.AcademicYears
                .Include(a => a.Class)
                .Include(a => a.ExamGroup)
                .Include(a => a.Session)
                .Include(a => a.SubjectGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicYear == null)
            {
                return NotFound();
            }

            return View(academicYear);
        }

        // GET: Academic/AcademicYears/Create
        public IActionResult Create()
        {
            var sessions = (from s in _context.Sessions
                                 select new
                                 {
                                     SessionId= s.Id,
                                     RosterName = s.Year + " " + s.SchoolType,
                                 }).ToList();

            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name");
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name");
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name");
            ViewData["SessionId"] = new SelectList(sessions, "SessionId","RosterName");
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Name");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "IDNo");
            return View();
        }

        // POST: Academic/AcademicYears/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SessionId,ClassId,SubjectGroupId,ExamGroupId,StudentId,SemesterId")] AcademicYear academicYear)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicYear);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var sessions = (from s in _context.Sessions
                                 select new
                                 {
                                     SessionId = s.Id,
                                     RosterName = s.Year + " " + s.SchoolType,
                                 }).ToList();
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", academicYear.SemesterId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", academicYear.ClassId);
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name", academicYear.ExamGroupId);
            ViewData["SessionId"] = new SelectList(sessions, "SessionId", "RosterName", academicYear.SessionId);
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Name", academicYear.SubjectGroupId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "IDNo", academicYear.StudentId);
            return View(academicYear);
        }

        // GET: Academic/AcademicYears/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AcademicYears == null)
            {
                return NotFound();
            }

            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }

            var sessions = (from s in _context.Sessions
                            select new
                            {
                                SessionId = s.Id,
                                RosterName = s.Year + " " + s.SchoolType,
                            }).ToList();
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", academicYear.SemesterId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", academicYear.ClassId);
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name", academicYear.ExamGroupId);
            ViewData["SessionId"] = new SelectList(sessions, "SessionId", "RosterName", academicYear.SessionId);
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Name", academicYear.SubjectGroupId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "IDNo", academicYear.StudentId);
            return View(academicYear);
        }

        // POST: Academic/AcademicYears/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SessionId,ClassId,SubjectGroupId,ExamGroupId,StudentId,SemesterId")] AcademicYear academicYear)
        {
            if (id != academicYear.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicYear);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicYearExists(academicYear.Id))
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
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", academicYear.SemesterId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", academicYear.ClassId);
            ViewData["ExamGroupId"] = new SelectList(_context.ExamGroups, "Id", "Name", academicYear.ExamGroupId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Year", academicYear.SessionId);
            ViewData["SubjectGroupId"] = new SelectList(_context.SubjectGroups, "Id", "Name", academicYear.SubjectGroupId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "IDNo", academicYear.StudentId);
            return View(academicYear);
        }

        // GET: Academic/AcademicYears/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AcademicYears == null)
            {
                return NotFound();
            }

            var academicYear = await _context.AcademicYears
                .Include(a => a.Class)
                .Include(a => a.ExamGroup)
                .Include(a => a.Session)
                .Include(a => a.SubjectGroup)
                .Include(a=>a.Semester)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicYear == null)
            {
                return NotFound();
            }

            return View(academicYear);
        }

        // POST: Academic/AcademicYears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AcademicYears == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AcademicYears'  is null.");
            }
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear != null)
            {
                _context.AcademicYears.Remove(academicYear);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicYearExists(int id)
        {
          return _context.AcademicYears.Any(e => e.Id == id);
        }
    }
}
