using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Inventory;
using SchoolERP.Data;

namespace SchoolERP.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class PackageItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PackageItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory/PackageItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PackageItems.Include(p => p.Package).Include(p => p.ProductBatch);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inventory/PackageItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PackageItems == null)
            {
                return NotFound();
            }

            var packageItem = await _context.PackageItems
                .Include(p => p.Package)
                .Include(p => p.ProductBatch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packageItem == null)
            {
                return NotFound();
            }

            return View(packageItem);
        }

        // GET: Inventory/PackageItems/Create
        public IActionResult Create()
        {
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name");
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo");
            return View();
        }

        // POST: Inventory/PackageItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PackageId,ProductBatchId,Quantity")] PackageItem packageItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(packageItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name", packageItem.PackageId);
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo", packageItem.ProductBatchId);
            return View(packageItem);
        }

        // GET: Inventory/PackageItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PackageItems == null)
            {
                return NotFound();
            }

            var packageItem = await _context.PackageItems.FindAsync(id);
            if (packageItem == null)
            {
                return NotFound();
            }
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name", packageItem.PackageId);
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo", packageItem.ProductBatchId);
            return View(packageItem);
        }

        // POST: Inventory/PackageItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PackageId,ProductBatchId,Quantity")] PackageItem packageItem)
        {
            if (id != packageItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(packageItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageItemExists(packageItem.Id))
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
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name", packageItem.PackageId);
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo", packageItem.ProductBatchId);
            return View(packageItem);
        }

        // GET: Inventory/PackageItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PackageItems == null)
            {
                return NotFound();
            }

            var packageItem = await _context.PackageItems
                .Include(p => p.Package)
                .Include(p => p.ProductBatch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packageItem == null)
            {
                return NotFound();
            }

            return View(packageItem);
        }

        // POST: Inventory/PackageItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PackageItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PackageItems'  is null.");
            }
            var packageItem = await _context.PackageItems.FindAsync(id);
            if (packageItem != null)
            {
                _context.PackageItems.Remove(packageItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageItemExists(int id)
        {
          return _context.PackageItems.Any(e => e.Id == id);
        }
    }
}
