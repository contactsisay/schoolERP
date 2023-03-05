using BALibrary.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Report.Controllers
{
    [Area("Report")]
    public class SalesReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Report/SalesReports
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
            var queryResult = from inv in _context.Invoices.Include(a => a.InvoiceType).Include(a => a.Customer).Where(a => a.Status != 0)
                              join invD in _context.InvoiceDetails on inv.Id equals invD.InvoiceId
                              join pb in _context.ProductBatches.Include(pb => pb.Product) on invD.ProductBatchId equals pb.Id
                              join pc in _context.ProductCategories on pb.Product.ProductCategoryId equals pc.Id
                              join e in _context.Employees on inv.EmployeeId equals e.Id
                              select new
                              {
                                  inv.InvoiceNo,
                                  InvoiceTypeName = inv.InvoiceType.Name,
                                  ProductName = pb.Product.Name,
                                  ProductTypeName = pc.Name,
                                  ProductCode = pb.Product.Code,
                                  CustomerTINNo = inv.Customer.TINNo,
                                  CustomerName = inv.Customer.Name,
                                  ProductBatchNo = pb.BatchNo,
                                  inv.InvoiceDate,
                                  EmployeeFullName = e.FirstName + e.MiddleName + e.LastName,
                                  e.Gender,
                                  ItemQuantity = invD.Quantity,
                                  ItemSellingPrice = invD.SellingPrice,
                                  InvoiceRowTotal = invD.RowTotal
                              };

            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", EmployeeId);
            ViewData["Title"] = "Sales Report";
            ViewData["queryResult"] = queryResult;

            return View();
        }

        // GET: Report/SalesReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesReport == null)
            {
                return NotFound();
            }

            return View(salesReport);
        }

        // GET: Report/SalesReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Report/SalesReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FromDate,ToDate,EmployeeId")] SalesReport salesReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesReport);
        }

        // GET: Report/SalesReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports.FindAsync(id);
            if (salesReport == null)
            {
                return NotFound();
            }
            return View(salesReport);
        }

        // POST: Report/SalesReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromDate,ToDate,EmployeeId")] SalesReport salesReport)
        {
            if (id != salesReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesReportExists(salesReport.Id))
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
            return View(salesReport);
        }

        // GET: Report/SalesReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesReport == null)
            {
                return NotFound();
            }

            return View(salesReport);
        }

        // POST: Report/SalesReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SalesReports == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalesReports'  is null.");
            }
            var salesReport = await _context.SalesReports.FindAsync(id);
            if (salesReport != null)
            {
                _context.SalesReports.Remove(salesReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesReportExists(int id)
        {
            return _context.SalesReports.Any(e => e.Id == id);
        }
    }
}
