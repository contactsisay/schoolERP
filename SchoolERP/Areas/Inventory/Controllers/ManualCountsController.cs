using BALibrary.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ManualCountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManualCountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory/ManualCounts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ManualCounts.Include(m => m.ProductBatch);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inventory/ManualCounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null || _context.ManualCounts == null)
            {
                return NotFound();
            }

            var manualCount = await _context.ManualCounts
                .Include(m => m.ProductBatch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manualCount == null)
            {
                return NotFound();
            }

            return View(manualCount);
        }

        // GET: Inventory/ManualCounts/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo");
            ViewData["ProductBatches"] = _context.ProductBatches.Include(p => p.Product).Where(p => p.Status == 1).ToList();
            return View();
        }

        // POST: Inventory/ManualCounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductBatchId,CountValue,CountedDate,Description")] ManualCount manualCount, IFormCollection formCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            List<ProductBatch> productBatches = _context.ProductBatches.Include(p => p.Product).Where(p => p.Status == 1).ToList();
            foreach (ProductBatch pb in productBatches)
            {
                if (!string.IsNullOrEmpty(formCollection["productbatchid_" + pb.Id]) && Convert.ToInt32(formCollection["productbatchid_" + pb.Id]) > 0)
                {
                    ManualCount mCount = new ManualCount();
                    mCount.ProductBatchId = pb.Id;
                    mCount.CountedDate = DateTime.Today;
                    mCount.EmployeeId = currentUserId;
                    mCount.Status = 1;

                    if (!string.IsNullOrEmpty(formCollection["count_value_" + pb.Id]))
                        mCount.CountValue = Convert.ToInt32(formCollection["count_value_" + pb.Id]);
                    else
                        mCount.CountValue = 0;

                    if (!string.IsNullOrEmpty(formCollection["description_" + pb.Id]))
                        mCount.Description = formCollection["description_" + pb.Id].ToString();
                    else
                        mCount.Description = string.Empty;

                    _context.Add(mCount);
                    int pass = await _context.SaveChangesAsync();

                    if (pass > 0)
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Saved!");
                    }
                }
            }

            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo", manualCount.ProductBatchId);
            ViewData["ProductBatches"] = _context.ProductBatches.Include(p => p.Product).Where(p => p.Status == 1).ToList();
            return RedirectToAction(nameof(Index));
        }

        // GET: Inventory/ManualCounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.ManualCounts == null)
            {
                return NotFound();
            }

            var manualCount = await _context.ManualCounts.FindAsync(id);
            if (manualCount == null)
            {
                return NotFound();
            }
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo", manualCount.ProductBatchId);
            return View(manualCount);
        }

        // POST: Inventory/ManualCounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductBatchId,CountValue,CountedDate,Description")] ManualCount manualCount)
        {
            if (id != manualCount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manualCount);
                    int pass = await _context.SaveChangesAsync();

                    if (pass > 0)
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Updated Successfully!");
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Updated!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManualCountExists(manualCount.Id))
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
            ViewData["ProductBatchId"] = new SelectList(_context.ProductBatches, "Id", "BatchNo", manualCount.ProductBatchId);
            return View(manualCount);
        }

        // GET: Inventory/ManualCounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.ManualCounts == null)
            {
                return NotFound();
            }

            var manualCount = await _context.ManualCounts
                .Include(m => m.ProductBatch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manualCount == null)
            {
                return NotFound();
            }

            return View(manualCount);
        }

        // POST: Inventory/ManualCounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ManualCounts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ManualCounts'  is null.");
            }
            var manualCount = await _context.ManualCounts.FindAsync(id);
            if (manualCount != null)
            {
                _context.ManualCounts.Remove(manualCount);
            }

            int pass = await _context.SaveChangesAsync();

            if (pass > 0)
            {
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Deleted Successfully!");
            }
            else
            {
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Deleted!");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ManualCountExists(int id)
        {
            return _context.ManualCounts.Any(e => e.Id == id);
        }
    }
}
