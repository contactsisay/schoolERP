using BALibrary.Academic;
using BALibrary.Admin;
using BALibrary.HR;
using BALibrary.Registrar;
using ExcelDataReader.Log.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;
using System.ComponentModel;
using static System.Collections.Specialized.BitVector32;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class TimeTablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registrar/TimeTables
        public async Task<IActionResult> Index()
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            var teachers = (from e in _context.Employees.Include(a => a.Role)
                            select new
                            {
                                e.Id,
                                FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName + "(" + e.Code + ")",
                            }).ToList();

            ViewData["Classes"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            ViewData["Teachers"] = new SelectList(teachers, "Id", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PrintTimetable(string ClassId, string SectionId, string TeacherId, string ReportType)
        {
            string[] weekdays = { "MONDAY", "TUESDAY", "WEDNSDAY", "THURSDAY", "FRIDAY" };

            #region Timetable Data
            var timeTableDetails = (from tTD in _context.TimeTableDetails.Include(a => a.TimeTable).Include(a => a.Period)
                                   join sub in _context.Subjects on tTD.SubjectId equals sub.Id
                                   join aYS in _context.AcademicYearSections.Include(a => a.Section) on tTD.TimeTable.AcademicYearSectionId equals aYS.Id
                                   join aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                   join e in _context.Employees on tTD.EmployeeId equals e.Id
                                   select new
                                   {
                                       aYId = aY.Id,
                                       aYSId = aYS.Id,
                                       aY.ClassId,
                                       aYS.SectionId,
                                       SubjectId = sub.Id,
                                       TeacherId = tTD.EmployeeId,
                                       TeacherName = e.FirstName + " " + e.MiddleName,
                                       tTD.PeriodId,
                                       PeriodName = tTD.Period.Name,
                                       PeriodFrom = tTD.Period.FromTime,
                                       PeriodTo = tTD.Period.ToTime,
                                       SubjectName = sub.Name,
                                       SubjectCode = sub.Code,
                                       tTD.WeekDay,
                                       ClassName = aY.Class.Name,
                                       SectionName = aYS.Section.Name,
                                   }).ToList();

            if (!string.IsNullOrEmpty(SectionId) && Convert.ToInt32(SectionId) > 0) {
                int sId = Convert.ToInt32(SectionId);
                timeTableDetails = timeTableDetails.Where(a => a.aYSId == sId).ToList();
            }

            #endregion

            ViewData["Weekdays"] = weekdays;
            ViewData["queryResult"] = timeTableDetails;
            ViewData["Periods"] = _context.Periods.ToList();
            ViewData["ReportType"] = ReportType;
            return View();
        }

        public async Task<IActionResult> AutomaticTimeTableGenerator()
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            ViewData["AcademicYears"] = new SelectList(academicYears,"AcademicYearId","RosterName");
            ViewData["SinglePeriodTimes"] = new SelectList(Common.GetTimeComboBox(0.30, 0.60), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutomaticTimeTableGenerator(IFormCollection iFormCollection)
        {
            int academicYearSectionId = 0;
            if (!string.IsNullOrEmpty(iFormCollection["AcademicYearSectionId"]) && Convert.ToInt32(iFormCollection["AcademicYearSectionId"]) > 0)
                academicYearSectionId = Convert.ToInt32(iFormCollection["AcademicYearSectionId"]);

            //teachers, section, subject, maximum class per week
            List<SectionTeacherSubject> sectionTeacherSubjects = _context.SectionTeacherSubjects.Where(a=>a.AcademicYearSectionId == academicYearSectionId).ToList();
            var periods = _context.Periods.ToList();

            List<List<string>> freePeriods = new List<List<string>>();
            //adding week days
            List<string> weekDays = new List<string>();
            weekDays.Add("MONDAY");
            weekDays.Add("TUESDAY");
            weekDays.Add("WEDNSDAY");
            weekDays.Add("THURSDAY");
            weekDays.Add("FRIDAY");
            freePeriods.Add(weekDays);//adding week days to the top row

            #region CREATE FREE PERIODS
            //creating free periods (on the first row already week days name added)
            for (var k = 1; k <= periods.Count; k++)
            {
                List<string> freePeriod = new List<string>();                
                for (var j = 0; j < weekDays.Count; j++)
                    freePeriod.Add("0");

                freePeriods.Add(freePeriod);
            }
            #endregion

            int i = 0;
            Random rnd = new Random();

            do {

                SectionTeacherSubject sectionTeacherSubject = sectionTeacherSubjects[i];
                for (int j = 0; j < sectionTeacherSubject.TotalClassPerWeek; j++)
                {
                    List<int> indexes = lookRandomForFree(freePeriods, weekDays.Count, periods.Count);
                    if (indexes.Count == 2)
                        freePeriods[indexes[0]][indexes[1]] = sectionTeacherSubject.Id.ToString();
                }

                sectionTeacherSubjects.Remove(sectionTeacherSubject);//removing from list (after period assigned)

                if (sectionTeacherSubjects.Count > 0) {
                    i = 0;
                }
                
            } while (sectionTeacherSubjects.Count>0);

            #region Old way
            ////iterating on assigned sectionss
            //Random rnd = new Random();
            //for (int i=0;i<sectionTeacherSubjects.Count;i++) 
            //{
            //    SectionTeacherSubject sectionTeacherSubject = sectionTeacherSubjects[i];
            //    for (int j = 0; j < sectionTeacherSubject.TotalClassPerWeek; j++) 
            //    {
            //        List<int> indexes = lookRandomForFree(freePeriods, weekDays.Count, periods.Count);
            //        if(indexes.Count == 2)
            //            freePeriods[indexes[0]][indexes[1]] = sectionTeacherSubject.Id.ToString();
            //    }

            //    sectionTeacherSubjects.Remove(sectionTeacherSubject);//removing from list (after period assigned)

            //    if(sectionTeacherSubjects.Count > 0)
            //        i = 0;
            //}
            #endregion

            if (academicYearSectionId > 0)
            {
                #region Cleaning previous entries
                //cleaning previous entries
                var timeTables = _context.TimeTables.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();
                
                foreach (var timeTable in timeTables)
                {
                    var timeTableDetails = _context.TimeTableDetails.Where(a => a.TimeTableId == timeTable.Id).ToList();
                    _context.TimeTableDetails.RemoveRange(timeTableDetails);
                    _context.SaveChanges();
                }

                _context.TimeTables.RemoveRange(timeTables);
                _context.SaveChanges();
                #endregion

                TimeTable timeTable1 = new TimeTable();
                timeTable1.AcademicYearSectionId = academicYearSectionId;
                timeTable1.Status = 1;
                _context.TimeTables.Add(timeTable1);
                int pass = _context.SaveChanges();

                if (pass > 0) 
                {
                    //saving to details to database
                    for (int k = 1; k < freePeriods.Count; k++)
                    {
                        for (int l = 0; l < freePeriods[0].Count; l++)
                        {
                            if (freePeriods[k][l] != "0")
                            {
                                int stsId = Convert.ToInt32(freePeriods[k][l]);
                                SectionTeacherSubject sectionTeacherSubject = _context.SectionTeacherSubjects.Find(stsId);
                                if (sectionTeacherSubject != null) 
                                {
                                    SubjectGroupDetail subjectGroupDetail = _context.SubjectGroupDetails.Find(sectionTeacherSubject.SubjectGroupDetailId);
                                    
                                    TimeTableDetail timeTableDetail = new TimeTableDetail();
                                    timeTableDetail.SubjectId = subjectGroupDetail.SubjectId;
                                    timeTableDetail.PeriodId = periods[k-1].Id;
                                    timeTableDetail.EmployeeId = sectionTeacherSubject.TeacherId;
                                    timeTableDetail.TimeTableId = timeTable1.Id;
                                    timeTableDetail.WeekDay = weekDays[l];
                                    timeTableDetail.Status = 1;

                                    _context.TimeTableDetails.Add(timeTableDetail);                                    
                                }
                            }
                        }
                    }

                    //applying changes
                    _context.SaveChanges();
                }           
            }

            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            ViewData["SinglePeriodTimes"] = new SelectList(Common.GetTimeComboBox(0.30, 0.60), "Value", "Text");
            return RedirectToAction(nameof(Index));
        }

        public List<int> lookRandomForFree(List<List<string>> freePeriods, int WeekDaysCount, int PeriodsCount)
        {
            List<int> indexes = new List<int>();
            bool found = false;
            int RandWeekDayIndex = 0;
            int RandPeriodIndex = 1;
            int MaxWeekDayIndex = WeekDaysCount - 1;
            int MaxPeriodIndex = PeriodsCount;

            //iterate through freePeriods list until it gets free period and return the indexes
            do {
                
                Random rnd = new Random();
                RandWeekDayIndex = rnd.Next(MaxWeekDayIndex);
                RandPeriodIndex = rnd.Next(MaxPeriodIndex);

                if (freePeriods[RandPeriodIndex][RandWeekDayIndex] == "0") 
                {
                    found = true;
                    
                    //return indexes
                    indexes.Add(RandPeriodIndex);
                    indexes.Add(RandWeekDayIndex);
                }

            } while (!found);

            return indexes;
        }

        public List<List<string>> lookFromRight(List<List<string>> freePeriods, SectionTeacherSubject sectionTeacherSubject) {
            List<string> weekDays = freePeriods[0];
            for (int i = (freePeriods.Count-1); i > 0; i--) 
            {
                bool done = false;
                for (int j = (weekDays.Count - 1); j >= 0; j--) 
                {
                    if (freePeriods[i][j] == "0") 
                    {
                        freePeriods[i][j] = sectionTeacherSubject.Id.ToString();
                        done = true;
                        break;
                    }   
                }

                if (done)
                    break;
            }

            return freePeriods;
        }

        public List<List<string>> lookFromLeft(List<List<string>> freePeriods, SectionTeacherSubject sectionTeacherSubject)
        {
            List<string> weekDays = freePeriods[0];
            for (int i = 1; i < freePeriods.Count; i++)
            {
                bool done = false;
                for (int j = 0; j < weekDays.Count; j++)
                {
                    if (freePeriods[i][j] == "0")
                    {
                        freePeriods[i][j] = sectionTeacherSubject.Id.ToString();
                        done = true;
                        break;
                    }
                }

                if (done)
                    break;
            }

            return freePeriods;
        }

        // GET: Registrar/TimeTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TimeTables == null)
            {
                return NotFound();
            }

            var timeTable = await _context.TimeTables
                .Include(t => t.AcademicYearSection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeTable == null)
            {
                return NotFound();
            }

            return View(timeTable);
        }

        // GET: Registrar/TimeTables/Create
        public IActionResult Create()
        {
            var periods = string.Empty;
            var subjects = string.Empty;
            string weekdays = "MONDAY,TUESDAY,WEDNSDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY";
            var teachers = string.Empty;

            #region Preparing parameters
            var periodLists = _context.Periods;
            foreach (Period p in periodLists)
            {
                periods += p.Id + "#" + (p.Name + "(" + p.FromTime + "-"+p.ToTime+")") + ",";//id#(name+(fromtime-totime))
            }
            var subjectLists = _context.Subjects;
            foreach (Subject p in subjectLists)
            {
                subjects += p.Id + "#" + (p.Name + "(" + p.Code + ")") + ",";//id#(name+(code))
            }
            var teacherLists = _context.Employees.ToList(); //.Where(e=>e.Role.Name.Contains("teacher"));//chosing only teachers
            foreach (Employee p in teacherLists)
            {
                teachers += p.Id + "#" + (p.FirstName + " "+ p.MiddleName + " " + p.LastName) + ",";//id#(full-name)
            }
            #endregion

            ViewData["Periods"] = periods;
            ViewData["Subjects"] = subjects;
            ViewData["Weekdays"] = weekdays;
            ViewData["Teachers"] = teachers;
            ViewData["Years"] = new SelectList(Common.GetYearsComboBox(), "Value", "Text");
            ViewData["Sessions"] = new SelectList(Common.GetCustomSelectList("Sessions", new List<string> { "Id","SchoolType","Year" }), "Value", "Text");
            ViewData["Classes"] = new SelectList(_context.Classes, "Id", "Name");
            ViewData["Sections"] = new SelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: Registrar/TimeTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AcademicYearSectionId")] TimeTable timeTable, IFormCollection iformCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            //saving timetable
            if (!string.IsNullOrEmpty(iformCollection["YearId"]) && !string.IsNullOrEmpty(iformCollection["SessionId"]) && !string.IsNullOrEmpty(iformCollection["ClassId"]) && !string.IsNullOrEmpty(iformCollection["SectionId"])) 
            {
                int yearId = Convert.ToInt32(iformCollection["YearId"]);
                int sessionId = Convert.ToInt32(iformCollection["SessionId"]);
                int classId = Convert.ToInt32(iformCollection["ClassId"]);
                int sectionId = Convert.ToInt32(iformCollection["SectionId"]);

                #region queryResult - academic year section searching
                var queryResult = from aYS in _context.AcademicYearSections.Where(a => a.Status != 0)
                                  join aY in _context.AcademicYears on aYS.AcademicYearId equals aY.Id
                                  join s in _context.Sessions on aY.SessionId equals s.Id
                                  where s.Year == yearId && s.Id == sessionId && aY.ClassId == classId && aYS.SectionId == sectionId
                                  select new
                                  {
                                      AcademicYearSectionId = aYS.Id,
                                      AcademicYear = s.Year,
                                      SessionId = s.Id,
                                      aY.ClassId,
                                      aYS.SectionId,
                                      aYS.NoOfStudent,
                                      s.MinStudent,
                                      s.MaxStudent,
                                      s.OpeningDate,
                                      s.ClosingDate,
                                  };
                #endregion

                var academicYearSections = queryResult.ToList();
                if (academicYearSections.Count > 0) 
                {
                    int academicYearSectionId = academicYearSections[0].AcademicYearSectionId;

                    //saving time table
                    TimeTable tB = new TimeTable();
                    tB.AcademicYearSectionId = academicYearSectionId;
                    tB.Status = 1;
                    _context.TimeTables.Add(tB);
                    int pass = _context.SaveChanges();

                    if (pass > 0) 
                    {
                        //saving time table details

                        for (int i = 0; i < iformCollection.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(iformCollection["period_id_" + i]) && !string.IsNullOrEmpty(iformCollection["subject_id_" + i]) && !string.IsNullOrEmpty(iformCollection["weekday_id_" + i]) && !string.IsNullOrEmpty(iformCollection["teacher_id_" + i]))
                            {
                                int periodId = Convert.ToInt32(iformCollection["period_id_" + i]);
                                int subjectId = Convert.ToInt32(iformCollection["subject_id_" + i]);
                                string weekdayId = iformCollection["weekday_id_" + i].ToString();
                                int teacherId = Convert.ToInt32(iformCollection["teacher_id_" + i]);

                                TimeTableDetail timeTableDetail = new TimeTableDetail();
                                timeTableDetail.TimeTableId = tB.Id;//time table Id
                                timeTableDetail.PeriodId = periodId;
                                timeTableDetail.SubjectId = subjectId;
                                timeTableDetail.WeekDay = weekdayId;
                                timeTableDetail.EmployeeId = teacherId;
                                timeTableDetail.Status = 1;

                                _context.TimeTableDetails.Add(timeTableDetail);
                                int pass2 = _context.SaveChanges();

                            }
                        }


                    }
                }
    
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Registrar/TimeTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TimeTables == null)
            {
                return NotFound();
            }

            var timeTable = await _context.TimeTables.FindAsync(id);
            if (timeTable == null)
            {
                return NotFound();
            }
            ViewData["AcademicYearSectionId"] = new SelectList(_context.AcademicYearSections, "Id", "Id", timeTable.AcademicYearSectionId);
            return View(timeTable);
        }

        // POST: Registrar/TimeTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AcademicYearSectionId")] TimeTable timeTable)
        {
            if (id != timeTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeTableExists(timeTable.Id))
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
            ViewData["AcademicYearSectionId"] = new SelectList(_context.AcademicYearSections, "Id", "Id", timeTable.AcademicYearSectionId);
            return View(timeTable);
        }

        // GET: Registrar/TimeTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TimeTables == null)
            {
                return NotFound();
            }

            var timeTable = await _context.TimeTables
                .Include(t => t.AcademicYearSection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeTable == null)
            {
                return NotFound();
            }

            return View(timeTable);
        }

        // POST: Registrar/TimeTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TimeTables == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TimeTables'  is null.");
            }
            var timeTable = await _context.TimeTables.FindAsync(id);
            if (timeTable != null)
            {
                _context.TimeTables.Remove(timeTable);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeTableExists(int id)
        {
          return _context.TimeTables.Any(e => e.Id == id);
        }
    }
}
