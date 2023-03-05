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
    public class GradingRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradingRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/GradingRules
        public async Task<IActionResult> Index()
        {
              return View(await _context.GradingRules.ToListAsync());
        }

        // GET: Academic/GradingRules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GradingRules == null)
            {
                return NotFound();
            }

            var gradingRule = await _context.GradingRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gradingRule == null)
            {
                return NotFound();
            }

            return View(gradingRule);
        }

        // GET: Academic/GradingRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Academic/GradingRules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,MarkFrom,Middle,MarkTo")] GradingRule gradingRule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gradingRule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gradingRule);
        }

        // GET: Academic/GradingRules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GradingRules == null)
            {
                return NotFound();
            }

            var gradingRule = await _context.GradingRules.FindAsync(id);
            if (gradingRule == null)
            {
                return NotFound();
            }
            return View(gradingRule);
        }

        // POST: Academic/GradingRules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,MarkFrom,Middle,MarkTo")] GradingRule gradingRule)
        {
            if (id != gradingRule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gradingRule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradingRuleExists(gradingRule.Id))
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
            return View(gradingRule);
        }

        // GET: Academic/GradingRules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GradingRules == null)
            {
                return NotFound();
            }

            var gradingRule = await _context.GradingRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gradingRule == null)
            {
                return NotFound();
            }

            return View(gradingRule);
        }

        // POST: Academic/GradingRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GradingRules == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GradingRules'  is null.");
            }
            var gradingRule = await _context.GradingRules.FindAsync(id);
            if (gradingRule != null)
            {
                _context.GradingRules.Remove(gradingRule);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradingRuleExists(int id)
        {
          return _context.GradingRules.Any(e => e.Id == id);
        }
    }
}
