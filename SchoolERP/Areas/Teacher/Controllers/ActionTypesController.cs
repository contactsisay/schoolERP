using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Teacher;
using SchoolERP.Data;

namespace SchoolERP.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class ActionTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActionTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teacher/ActionTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.ActionTypes.ToListAsync());
        }

        // GET: Teacher/ActionTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ActionTypes == null)
            {
                return NotFound();
            }

            var actionType = await _context.ActionTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actionType == null)
            {
                return NotFound();
            }

            return View(actionType);
        }

        // GET: Teacher/ActionTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teacher/ActionTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ActionType actionType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actionType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actionType);
        }

        // GET: Teacher/ActionTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ActionTypes == null)
            {
                return NotFound();
            }

            var actionType = await _context.ActionTypes.FindAsync(id);
            if (actionType == null)
            {
                return NotFound();
            }
            return View(actionType);
        }

        // POST: Teacher/ActionTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ActionType actionType)
        {
            if (id != actionType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actionType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionTypeExists(actionType.Id))
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
            return View(actionType);
        }

        // GET: Teacher/ActionTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ActionTypes == null)
            {
                return NotFound();
            }

            var actionType = await _context.ActionTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actionType == null)
            {
                return NotFound();
            }

            return View(actionType);
        }

        // POST: Teacher/ActionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ActionTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ActionTypes'  is null.");
            }
            var actionType = await _context.ActionTypes.FindAsync(id);
            if (actionType != null)
            {
                _context.ActionTypes.Remove(actionType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActionTypeExists(int id)
        {
          return _context.ActionTypes.Any(e => e.Id == id);
        }
    }
}
