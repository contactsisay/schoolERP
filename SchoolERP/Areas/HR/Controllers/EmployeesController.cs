using BALibrary.HR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;

namespace SchoolERP.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HR/Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.EmploymentType).Include(e => e.JobPosition).Include(e => e.Role).Include(e => e.UserType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HR/Employees/Details/5
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

        // GET: HR/Employees/Create
        public IActionResult Create()
        {
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "Id", "Name");
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Name");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name");
            ViewData["Gender"] = new SelectList(Common.GetGenderComboBox(), "Value", "Text");
            ViewData["MaritalStatus"] = new SelectList(Common.GetMaritalStatusComboBox(), "Value", "Text");
            return View();
        }

        // POST: HR/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserTypeId,JobPositionId,EmploymentTypeId,Designation,Code,FirstName,MiddleName,LastName,MotherName,Gender,Dob,JoinedDate,PhoneNo,EmergencyContactNo,MaritalStatus,CurrentAddress,PermanentAddress,Note,PhotoPath,Qualification,WorkExperience,RoleId,EmailAddress,Password")] Employee employee)
        {
            if (!string.IsNullOrEmpty(employee.EmailAddress) && !string.IsNullOrEmpty(employee.Password))
                employee.Password = BCrypt.Net.BCrypt.HashPassword(employee.Password);

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "Id", "Name", employee.EmploymentTypeId);
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Name", employee.JobPositionId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name", employee.UserTypeId);
            ViewData["Gender"] = new SelectList(Common.GetGenderComboBox(), "Value", "Text", employee.Gender);
            ViewData["MaritalStatus"] = new SelectList(Common.GetMaritalStatusComboBox(), "Value", "Text", employee.MaritalStatus);
            return View(employee);
        }

        // GET: HR/Employees/Edit/5
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name", employee.UserTypeId);
            ViewData["Gender"] = new SelectList(Common.GetGenderComboBox(), "Value", "Text", employee.Gender);
            ViewData["MaritalStatus"] = new SelectList(Common.GetMaritalStatusComboBox(), "Value", "Text", employee.MaritalStatus);
            return View(employee);
        }

        // POST: HR/Employees/Edit/5
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", employee.RoleId);
            ViewData["UserTypeId"] = new SelectList(_context.UserTypes, "Id", "Name", employee.UserTypeId);
            ViewData["Gender"] = new SelectList(Common.GetGenderComboBox(), "Value", "Text", employee.Gender);
            ViewData["MaritalStatus"] = new SelectList(Common.GetMaritalStatusComboBox(), "Value", "Text", employee.MaritalStatus);
            return View(employee);
        }

        // GET: HR/Employees/Delete/5
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

        // POST: HR/Employees/Delete/5
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
