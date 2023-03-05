using BALibrary.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Report.Controllers
{
    [Area("Report")]
    public class ProfitLossReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfitLossReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report/ProfitLossReports
        public async Task<IActionResult> Index()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName");
            ViewData["Title"] = "Report Filter";
            return View(await _context.ProfitLossReports.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreviewReport(string? FromDate, string? ToDate, string? EmployeeId)
        {
            var queryResult = from inv in _context.Invoices.Include(a => a.InvoiceType).Where(a => a.Status != 0)
                              join invD in _context.InvoiceDetails on inv.Id equals invD.InvoiceId
                              join pb in _context.ProductBatches.Include(pb => pb.Product) on invD.ProductBatchId equals pb.Id
                              join pc in _context.ProductCategories on pb.Product.ProductCategoryId equals pc.Id
                              join s in _context.Stocks on invD.ProductBatchId equals s.ProductBatchId
                              join e in _context.Employees on inv.EmployeeId equals e.Id
                              select new
                              {
                                  ProductName = pb.Product.Name,
                                  ProductTypeName = pc.Name,
                                  ProductCode = pb.Product.Code,
                                  ProductBatchNo = pb.BatchNo,
                                  EmployeeFullName = e.FirstName + e.MiddleName + e.LastName,
                                  e.Gender,
                                  s.InitialQuantity,
                                  s.SoldQuantity,
                                  ItemPurchasingPrice = pb.PurchasingPrice,
                                  TotalExpense = (pb.PurchasingPrice * s.SoldQuantity),
                                  ItemSellingPrice = invD.SellingPrice,
                                  TotalIncome = (invD.SellingPrice * s.SoldQuantity),
                                  ProfitOrLoss = ((invD.SellingPrice * s.SoldQuantity) - (pb.PurchasingPrice * s.SoldQuantity)),
                              };

            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", EmployeeId);
            ViewData["Title"] = "Sales Report";
            ViewData["queryResult"] = queryResult;

            return View();
        }

        // GET: Report/ProfitLossReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProfitLossReports == null)
            {
                return NotFound();
            }

            var profitLossReport = await _context.ProfitLossReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profitLossReport == null)
            {
                return NotFound();
            }

            return View(profitLossReport);
        }

        // GET: Report/ProfitLossReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Report/ProfitLossReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FromDate,ToDate,EmployeeId")] ProfitLossReport profitLossReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profitLossReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profitLossReport);
        }

        // GET: Report/ProfitLossReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProfitLossReports == null)
            {
                return NotFound();
            }

            var profitLossReport = await _context.ProfitLossReports.FindAsync(id);
            if (profitLossReport == null)
            {
                return NotFound();
            }
            return View(profitLossReport);
        }

        // POST: Report/ProfitLossReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromDate,ToDate,EmployeeId")] ProfitLossReport profitLossReport)
        {
            if (id != profitLossReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profitLossReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfitLossReportExists(profitLossReport.Id))
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
            return View(profitLossReport);
        }

        // GET: Report/ProfitLossReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProfitLossReports == null)
            {
                return NotFound();
            }

            var profitLossReport = await _context.ProfitLossReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profitLossReport == null)
            {
                return NotFound();
            }

            return View(profitLossReport);
        }

        // POST: Report/ProfitLossReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProfitLossReports == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProfitLossReports'  is null.");
            }
            var profitLossReport = await _context.ProfitLossReports.FindAsync(id);
            if (profitLossReport != null)
            {
                _context.ProfitLossReports.Remove(profitLossReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfitLossReportExists(int id)
        {
            return _context.ProfitLossReports.Any(e => e.Id == id);
        }
    }
}
