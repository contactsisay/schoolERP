using BALibrary.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ProductBatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductBatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory/ProductBatches
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductBatches.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inventory/ProductBatches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductBatches == null)
            {
                return NotFound();
            }

            var productBatch = await _context.ProductBatches
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productBatch == null)
            {
                return NotFound();
            }

            return View(productBatch);
        }

        // GET: Inventory/ProductBatches/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code");
            return View();
        }

        // POST: Inventory/ProductBatches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,BatchNo,BestBefore,ManufacturedDate,ExpirationDate,PurchasingPrice,SellingPrice,IsSellable,IsTaxable")] ProductBatch productBatch)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if (ModelState.IsValid)
            {
                productBatch.EmployeeId = currentUserId;

                _context.Add(productBatch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", productBatch.ProductId);
            return View(productBatch);
        }

        // GET: Inventory/ProductBatches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductBatches == null)
            {
                return NotFound();
            }

            var productBatch = await _context.ProductBatches.FindAsync(id);
            if (productBatch == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", productBatch.ProductId);
            return View(productBatch);
        }

        // POST: Inventory/ProductBatches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,BatchNo,BestBefore,ManufacturedDate,ExpirationDate,PurchasingPrice,SellingPrice,IsSellable,IsTaxable")] ProductBatch productBatch)
        {
            if (id != productBatch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productBatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductBatchExists(productBatch.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", productBatch.ProductId);
            return View(productBatch);
        }

        // GET: Inventory/ProductBatches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductBatches == null)
            {
                return NotFound();
            }

            var productBatch = await _context.ProductBatches
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productBatch == null)
            {
                return NotFound();
            }

            return View(productBatch);
        }

        // POST: Inventory/ProductBatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductBatches == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductBatches'  is null.");
            }
            var productBatch = await _context.ProductBatches.FindAsync(id);
            if (productBatch != null)
            {
                _context.ProductBatches.Remove(productBatch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductBatchExists(int id)
        {
            return _context.ProductBatches.Any(e => e.Id == id);
        }
    }
}
