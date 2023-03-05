using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;
using SchoolERP.Models;
using BALibrary.Academic;
using NuGet.Protocol.Core.Types;
using BALibrary.Inventory;
using BALibrary.Purchase;
using ExcelDataReader;
using System.ComponentModel;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using System.Composition;
using System.Reflection;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Registrar/Students
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = (from s in _context.Students.Include(s => s.Role)
                                       join sP in _context.StudentPromotions.Include(a => a.AcademicYearSection).Include(a=>a.AcademicYearSection.Section) on s.Id equals sP.StudentId
                                       join aY in _context.AcademicYears.Include(a=>a.Class) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                       select new
                                       {
                                           s.Id,
                                           s.IDNo,
                                           s.FirstName,
                                           s.MiddleName,
                                           s.LastName,
                                           s.Gender,
                                           AdmissionDate = s.AdmissionDate.ToShortDateString(),
                                           s.EmailAddress,
                                           ClassName = aY.Class.Name,
                                           SectionName = sP.AcademicYearSection.Section.Name,
                                       });//.Take(20)

            ViewData["queryResult"] = applicationDbContext.ToList();
            return View();
        }

        public IActionResult GenerateIDCard(string AcademicYearId, string AcademicYearSectionId)
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            if (!string.IsNullOrEmpty(AcademicYearId) && !string.IsNullOrEmpty(AcademicYearSectionId)) 
            {
                int academicYearId = Convert.ToInt32(AcademicYearId);
                int academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);
                string defaultImagePath = new Uri($"{_webHostEnvironment.WebRootPath}\\images\\noimage.png").AbsoluteUri;

                var students = (from s in _context.Students
                                join sP in _context.StudentPromotions on s.Id equals sP.StudentId
                                join aYS in _context.AcademicYearSections.Include(a => a.Section).Include(a => a.AcademicYear).Include(a => a.AcademicYear.Class).Include(a => a.AcademicYear.Session).Include(a => a.AcademicYear.Semester) on sP.AcademicYearSectionId equals aYS.Id
                                join r in _context.Roles on s.RoleId equals r.Id
                                where aYS.AcademicYearId == academicYearId && aYS.Id == academicYearSectionId
                                select new
                                {
                                    s.Id,
                                    s.IDNo,
                                    StudentPromotionId = sP.Id,
                                    FullName = s.FirstName + " " + s.MiddleName + " " + s.LastName,
                                    s.Gender,
                                    s.MobileNo,
                                    RoleName = r.Name,
                                    IsActive = s.Status == 0 ? "Active" : "In Active",
                                    AdmissionDate = s.AdmissionDate.ToShortDateString(),
                                    s.Dob,
                                    PhotoPath = (!string.IsNullOrEmpty(s.PhotoPath) ? new Uri($"{_webHostEnvironment.WebRootPath}\\images\\" + s.PhotoPath).AbsoluteUri : defaultImagePath),
                                    s.BloogGroup,
                                    ClassName = aYS.AcademicYear.Class.Name,
                                    SectionName = aYS.Section.Name,
                                    SessionName = aYS.AcademicYear.Session.Year + '-' + aYS.AcademicYear.Session.SchoolType.ToString(),
                                    SemesterName = aYS.AcademicYear.Semester.Name,
                                }).ToList();

                
                ViewData["academicYearId"] = academicYearId;
                ViewData["academicYearSectionId"] = academicYearSectionId;
                ViewData["queryResult"] = students;
            }

            ViewData["academicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportCardToPdf(IFormCollection iFormCollection)
        {
            var AcademicYearId = iFormCollection["AcademicYearIdd"];
            var AcademicYearSectionId = iFormCollection["AcademicYearSectionIdd"];
            var studentPromotionId = iFormCollection["StudentPromotionId_0"];

            if (!string.IsNullOrEmpty(AcademicYearId) && !string.IsNullOrEmpty(AcademicYearSectionId)) 
            {
                int aYId = Convert.ToInt32(AcademicYearId);
                int aYSId = Convert.ToInt32(AcademicYearSectionId);

                string defaultImagePath = new Uri($"{_webHostEnvironment.WebRootPath}\\images\\noimage.png").AbsoluteUri;
                var students = (from s in _context.Students
                                join sP in _context.StudentPromotions on s.Id equals sP.StudentId
                                join aYS in _context.AcademicYearSections.Include(a => a.Section).Include(a => a.AcademicYear).Include(a => a.AcademicYear.Class).Include(a => a.AcademicYear.Session).Include(a => a.AcademicYear.Semester) on sP.AcademicYearSectionId equals aYS.Id
                                join r in _context.Roles on s.RoleId equals r.Id
                                where aYS.Id == aYSId
                                select new
                                {
                                    s.Id,
                                    s.IDNo,
                                    FullName = s.FirstName + " " + s.MiddleName + " " + s.LastName,
                                    s.Gender,
                                    s.MobileNo,
                                    RoleName = r.Name,
                                    IsActive = s.Status == 0 ? "Active" : "In Active",
                                    AdmissionDate = s.AdmissionDate.ToShortDateString(),
                                    s.Dob,
                                    PhotoPath = (!string.IsNullOrEmpty(s.PhotoPath) ? new Uri($"{_webHostEnvironment.WebRootPath}\\images\\" + s.PhotoPath).AbsoluteUri : defaultImagePath),
                                    s.BloogGroup,
                                    ClassName = aYS.AcademicYear.Class.Name,
                                    SectionName = aYS.Section.Name,
                                    SessionName = aYS.AcademicYear.Session.Year + '-' + aYS.AcademicYear.Session.SchoolType.ToString(),
                                    SemesterName = aYS.AcademicYear.Semester.Name
                                }).ToList();


                string mimetype = "";
                int extension = 1;
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rptStudentIDCard.rdlc";

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("rpt1", "Header");
                AspNetCore.Reporting.LocalReport localReport = new AspNetCore.Reporting.LocalReport(path);
                BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                FieldInfo field = localReport.GetType().GetField("localReport", bindFlags);
                object rptObj = field.GetValue(localReport);
                Type type = rptObj.GetType();
                PropertyInfo pi = type.GetProperty("EnableExternalImages");
                pi.SetValue(rptObj, true, null);

                localReport.AddDataSource("DataSet1", students);
                var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

                return File(result.MainStream, "application/pdf");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Registrar/Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Registrar/Students/Create
        public IActionResult Create()
        {
            var relationships = string.Empty;
            var rlnshp = _context.Relationships.ToList();
            foreach (var r in rlnshp)
                relationships += r.Id + "#" + r.Name + ",";//id#name

            #region Creating Classes list
            var academicYearSections = from aYS in _context.AcademicYearSections.Include(a => a.Section)
                                       join aY in _context.AcademicYears.Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                       select new
                                       {
                                           AYId = aY.Id,
                                           AYSId = aYS.Id,
                                           aY.ClassId,
                                           ClassName = aY.Class.Name,
                                           aYS.SectionId,
                                           SectionName = aYS.Section.Name,
                                           SectionName2 = aY.Class.Name + "(" + aYS.Section.Name + ")",
                                       };


            var classes = _context.Classes.ToList();
            List<SelectListItem> classListItems = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Value = "-1";
            item.Text = "Select";
            classListItems.Add(item);
            foreach (var cl in classes)
            {
                bool alreadyAdded = false;
                foreach (var a in academicYearSections)
                {
                    if (a.ClassId == cl.Id && !alreadyAdded)
                    {
                        item = new SelectListItem();
                        item.Value = a.ClassId.ToString();
                        item.Text = a.ClassName;
                        classListItems.Add(item);
                        alreadyAdded = true;
                    }
                }
            }
            #endregion

            ViewData["ClassId"] = new SelectList(classListItems, "Value", "Text");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            ViewData["Relationships"] = relationships;
            return View();
        }

        // POST: Registrar/Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IDNo,FirstName,MiddleName,LastName,Gender,Dob,MobileNo,AdmissionDate,BloogGroup,Height,Weight,Religion,RoleId,EmailAddress,Password,PhotoPath")] Student student, IFormCollection iFormCollection, IFormFile file)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            //if (ModelState.IsValid)
            //{
            int id = StudentExists(student);
            if (id == 0)//if not exists
            {
                student.Password = BCrypt.Net.BCrypt.HashPassword(student.Password);
                _context.Add(student);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    #region Promoting Student (Initial-Admission)
                    if (!string.IsNullOrEmpty(iFormCollection["SectionId"]) && Convert.ToInt32(iFormCollection["SectionId"]) > 0)
                    {
                        StudentPromotion studentPromotion = new StudentPromotion();
                        studentPromotion.StudentId = student.Id;
                        studentPromotion.AcademicYearSectionId = Convert.ToInt32(iFormCollection["SectionId"]);
                        studentPromotion.Status = 1;
                        studentPromotion.IsClassChange = true;
                        studentPromotion.PromotedBy = currentUserId;
                        studentPromotion.PromotedDate = DateTime.Now;
                        _context.StudentPromotions.Add(studentPromotion);
                        _context.SaveChanges();
                    }

                    #endregion

                    #region Saving Student Guardians
                    if (!string.IsNullOrEmpty(iFormCollection["relationship_id_1"]))
                    {
                        //saving student guardians and/or parents
                        StudentGuardian studentGuardian = new StudentGuardian();
                        for (int i = 1; i <= iFormCollection.Count; i++)
                        {
                            studentGuardian = new StudentGuardian();
                            if (!string.IsNullOrEmpty(iFormCollection["relationship_id_" + i]) && !string.IsNullOrEmpty(iFormCollection["full_name_" + i]) && !string.IsNullOrEmpty(iFormCollection["mobile_no_" + i]))
                            {
                                studentGuardian.RelationshipId = Convert.ToInt32(iFormCollection["relationship_id_" + i]);
                                studentGuardian.FullName = iFormCollection["full_name_" + i];
                                studentGuardian.MobileNo = iFormCollection["mobile_no_" + i];
                                studentGuardian.EmailAddress = iFormCollection["email_address_" + i];
                                studentGuardian.Occupation = "Unknown";
                                studentGuardian.PhotoPath = "";
                                studentGuardian.Password = BCrypt.Net.BCrypt.HashPassword(iFormCollection["Password"]);

                                _context.StudentGuardians.Add(studentGuardian);
                                _context.SaveChanges();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    if (file != null)
                    {
                        #region Uploading Photo
                        string fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\students_" + student.Id.ToString() + "_" + file.FileName);
                        using (FileStream fileStream = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fileStream);
                            fileStream.Flush();
                        }
                        #endregion
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            //}
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description", student.RoleId);
            return View(student);
        }

        public async Task<IActionResult> ImportFromExcel()
        {
            #region Creating Classes list
            var academicYearSections = from aYS in _context.AcademicYearSections.Include(a => a.Section)
                                       join aY in _context.AcademicYears.Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                       select new
                                       {
                                           AYId = aY.Id,
                                           AYSId = aYS.Id,
                                           aY.ClassId,
                                           ClassName = aY.Class.Name,
                                           aYS.SectionId,
                                           SectionName = aYS.Section.Name,
                                           SectionName2 = aY.Class.Name + "(" + aYS.Section.Name + ")",
                                       };


            var classes = _context.Classes.ToList();
            List<SelectListItem> classListItems = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Value = "-1";
            item.Text = "Select";
            classListItems.Add(item);
            foreach (var cl in classes)
            {
                bool alreadyAdded = false;
                foreach (var a in academicYearSections)
                {
                    if (a.ClassId == cl.Id && !alreadyAdded)
                    {
                        item = new SelectListItem();
                        item.Value = a.ClassId.ToString();
                        item.Text = a.ClassName;
                        classListItems.Add(item);
                        alreadyAdded = true;
                    }
                }
            }
            #endregion

            ViewData["ClassId"] = new SelectList(classListItems, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromExcel(string SectionId, IFormFile file)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            List<Student> importedStudents = new List<Student>();
            if (file != null)
            {
                //uploading file
                #region Uploading File
                string fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\import_students_" + file.FileName);
                using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                #endregion

                //Reading excel and importing data
                int i = 0;
                #region Reading excel and importing data
                using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                #region READING FROM EXCEL AND SETTING STUDENT DAT
                                int academicYearSectionId = 0;
                                try
                                {
                                    academicYearSectionId = reader.GetDouble(0) > 0 ? Convert.ToInt32(reader.GetDouble(0)) : 0;
                                }
                                catch (Exception ex)
                                {
                                    academicYearSectionId = string.IsNullOrEmpty(reader.GetString(0)) ? 0 : Convert.ToInt32(reader.GetString(0));
                                }

                                if (academicYearSectionId > 0)
                                {
                                    string admissionNo = string.IsNullOrEmpty(reader.GetString(1)) ? string.Empty : reader.GetString(1);
                                    string firstName = string.IsNullOrEmpty(reader.GetString(3)) ? string.Empty : reader.GetString(3);
                                    string middleName = string.IsNullOrEmpty(reader.GetString(4)) ? string.Empty : reader.GetString(4);
                                    string gender = string.IsNullOrEmpty(reader.GetString(5)) ? string.Empty : reader.GetString(5);
                                    DateTime dob = DateTime.Now;
                                    try
                                    {
                                        dob = reader.GetDateTime(6) != null ? reader.GetDateTime(6) : DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        dob = string.IsNullOrEmpty(reader.GetString(6)) ? DateTime.Now : Convert.ToDateTime(reader.GetString(6));
                                    }

                                    string category = string.IsNullOrEmpty(reader.GetString(7)) ? string.Empty : reader.GetString(7);
                                    string mobileNo = string.Empty;

                                    try
                                    {
                                        mobileNo = reader.GetDouble(10) > 0 ? reader.GetDouble(10).ToString() : string.Empty;
                                    }
                                    catch (Exception ex)
                                    {
                                        mobileNo = string.IsNullOrEmpty(reader.GetString(10)) ? string.Empty : reader.GetString(10);
                                    }

                                    string emailAddress = string.IsNullOrEmpty(reader.GetString(11)) ? string.Empty : reader.GetString(11);
                                    string bloodGroup = string.IsNullOrEmpty(reader.GetString(13)) ? string.Empty : reader.GetString(13);
                                    string height = "0"; //string.IsNullOrEmpty(reader.GetString(15)) ? string.Empty : reader.GetString(15);
                                    string weight = "0"; //string.IsNullOrEmpty(reader.GetString(16)) ? string.Empty : reader.GetString(16);
                                    string fatherName = string.IsNullOrEmpty(reader.GetString(18)) ? string.Empty : reader.GetString(18);//extract grandfather name

                                    Student student = new Student();
                                    student.IDNo = admissionNo;
                                    student.FirstName = firstName;
                                    student.MiddleName = middleName;
                                    student.LastName = fatherName.Split(" ").Length > 1 ? fatherName.Split(" ")[1] : string.Empty;
                                    student.Gender = gender;
                                    student.Dob = dob;
                                    //student.Category = string.Empty;
                                    student.MobileNo = mobileNo.ToString();
                                    student.EmailAddress = emailAddress;
                                    student.BloogGroup = bloodGroup;
                                    student.Height = height != string.Empty ? 0 : Convert.ToDecimal(height);
                                    student.Weight = weight != string.Empty ? 0 : Convert.ToDecimal(weight);
                                    student.Status = 1;
                                    student.PhotoPath = string.Empty;
                                    student.AdmissionDate = DateTime.Now;
                                    student.Password = BCrypt.Net.BCrypt.HashPassword("123");
                                    student.Religion = "Unknown";
                                    student.RoleId = 2;//student role
                                    #endregion

                                    if (StudentExists(student) == 0)
                                    {
                                        _context.Students.Add(student);
                                        int pass = _context.SaveChanges();

                                        //if student is imported
                                        if (pass > 0)
                                        {
                                            StudentPromotion studentPromotion = new StudentPromotion();
                                            studentPromotion.StudentId = student.Id;
                                            studentPromotion.AcademicYearSectionId = academicYearSectionId;
                                            studentPromotion.IsClassChange = true;
                                            studentPromotion.PromotedBy = currentUserId;
                                            studentPromotion.PromotedDate = DateTime.Now;
                                            studentPromotion.Status = 1;
                                            _context.StudentPromotions.Add(studentPromotion);
                                            _context.SaveChanges();

                                            //adding to list
                                            importedStudents.Add(student);
                                        }
                                    }

                                }
                            }
                        } while (reader.NextResult());
                    }
                }
                #endregion
            }

            ViewData["ImportedStudents"] = importedStudents;
            return View();
        }

        // GET: Registrar/Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description", student.RoleId);
            return View(student);
        }

        // POST: Registrar/Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IDNo,FirstName,MiddleName,LastName,Gender,Dob,MobileNo,AdmissionDate,BloogGroup,Height,Weight,Religion,RoleId,EmailAddress,Password,PhotoPath")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (StudentExists(student) == 0)
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Description", student.RoleId);
            return View(student);
        }

        // GET: Registrar/Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Registrar/Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private int StudentExists(Student student)
        {
            var students = _context.Students.Where(e => e.FirstName == student.FirstName).Where(e => e.MiddleName == student.MiddleName).Where(e => e.LastName == student.LastName).OrderByDescending(e => e.Id).ToList();
            return students.Count > 0 ? students[0].Id : 0;
        }
    }
}
