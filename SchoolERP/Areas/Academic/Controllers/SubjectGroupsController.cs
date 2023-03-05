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
    public class SubjectGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/SubjectGroups
        public async Task<IActionResult> Index()
        {
              return View(await _context.SubjectGroups.ToListAsync());
        }

        // GET: Academic/SubjectGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SubjectGroups == null)
            {
                return NotFound();
            }

            var subjectGroup = await _context.SubjectGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectGroup == null)
            {
                return NotFound();
            }

            return View(subjectGroup);
        }

        // GET: Academic/SubjectGroups/Create
        public IActionResult Create()
        {
            ViewData["Subjects"] = _context.Subjects.ToList();
            return View();
        }

        // POST: Academic/SubjectGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] SubjectGroup subjectGroup, IFormCollection iFormCollection)
        {
            if (ModelState.IsValid)
            {
                //saving subject group
                _context.Add(subjectGroup);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    //saving subject group details
                    for (int i = 0; i < iFormCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]) && !string.IsNullOrEmpty(iFormCollection["subject_id_" + i]))
                        {
                            SubjectGroupDetail subjectGroupDetail = new SubjectGroupDetail();
                            subjectGroupDetail.SubjectGroupId = subjectGroup.Id;
                            subjectGroupDetail.SubjectId = Convert.ToInt32(iFormCollection["subject_id_" + i]);
                            subjectGroupDetail.Status = 1;
                            _context.SubjectGroupDetails.Add(subjectGroupDetail);
                            int pass2 = await _context.SaveChangesAsync();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(subjectGroup);
        }

        // GET: Academic/SubjectGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubjectGroups == null)
            {
                return NotFound();
            }

            var subjectGroup = await _context.SubjectGroups.FindAsync(id);
            if (subjectGroup == null)
            {
                return NotFound();
            }
            return View(subjectGroup);
        }

        // POST: Academic/SubjectGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] SubjectGroup subjectGroup, IFormCollection iFormCollection)
        {
            if (id != subjectGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectGroup);
                    int pass = await _context.SaveChangesAsync();

                    //clean the existing
                    var subjectGroupDetails = _context.SubjectGroupDetails.Where(a => a.SubjectGroupId == subjectGroup.Id).ToList();
                    _context.SubjectGroupDetails.RemoveRange(subjectGroupDetails);
                    _context.SaveChanges();

                    //updating subject group details
                    for (int i = 0; i < iFormCollection.Count; i++) 
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]))
                        {
                            SubjectGroupDetail ayc = new SubjectGroupDetail();
                            ayc.SubjectGroupId = subjectGroup.Id;
                            ayc.SubjectId = Convert.ToInt32(iFormCollection["SubjectId_" + i]);
                            ayc.Description = iFormCollection["Description_" + i].ToString();
                            ayc.Status = 1;
                            _context.SubjectGroupDetails.Add(ayc);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectGroupExists(subjectGroup.Id))
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
            return View(subjectGroup);
        }

        // GET: Academic/SubjectGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubjectGroups == null)
            {
                return NotFound();
            }

            var subjectGroup = await _context.SubjectGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectGroup == null)
            {
                return NotFound();
            }

            return View(subjectGroup);
        }

        // POST: Academic/SubjectGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubjectGroups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubjectGroups'  is null.");
            }
            var subjectGroup = await _context.SubjectGroups.FindAsync(id);
            if (subjectGroup != null)
            {
                _context.SubjectGroups.Remove(subjectGroup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectGroupExists(int id)
        {
          return _context.SubjectGroups.Any(e => e.Id == id);
        }
    }
}
