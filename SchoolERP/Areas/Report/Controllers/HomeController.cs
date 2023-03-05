using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Report.Controllers
{
    [Area("Report")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Reports Home";
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }

        public async Task<IActionResult> SalesReport(string? FromDate, string? ToDate, string? EmployeeId)
        {
            var queryResult = from d in _context.InvoiceDetails
                              join dr in _context.Invoices on d.InvoiceId equals dr.Id
                              select new
                              {
                                  dr.InvoiceDate,
                                  dr.InvoiceNo,
                                  dr.InvoiceTypeId,
                                  dr.CustomerId,
                                  dr.EmployeeId,
                                  d.ProductBatchId,
                                  d.Quantity,
                                  d.SellingPrice,
                                  d.RowTotal
                              };

            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", EmployeeId);
            ViewData["Title"] = "Sales Report";

            ViewData["queryResult"] = queryResult;
            return View();
        }

    }
}
