using BALibrary.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Report.Controllers
{
    [Area("Report")]
    public class StockReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report/StockReports
        public async Task<IActionResult> Index()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName");
            ViewData["Title"] = "Report Filter";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreviewReport(string? FromDate, string? ToDate, string? EmployeeId)
        {
            var queryResult = from p in _context.Products.Include(p => p.ProductCategory).Include(p => p.Uom)
                              join pb in _context.ProductBatches on p.Id equals pb.ProductId
                              join s in _context.Stocks on pb.Id equals s.ProductBatchId
                              join e in _context.Employees on pb.EmployeeId equals e.Id
                              select new
                              {
                                  ProductBatchNo = s.ProductBatch.BatchNo,
                                  ProductName = p.Name,
                                  ProductTypeName = p.ProductCategory.Name,
                                  ProductCode = p.Code,
                                  ProductUomNme = p.Uom.Name,
                                  EmployeeFullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                  s.InitialQuantity,
                                  s.SoldQuantity,
                                  StockBalance = s.CurrentQuantity,
                                  s.ActionTaken,
                                  s.Description,
                                  StockUpdatedAt = s.UpdatedAt,
                                  p.MinimumOrderLevel,
                                  UnderStock = (s.CurrentQuantity <= p.MinimumOrderLevel ? "YES" : "NO"),
                                  pb.ExpirationDate,
                                  pb.BestBefore,
                                  pb.ManufacturedDate,
                              };

            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", EmployeeId);
            ViewData["Title"] = "Stock Report";
            ViewData["queryResult"] = queryResult;

            return View();
        }

        // GET: Report/StockReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StockReports == null)
            {
                return NotFound();
            }

            var stockReport = await _context.StockReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockReport == null)
            {
                return NotFound();
            }

            return View(stockReport);
        }

        // GET: Report/StockReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Report/StockReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FromDate,ToDate,EmployeeId")] StockReport stockReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stockReport);
        }

        // GET: Report/StockReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StockReports == null)
            {
                return NotFound();
            }

            var stockReport = await _context.StockReports.FindAsync(id);
            if (stockReport == null)
            {
                return NotFound();
            }
            return View(stockReport);
        }

        // POST: Report/StockReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromDate,ToDate,EmployeeId")] StockReport stockReport)
        {
            if (id != stockReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockReportExists(stockReport.Id))
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
            return View(stockReport);
        }

        // GET: Report/StockReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StockReports == null)
            {
                return NotFound();
            }

            var stockReport = await _context.StockReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockReport == null)
            {
                return NotFound();
            }

            return View(stockReport);
        }

        // POST: Report/StockReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StockReports == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StockReports'  is null.");
            }
            var stockReport = await _context.StockReports.FindAsync(id);
            if (stockReport != null)
            {
                _context.StockReports.Remove(stockReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockReportExists(int id)
        {
            return _context.StockReports.Any(e => e.Id == id);
        }
    }
}
