using BALibrary.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;

namespace SchoolERP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleModuleExceptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleModuleExceptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserModule/RoleModuleExceptions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RoleModuleExceptions.Include(r => r.RoleModule);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserModule/RoleModuleExceptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleModuleException = await _context.RoleModuleExceptions
                .Include(r => r.RoleModule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModuleException == null)
            {
                return NotFound();
            }

            return View(roleModuleException);
        }

        // GET: UserModule/RoleModuleExceptions/Create
        public IActionResult Create()
        {
            ViewData["RoleModuleId"] = new SelectList(_context.RoleModules, "Id", "Id");
            return View();
        }

        // POST: UserModule/RoleModuleExceptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TableName,Browse,Read,Edit,Add,Delete,FullyGranted,FullyDenied,AccessRightName")] RoleModuleException roleModuleException)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roleModuleException);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleModuleId"] = new SelectList(_context.RoleModules, "Id", "Id", roleModuleException.RoleModuleId);
            return View(roleModuleException);
        }

        // GET: UserModule/RoleModuleExceptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleModuleException = await _context.RoleModuleExceptions.FindAsync(id);
            if (roleModuleException == null)
            {
                return NotFound();
            }
            ViewData["RoleModuleId"] = new SelectList(_context.RoleModules, "Id", "Id", roleModuleException.RoleModuleId);
            return View(roleModuleException);
        }

        // POST: UserModule/RoleModuleExceptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TableName,Browse,Read,Edit,Add,Delete,FullyGranted,FullyDenied,AccessRightName")] RoleModuleException roleModuleException)
        {
            if (id != roleModuleException.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roleModuleException);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleModuleExceptionExists(roleModuleException.Id))
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
            ViewData["RoleModuleId"] = new SelectList(_context.RoleModules, "Id", "Id", roleModuleException.RoleModuleId);
            return View(roleModuleException);
        }

        // GET: UserModule/RoleModuleExceptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleModuleException = await _context.RoleModuleExceptions
                .Include(r => r.RoleModule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModuleException == null)
            {
                return NotFound();
            }

            return View(roleModuleException);
        }

        // POST: UserModule/RoleModuleExceptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roleModuleException = await _context.RoleModuleExceptions.FindAsync(id);
            _context.RoleModuleExceptions.Remove(roleModuleException);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleModuleExceptionExists(int id)
        {
            return _context.RoleModuleExceptions.Any(e => e.Id == id);
        }
    }
}
