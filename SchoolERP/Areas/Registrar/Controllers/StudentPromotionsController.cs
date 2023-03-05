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
using SchoolERP.Models;
using BALibrary.Examination;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class StudentPromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentPromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/StudentPromotions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentPromotions.Include(s => s.AcademicYearSection).Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Registrar/StudentPromotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentPromotions == null)
            {
                return NotFound();
            }

            var studentPromotion = await _context.StudentPromotions
                .Include(s => s.AcademicYearSection)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentPromotion == null)
            {
                return NotFound();
            }

            return View(studentPromotion);
        }

        // GET: Registrar/StudentPromotions/Create
        public IActionResult Create(string Year, string ClassId, string SectionId, string ClassId3, string SectionId3)
        {
            if ((!string.IsNullOrEmpty(Year) && Convert.ToInt32(Year) > 0) && !string.IsNullOrEmpty(ClassId) && !string.IsNullOrEmpty(SectionId) && !string.IsNullOrEmpty(ClassId3) && !string.IsNullOrEmpty(SectionId3))
            {
                var year = Convert.ToInt32(Year);
                var classId = Convert.ToInt32(ClassId);
                var sectionId = Convert.ToInt32(SectionId);
                var classId3 = Convert.ToInt32(ClassId3);
                var sectionId3 = Convert.ToInt32(SectionId3);

                var studentPromotions = (from sP in _context.StudentPromotions.Include(a => a.Student).Include(a => a.AcademicYearSection)
                                         join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a=>a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                         where sP.AcademicYearSectionId == sectionId
                                         select new
                                         {
                                             SessionId = aY.Session.Id,
                                             AcademicYearId = aY.Id,
                                             SemesterId = aY.SemesterId,
                                             StudentPromotionId = sP.Id,
                                             sP.AcademicYearSectionId,
                                             aY.ClassId,
                                             sP.Student.IDNo,
                                             FullName = sP.Student.FirstName + " " + sP.Student.MiddleName,
                                             sP.StudentId,
                                             sP.Student.Gender,
                                             Semester = aY.Semester.Name,
                                             ClassName = aY.Class.Name,
                                             SectionName = aY.Class.Name + " (" + sP.AcademicYearSection.Section.Name + ")",
                                             aY.ExamGroupId,
                                             aY.SubjectGroupId,
                                         }).ToList();

                ViewData["year"] = year;
                ViewData["classId"] = classId;
                ViewData["sectionId"] = sectionId;
                ViewData["classId3"] = classId3;
                ViewData["sectionId3"] = sectionId3;
                ViewData["queryResult"] = studentPromotions;
                ViewData["studentPromotionsCount"] = studentPromotions.Count;
                ViewData["yearList"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text", year);
                return View();
            }

            ViewData["yearList"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text");
            return View();
        }

        // POST: Registrar/StudentPromotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromotion(IFormCollection iFormCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if ((!string.IsNullOrEmpty(iFormCollection["Year"]) && Convert.ToInt32(iFormCollection["Year"]) > 0) && !string.IsNullOrEmpty(iFormCollection["SectionId"]) && !string.IsNullOrEmpty(iFormCollection["ClassId"]) && !string.IsNullOrEmpty(iFormCollection["ClassId3"]) && !string.IsNullOrEmpty(iFormCollection["SectionId3"]))
            {
                var year = Convert.ToInt32(iFormCollection["Year"]);
                var academicYearSectionId = Convert.ToInt32(iFormCollection["SectionId"]);

                //saving exam group details
                for (int i = 0; i < iFormCollection.Count; i++)
                {
                    if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]) && !string.IsNullOrEmpty(iFormCollection["StudentPromotionId_" + i]))
                    {
                        StudentPromotion studentPromotion1 = new StudentPromotion();
                        studentPromotion1.PromotedBy = currentUserId;
                        studentPromotion1.StudentId = Convert.ToInt32(iFormCollection["StudentId_" + i]);
                        
                        //looking for next promotion, if it is next semester or next year 
                        studentPromotion1.AcademicYearSectionId = Convert.ToInt32(iFormCollection["SectionId3"]);
                        studentPromotion1.PromotedDate = DateTime.Now;
                        studentPromotion1.IsClassChange = false;
                        
                        studentPromotion1.Status = 1;
                        _context.StudentPromotions.Add(studentPromotion1);
                        int pass2 = await _context.SaveChangesAsync();
                    }
                }

                ViewData["yearList"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text", year);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Registrar/StudentPromotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentPromotions == null)
            {
                return NotFound();
            }

            var studentPromotion = await _context.StudentPromotions.FindAsync(id);
            if (studentPromotion == null)
            {
                return NotFound();
            }
            ViewData["AcademicYearSectionId"] = new SelectList(_context.AcademicYearSections, "Id", "Id", studentPromotion.AcademicYearSectionId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "BloogGroup", studentPromotion.StudentId);
            return View(studentPromotion);
        }

        // POST: Registrar/StudentPromotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,PromotedDate,PromotedBy,IsClassChange")] StudentPromotion studentPromotion)
        {
            if (id != studentPromotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentPromotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentPromotionExists(studentPromotion.Id))
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
            ViewData["AcademicYearSectionId"] = new SelectList(_context.AcademicYearSections, "Id", "Id", studentPromotion.AcademicYearSectionId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "BloogGroup", studentPromotion.StudentId);
            return View(studentPromotion);
        }

        // GET: Registrar/StudentPromotions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentPromotions == null)
            {
                return NotFound();
            }

            var studentPromotion = await _context.StudentPromotions
                .Include(s => s.AcademicYearSection)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentPromotion == null)
            {
                return NotFound();
            }

            return View(studentPromotion);
        }

        // POST: Registrar/StudentPromotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentPromotions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentPromotions'  is null.");
            }
            var studentPromotion = await _context.StudentPromotions.FindAsync(id);
            if (studentPromotion != null)
            {
                _context.StudentPromotions.Remove(studentPromotion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentPromotionExists(int id)
        {
          return _context.StudentPromotions.Any(e => e.Id == id);
        }
    }
}
