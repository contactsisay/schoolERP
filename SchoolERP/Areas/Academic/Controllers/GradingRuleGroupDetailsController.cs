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
    public class GradingRuleGroupDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradingRuleGroupDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/GradingRuleGroupDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GradingRuleGroupDetails.Include(g => g.GradingRule).Include(g => g.GradingRuleGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Academic/GradingRuleGroupDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GradingRuleGroupDetails == null)
            {
                return NotFound();
            }

            var gradingRuleGroupDetail = await _context.GradingRuleGroupDetails
                .Include(g => g.GradingRule)
                .Include(g => g.GradingRuleGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gradingRuleGroupDetail == null)
            {
                return NotFound();
            }

            return View(gradingRuleGroupDetail);
        }

        // GET: Academic/GradingRuleGroupDetails/Create
        public IActionResult Create()
        {
            ViewData["GradingRuleId"] = new SelectList(_context.GradingRules, "Id", "Name");
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Id");
            return View();
        }

        // POST: Academic/GradingRuleGroupDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GradingRuleGroupId,GradingRuleId")] GradingRuleGroupDetail gradingRuleGroupDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gradingRuleGroupDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GradingRuleId"] = new SelectList(_context.GradingRules, "Id", "Name", gradingRuleGroupDetail.GradingRuleId);
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Id", gradingRuleGroupDetail.GradingRuleGroupId);
            return View(gradingRuleGroupDetail);
        }

        // GET: Academic/GradingRuleGroupDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GradingRuleGroupDetails == null)
            {
                return NotFound();
            }

            var gradingRuleGroupDetail = await _context.GradingRuleGroupDetails.FindAsync(id);
            if (gradingRuleGroupDetail == null)
            {
                return NotFound();
            }
            ViewData["GradingRuleId"] = new SelectList(_context.GradingRules, "Id", "Name", gradingRuleGroupDetail.GradingRuleId);
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Id", gradingRuleGroupDetail.GradingRuleGroupId);
            return View(gradingRuleGroupDetail);
        }

        // POST: Academic/GradingRuleGroupDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GradingRuleGroupId,GradingRuleId")] GradingRuleGroupDetail gradingRuleGroupDetail)
        {
            if (id != gradingRuleGroupDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gradingRuleGroupDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradingRuleGroupDetailExists(gradingRuleGroupDetail.Id))
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
            ViewData["GradingRuleId"] = new SelectList(_context.GradingRules, "Id", "Name", gradingRuleGroupDetail.GradingRuleId);
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Id", gradingRuleGroupDetail.GradingRuleGroupId);
            return View(gradingRuleGroupDetail);
        }

        // GET: Academic/GradingRuleGroupDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GradingRuleGroupDetails == null)
            {
                return NotFound();
            }

            var gradingRuleGroupDetail = await _context.GradingRuleGroupDetails
                .Include(g => g.GradingRule)
                .Include(g => g.GradingRuleGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gradingRuleGroupDetail == null)
            {
                return NotFound();
            }

            return View(gradingRuleGroupDetail);
        }

        // POST: Academic/GradingRuleGroupDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GradingRuleGroupDetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GradingRuleGroupDetails'  is null.");
            }
            var gradingRuleGroupDetail = await _context.GradingRuleGroupDetails.FindAsync(id);
            if (gradingRuleGroupDetail != null)
            {
                _context.GradingRuleGroupDetails.Remove(gradingRuleGroupDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradingRuleGroupDetailExists(int id)
        {
          return _context.GradingRuleGroupDetails.Any(e => e.Id == id);
        }
    }
}
