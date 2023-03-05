using BALibrary.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleModulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserModule/RoleModules
        public async Task<IActionResult> Index(int id)
        {
            HttpContext.Session.SetInt32("_SelectedId", id);
            ViewData["Roles"] = _context.Roles.Where(r => r.Id == id).ToList();
            ViewData["ModuleNames"] = SchoolERP.Models.Common.GetModuleNames();
            var applicationDbContext = _context.RoleModules.Include(r => r.Role).Where(rm => rm.RoleId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserModule/RoleModules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null)
            {
                return NotFound();
            }

            var roleModule = await _context.RoleModules
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModule == null)
            {
                return NotFound();
            }

            return View(roleModule);
        }

        // GET: UserModule/RoleModules/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRoleModules(IFormCollection iFormCollection)
        {
            int id = (int)HttpContext.Session.GetInt32("_SelectedId");
            if (!string.IsNullOrEmpty(iFormCollection["roleId"]))
            {
                int roleId = Convert.ToInt32(iFormCollection["roleId"]);
                string[] moduleNames = Common.GetModuleNames();
                foreach (string moduleName in moduleNames)
                {
                    string m = iFormCollection[moduleName.ToUpper() + "_" + roleId];
                    if (!string.IsNullOrEmpty(m))
                    {
                        int moduleId = (int)Common.GetModuleId(moduleName);
                        if (moduleId > 0)
                        {
                            List<RoleModule> roleModules = _context.RoleModules.Where(rm => rm.RoleId == roleId).Where(rm => rm.ModuleId == moduleId).ToList();

                            if (roleModules.Count <= 0)
                            {
                                RoleModule roleM = new RoleModule();
                                roleM.RoleId = roleId;
                                roleM.ModuleId = moduleId;
                                _context.RoleModules.Add(roleM);
                                await _context.SaveChangesAsync();
                            }
                        }                        
                    }
                    else
                    {
                        int moduleId = (int)Common.GetModuleId(moduleName);
                        List<RoleModule> roleModules = _context.RoleModules.Where(rm => rm.RoleId == roleId).Where(rm => rm.ModuleId == moduleId).ToList();
                        foreach (RoleModule rm in roleModules)
                        {
                            //Looking in role module exceptions
                            List<RoleModuleException> roleMExceptions = _context.RoleModuleExceptions.Where(r => r.RoleModuleId == rm.Id).ToList();
                            if (roleMExceptions.Count > 0)
                                _context.RoleModuleExceptions.RemoveRange(roleMExceptions);

                            _context.RoleModules.Remove(rm);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Roles", new { area = "Admin" });
        }

        // POST: UserModule/RoleModules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleId,ModuleId")] RoleModule roleModule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roleModule);
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", roleModule.RoleId);
            return View(roleModule);
        }

        public async Task<ActionResult> RoleModuleExceptions(int id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            List<RoleModule> roleModules = _context.RoleModules.Include(r => r.Role).Where(r => r.Id == id).ToList();
            var applicationDbContext = _context.RoleModuleExceptions.Include(r => r.RoleModule).Where(r => r.RoleModuleId == id);
            ViewData["RoleModules"] = roleModules;
            ViewData["ModuleId"] = new SelectList(Common.FillModuleComboBox(), "Value", "Text");
            ViewData["TableNames"] = Common.GetCustomSelectList("ModuleTables", new List<string> { "Id", "TableName" }, " ModuleId=" + roleModules[0].ModuleId);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> SaveRoleModuleExceptions(IFormCollection form)
        {
            int id = (int)HttpContext.Session.GetInt32("_SelectedId");//roleModuleId

            if (!string.IsNullOrEmpty(form["roleModuleId"]))
            {
                int roleModuleId = Convert.ToInt32(form["roleModuleId"].ToString());
                List<RoleModuleException> roleModuleExceptions = _context.RoleModuleExceptions.Where(rme => rme.RoleModuleId == roleModuleId).ToList();

                RoleModule roleModule = _context.RoleModules.Find(roleModuleId);
                List<SelectListItem> moduleTables = Common.GetCustomSelectList("ModuleTables", new List<string> { "Id", "TableName" }, " ModuleId=" + roleModule.ModuleId);

                if (roleModuleExceptions.Count > 0)//exceptions already added for the selected role module (update)
                {
                    foreach (SelectListItem moduleTable in moduleTables)
                    {
                        roleModuleExceptions = roleModuleExceptions.Where(rme => rme.TableName.Equals(moduleTable.Text.ToString())).OrderByDescending(rme => rme.Id).ToList();
                        if (roleModuleExceptions.Count > 0)
                        {
                            RoleModuleException rme = roleModuleExceptions[0];
                            rme.Browse = !string.IsNullOrEmpty(form[moduleTable.Text + "_b"].ToString()) ? true : false;
                            rme.Read = !string.IsNullOrEmpty(form[moduleTable.Text + "_r"].ToString()) ? true : false;
                            rme.Edit = !string.IsNullOrEmpty(form[moduleTable.Text + "_e"].ToString()) ? true : false;
                            rme.Add = !string.IsNullOrEmpty(form[moduleTable.Text + "_a"].ToString()) ? true : false;
                            rme.Delete = !string.IsNullOrEmpty(form[moduleTable.Text + "_d"].ToString()) ? true : false;
                            rme.Status = 1;

                            if (rme.Browse && rme.Read && rme.Edit && rme.Add && rme.Delete)
                            {
                                rme.FullyGranted = true;
                                rme.FullyDenied = false;
                            }
                            if (!rme.Browse && !rme.Read && !rme.Edit && !rme.Add && !rme.Delete)
                            {
                                rme.FullyDenied = true;
                                rme.FullyGranted = false;
                            }

                            _context.RoleModuleExceptions.Update(rme);
                            await _context.SaveChangesAsync();
                        }
                    }

                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Updated Successfully!");
                }
                else //adding new exceptions
                {
                    foreach (SelectListItem moduleTable in moduleTables)
                    {
                        RoleModuleException rme = new RoleModuleException();
                        rme.RoleModuleId = roleModuleId;
                        rme.TableName = moduleTable.Text;
                        rme.Browse = !string.IsNullOrEmpty(form[moduleTable.Text + "_b"].ToString()) ? true : false;
                        rme.Read = !string.IsNullOrEmpty(form[moduleTable.Text + "_r"].ToString()) ? true : false;
                        rme.Edit = !string.IsNullOrEmpty(form[moduleTable.Text + "_e"].ToString()) ? true : false;
                        rme.Add = !string.IsNullOrEmpty(form[moduleTable.Text + "_a"].ToString()) ? true : false;
                        rme.Delete = !string.IsNullOrEmpty(form[moduleTable.Text + "_d"].ToString()) ? true : false;
                        rme.Status = 1;

                        if (rme.Browse && rme.Read && rme.Edit && rme.Add && rme.Delete)
                        {
                            rme.FullyGranted = true;
                            rme.FullyDenied = false;
                        }
                        if (!rme.Browse && !rme.Read && !rme.Edit && !rme.Add && !rme.Delete)
                        {
                            rme.FullyDenied = true;
                            rme.FullyGranted = false;
                        }

                        _context.RoleModuleExceptions.Add(rme);
                        await _context.SaveChangesAsync();
                    }

                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                }
            }

            return RedirectToAction("Index", "Roles", new { area = "Admin" });
        }

        // GET: UserModule/RoleModules/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null)
            {
                return NotFound();
            }

            var roleModule = await _context.RoleModules.FindAsync(id);
            if (roleModule == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", roleModule.RoleId);
            return View(roleModule);
        }

        // POST: UserModule/RoleModules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleId,ModuleId")] RoleModule roleModule)
        {
            if (id != roleModule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roleModule);
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
                    if (!RoleModuleExists(roleModule.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", roleModule.RoleId);
            return View(roleModule);
        }

        // GET: UserModule/RoleModules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null)
            {
                return NotFound();
            }

            var roleModule = await _context.RoleModules
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roleModule == null)
            {
                return NotFound();
            }

            return View(roleModule);
        }

        // POST: UserModule/RoleModules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roleModule = await _context.RoleModules.FindAsync(id);
            _context.RoleModules.Remove(roleModule);
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

        private bool RoleModuleExists(int id)
        {
            return _context.RoleModules.Any(e => e.Id == id);
        }
    }
}
