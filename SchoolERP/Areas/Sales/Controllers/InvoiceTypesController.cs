using BALibrary.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class InvoiceTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales/InvoiceTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.InvoiceTypes.ToListAsync());
        }

        // GET: Sales/InvoiceTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null || _context.InvoiceTypes == null)
            {
                return NotFound();
            }

            var invoiceType = await _context.InvoiceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoiceType == null)
            {
                return NotFound();
            }

            return View(invoiceType);
        }

        // GET: Sales/InvoiceTypes/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }

        // POST: Sales/InvoiceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] InvoiceType invoiceType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoiceType);
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
                return RedirectToAction(nameof(Index));
            }
            return View(invoiceType);
        }

        // GET: Sales/InvoiceTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.InvoiceTypes == null)
            {
                return NotFound();
            }

            var invoiceType = await _context.InvoiceTypes.FindAsync(id);
            if (invoiceType == null)
            {
                return NotFound();
            }
            return View(invoiceType);
        }

        // POST: Sales/InvoiceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] InvoiceType invoiceType)
        {
            if (id != invoiceType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoiceType);
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
                    if (!InvoiceTypeExists(invoiceType.Id))
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
            return View(invoiceType);
        }

        // GET: Sales/InvoiceTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.InvoiceTypes == null)
            {
                return NotFound();
            }

            var invoiceType = await _context.InvoiceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoiceType == null)
            {
                return NotFound();
            }

            return View(invoiceType);
        }

        // POST: Sales/InvoiceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InvoiceTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.InvoiceTypes'  is null.");
            }
            var invoiceType = await _context.InvoiceTypes.FindAsync(id);
            if (invoiceType != null)
            {
                _context.InvoiceTypes.Remove(invoiceType);
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

        private bool InvoiceTypeExists(int id)
        {
            return _context.InvoiceTypes.Any(e => e.Id == id);
        }
    }
}
