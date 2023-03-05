using BALibrary.Academic;
using BALibrary.Admin;
using BALibrary.Examination;
using BALibrary.HR;
using BALibrary.Registrar;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Data;
using SchoolERP.Models;
using System.Diagnostics;

namespace MyPharmacy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            #region Database Cleared? No Problem. I will create one for you
            int passed = 0;
            List<Employee> employees = _context.Employees.ToList();
            if (employees.Count == 0)
            {
                #region EXAMS
                Exam exam = new Exam();
                exam.Name = "Test 1 (10%)";
                exam.Weight = 10;
                exam.Status = 1;
                _context.Exams.Add(exam);
                _context.SaveChanges();

                exam = new Exam();
                exam.Name = "Cont. Ass. 1 (10%)";
                exam.Weight = 10;
                exam.Status = 1;
                _context.Exams.Add(exam);
                _context.SaveChanges();

                exam = new Exam();
                exam.Name = "MID. (20%)";
                exam.Weight = 10;
                exam.Status = 1;
                _context.Exams.Add(exam);
                _context.SaveChanges();

                exam = new Exam();
                exam.Name = "Test 2 (10%)";
                exam.Weight = 10;
                exam.Status = 1;
                _context.Exams.Add(exam);
                _context.SaveChanges();

                exam = new Exam();
                exam.Name = "Cont. Ass. 2 (10%)";
                exam.Weight = 10;
                exam.Status = 1;
                _context.Exams.Add(exam);
                _context.SaveChanges();


                exam = new Exam();
                exam.Name = "FINAL (40%)";
                exam.Weight = 10;
                exam.Status = 1;
                _context.Exams.Add(exam);
                _context.SaveChanges();

                ExamGroup examGroup = new ExamGroup();
                examGroup.Name = "ELEMENTRAY EXAMS";
                examGroup.MaximumMark = 100;
                examGroup.Status = 1;
                examGroup.PassingMark = 50;
                _context.ExamGroups.Add(examGroup);
                _context.SaveChanges();

                List<Exam> exams = _context.Exams.ToList();
                foreach (Exam e in exams)
                {
                    ExamGroupDetail examGroupDetail = new ExamGroupDetail();
                    examGroupDetail.ExamGroupId = 1;
                    examGroupDetail.ExamId = e.Id;
                    examGroupDetail.PassingMark = 50;
                    examGroupDetail.Status = 1;
                    _context.ExamGroupDetails.Add(examGroupDetail);
                    _context.SaveChanges();
                }

                examGroup = new ExamGroup();
                examGroup.Name = "HIGHSCHOOL EXAMS";
                examGroup.MaximumMark = 100;
                examGroup.Status = 1;
                examGroup.PassingMark = 50;
                _context.ExamGroups.Add(examGroup);
                _context.SaveChanges();

                foreach (Exam e in exams)
                {
                    ExamGroupDetail examGroupDetail = new ExamGroupDetail();
                    examGroupDetail.ExamGroupId = 2;
                    examGroupDetail.ExamId = e.Id;
                    examGroupDetail.PassingMark = 50;
                    examGroupDetail.Status = 1;
                    _context.ExamGroupDetails.Add(examGroupDetail);
                    _context.SaveChanges();
                }

                examGroup = new ExamGroup();
                examGroup.Name = "PREPARATORY EXAMS";
                examGroup.MaximumMark = 100;
                examGroup.Status = 1;
                examGroup.PassingMark = 50;
                _context.ExamGroups.Add(examGroup);
                _context.SaveChanges();

                foreach (Exam e in exams)
                {
                    ExamGroupDetail examGroupDetail = new ExamGroupDetail();
                    examGroupDetail.ExamGroupId = 3;
                    examGroupDetail.ExamId = e.Id;
                    examGroupDetail.PassingMark = 50;
                    examGroupDetail.Status = 1;
                    _context.ExamGroupDetails.Add(examGroupDetail);
                    _context.SaveChanges();
                }
                #endregion

                #region ROOMS
                Room r = new Room();              
                string[] sections = {"A", "B", "C", "D", "E", "F"};

                //Grade 9 (A-F)
                for (int i = 0; i < sections.Length; i++) 
                {
                    r = new Room();
                    r.RoomNo = "9"+sections[i];
                    r.Capacity = 45;
                    r.Status = 1;
                    _context.Rooms.Add(r);
                    _context.SaveChanges();
                }

                //Grade 10 (A-D)
                for (int i = 0; i <= 3; i++)
                {
                    r = new Room();
                    r.RoomNo = "10" + sections[i];
                    r.Capacity = 45;
                    r.Status = 1;
                    _context.Rooms.Add(r);
                    _context.SaveChanges();
                }

                //Grade 11 NS (A-C)
                for (int i = 0; i <= 2; i++)
                {
                    r = new Room();
                    r.RoomNo = "11-NS-" + sections[i];
                    r.Capacity = 45;
                    r.Status = 1;
                    _context.Rooms.Add(r);
                    _context.SaveChanges();
                }

                //Grade 11 SS (D-F)
                for (int i = 3; i <= 5; i++)
                {
                    r = new Room();
                    r.RoomNo = "11-SS-" + sections[i];
                    r.Capacity = 45;
                    r.Status = 1;
                    _context.Rooms.Add(r);
                    _context.SaveChanges();
                }

                //Grade 12 NS (A-D)
                for (int i = 0; i <= 3; i++)
                {
                    r = new Room();
                    r.RoomNo = "12-NS-" + sections[i];
                    r.Capacity = 45;
                    r.Status = 1;
                    _context.Rooms.Add(r);
                    _context.SaveChanges();
                }

                //Grade 12 NS (E-F)
                for (int i = 4; i <= 5; i++)
                {
                    r = new Room();
                    r.RoomNo = "12-SS-" + sections[i];
                    r.Capacity = 45;
                    r.Status = 1;
                    _context.Rooms.Add(r);
                    _context.SaveChanges();
                }
                #endregion

                #region PERIODS
                Period p = new Period();
                p.Name = "1";
                p.FromTime = "08:00";
                p.ToTime = "08:45";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "2";
                p.FromTime = "08:45";
                p.ToTime = "09:30";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "3";
                p.FromTime = "09:30";
                p.ToTime = "10:15";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "4";
                p.FromTime = "10:15";
                p.ToTime = "11:00";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "5";
                p.FromTime = "11:30";
                p.ToTime = "12:15";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "6";
                p.FromTime = "12:15";
                p.ToTime = "01:00";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "7";
                p.FromTime = "02:00";
                p.ToTime = "02:45";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();

                p = new Period();
                p.Name = "8";
                p.FromTime = "02:45";
                p.ToTime = "03:30";
                p.Status = 1;
                _context.Periods.Add(p);
                _context.SaveChanges();
                #endregion

                #region CLASSES
                Class cl = new Class();
                cl.Name = "9";
                cl.Status = 1;
                _context.Classes.Add(cl);
                _context.SaveChanges();

                cl = new Class();
                cl.Name = "10";
                cl.Status = 1;
                _context.Classes.Add(cl);
                _context.SaveChanges();

                cl = new Class();
                cl.Name = "11 NS";
                cl.Status = 1;
                _context.Classes.Add(cl);
                _context.SaveChanges();

                cl = new Class();
                cl.Name = "11 SS";
                cl.Status = 1;
                _context.Classes.Add(cl);
                _context.SaveChanges();

                cl = new Class();
                cl.Name = "12 NS";
                cl.Status = 1;
                _context.Classes.Add(cl);
                _context.SaveChanges();

                cl = new Class();
                cl.Name = "12 SS";
                cl.Status = 1;
                _context.Classes.Add(cl);
                _context.SaveChanges();
                #endregion

                #region SUBJECTS
                Subject subject = new Subject();
                subject.Name = "AMHARIC";
                subject.IsPractical = false;
                subject.Code = "AMH";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "ENGLISH";
                subject.IsPractical = false;
                subject.Code = "ENG";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "MATHEMATICS";
                subject.IsPractical = false;
                subject.Code = "MATH";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "PHYSICS";
                subject.IsPractical = false;
                subject.Code = "PHY";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "TECHNICAL DRAWING";
                subject.IsPractical = false;
                subject.Code = "TD";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "PHYSICAL EDUCATION";
                subject.IsPractical = true;
                subject.Code = "HPE";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "HISTORY";
                subject.IsPractical = false;
                subject.Code = "HIS";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "GEOGRAPHY";
                subject.IsPractical = false;
                subject.Code = "GEO";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "CIVICS";
                subject.IsPractical = false;
                subject.Code = "CIV";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "BIOLOGY";
                subject.IsPractical = false;
                subject.Code = "BIO";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                subject = new Subject();
                subject.Name = "CHEMISTRY";
                subject.IsPractical = false;
                subject.Code = "CHEM";
                subject.Status = 1;
                _context.Subjects.Add(subject);
                _context.SaveChanges();

                SubjectGroup subjectGroup = new SubjectGroup();
                subjectGroup.Name = "ELEMENTARY SUBJECTS";
                subjectGroup.Description = string.Empty;
                subjectGroup.Status = 1;
                _context.SubjectGroups.Add(subjectGroup);
                _context.SaveChanges();

                SubjectGroupDetail subjectGroupDetail = new SubjectGroupDetail();
                subjectGroupDetail.SubjectGroupId = 1;
                subjectGroupDetail.SubjectId = 1;
                subjectGroupDetail.Description = string.Empty;
                _context.SubjectGroupDetails.Add(subjectGroupDetail);
                _context.SaveChanges();

                subjectGroup = new SubjectGroup();
                subjectGroup.Name = "HIGHSCHOOL SUBJECTS";
                subjectGroup.Description = string.Empty;
                subjectGroup.Status = 1;
                _context.SubjectGroups.Add(subjectGroup);
                _context.SaveChanges();

                foreach (var subj in _context.Subjects) 
                {
                    subjectGroupDetail = new SubjectGroupDetail();
                    subjectGroupDetail.SubjectGroupId = 2;
                    subjectGroupDetail.SubjectId = subj.Id;
                    subjectGroupDetail.Description = string.Empty;
                    _context.SubjectGroupDetails.Add(subjectGroupDetail);                    
                }
                _context.SaveChanges();

                subjectGroup = new SubjectGroup();
                subjectGroup.Name = "GRADE 11 (NS) SUBJECTS";
                subjectGroup.Description = string.Empty;
                subjectGroup.Status = 1;
                _context.SubjectGroups.Add(subjectGroup);
                _context.SaveChanges();

                subjectGroupDetail = new SubjectGroupDetail();
                subjectGroupDetail.SubjectGroupId = 3;
                subjectGroupDetail.SubjectId = 1;
                subjectGroupDetail.Description = string.Empty;
                _context.SubjectGroupDetails.Add(subjectGroupDetail);
                _context.SaveChanges();

                subjectGroup = new SubjectGroup();
                subjectGroup.Name = "GRADE 11 (SS) SUBJECTS";
                subjectGroup.Description = string.Empty;
                subjectGroup.Status = 1;
                _context.SubjectGroups.Add(subjectGroup);
                _context.SaveChanges();

                subjectGroupDetail = new SubjectGroupDetail();
                subjectGroupDetail.SubjectGroupId = 4;
                subjectGroupDetail.SubjectId = 1;
                subjectGroupDetail.Description = string.Empty;
                _context.SubjectGroupDetails.Add(subjectGroupDetail);
                _context.SaveChanges();

                subjectGroup = new SubjectGroup();
                subjectGroup.Name = "GRADE 12 (NS) SUBJECTS";
                subjectGroup.Description = string.Empty;
                subjectGroup.Status = 1;
                _context.SubjectGroups.Add(subjectGroup);
                _context.SaveChanges();

                subjectGroupDetail = new SubjectGroupDetail();
                subjectGroupDetail.SubjectGroupId = 5;
                subjectGroupDetail.SubjectId = 1;
                subjectGroupDetail.Description = string.Empty;
                _context.SubjectGroupDetails.Add(subjectGroupDetail);
                _context.SaveChanges();

                subjectGroup = new SubjectGroup();
                subjectGroup.Name = "GRADE 12 (SS) SUBJECTS";
                subjectGroup.Description = string.Empty;
                subjectGroup.Status = 1;
                _context.SubjectGroups.Add(subjectGroup);
                _context.SaveChanges();

                subjectGroupDetail = new SubjectGroupDetail();
                subjectGroupDetail.SubjectGroupId = 6;
                subjectGroupDetail.SubjectId = 1;
                subjectGroupDetail.Description = string.Empty;
                _context.SubjectGroupDetails.Add(subjectGroupDetail);
                _context.SaveChanges();
                #endregion

                #region SECTIONS
                Section section = new Section();
                section.Name = "A";
                section.Status = 1;
                _context.Sections.Add(section);
                _context.SaveChanges();

                section = new Section();
                section.Name = "B";
                section.Status = 1;
                _context.Sections.Add(section);
                _context.SaveChanges();

                section = new Section();
                section.Name = "C";
                section.Status = 1;
                _context.Sections.Add(section);
                _context.SaveChanges();

                section = new Section();
                section.Name = "D";
                section.Status = 1;
                _context.Sections.Add(section);
                _context.SaveChanges();

                section = new Section();
                section.Name = "E";
                section.Status = 1;
                _context.Sections.Add(section);
                _context.SaveChanges();

                section = new Section();
                section.Name = "F";
                section.Status = 1;
                _context.Sections.Add(section);
                _context.SaveChanges();
                #endregion

                #region SEMESTERS
                Semester semester = new Semester();
                semester.Name = "I";
                semester.Status = 1;
                _context.Semesters.Add(semester);
                _context.SaveChanges();

                semester = new Semester();
                semester.Name = "II";
                semester.Status = 1;
                _context.Semesters.Add(semester);
                _context.SaveChanges();
                #endregion

                #region EMPLOYEES
                //user type
                UserType ut = new UserType();
                ut.Name = "Staff";
                ut.Status = 1;
                _context.UserTypes.Add(ut);
                _context.SaveChanges();

                //department
                Department dept = new Department();
                dept.Name = "HR";
                dept.Status = 1;
                _context.Departments.Add(dept);
                _context.SaveChanges();

                //job position
                JobPosition jp = new JobPosition();
                jp.DepartmentId = dept.Id;
                jp.InitialSalary = 5500;
                jp.Name = "Administrator";
                jp.Status = 1;
                _context.JobPositions.Add(jp);
                _context.SaveChanges();

                //employment type
                EmploymentType et = new EmploymentType();
                et.Name = "Permanent";
                et.Status = 1;
                _context.EmploymentTypes.Add(et);
                _context.SaveChanges();

                //role
                Role role = new Role();
                role.Name = "Administrator Role";
                role.Description = "";
                role.Status = 1;
                _context.Roles.Add(role);
                _context.SaveChanges();

                //role 2
                role = new Role();
                role.Name = "Student Role";
                role.Description = "";
                role.Status = 1;
                _context.Roles.Add(role);
                _context.SaveChanges();

                //role module
                RoleModule rm = new RoleModule();
                rm.RoleId = 1;
                rm.ModuleId = (int)Common.ModuleName.ADMIN;
                rm.Status = 1;
                _context.RoleModules.Add(rm);
                _context.SaveChanges();

                //employee
                Employee employee = new Employee();
                employee.FirstName = "Sisay";
                employee.MiddleName = "Mersha";
                employee.LastName = "Mekonnen";
                employee.Gender = "Male";
                employee.JoinedDate = DateTime.Now;
                employee.PhoneNo = "251913011370";
                employee.PermanentAddress = "Ziway";
                employee.Qualification = "Bsc";
                employee.UserTypeId = ut.Id;
                employee.JobPositionId = jp.Id;
                employee.EmploymentTypeId = et.Id;
                employee.RoleId = 1;
                employee.Code = "EMP-001";
                employee.WorkExperience = 10;
                employee.PhotoPath = "photo.png";
                employee.MaritalStatus = "Married";
                employee.CurrentAddress = "Ziway";
                employee.Note = "a";
                employee.EmergencyContactNo = "251913011370";
                employee.EmailAddress = "contactsisay@gmail.com";
                employee.Password = BCrypt.Net.BCrypt.HashPassword("asdf");
                employee.Status = 1;
                _context.Employees.Add(employee);
                passed = _context.SaveChanges();
                #endregion

                #region GRADING RULES
                GradingRule gradingRule = new GradingRule();
                gradingRule.Name = "Pass (>=50%)";
                gradingRule.MarkFrom = 0;
                gradingRule.Middle = 50;
                gradingRule.MarkTo = 100;
                gradingRule.Status = 1;
                _context.GradingRules.Add(gradingRule);
                _context.SaveChanges();

                gradingRule = new GradingRule();
                gradingRule.Name = "Fail (<50%)";
                gradingRule.MarkFrom = 0;
                gradingRule.Middle = 50;
                gradingRule.MarkTo = 0;
                gradingRule.Status = 1;
                _context.GradingRules.Add(gradingRule);
                _context.SaveChanges();

                GradingRuleGroup gradingRuleGroup = new GradingRuleGroup();
                gradingRuleGroup.Name = "ELEMENTARY GRADING RULES";
                gradingRuleGroup.Status = 1;
                _context.GradingRuleGroups.Add(gradingRuleGroup);
                _context.SaveChanges();

                List<GradingRule> gradingRules = _context.GradingRules.ToList();
                foreach (GradingRule gr in gradingRules)
                {
                    GradingRuleGroupDetail gradingRuleGroupDetail = new GradingRuleGroupDetail();
                    gradingRuleGroupDetail.GradingRuleGroupId = 1;
                    gradingRuleGroupDetail.GradingRuleId = gr.Id;
                    gradingRuleGroupDetail.Status = 1;
                    _context.GradingRuleGroupDetails.Add(gradingRuleGroupDetail);
                    _context.SaveChanges();
                }

                gradingRuleGroup = new GradingRuleGroup();
                gradingRuleGroup.Name = "HIGHSCHOOL GRADING RULES";
                gradingRuleGroup.Status = 1;
                _context.GradingRuleGroups.Add(gradingRuleGroup);
                _context.SaveChanges();

                foreach (GradingRule gr in gradingRules)
                {
                    GradingRuleGroupDetail gradingRuleGroupDetail = new GradingRuleGroupDetail();
                    gradingRuleGroupDetail.GradingRuleGroupId = 2;
                    gradingRuleGroupDetail.GradingRuleId = gr.Id;
                    gradingRuleGroupDetail.Status = 1;
                    _context.GradingRuleGroupDetails.Add(gradingRuleGroupDetail);
                    _context.SaveChanges();
                }

                gradingRuleGroup = new GradingRuleGroup();
                gradingRuleGroup.Name = "PREPARATORY GRADING RULES";
                gradingRuleGroup.Status = 1;
                _context.GradingRuleGroups.Add(gradingRuleGroup);
                _context.SaveChanges();

                foreach (GradingRule gr in gradingRules)
                {
                    GradingRuleGroupDetail gradingRuleGroupDetail = new GradingRuleGroupDetail();
                    gradingRuleGroupDetail.GradingRuleGroupId = 3;
                    gradingRuleGroupDetail.GradingRuleId = gr.Id;
                    gradingRuleGroupDetail.Status = 1;
                    _context.GradingRuleGroupDetails.Add(gradingRuleGroupDetail);
                    _context.SaveChanges();
                }
                #endregion

                #region SESSIONS
                Session session = new Session();
                session.GradingRuleGroupId = 1;
                session.Year = DateTime.Now.Year;
                session.MinStudent = 250;
                session.MaxStudent = 500;
                session.OpeningDate = DateTime.Now;
                session.ClosingDate = DateTime.Now.AddYears(1);
                session.SchoolType = "ELEMENTARY";
                session.Status = 1;
                _context.Sessions.Add(session);
                _context.SaveChanges();

                session = new Session();
                session.GradingRuleGroupId = 2;
                session.Year = DateTime.Now.Year;
                session.MinStudent = 250;
                session.MaxStudent = 500;
                session.OpeningDate = DateTime.Now;
                session.ClosingDate = DateTime.Now.AddYears(1);
                session.SchoolType = "HIGHSCHOOL";
                session.Status = 1;
                _context.Sessions.Add(session);
                _context.SaveChanges();

                session = new Session();
                session.GradingRuleGroupId = 3;
                session.Year = DateTime.Now.Year;
                session.MinStudent = 250;
                session.MaxStudent = 500;
                session.OpeningDate = DateTime.Now;
                session.ClosingDate = DateTime.Now.AddYears(1);
                session.SchoolType = "PREPARATORY";
                session.Status = 1;
                _context.Sessions.Add(session);
                _context.SaveChanges();

                #endregion
            }
            #endregion

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string emlAddress, string passWrd)
        {
            if (!string.IsNullOrEmpty(emlAddress) && !string.IsNullOrEmpty(passWrd))
            {
                List<Employee> emps = _context.Employees.Where(e => e.EmailAddress == emlAddress).ToList();
                if (emps.Count > 0)
                {
                    bool passed = false;
                    foreach (Employee emp in emps)
                    {
                        //checking if password is correct (verifying password)
                        bool isValidPassword = BCrypt.Net.BCrypt.Verify(passWrd, emp.Password);
                        if (isValidPassword)
                        {
                            HttpContext.Session.SetString("SessionKeyUserId", emp.Id.ToString());
                            HttpContext.Session.SetString("SessionKeyUserEmail", emp.EmailAddress.ToString());
                            HttpContext.Session.SetString("SessionKeyUserRoleId", emp.RoleId.ToString());
                            HttpContext.Session.SetString("SessionKeySessionId", Guid.NewGuid().ToString());

                            passed = true;
                            break;//stopping loop if employee password is verified and correct
                        }
                    }

                    if (passed)
                    {
                        TempData["MessageType"] = "success";
                        TempData["Message"] = "Successfully Logged In";
                        return RedirectToAction("Index", "Home", new { area = "Dashboard" });
                    }
                }
            }

            TempData["MessageType"] = "error";
            TempData["Message"] = "Account not found! Please check your email address and/or password";
            return RedirectToAction("Index", "Home", new { area = "Default" });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyUserId);
            HttpContext.Session.Remove(SessionVariable.SessionKeyUserEmail);
            HttpContext.Session.Remove(SessionVariable.SessionKeyUserRoleId);
            HttpContext.Session.Remove(SessionVariable.SessionKeySessionId);

            TempData["MessageType"] = "success";
            TempData["Message"] = "Successfully Logged Out";
            return RedirectToAction("Index", "Home", new { area = "Default" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}