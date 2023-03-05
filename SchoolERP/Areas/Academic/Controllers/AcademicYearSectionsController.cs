using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Academic;
using SchoolERP.Data;
using System.ComponentModel;

namespace SchoolERP.Areas.Academic.Controllers
{
    [Area("Academic")]
    public class AcademicYearSectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicYearSectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/AcademicYearSections
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "AcademicYears");

            List<AcademicYearSection> applicationDbContext = await _context.AcademicYearSections.Include(a => a.Section).ToListAsync();
            if (id != null) 
                applicationDbContext = applicationDbContext.Where(a => a.AcademicYearId == id).ToList();

            var academicYear = _context.AcademicYears.Find(id);
            ViewData["AcademicYear"] = academicYear;
            return View(applicationDbContext);
        }

        // GET: Academic/AcademicYearSections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AcademicYearSections == null)
            {
                return NotFound();
            }

            var academicYearSection = await _context.AcademicYearSections
                .Include(a => a.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicYearSection == null)
            {
                return NotFound();
            }

            return View(academicYearSection);
        }

        // GET: Academic/AcademicYearSections/Create
        public IActionResult Create(int id)
        {
            ViewData["AcademicYearId"] = id;
            ViewData["Sections"] = _context.Sections.ToList();
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNo");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "IDNo");
            return View();
        }

        // POST: Academic/AcademicYearSections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AcademicYearId,SectionId,NoOfStudent,StudentId,RoomId")] AcademicYearSection academicYearSection, IFormCollection iFormCollection)
        {
            if (!string.IsNullOrEmpty(iFormCollection["AcademicYearId"]))
            {
                int AcademicYearId = Convert.ToInt32(iFormCollection["AcademicYearId"]);
                AcademicYear academicYear = _context.AcademicYears.Find(AcademicYearId);
                if (academicYear != null) {
                    for (int i = 0; i < iFormCollection.Count; i++) {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]) && !string.IsNullOrEmpty(iFormCollection["SectionId_" + i]))
                        {
                            if (string.IsNullOrEmpty(iFormCollection["AcademicYearSectionId_" + i]))
                            {
                                AcademicYearSection ayc = new AcademicYearSection();
                                ayc.AcademicYearId = AcademicYearId;
                                ayc.SectionId = Convert.ToInt32(iFormCollection["SectionId_" + i]);
                                ayc.NoOfStudent = Convert.ToInt32(iFormCollection["NoOfStudent_" + i]);
                                ayc.StudentId = Convert.ToInt32(iFormCollection["StudentId_" + i]) == 0 ? null : Convert.ToInt32(iFormCollection["StudentId_" + i]);
                                ayc.RoomId = Convert.ToInt32(iFormCollection["RoomId_" + i]) == 0 ? null : Convert.ToInt32(iFormCollection["RoomId_" + i]);
                                ayc.Status = 1;
                                _context.AcademicYearSections.Add(ayc);
                                await _context.SaveChangesAsync();
                            }
                        }
                        else 
                        {
                            if (!string.IsNullOrEmpty(iFormCollection["AcademicYearSectionId_" + i]) && Convert.ToInt32(iFormCollection["AcademicYearSectionId_" + i]) > 0) 
                            {
                                int aysId = Convert.ToInt32(iFormCollection["AcademicYearSectionId_" + i]);
                                _context.AcademicYearSections.Remove(_context.AcademicYearSections.Find(aysId));
                                _context.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index), new { id = AcademicYearId  });
                }
            }

            ViewData["Sections"] = _context.Sections.ToList();
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "IDNo", academicYearSection.StudentId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNo", academicYearSection.RoomId);
            return View(academicYearSection);
        }

        // GET: Academic/AcademicYearSections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AcademicYearSections == null)
            {
                return NotFound();
            }

            var academicYearSection = await _context.AcademicYearSections.FindAsync(id);
            if (academicYearSection == null)
            {
                return NotFound();
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", academicYearSection.SectionId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNo", academicYearSection.RoomId);
            return View(academicYearSection);
        }

        // POST: Academic/AcademicYearSections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AcademicYearId,SectionId,NoOfStudent,StudentId,RoomId")] AcademicYearSection academicYearSection)
        {
            if (id != academicYearSection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicYearSection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicYearSectionExists(academicYearSection.Id))
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
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", academicYearSection.SectionId);
            return View(academicYearSection);
        }

        // GET: Academic/AcademicYearSections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AcademicYearSections == null)
            {
                return NotFound();
            }

            var academicYearSection = await _context.AcademicYearSections
                .Include(a => a.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicYearSection == null)
            {
                return NotFound();
            }

            return View(academicYearSection);
        }

        // POST: Academic/AcademicYearSections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AcademicYearSections == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AcademicYearSections'  is null.");
            }
            var academicYearSection = await _context.AcademicYearSections.FindAsync(id);
            if (academicYearSection != null)
            {
                _context.AcademicYearSections.Remove(academicYearSection);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicYearSectionExists(int id)
        {
          return _context.AcademicYearSections.Any(e => e.Id == id);
        }
    }
}
