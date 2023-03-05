using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class RelationshipsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RelationshipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/Relationships
        public async Task<IActionResult> Index()
        {
              return View(await _context.Relationships.ToListAsync());
        }

        // GET: Registrar/Relationships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Relationships == null)
            {
                return NotFound();
            }

            var relationship = await _context.Relationships
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relationship == null)
            {
                return NotFound();
            }

            return View(relationship);
        }

        // GET: Registrar/Relationships/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registrar/Relationships/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Relationship relationship)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relationship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(relationship);
        }

        // GET: Registrar/Relationships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Relationships == null)
            {
                return NotFound();
            }

            var relationship = await _context.Relationships.FindAsync(id);
            if (relationship == null)
            {
                return NotFound();
            }
            return View(relationship);
        }

        // POST: Registrar/Relationships/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Relationship relationship)
        {
            if (id != relationship.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relationship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelationshipExists(relationship.Id))
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
            return View(relationship);
        }

        // GET: Registrar/Relationships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Relationships == null)
            {
                return NotFound();
            }

            var relationship = await _context.Relationships
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relationship == null)
            {
                return NotFound();
            }

            return View(relationship);
        }

        // POST: Registrar/Relationships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Relationships == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Relationships'  is null.");
            }
            var relationship = await _context.Relationships.FindAsync(id);
            if (relationship != null)
            {
                _context.Relationships.Remove(relationship);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelationshipExists(int id)
        {
          return _context.Relationships.Any(e => e.Id == id);
        }
    }
}
