using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Academic;
using SchoolERP.Data;
using Microsoft.AspNetCore.Http;

namespace SchoolERP.Areas.Academic.Controllers
{
    [Area("Academic")]
    public class GradingRuleGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradingRuleGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/GradingRuleGroups
        public async Task<IActionResult> Index()
        {
              return View(await _context.GradingRuleGroups.ToListAsync());
        }

        // GET: Academic/GradingRuleGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GradingRuleGroups == null)
            {
                return NotFound();
            }

            var gradingRuleGroup = await _context.GradingRuleGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gradingRuleGroup == null)
            {
                return NotFound();
            }

            return View(gradingRuleGroup);
        }

        // GET: Academic/GradingRuleGroups/Create
        public IActionResult Create()
        {
            ViewData["GradingRules"] = _context.GradingRules.ToList();
            return View();
        }

        // POST: Academic/GradingRuleGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] GradingRuleGroup gradingRuleGroup, IFormCollection iFormCollection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gradingRuleGroup);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    //saving grading rule group details
                    for (int i = 0; i < iFormCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]) && !string.IsNullOrEmpty(iFormCollection["grading_rule_id_" + i]))
                        {
                            GradingRuleGroupDetail gradingRuleGroupDetail = new GradingRuleGroupDetail();
                            gradingRuleGroupDetail.GradingRuleGroupId = gradingRuleGroup.Id;
                            gradingRuleGroupDetail.GradingRuleId = Convert.ToInt32(iFormCollection["grading_rule_id_" + i]);
                            gradingRuleGroupDetail.Status = 1;
                            _context.GradingRuleGroupDetails.Add(gradingRuleGroupDetail);
                            int pass2 = await _context.SaveChangesAsync();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(gradingRuleGroup);
        }

        // GET: Academic/GradingRuleGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GradingRuleGroups == null)
            {
                return NotFound();
            }

            var gradingRuleGroup = await _context.GradingRuleGroups.FindAsync(id);
            if (gradingRuleGroup == null)
            {
                return NotFound();
            }
            return View(gradingRuleGroup);
        }

        // POST: Academic/GradingRuleGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] GradingRuleGroup gradingRuleGroup, IFormCollection iFormCollection)
        {
            if (id != gradingRuleGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gradingRuleGroup);
                    int pass = await _context.SaveChangesAsync();

                    //clean the existing
                    var items = _context.GradingRuleGroupDetails.Where(a => a.GradingRuleGroupId == gradingRuleGroup.Id).ToList();
                    _context.GradingRuleGroupDetails.RemoveRange(items);
                    _context.SaveChanges();

                    //updating
                    for (int i = 0; i < iFormCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]))
                        {
                            GradingRuleGroupDetail ayc = new GradingRuleGroupDetail();
                            ayc.GradingRuleGroupId = gradingRuleGroup.Id;
                            ayc.GradingRuleId = Convert.ToInt32(iFormCollection["GradingRuleId_" + i]);
                            ayc.Status = 1;
                            _context.GradingRuleGroupDetails.Add(ayc);
                            await _context.SaveChangesAsync();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradingRuleGroupExists(gradingRuleGroup.Id))
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
            return View(gradingRuleGroup);
        }

        // GET: Academic/GradingRuleGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GradingRuleGroups == null)
            {
                return NotFound();
            }

            var gradingRuleGroup = await _context.GradingRuleGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gradingRuleGroup == null)
            {
                return NotFound();
            }

            return View(gradingRuleGroup);
        }

        // POST: Academic/GradingRuleGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GradingRuleGroups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GradingRuleGroups'  is null.");
            }
            var gradingRuleGroup = await _context.GradingRuleGroups.FindAsync(id);
            if (gradingRuleGroup != null)
            {
                _context.GradingRuleGroups.Remove(gradingRuleGroup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradingRuleGroupExists(int id)
        {
          return _context.GradingRuleGroups.Any(e => e.Id == id);
        }
    }
}
