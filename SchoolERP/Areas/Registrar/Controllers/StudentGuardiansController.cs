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
    public class StudentGuardiansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentGuardiansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/StudentGuardians
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentGuardians.Include(s => s.Relationship);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Registrar/StudentGuardians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentGuardians == null)
            {
                return NotFound();
            }

            var studentGuardian = await _context.StudentGuardians
                .Include(s => s.Relationship)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentGuardian == null)
            {
                return NotFound();
            }

            return View(studentGuardian);
        }

        // GET: Registrar/StudentGuardians/Create
        public IActionResult Create()
        {
            ViewData["RelationshipId"] = new SelectList(_context.Relationships, "Id", "Name");
            return View();
        }

        // POST: Registrar/StudentGuardians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,RelationshipId,FullName,MobileNo,EmailAddress,Occupation,PhotoPath,Password")] StudentGuardian studentGuardian)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentGuardian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RelationshipId"] = new SelectList(_context.Relationships, "Id", "Name", studentGuardian.RelationshipId);
            return View(studentGuardian);
        }

        // GET: Registrar/StudentGuardians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentGuardians == null)
            {
                return NotFound();
            }

            var studentGuardian = await _context.StudentGuardians.FindAsync(id);
            if (studentGuardian == null)
            {
                return NotFound();
            }
            ViewData["RelationshipId"] = new SelectList(_context.Relationships, "Id", "Name", studentGuardian.RelationshipId);
            return View(studentGuardian);
        }

        // POST: Registrar/StudentGuardians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,RelationshipId,FullName,MobileNo,EmailAddress,Occupation,PhotoPath,Password")] StudentGuardian studentGuardian)
        {
            if (id != studentGuardian.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentGuardian);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentGuardianExists(studentGuardian.Id))
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
            ViewData["RelationshipId"] = new SelectList(_context.Relationships, "Id", "Name", studentGuardian.RelationshipId);
            return View(studentGuardian);
        }

        // GET: Registrar/StudentGuardians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentGuardians == null)
            {
                return NotFound();
            }

            var studentGuardian = await _context.StudentGuardians
                .Include(s => s.Relationship)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentGuardian == null)
            {
                return NotFound();
            }

            return View(studentGuardian);
        }

        // POST: Registrar/StudentGuardians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentGuardians == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentGuardians'  is null.");
            }
            var studentGuardian = await _context.StudentGuardians.FindAsync(id);
            if (studentGuardian != null)
            {
                _context.StudentGuardians.Remove(studentGuardian);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentGuardianExists(int id)
        {
          return _context.StudentGuardians.Any(e => e.Id == id);
        }
    }
}
