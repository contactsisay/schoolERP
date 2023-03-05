using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Academic;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.Academic.Controllers
{
    [Area("Academic")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Academic/Sessions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sessions.Include(s=>s.GradingRuleGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Academic/Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.Sessions == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Academic/Sessions/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Name");
            ViewData["Year"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text");
            ViewData["SchoolType"] = new SelectList(Common.FillSchoolTypeComboBox(), "Value", "Text");
            return View();
        }

        // POST: Academic/Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SchoolType,Year,GradingRuleGroupId,MinStudent,MaxStudent,OpeningDate,ClosingDate")] Session session, string SchoolType)
        {
            if (!string.IsNullOrEmpty(SchoolType))
            {
                session.SchoolType = SchoolType;

                if (!string.IsNullOrEmpty(SchoolType))
                {
                    session.Status = 1;
                    _context.Add(session);
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
                else {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Saved!");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Name", session.GradingRuleGroupId);
            ViewData["Year"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text", session.Year);
            ViewData["SchoolType"] = new SelectList(Common.FillSchoolTypeComboBox(), "Value", "Text", session.SchoolType);
            return View(session);
        }

        // GET: Academic/Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.Sessions == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Name", session.GradingRuleGroupId);
            ViewData["Year"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text", session.Year);
            ViewData["SchoolType"] = new SelectList(Common.FillSchoolTypeComboBox(), "Value", "Text", session.SchoolType);
            return View(session);
        }

        // POST: Academic/Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SchoolType,Year,GradingRuleGroupId,MinStudent,MaxStudent,OpeningDate,ClosingDate")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
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
                    if (!SessionExists(session.Id))
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
            ViewData["GradingRuleGroupId"] = new SelectList(_context.GradingRuleGroups, "Id", "Name", session.GradingRuleGroupId);
            ViewData["Year"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text", session.Year);
            ViewData["SchoolType"] = new SelectList(Common.FillSchoolTypeComboBox(), "Value", "Text", session.SchoolType);
            return View(session);
        }

        // GET: Academic/Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.Sessions == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Academic/Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sessions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sessions'  is null.");
            }
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
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

        private bool SessionExists(int id)
        {
          return _context.Sessions.Any(e => e.Id == id);
        }
    }
}
