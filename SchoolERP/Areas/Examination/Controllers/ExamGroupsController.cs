using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Examination;
using SchoolERP.Data;
using BALibrary.Academic;

namespace SchoolERP.Areas.Examination.Controllers
{
    [Area("Examination")]
    public class ExamGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Examination/ExamGroups
        public async Task<IActionResult> Index()
        {
              return View(await _context.ExamGroups.ToListAsync());
        }

        // GET: Examination/ExamGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ExamGroups == null)
            {
                return NotFound();
            }

            var examGroup = await _context.ExamGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examGroup == null)
            {
                return NotFound();
            }

            return View(examGroup);
        }

        // GET: Examination/ExamGroups/Create
        public IActionResult Create()
        {
            ViewData["Exams"] = _context.Exams.ToList();
            return View();
        }

        // POST: Examination/ExamGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,MaximumMark,PassingMark")] ExamGroup examGroup, IFormCollection iFormCollection)
        {
            if (ModelState.IsValid)
            {
                //saving exam group
                _context.Add(examGroup);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    //saving exam group details
                    for (int i = 0; i < iFormCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]) && !string.IsNullOrEmpty(iFormCollection["exam_id_" + i]))
                        {
                            ExamGroupDetail examGroupDetail = new ExamGroupDetail();
                            examGroupDetail.ExamGroupId = examGroup.Id;
                            examGroupDetail.ExamId = Convert.ToInt32(iFormCollection["exam_id_" + i]);
                            examGroupDetail.Status = 1;
                            _context.ExamGroupDetails.Add(examGroupDetail);
                            int pass2 = await _context.SaveChangesAsync();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(examGroup);
        }

        // GET: Examination/ExamGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ExamGroups == null)
            {
                return NotFound();
            }

            var examGroup = await _context.ExamGroups.FindAsync(id);
            if (examGroup == null)
            {
                return NotFound();
            }
            return View(examGroup);
        }

        // POST: Examination/ExamGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,MaximumMark,PassingMark")] ExamGroup examGroup, IFormCollection iFormCollection)
        {
            if (id != examGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examGroup);
                    int pass = await _context.SaveChangesAsync();
                                        
                    //clean the existing
                    var items = _context.ExamGroupDetails.Where(a => a.ExamGroupId == examGroup.Id).ToList();
                    _context.ExamGroupDetails.RemoveRange(items);
                    _context.SaveChanges();

                    //updating
                    for (int i = 0; i < iFormCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]))
                        {
                            ExamGroupDetail ayc = new ExamGroupDetail();
                            ayc.ExamGroupId = examGroup.Id;
                            ayc.ExamId = Convert.ToInt32(iFormCollection["ExamId_" + i]);
                            ayc.Status = 1;
                            _context.ExamGroupDetails.Add(ayc);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamGroupExists(examGroup.Id))
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
            return View(examGroup);
        }

        // GET: Examination/ExamGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ExamGroups == null)
            {
                return NotFound();
            }

            var examGroup = await _context.ExamGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examGroup == null)
            {
                return NotFound();
            }

            return View(examGroup);
        }

        // POST: Examination/ExamGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ExamGroups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ExamGroups'  is null.");
            }
            var examGroup = await _context.ExamGroups.FindAsync(id);
            if (examGroup != null)
            {
                _context.ExamGroups.Remove(examGroup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamGroupExists(int id)
        {
          return _context.ExamGroups.Any(e => e.Id == id);
        }
    }
}
