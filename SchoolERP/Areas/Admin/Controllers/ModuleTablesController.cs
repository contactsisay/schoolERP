using BALibrary.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ModuleTablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ModuleTables
        public async Task<IActionResult> Index()
        {
            return View(await _context.ModuleTables.ToListAsync());
        }

        // GET: Admin/ModuleTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null || _context.ModuleTables == null)
            {
                return NotFound();
            }

            var moduleTable = await _context.ModuleTables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moduleTable == null)
            {
                return NotFound();
            }

            return View(moduleTable);
        }

        // GET: Admin/ModuleTables/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["ModuleId"] = SchoolERP.Models.Common.FillModuleComboBox();
            ViewData["TableNames"] = SchoolERP.Models.Common.GetTableNames();
            return View();
        }

        // POST: Admin/ModuleTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleId,TableName")] ModuleTable moduleTable, IFormCollection iFormCollection)
        {
            if (!string.IsNullOrEmpty(iFormCollection["ModuleId"]))
            {
                ModuleTable mT = new ModuleTable();
                if (iFormCollection.Count > 0)
                {
                    for (var i = 0; i <= iFormCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(iFormCollection["chk_" + i]))
                        {
                            mT = new ModuleTable();
                            mT.ModuleId = (int)Common.GetModuleId(iFormCollection["ModuleId"].ToString());
                            mT.TableName = iFormCollection["table_name_" + i].ToString();

                            _context.Add(mT);
                            int pass = await _context.SaveChangesAsync();
                            if (pass > 0)
                            {

                                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                                return RedirectToAction(nameof(Index));
                            }
                        }
                    }
                }

            }

            HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
            HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saving NOT Successfull!");
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ModuleTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.ModuleTables == null)
            {
                return NotFound();
            }

            var moduleTable = await _context.ModuleTables.FindAsync(id);
            if (moduleTable == null)
            {
                return NotFound();
            }
            return View(moduleTable);
        }

        // POST: Admin/ModuleTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleId,TableName")] ModuleTable moduleTable)
        {
            if (id != moduleTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(moduleTable);
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
                    if (!ModuleTableExists(moduleTable.Id))
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
            return View(moduleTable);
        }

        // GET: Admin/ModuleTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.ModuleTables == null)
            {
                return NotFound();
            }

            var moduleTable = await _context.ModuleTables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moduleTable == null)
            {
                return NotFound();
            }

            return View(moduleTable);
        }

        // POST: Admin/ModuleTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ModuleTables == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ModuleTables'  is null.");
            }
            var moduleTable = await _context.ModuleTables.FindAsync(id);
            if (moduleTable != null)
            {
                _context.ModuleTables.Remove(moduleTable);
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

        private bool ModuleTableExists(int id)
        {
            return _context.ModuleTables.Any(e => e.Id == id);
        }
    }
}
