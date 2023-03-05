using BALibrary.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ProductCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory/ProductCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductCategories.ToListAsync());
        }

        // GET: Inventory/ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: Inventory/ProductCategories/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }

        // POST: Inventory/ProductCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
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
            return View(productCategory);
        }

        // GET: Inventory/ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(productCategory);
        }

        // POST: Inventory/ProductCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
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
                    if (!ProductCategoryExists(productCategory.Id))
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
            return View(productCategory);
        }

        // GET: Inventory/ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: Inventory/ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductCategories'  is null.");
            }
            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory != null)
            {
                _context.ProductCategories.Remove(productCategory);
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

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.Id == id);
        }
    }
}
