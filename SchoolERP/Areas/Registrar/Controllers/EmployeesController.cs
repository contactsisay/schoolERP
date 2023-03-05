using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.HR;
using SchoolERP.Data;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Registrar/Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.EmploymentType).Include(e => e.JobPosition).Include(e => e.Role).Include(e => e.UserType);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult PrintIdCard()
        {
            var employees = (from e in _context.Employees 
                             join jP in _context.JobPositions on e.JobPositionId equals jP.Id
                             join r in _context.Roles on e.RoleId equals r.Id
                             select new 
                             {
                                 e.Id,
                                 e.Code,
                                 FullName = e.FirstName+" "+e.MiddleName+" "+e.LastName,
                                 e.Gender,
                                 e.EmergencyContactNo,
                                 e.MotherName,
                                 RoleName = r.Name,
                                 IsActive = e.Status == 0 ? "Active" : "In Active",
                                 JoinedDate = e.JoinedDate.ToShortDateString(),
                                 PositionName = jP.Name,
                                 e.MaritalStatus,
                                 e.PhoneNo,                                 
                                 e.Dob,
                                 e.PhotoPath,
                             }).ToList();

            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rptStaffID.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("rpt1", "Header");
            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("DataSet1", employees);
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

            return File(result.MainStream, "application/pdf");
        }

        // GET: Registrar/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.EmploymentType)
                .Include(e => e.JobPosition)
                .Include(e => e.Role)
                .Include(e => e.UserType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Registrar/Employees/Create
        public IActionResult Create()
        {
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "Id", "Name");
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Name");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description");
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name");
            return View();
        }

        // POST: Registrar/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserTypeId,JobPositionId,EmploymentTypeId,Designation,Code,FirstName,MiddleName,LastName,MotherName,Gender,Dob,JoinedDate,PhoneNo,EmergencyContactNo,MaritalStatus,CurrentAddress,PermanentAddress,Note,PhotoPath,Qualification,WorkExperience,RoleId,EmailAddress,Password")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "Id", "Name", employee.EmploymentTypeId);
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Name", employee.JobPositionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description", employee.RoleId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name", employee.UserTypeId);
            return View(employee);
        }

        // GET: Registrar/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "Id", "Name", employee.EmploymentTypeId);
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Name", employee.JobPositionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description", employee.RoleId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name", employee.UserTypeId);
            return View(employee);
        }

        // POST: Registrar/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserTypeId,JobPositionId,EmploymentTypeId,Designation,Code,FirstName,MiddleName,LastName,MotherName,Gender,Dob,JoinedDate,PhoneNo,EmergencyContactNo,MaritalStatus,CurrentAddress,PermanentAddress,Note,PhotoPath,Qualification,WorkExperience,RoleId,EmailAddress,Password")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "Id", "Name", employee.EmploymentTypeId);
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Name", employee.JobPositionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description", employee.RoleId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name", employee.UserTypeId);
            return View(employee);
        }

        // GET: Registrar/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.EmploymentType)
                .Include(e => e.JobPosition)
                .Include(e => e.Role)
                .Include(e => e.UserType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Registrar/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return _context.Employees.Any(e => e.Id == id);
        }
    }
}
