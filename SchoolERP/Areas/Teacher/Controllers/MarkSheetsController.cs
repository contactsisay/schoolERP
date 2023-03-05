using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BALibrary.Registrar;
using SchoolERP.Data;
using System.ComponentModel;
using BALibrary.Examination;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Serialization;
using ExcelDataReader.Log.Logger;
using SchoolERP.Models;
using BALibrary.Academic;
using static System.Collections.Specialized.BitVector32;
using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using AspNetCore.Reporting;
using ExcelDataReader;

namespace SchoolERP.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class MarkSheetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MarkSheetsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #region LOADING DATA
        [HttpPost]
        public JsonResult GetSessionClasses(string sid)
        {
            var classes = from aY in _context.AcademicYears.Include(a => a.Class)
                          where aY.SessionId == Convert.ToInt32(sid)
                          select new
                          {
                              AYId = aY.Id,
                              aY.ClassId,
                              ClassName = "Grade " + aY.Class.Name,
                          };

            return Json(new SelectList(classes, "AYId", "ClassName"));
        }

        [HttpPost]
        public JsonResult GetYearClasses(string year)
        {
            var classes = from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Semester)
                          where aY.Session.Year == Convert.ToInt32(year)
                          select new
                          {
                              AYId = aY.Id,
                              aY.ClassId,
                              ClassName = aY.Session.Year + "-" + aY.Session.SchoolType.ToUpper() + "(" + aY.Class.Name + ")",
                          };

            return Json(new SelectList(classes, "AYId", "ClassName"));
        }

        [HttpPost]
        public JsonResult GetClassSections(string cid)
        {
            int classId = Convert.ToInt32(cid);
            var academicYearSections = from aYS in _context.AcademicYearSections.Include(a => a.Section)
                                       join aY in _context.AcademicYears.Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                       where aY.Id == Convert.ToInt32(cid)
                                       select new
                                       {
                                           AYId = aY.Id,
                                           AYSId = aYS.Id,
                                           aY.ClassId,
                                           ClassName = aY.Class.Name,
                                           aYS.SectionId,
                                           SectionName2 = aYS.Section.Name,
                                           SectionName = aY.Class.Name + "(" + aYS.Section.Name + ")",
                                       };

            return Json(new SelectList(academicYearSections, "AYSId", "SectionName"));
        }

        [HttpPost]
        public JsonResult GetClassSections2(string cid)
        {
            int classId = Convert.ToInt32(cid);
            var academicYearSections = from aYS in _context.AcademicYearSections.Include(a => a.Section)
                                       join aY in _context.AcademicYears.Include(a => a.Class) on aYS.AcademicYearId equals aY.Id
                                       where aY.ClassId == classId
                                       select new
                                       {
                                           AYId = aY.Id,
                                           AYSId = aYS.Id,
                                           aY.ClassId,
                                           ClassName = aY.Class.Name,
                                           aYS.SectionId,
                                           SectionName2 = aYS.Section.Name,
                                           SectionName = aY.Class.Name + "(" + aYS.Section.Name + ")",
                                       };

            return Json(new SelectList(academicYearSections, "AYSId", "SectionName"));
        }

        [HttpPost]
        public JsonResult GetClassSubjects(string cid)
        {
            string options = string.Empty;
            int classId = Convert.ToInt32(cid);
            var academicYears = from aY in _context.AcademicYears.Include(a => a.Class)
                                join sGD in _context.SubjectGroupDetails on aY.SubjectGroupId equals sGD.SubjectGroupId
                                join s in _context.Subjects on sGD.SubjectId equals s.Id
                                where aY.Id == Convert.ToInt32(cid)
                                select new
                                {
                                    SGDId = sGD.Id,
                                    AYId = aY.Id,
                                    aY.ClassId,
                                    ClassName = aY.Class.Name,
                                    SubjectId = s.Id,
                                    SubjectName = s.Name + "(" + s.Code + ")",
                                };

            return Json(new SelectList(academicYears, "SGDId", "SubjectName"));
        }

        [HttpPost]
        public JsonResult GetClassExams(string cid)
        {
            int classId = Convert.ToInt32(cid);
            var academicYears = from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup)
                                join eGD in _context.ExamGroupDetails on aY.ExamGroupId equals eGD.ExamGroupId
                                join e in _context.Exams on eGD.ExamId equals e.Id
                                where aY.Id == Convert.ToInt32(cid)
                                select new
                                {
                                    EGDId = eGD.Id,
                                    AYId = aY.Id,
                                    aY.ClassId,
                                    ClassName = aY.Class.Name,
                                    ExamId = e.Id,
                                    ExamName = e.Name,
                                };

            return Json(new SelectList(academicYears, "EGDId", "ExamName"));
        }

        #endregion

        // GET: Registrar/MarkSheets
        public async Task<IActionResult> Index(string AcademicYearId, string AcademicYearSectionId, string SubjectGroupDetailId, string ExamGroupDetailId, IFormFile file)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            if ((AcademicYearId != null && Convert.ToInt32(AcademicYearId) > 0) && (AcademicYearSectionId != null && Convert.ToInt32(AcademicYearSectionId) > 0) && (SubjectGroupDetailId != null && Convert.ToInt32(SubjectGroupDetailId) > 0))
            {
                int academicYearId = Convert.ToInt32(AcademicYearId);
                int academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);
                int subjectGroupDetailId = Convert.ToInt32(SubjectGroupDetailId);
                int examGroupDetailId = 0;

                var subjectMarks = (from mS in _context.MarksSheets
                                  join sP in _context.StudentPromotions on mS.StudentPromotionId equals sP.Id
                                  where sP.AcademicYearSectionId == academicYearSectionId && mS.SubjectGroupDetailId == subjectGroupDetailId
                                  select new 
                                  {
                                     mS.Id,
                                  }).ToList();

                //if marks is already added, then redirect to summary page
                if (subjectMarks.Count > 0)
                    return RedirectToAction(nameof(MarkSummaryPerSubject), new { AcademicYearId = academicYearId, AcademicYearSectionId = academicYearSectionId, SubjectGroupDetailId = subjectGroupDetailId });

                #region Preparing data
                if (!string.IsNullOrEmpty(ExamGroupDetailId) && Convert.ToInt32(ExamGroupDetailId) > 0)
                    examGroupDetailId = Convert.ToInt32(ExamGroupDetailId);

                var students = (from sP in _context.StudentPromotions.Include(a => a.AcademicYearSection).Include(a => a.Student).Include(a => a.AcademicYearSection.Section)
                                join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                where aY.Id == academicYearId && sP.AcademicYearSectionId == academicYearSectionId
                                select new
                                {
                                    SessionId = aY.Session.Id,
                                    AcademicYearId = aY.Id,
                                    AYSId = sP.AcademicYearSectionId,
                                    aY.ClassId,
                                    StudentPromotionId = sP.Id,
                                    aY.SemesterId,
                                    sP.AcademicYearSection.SectionId,
                                    sP.Student.IDNo,
                                    FullName = sP.Student.FirstName + " " + sP.Student.MiddleName + " " + sP.Student.LastName,
                                    Semester = aY.Semester.Name,
                                    ClassName = aY.Class.Name,
                                    SectionName = aY.Class.Name + "(" + sP.AcademicYearSection.Section.Name + ")",
                                }).ToList();

                var examList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup)
                                join eGD in _context.ExamGroupDetails.Include(a => a.Exam) on aY.ExamGroupId equals eGD.ExamGroupId
                                where aY.Id == academicYearId
                                select new
                                {
                                    aYId = aY.Id,
                                    ExamGroupDetailId = eGD.Id,
                                    aY.ClassId,
                                    ExamName = eGD.Exam.Name,
                                    Weight = eGD.Exam.Weight,
                                }).ToList();

                if (examGroupDetailId > 0)
                    examList = examList.Where(a => a.ExamGroupDetailId == Convert.ToInt32(ExamGroupDetailId)).ToList();
                #endregion

                //if file is attached, then import from the file
                if (file != null)
                {
                    //uploading file
                    #region Uploading File
                    string fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\import_marksheets_" + file.FileName);
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
                                    //passing header
                                    if (i < 1) {
                                        i++;
                                        continue;
                                    }
                                    
                                    int studentPromotionId = 0;
                                    #region GET STUDENT PROMOTION ID
                                    var studentIdNo = reader.GetString(2);                                    
                                    //search student in promotions
                                    foreach (var sp in students) 
                                    {
                                        if (sp.IDNo.Equals(studentIdNo)) {
                                            studentPromotionId = sp.StudentPromotionId;
                                            break;
                                        }
                                    }
                                    #endregion

                                    if (studentPromotionId > 0) 
                                    {
                                        MarkSheet markSheet = new MarkSheet();
                                        markSheet.StudentPromotionId = studentPromotionId;
                                        markSheet.SubjectGroupDetailId = subjectGroupDetailId;
                                        markSheet.Average = 0;
                                        markSheet.ApprovedAt = DateTime.Now;
                                        markSheet.ApprovedBy = currentUserId;
                                        markSheet.RegisteredAt = DateTime.Now;
                                        markSheet.RegisteredBy = currentUserId;
                                        markSheet.ModifiedAt = DateTime.Now;
                                        markSheet.ModifiedBy = currentUserId;
                                        markSheet.Status = 1;
                                        _context.MarksSheets.Add(markSheet);
                                        int pass = _context.SaveChanges();

                                        if (pass > 0)
                                        {
                                            //saving marksheet details
                                            MarkSheetDetail markSheetDetail = new MarkSheetDetail();

                                            #region SEMESTER I
                                            int index = 7;
                                            //checking if excel contains first semester marks
                                            if (reader.GetValue(index) != null)
                                            {
                                                #region SEMESTER I MARKS
                                                decimal test1Mark, contAss1Mark, midMark, test2Mark, contAss2Mark, finalMark = 0;
                                                try
                                                {
                                                    test1Mark = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                                }
                                                catch (Exception ex)
                                                {
                                                    test1Mark = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                                }

                                                markSheetDetail = new MarkSheetDetail();
                                                markSheetDetail.MarkSheetId = markSheet.Id;
                                                markSheetDetail.ExamGroupDetailId = examList[0].ExamGroupDetailId;
                                                markSheetDetail.ExamMark = test1Mark;
                                                markSheetDetail.RarkFromGrade = 0;
                                                markSheetDetail.RarkFromSection = 0;
                                                markSheetDetail.IsPassed = test1Mark >= (examList[0].Weight/2) ? true : false;
                                                markSheetDetail.Status = 1;
                                                _context.MarksSheetDetails.Add(markSheetDetail);
                                                _context.SaveChanges();

                                                index = 8;
                                                try
                                                {
                                                    contAss1Mark = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                                }
                                                catch (Exception ex)
                                                {
                                                    contAss1Mark = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                                }

                                                markSheetDetail = new MarkSheetDetail();
                                                markSheetDetail.MarkSheetId = markSheet.Id;
                                                markSheetDetail.ExamGroupDetailId = examList[1].ExamGroupDetailId;
                                                markSheetDetail.ExamMark = contAss1Mark;
                                                markSheetDetail.RarkFromGrade = 0;
                                                markSheetDetail.RarkFromSection = 0;
                                                markSheetDetail.IsPassed = contAss1Mark >= (examList[1].Weight/2) ? true : false;
                                                markSheetDetail.Status = 1;
                                                _context.MarksSheetDetails.Add(markSheetDetail);
                                                _context.SaveChanges();

                                                index = 9;
                                                try
                                                {
                                                    midMark = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                                }
                                                catch (Exception ex)
                                                {
                                                    midMark = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                                }

                                                markSheetDetail = new MarkSheetDetail();
                                                markSheetDetail.MarkSheetId = markSheet.Id;
                                                markSheetDetail.ExamGroupDetailId = examList[2].ExamGroupDetailId;
                                                markSheetDetail.ExamMark = midMark;
                                                markSheetDetail.RarkFromGrade = 0;
                                                markSheetDetail.RarkFromSection = 0;
                                                markSheetDetail.IsPassed = midMark >= (examList[2].Weight / 2) ? true : false;
                                                markSheetDetail.Status = 1;
                                                _context.MarksSheetDetails.Add(markSheetDetail);
                                                _context.SaveChanges();

                                                index = 11;
                                                try
                                                {
                                                    test2Mark = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                                }
                                                catch (Exception ex)
                                                {
                                                    test2Mark = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                                }

                                                markSheetDetail = new MarkSheetDetail();
                                                markSheetDetail.MarkSheetId = markSheet.Id;
                                                markSheetDetail.ExamGroupDetailId = examList[3].ExamGroupDetailId;
                                                markSheetDetail.ExamMark = test2Mark;
                                                markSheetDetail.RarkFromGrade = 0;
                                                markSheetDetail.RarkFromSection = 0;
                                                markSheetDetail.IsPassed = test2Mark >= (examList[3].Weight / 2) ? true : false;
                                                markSheetDetail.Status = 1;
                                                _context.MarksSheetDetails.Add(markSheetDetail);
                                                _context.SaveChanges();

                                                index = 12;
                                                try
                                                {
                                                    contAss2Mark = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                                }
                                                catch (Exception ex)
                                                {
                                                    contAss2Mark = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                                }

                                                markSheetDetail = new MarkSheetDetail();
                                                markSheetDetail.MarkSheetId = markSheet.Id;
                                                markSheetDetail.ExamGroupDetailId = examList[4].ExamGroupDetailId;
                                                markSheetDetail.ExamMark = contAss2Mark;
                                                markSheetDetail.RarkFromGrade = 0;
                                                markSheetDetail.RarkFromSection = 0;
                                                markSheetDetail.IsPassed = contAss2Mark >= (examList[4].Weight / 2) ? true : false;
                                                markSheetDetail.Status = 1;
                                                _context.MarksSheetDetails.Add(markSheetDetail);
                                                _context.SaveChanges();

                                                index = 13;
                                                try
                                                {
                                                    finalMark = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                                }
                                                catch (Exception ex)
                                                {
                                                    finalMark = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                                }

                                                markSheetDetail = new MarkSheetDetail();
                                                markSheetDetail.MarkSheetId = markSheet.Id;
                                                markSheetDetail.ExamGroupDetailId = examList[5].ExamGroupDetailId;
                                                markSheetDetail.ExamMark = finalMark;
                                                markSheetDetail.RarkFromGrade = 0;
                                                markSheetDetail.RarkFromSection = 0;
                                                markSheetDetail.IsPassed = finalMark >= (examList[5].Weight / 2) ? true : false;
                                                markSheetDetail.Status = 1;
                                                _context.MarksSheetDetails.Add(markSheetDetail);
                                                _context.SaveChanges();
                                                #endregion
                                            }
                                            #endregion

                                            #region SEMESTER II
                                            //index = 16;
                                            ////checking if excel contains second semester marks
                                            //if (reader.GetValue(index) != null && reader.GetDouble(index) > 0)
                                            //{
                                            //    #region SEMESTER II MARKS
                                            //    decimal test1Mark2, contAss1Mark2, midMark2, test2Mark2, contAss2Mark2, finalMark2 = 0;
                                            //    try
                                            //    {
                                            //        test1Mark2 = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        test1Mark2 = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                            //    }

                                            //    markSheetDetail = new MarkSheetDetail();
                                            //    markSheetDetail.MarkSheetId = markSheet.Id;
                                            //    markSheetDetail.ExamGroupDetailId = examList[0].ExamGroupDetailId;
                                            //    markSheetDetail.ExamMark = test1Mark2;
                                            //    markSheetDetail.RarkFromGrade = 0;
                                            //    markSheetDetail.RarkFromSection = 0;
                                            //    markSheetDetail.IsPassed = test1Mark2 >= (examList[0].Weight/2) ? true : false;
                                            //    markSheetDetail.Status = 1;
                                            //    _context.MarksSheetDetails.Add(markSheetDetail);
                                            //    _context.SaveChanges();

                                            //    index = 17;
                                            //    try
                                            //    {
                                            //        contAss2Mark2 = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        contAss2Mark2 = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                            //    }

                                            //    markSheetDetail = new MarkSheetDetail();
                                            //    markSheetDetail.MarkSheetId = markSheet.Id;
                                            //    markSheetDetail.ExamGroupDetailId = examList[1].ExamGroupDetailId;
                                            //    markSheetDetail.ExamMark = contAss2Mark2;
                                            //    markSheetDetail.RarkFromGrade = 0;
                                            //    markSheetDetail.RarkFromSection = 0;
                                            //    markSheetDetail.IsPassed = contAss2Mark2 >= (examList[1].Weight/2) ? true : false;
                                            //    markSheetDetail.Status = 1;
                                            //    _context.MarksSheetDetails.Add(markSheetDetail);
                                            //    _context.SaveChanges();

                                            //    index = 18;
                                            //    try
                                            //    {
                                            //        midMark2 = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        midMark2 = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                            //    }

                                            //    markSheetDetail = new MarkSheetDetail();
                                            //    markSheetDetail.MarkSheetId = markSheet.Id;
                                            //    markSheetDetail.ExamGroupDetailId = examList[2].ExamGroupDetailId;
                                            //    markSheetDetail.ExamMark = midMark2;
                                            //    markSheetDetail.RarkFromGrade = 0;
                                            //    markSheetDetail.RarkFromSection = 0;
                                            //    markSheetDetail.IsPassed = midMark2 >= (examList[2].Weight/2) ? true : false;
                                            //    markSheetDetail.Status = 1;
                                            //    _context.MarksSheetDetails.Add(markSheetDetail);
                                            //    _context.SaveChanges();

                                            //    index = 20;
                                            //    try
                                            //    {
                                            //        test2Mark2 = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        test2Mark2 = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                            //    }

                                            //    markSheetDetail = new MarkSheetDetail();
                                            //    markSheetDetail.MarkSheetId = markSheet.Id;
                                            //    markSheetDetail.ExamGroupDetailId = examList[3].ExamGroupDetailId;
                                            //    markSheetDetail.ExamMark = test2Mark2;
                                            //    markSheetDetail.RarkFromGrade = 0;
                                            //    markSheetDetail.RarkFromSection = 0;
                                            //    markSheetDetail.IsPassed = test2Mark2 >= (examList[3].Weight/2) ? true : false;
                                            //    markSheetDetail.Status = 1;
                                            //    _context.MarksSheetDetails.Add(markSheetDetail);
                                            //    _context.SaveChanges();

                                            //    index = 21;
                                            //    try
                                            //    {
                                            //        contAss2Mark2 = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        contAss2Mark2 = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                            //    }

                                            //    markSheetDetail = new MarkSheetDetail();
                                            //    markSheetDetail.MarkSheetId = markSheet.Id;
                                            //    markSheetDetail.ExamGroupDetailId = examList[4].ExamGroupDetailId;
                                            //    markSheetDetail.ExamMark = contAss2Mark2;
                                            //    markSheetDetail.RarkFromGrade = 0;
                                            //    markSheetDetail.RarkFromSection = 0;
                                            //    markSheetDetail.IsPassed = contAss2Mark2 >= (examList[4].Weight/2) ? true : false;
                                            //    markSheetDetail.Status = 1;
                                            //    _context.MarksSheetDetails.Add(markSheetDetail);
                                            //    _context.SaveChanges();

                                            //    index = 22;
                                            //    try
                                            //    {
                                            //        finalMark2 = reader.GetDouble(index) > 0 ? Convert.ToDecimal(reader.GetDouble(index)) : 0;
                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        finalMark2 = string.IsNullOrEmpty(reader.GetString(index)) ? 0 : Convert.ToDecimal(reader.GetString(index));
                                            //    }

                                            //    markSheetDetail = new MarkSheetDetail();
                                            //    markSheetDetail.MarkSheetId = markSheet.Id;
                                            //    markSheetDetail.ExamGroupDetailId = examList[5].ExamGroupDetailId;
                                            //    markSheetDetail.ExamMark = finalMark2;
                                            //    markSheetDetail.RarkFromGrade = 0;
                                            //    markSheetDetail.RarkFromSection = 0;
                                            //    markSheetDetail.IsPassed = finalMark2 >= (examList[5].Weight/2) ? true : false;
                                            //    markSheetDetail.Status = 1;
                                            //    _context.MarksSheetDetails.Add(markSheetDetail);
                                            //    _context.SaveChanges();
                                            //    #endregion
                                            //}

                                            #endregion
                                        }
                                    }
                                    
                                    i++;
                                }
                            } while (reader.NextResult());
                        }
                    }
                    #endregion

                    return RedirectToAction(nameof(MarkSummaryPerSubject), new { AcademicYearId=academicYearId,AcademicYearSectionId=academicYearSectionId,SubjectGroupDetailId=subjectGroupDetailId });
                }

                ViewData["exams"] = examList;
                ViewData["AcademicYears1"] = academicYears;//list
                ViewData["queryResult"] = students;
                ViewData["aYId"] = academicYearId;
                ViewData["academicYearSectionId"] = academicYearSectionId;
                ViewData["subjectGroupDetailId"] = subjectGroupDetailId;
                ViewData["examGroupDetailId"] = examGroupDetailId;
                ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
                return View();
            }

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return View();
        }

        // GET: Registrar/MarkSheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MarksSheets == null)
            {
                return NotFound();
            }

            var markSheet = await _context.MarksSheets
                .Include(m => m.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markSheet == null)
            {
                return NotFound();
            }

            return View(markSheet);
        }

        // GET: Registrar/MarkSheets/Create
        public IActionResult Create()
        {
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id");
            return View();
        }

        // POST: Registrar/MarkSheets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentPromotionId,SubjectGroupDetailId,Average,RegisteredBy,RegisteredAt,ModifiedBy,ModifiedAt,ApprovedBy,ApprovedAt")] MarkSheet markSheet, IFormCollection iFormCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if (!string.IsNullOrEmpty(iFormCollection["AYId"]) && !string.IsNullOrEmpty(iFormCollection["AcademicYearSectionId"]) && !string.IsNullOrEmpty(iFormCollection["SubjectGroupDetailId"]))
            {
                int academicYearId = Convert.ToInt32(iFormCollection["AYId"]);
                int academicYearSectionId = Convert.ToInt32(iFormCollection["AcademicYearSectionId"]);
                int subjectGroupDetailId = Convert.ToInt32(iFormCollection["SubjectGroupDetailId"]);
                int examGroupDetailId = 0;
                if (!string.IsNullOrEmpty(iFormCollection["ExamGroupDetailId"]))
                    examGroupDetailId = Convert.ToInt32(iFormCollection["ExamGroupDetailId"]);

                var examList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup)
                                join eGD in _context.ExamGroupDetails.Include(a => a.Exam) on aY.ExamGroupId equals eGD.ExamGroupId
                                where aY.Id == Convert.ToInt32(academicYearId)
                                select new
                                {
                                    aYId = aY.Id,
                                    ExamGroupDetailId = eGD.Id,
                                    aY.ClassId,
                                    ExamName = eGD.Exam.Name,
                                }).ToList();

                if (examGroupDetailId > 0)
                    examList = examList.Where(a => a.ExamGroupDetailId == examGroupDetailId).ToList();

                for (int i = 0; i < iFormCollection.Count; i++)
                {
                    if (!string.IsNullOrEmpty(iFormCollection["StudentPromotionId_" + i]))
                    {
                        //Saving marksheet is left
                        MarkSheet markSheet1 = new MarkSheet();
                        markSheet1.StudentPromotionId = Convert.ToInt32(iFormCollection["StudentPromotionId_" + i]);
                        markSheet1.SubjectGroupDetailId = subjectGroupDetailId;
                        markSheet1.Average = 0;
                        markSheet1.RegisteredAt = DateTime.Now;
                        markSheet1.RegisteredBy = currentUserId;
                        markSheet1.Status = 0;

                        _context.MarksSheets.Add(markSheet1);
                        int pass = _context.SaveChanges();

                        if (pass > 0)
                        {
                            //Saving marksheet details
                            foreach (var e in examList)
                            {
                                if (!string.IsNullOrEmpty(iFormCollection["mark_" + i + "_" + e.ExamGroupDetailId]))
                                {
                                    decimal ExamMark = Convert.ToDecimal(iFormCollection["mark_" + i + "_" + e.ExamGroupDetailId]);

                                    MarkSheetDetail markSheetDetail = new MarkSheetDetail();
                                    markSheetDetail.MarkSheetId = markSheet1.Id;
                                    markSheetDetail.ExamGroupDetailId = e.ExamGroupDetailId;
                                    markSheetDetail.ExamMark = ExamMark;
                                    markSheetDetail.RarkFromGrade = 0;
                                    markSheetDetail.RarkFromSection = 0;
                                    markSheetDetail.IsPassed = ExamMark >= 50 ? true : false;//checking with grading rule
                                    markSheetDetail.Status = 0;

                                    _context.MarksSheetDetails.Add(markSheetDetail);
                                }
                            }

                            _context.SaveChanges();
                        }
                    }
                }
            }

            var academicYears = from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                select new
                                {
                                    AcademicYearId = aY.Id,
                                    aY.ClassId,
                                    RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                };

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return RedirectToAction(nameof(Index));
        }

        //Summerize marks for one semester
        #region MARKS SUMMARY FOR ONE SEMESTER        
        public IActionResult MarkSummaryPerSubject(string AcademicYearId, string AcademicYearSectionId, string SubjectGroupDetailId)
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            if ((!string.IsNullOrEmpty(AcademicYearId) && Convert.ToInt32(AcademicYearId) > 0) && !string.IsNullOrEmpty(AcademicYearSectionId) && !string.IsNullOrEmpty(SubjectGroupDetailId))
            {
                var academicYearId = Convert.ToInt32(AcademicYearId);
                var academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);
                var subjectGroupDetailId = Convert.ToInt32(SubjectGroupDetailId);

                #region LOADING DATA
                var gradeStudentPromotions = (from sP in _context.StudentPromotions.Include(a => a.Student).Include(a => a.AcademicYearSection)
                                              join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                              where aY.Id == academicYearId
                                              select new
                                              {
                                                  SessionId = aY.Session.Id,
                                                  AcademicYearId = aY.Id,
                                                  SemesterId = aY.Semester.Id,
                                                  StudentPromotionId = sP.Id,
                                                  sP.AcademicYearSectionId,
                                                  aY.ClassId,
                                                  sP.Student.IDNo,
                                                  FullName = sP.Student.FirstName + " " + sP.Student.MiddleName,
                                                  sP.Student.Gender,
                                                  Semester = aY.Semester.Name,
                                                  ClassName = aY.Class.Name,
                                                  SectionName = aY.Class.Name + " (" + sP.AcademicYearSection.Section.Name + ")",
                                                  aY.ExamGroupId,
                                                  aY.SubjectGroupId,
                                              }).ToList();

                var sectionStudentPromotions = gradeStudentPromotions.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();

                var examList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup)
                                join eGD in _context.ExamGroupDetails.Include(a => a.Exam) on aY.ExamGroupId equals eGD.ExamGroupId
                                where aY.Id == academicYearId
                                select new
                                {
                                    aYId = aY.Id,
                                    ExamGroupDetailId = eGD.Id,
                                    aY.ClassId,
                                    ExamName = eGD.Exam.Name,
                                    ExamWeight = eGD.Exam.Weight,
                                }).ToList();

                var subjectList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.SubjectGroup)
                                   join sGD in _context.SubjectGroupDetails.Include(a => a.Subject) on aY.SubjectGroupId equals sGD.SubjectGroupId
                                   where aY.Id == academicYearId && sGD.Id == subjectGroupDetailId
                                   select new
                                   {
                                       aYId = aY.Id,
                                       SubjectGroupDetailId = sGD.Id,
                                       aY.ClassId,
                                       SubjectName = sGD.Subject.Name + "(" + sGD.Subject.Code + ")",
                                       SubjectCode = sGD.Subject.Code,
                                   }).ToList();
                #endregion

                #region PREPARING DATA FOR RANK PER SECTION
                //preparing data with viewmodel
                List<StudentMarkViewModel> sectionStudentMarkViewModels = new List<StudentMarkViewModel>();
                List<SubjectMarkViewModel> studentSubjectsMarkViewModels = new List<SubjectMarkViewModel>();
                foreach (var studentPromotion in sectionStudentPromotions)
                {
                    StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                    studentMarkViewModel.AcademicYearId = academicYearId;
                    studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                    studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                    studentMarkViewModel.IDNo = studentPromotion.IDNo;
                    studentMarkViewModel.FullName = studentPromotion.FullName;
                    studentMarkViewModel.Gender = studentPromotion.Gender;
                    studentMarkViewModel.SectionName = studentPromotion.SectionName;

                    //Calculating total mark
                    decimal overallMark = 0;//all subjects mark
                    var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                            join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                            where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                            select new
                                            {
                                                MarkSheetId = mS.Id,
                                                MarkSheeDetailId = mSD.Id,
                                                mS.SubjectGroupDetailId,
                                                mSD.ExamGroupDetailId,
                                                mSD.ExamMark,
                                            };

                    List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                    foreach (var subject in subjectList)
                    {
                        SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                        decimal subjectExamsMark = 0;
                        var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                        List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                        foreach (var subExam in subjectExams)
                        {
                            subjectExamsMark += subExam.ExamMark;

                            SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                            subjectExamMarkViewModel.ExamGroupDetailId = subExam.ExamGroupDetailId;
                            ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                            subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                            subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                            subjectExamMarkViewModel.ExamMark = subExam.ExamMark;

                            examMarks.Add(subjectExamMarkViewModel);
                        }

                        subjectMarkViewModel.RankFromGrade = 0;//with single subject
                        subjectMarkViewModel.RankFromSection = 0;//with single subject
                        subjectMarkViewModel.ExamMarks = examMarks;
                        subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)

                        subjectMarkViewModels.Add(subjectMarkViewModel);
                        studentSubjectsMarkViewModels.Add(subjectMarkViewModel);
                        overallMark += subjectExamsMark;
                    }

                    studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                    studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                    studentMarkViewModel.RankFromSection = 0;//with all subjects
                    studentMarkViewModel.RankFromGrade = 0;//with all subjects

                    sectionStudentMarkViewModels.Add(studentMarkViewModel);
                }

                sectionStudentMarkViewModels = sectionStudentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                studentSubjectsMarkViewModels = studentSubjectsMarkViewModels.OrderByDescending(a => a.TotalMark).ToList();

                #region SECTION RANK CALCULATION (FROM SINGLE SUBJECT & ALL EXAMS)
                //subject rank (from all exams)
                int count = 1;
                int similarCount = 0;
                for (int i = 0; i < sectionStudentMarkViewModels.Count; i++)
                {
                    if (i > 0)
                    {
                        if (sectionStudentMarkViewModels[i].OverallMark == sectionStudentMarkViewModels[i - 1].OverallMark)
                        {
                            sectionStudentMarkViewModels[i].RankFromSection = count;
                            similarCount++;
                        }
                        else
                        {
                            if (similarCount > 0)
                            {
                                count = similarCount + count + 1;
                                sectionStudentMarkViewModels[i].RankFromSection = count;
                                similarCount = 0;
                            }
                            else
                                sectionStudentMarkViewModels[i].RankFromSection = ++count;
                        }
                    }
                    else
                    {
                        sectionStudentMarkViewModels[i].RankFromSection = count;//taking the first
                    }
                }
                #endregion

                #endregion

                #region PREPARING DATA FOR RANK PER GRADE
                //preparing data with viewmodel
                List<StudentMarkViewModel> gradeStudentMarkViewModels = new List<StudentMarkViewModel>();
                List<SubjectMarkViewModel> gradeStudentSubjectsMarkViewModels = new List<SubjectMarkViewModel>();
                foreach (var studentPromotion in gradeStudentPromotions)
                {
                    StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                    studentMarkViewModel.AcademicYearId = academicYearId;
                    studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                    studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                    studentMarkViewModel.IDNo = studentPromotion.IDNo;
                    studentMarkViewModel.FullName = studentPromotion.FullName;
                    studentMarkViewModel.Gender = studentPromotion.Gender;
                    studentMarkViewModel.SectionName = studentPromotion.SectionName;

                    //Calculating total mark
                    decimal overallMark = 0;//all subjects mark
                    var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                            join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                            where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                            select new
                                            {
                                                MarkSheetId = mS.Id,
                                                MarkSheeDetailId = mSD.Id,
                                                mS.SubjectGroupDetailId,
                                                mSD.ExamGroupDetailId,
                                                mSD.ExamMark,
                                            };

                    List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                    foreach (var subject in subjectList)
                    {
                        SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                        decimal subjectExamsMark = 0;
                        var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                        List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                        foreach (var subExam in subjectExams)
                        {
                            subjectExamsMark += subExam.ExamMark;

                            SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                            subjectExamMarkViewModel.ExamGroupDetailId = subExam.ExamGroupDetailId;
                            ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                            subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                            subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                            subjectExamMarkViewModel.ExamMark = subExam.ExamMark;

                            examMarks.Add(subjectExamMarkViewModel);
                        }

                        subjectMarkViewModel.RankFromGrade = 0;//with single subject
                        subjectMarkViewModel.RankFromSection = 0;//with single subject
                        subjectMarkViewModel.ExamMarks = examMarks;
                        subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)

                        subjectMarkViewModels.Add(subjectMarkViewModel);
                        gradeStudentSubjectsMarkViewModels.Add(subjectMarkViewModel);
                        overallMark += subjectExamsMark;
                    }

                    studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                    studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                    studentMarkViewModel.RankFromSection = 0;//with all subjects
                    studentMarkViewModel.RankFromGrade = 0;//with all subjects

                    gradeStudentMarkViewModels.Add(studentMarkViewModel);
                }

                gradeStudentMarkViewModels = gradeStudentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                gradeStudentSubjectsMarkViewModels = gradeStudentSubjectsMarkViewModels.OrderByDescending(a => a.TotalMark).ToList();

                #region GRADE RANK CALCULATION (FROM SINGLE SUBJECT & ALL EXAMS & ALL STUDENTS)
                //subject rank (from all exams)
                int count1 = 1;
                int similarCount1 = 0;
                for (int i = 0; i < gradeStudentMarkViewModels.Count; i++)
                {
                    if (i > 0)
                    {
                        if (gradeStudentMarkViewModels[i].OverallMark == gradeStudentMarkViewModels[i - 1].OverallMark)
                        {
                            gradeStudentMarkViewModels[i].RankFromGrade = count1;
                            similarCount1++;
                        }
                        else
                        {
                            if (similarCount1 > 0)
                            {
                                count1 = similarCount1 + count1 + 1;
                                gradeStudentMarkViewModels[i].RankFromGrade = count1;
                                similarCount1 = 0;
                            }
                            else
                                gradeStudentMarkViewModels[i].RankFromGrade = ++count1;
                        }
                    }
                    else
                    {
                        gradeStudentMarkViewModels[i].RankFromGrade = count1;//taking the first
                    }
                }
                #endregion

                foreach (var sectionStudentMark in sectionStudentMarkViewModels)
                {
                    foreach (var gradeStudentMark in gradeStudentMarkViewModels)
                    {
                        if (gradeStudentMark.StudentPromotionId == sectionStudentMark.StudentPromotionId)
                        {
                            sectionStudentMark.RankFromGrade = gradeStudentMark.RankFromGrade;
                            break;
                        }
                    }
                }

                #endregion

                ViewData["exams"] = examList;
                ViewData["subjects"] = subjectList;
                ViewData["queryResult"] = sectionStudentMarkViewModels;
                ViewData["academicYearId"] = academicYearId;
                ViewData["academicYearSectionId"] = academicYearSectionId;
                ViewData["subjectGroupDetailId"] = subjectGroupDetailId;
                ViewData["academicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
                return View();
            }

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return View();
        }
        public IActionResult MIDMarkSummaryPerSubject(string AcademicYearId, string AcademicYearSectionId, string SubjectGroupDetailId)
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            if ((!string.IsNullOrEmpty(AcademicYearId) && Convert.ToInt32(AcademicYearId) > 0) && !string.IsNullOrEmpty(AcademicYearSectionId) && !string.IsNullOrEmpty(SubjectGroupDetailId))
            {
                var academicYearId = Convert.ToInt32(AcademicYearId);
                var academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);
                var subjectGroupDetailId = Convert.ToInt32(SubjectGroupDetailId);

                #region LOADING DATA
                var gradeStudentPromotions = (from sP in _context.StudentPromotions.Include(a => a.Student).Include(a => a.AcademicYearSection)
                                              join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                              where aY.Id == academicYearId
                                              select new
                                              {
                                                  SessionId = aY.Session.Id,
                                                  AcademicYearId = aY.Id,
                                                  SemesterId = aY.Semester.Id,
                                                  StudentPromotionId = sP.Id,
                                                  sP.AcademicYearSectionId,
                                                  aY.ClassId,
                                                  sP.Student.IDNo,
                                                  FullName = sP.Student.FirstName + " " + sP.Student.MiddleName,
                                                  sP.Student.Gender,
                                                  Semester = aY.Semester.Name,
                                                  ClassName = aY.Class.Name,
                                                  SectionName = aY.Class.Name + " (" + sP.AcademicYearSection.Section.Name + ")",
                                                  aY.ExamGroupId,
                                                  aY.SubjectGroupId,
                                              }).ToList();

                var sectionStudentPromotions = gradeStudentPromotions.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();

                var examList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup)
                                join eGD in _context.ExamGroupDetails.Include(a => a.Exam) on aY.ExamGroupId equals eGD.ExamGroupId
                                where aY.Id == academicYearId
                                select new
                                {
                                    aYId = aY.Id,
                                    ExamGroupDetailId = eGD.Id,
                                    aY.ClassId,
                                    ExamName = eGD.Exam.Name,
                                    ExamWeight = eGD.Exam.Weight,
                                }).ToList();

                var subjectList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.SubjectGroup)
                                   join sGD in _context.SubjectGroupDetails.Include(a => a.Subject) on aY.SubjectGroupId equals sGD.SubjectGroupId
                                   where aY.Id == academicYearId && sGD.Id == subjectGroupDetailId
                                   select new
                                   {
                                       aYId = aY.Id,
                                       SubjectGroupDetailId = sGD.Id,
                                       aY.ClassId,
                                       SubjectName = sGD.Subject.Name + "(" + sGD.Subject.Code + ")",
                                       SubjectCode = sGD.Subject.Code,
                                   }).ToList();
                #endregion

                #region PREPARING DATA FOR RANK PER SECTION
                //preparing data with viewmodel
                List<StudentMarkViewModel> sectionStudentMarkViewModels = new List<StudentMarkViewModel>();
                List<SubjectMarkViewModel> studentSubjectsMarkViewModels = new List<SubjectMarkViewModel>();
                foreach (var studentPromotion in sectionStudentPromotions)
                {
                    StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                    studentMarkViewModel.AcademicYearId = academicYearId;
                    studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                    studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                    studentMarkViewModel.IDNo = studentPromotion.IDNo;
                    studentMarkViewModel.FullName = studentPromotion.FullName;
                    studentMarkViewModel.Gender = studentPromotion.Gender;
                    studentMarkViewModel.SectionName = studentPromotion.SectionName;

                    //Calculating total mark
                    decimal overallMark = 0;//all subjects mark
                    var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                            join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                            where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                            select new
                                            {
                                                MarkSheetId = mS.Id,
                                                MarkSheeDetailId = mSD.Id,
                                                mS.SubjectGroupDetailId,
                                                mSD.ExamGroupDetailId,
                                                mSD.ExamMark,
                                            };

                    List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                    foreach (var subject in subjectList)
                    {
                        SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                        decimal subjectExamsMark = 0;
                        var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                        List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                        foreach (var subExam in subjectExams)
                        {
                            subjectExamsMark += subExam.ExamMark;

                            SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                            subjectExamMarkViewModel.ExamGroupDetailId = subExam.ExamGroupDetailId;
                            ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                            subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                            subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                            subjectExamMarkViewModel.ExamMark = subExam.ExamMark;

                            examMarks.Add(subjectExamMarkViewModel);
                        }

                        subjectMarkViewModel.RankFromGrade = 0;//with single subject
                        subjectMarkViewModel.RankFromSection = 0;//with single subject
                        subjectMarkViewModel.ExamMarks = examMarks;
                        subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)

                        subjectMarkViewModels.Add(subjectMarkViewModel);
                        studentSubjectsMarkViewModels.Add(subjectMarkViewModel);
                        overallMark += subjectExamsMark;
                    }

                    studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                    studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                    studentMarkViewModel.RankFromSection = 0;//with all subjects
                    studentMarkViewModel.RankFromGrade = 0;//with all subjects

                    sectionStudentMarkViewModels.Add(studentMarkViewModel);
                }

                sectionStudentMarkViewModels = sectionStudentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                studentSubjectsMarkViewModels = studentSubjectsMarkViewModels.OrderByDescending(a => a.TotalMark).ToList();

                #region SECTION RANK CALCULATION (FROM SINGLE SUBJECT & ALL EXAMS)
                //subject rank (from all exams)
                int count = 1;
                int similarCount = 0;
                for (int i = 0; i < sectionStudentMarkViewModels.Count; i++)
                {
                    if (i > 0)
                    {
                        if (sectionStudentMarkViewModels[i].OverallMark == sectionStudentMarkViewModels[i - 1].OverallMark)
                        {
                            sectionStudentMarkViewModels[i].RankFromSection = count;
                            similarCount++;
                        }
                        else
                        {
                            if (similarCount > 0)
                            {
                                count = similarCount + count + 1;
                                sectionStudentMarkViewModels[i].RankFromSection = count;
                                similarCount = 0;
                            }
                            else
                                sectionStudentMarkViewModels[i].RankFromSection = ++count;
                        }
                    }
                    else
                    {
                        sectionStudentMarkViewModels[i].RankFromSection = count;//taking the first
                    }
                }
                #endregion

                #endregion

                #region PREPARING DATA FOR RANK PER GRADE
                //preparing data with viewmodel
                List<StudentMarkViewModel> gradeStudentMarkViewModels = new List<StudentMarkViewModel>();
                List<SubjectMarkViewModel> gradeStudentSubjectsMarkViewModels = new List<SubjectMarkViewModel>();
                foreach (var studentPromotion in gradeStudentPromotions)
                {
                    StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                    studentMarkViewModel.AcademicYearId = academicYearId;
                    studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                    studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                    studentMarkViewModel.IDNo = studentPromotion.IDNo;
                    studentMarkViewModel.FullName = studentPromotion.FullName;
                    studentMarkViewModel.Gender = studentPromotion.Gender;
                    studentMarkViewModel.SectionName = studentPromotion.SectionName;

                    //Calculating total mark
                    decimal overallMark = 0;//all subjects mark
                    var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                            join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                            where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                            select new
                                            {
                                                MarkSheetId = mS.Id,
                                                MarkSheeDetailId = mSD.Id,
                                                mS.SubjectGroupDetailId,
                                                mSD.ExamGroupDetailId,
                                                mSD.ExamMark,
                                            };

                    List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                    foreach (var subject in subjectList)
                    {
                        SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                        decimal subjectExamsMark = 0;
                        var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                        List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                        foreach (var subExam in subjectExams)
                        {
                            subjectExamsMark += subExam.ExamMark;

                            SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                            subjectExamMarkViewModel.ExamGroupDetailId = subExam.ExamGroupDetailId;
                            ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                            subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                            subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                            subjectExamMarkViewModel.ExamMark = subExam.ExamMark;

                            examMarks.Add(subjectExamMarkViewModel);
                        }

                        subjectMarkViewModel.RankFromGrade = 0;//with single subject
                        subjectMarkViewModel.RankFromSection = 0;//with single subject
                        subjectMarkViewModel.ExamMarks = examMarks;
                        subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)

                        subjectMarkViewModels.Add(subjectMarkViewModel);
                        gradeStudentSubjectsMarkViewModels.Add(subjectMarkViewModel);
                        overallMark += subjectExamsMark;
                    }

                    studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                    studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                    studentMarkViewModel.RankFromSection = 0;//with all subjects
                    studentMarkViewModel.RankFromGrade = 0;//with all subjects

                    gradeStudentMarkViewModels.Add(studentMarkViewModel);
                }

                gradeStudentMarkViewModels = gradeStudentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                gradeStudentSubjectsMarkViewModels = gradeStudentSubjectsMarkViewModels.OrderByDescending(a => a.TotalMark).ToList();

                #region GRADE RANK CALCULATION (FROM SINGLE SUBJECT & ALL EXAMS & ALL STUDENTS)
                //subject rank (from all exams)
                int count1 = 1;
                int similarCount1 = 0;
                for (int i = 0; i < gradeStudentMarkViewModels.Count; i++)
                {
                    if (i > 0)
                    {
                        if (gradeStudentMarkViewModels[i].OverallMark == gradeStudentMarkViewModels[i - 1].OverallMark)
                        {
                            gradeStudentMarkViewModels[i].RankFromGrade = count1;
                            similarCount1++;
                        }
                        else
                        {
                            if (similarCount1 > 0)
                            {
                                count1 = similarCount1 + count1 + 1;
                                gradeStudentMarkViewModels[i].RankFromGrade = count1;
                                similarCount1 = 0;
                            }
                            else
                                gradeStudentMarkViewModels[i].RankFromGrade = ++count1;
                        }
                    }
                    else
                    {
                        gradeStudentMarkViewModels[i].RankFromGrade = count1;//taking the first
                    }
                }
                #endregion

                foreach (var sectionStudentMark in sectionStudentMarkViewModels)
                {
                    foreach (var gradeStudentMark in gradeStudentMarkViewModels)
                    {
                        if (gradeStudentMark.StudentPromotionId == sectionStudentMark.StudentPromotionId)
                        {
                            sectionStudentMark.RankFromGrade = gradeStudentMark.RankFromGrade;
                            break;
                        }
                    }
                }

                #endregion

                ViewData["exams"] = examList;
                ViewData["subjects"] = subjectList;
                ViewData["queryResult"] = sectionStudentMarkViewModels;
                ViewData["academicYearId"] = academicYearId;
                ViewData["academicYearSectionId"] = academicYearSectionId;
                ViewData["subjectGroupDetailId"] = subjectGroupDetailId;
                ViewData["academicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
                return View();
            }

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return View();
        }

        public IActionResult GenerateSemesterRoster(string AcademicYearId, string AcademicYearSectionId, string Overwrite)
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            if ((!string.IsNullOrEmpty(AcademicYearId) && Convert.ToInt32(AcademicYearId) > 0) && !string.IsNullOrEmpty(AcademicYearSectionId))
            {
                var academicYearId = Convert.ToInt32(AcademicYearId);
                var academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);

                #region LOADING DATA

                var examList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.ExamGroup)
                                join eGD in _context.ExamGroupDetails.Include(a => a.Exam) on aY.ExamGroupId equals eGD.ExamGroupId
                                where aY.Id == academicYearId
                                select new
                                {
                                    aYId = aY.Id,
                                    ExamGroupDetailId = eGD.Id,
                                    aY.ClassId,
                                    ExamName = eGD.Exam.Name,
                                    ExamWeight = eGD.Exam.Weight,
                                }).ToList();

                var subjectList = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.SubjectGroup)
                                   join sGD in _context.SubjectGroupDetails.Include(a => a.Subject) on aY.SubjectGroupId equals sGD.SubjectGroupId
                                   where aY.Id == academicYearId
                                   select new
                                   {
                                       aYId = aY.Id,
                                       SubjectGroupDetailId = sGD.Id,
                                       aY.ClassId,
                                       SubjectName = sGD.Subject.Name + "(" + sGD.Subject.Code + ")",
                                       SubjectCode = sGD.Subject.Code,
                                   }).ToList();

                var gradeStudentPromotions = (from sP in _context.StudentPromotions.Include(a => a.Student).Include(a => a.AcademicYearSection)
                                              join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                              where aY.Id == academicYearId
                                              select new
                                              {
                                                  SessionId = aY.Session.Id,
                                                  AcademicYearId = aY.Id,
                                                  SemesterId = aY.Semester.Id,
                                                  StudentPromotionId = sP.Id,
                                                  sP.AcademicYearSectionId,
                                                  aY.ClassId,
                                                  sP.Student.IDNo,
                                                  FullName = sP.Student.FirstName + " " + sP.Student.MiddleName,
                                                  sP.Student.Gender,
                                                  Semester = aY.Semester.Name,
                                                  ClassName = aY.Class.Name,
                                                  SectionName = aY.Class.Name + " (" + sP.AcademicYearSection.Section.Name + ")",
                                                  aY.ExamGroupId,
                                                  aY.SubjectGroupId,
                                              }).ToList();

                var sectionStudentPromotions = gradeStudentPromotions.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();

                #endregion

                //check if already saved
                var semesterMarkSummaries = _context.SMSummaries.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();
                if (semesterMarkSummaries.Count > 0) //if already saved
                {
                    if (!string.IsNullOrEmpty(Overwrite)) 
                    {
                        #region CALCULATING DATA For SECTION RANK
                        //preparing data with viewmodel
                        List<StudentMarkViewModel> studentMarkViewModels = new List<StudentMarkViewModel>();
                        foreach (var studentPromotion in sectionStudentPromotions)
                        {
                            //Calculating total mark
                            decimal overallMark = 0;//all subjects mark
                            var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                                    join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                                    where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                                    select new
                                                    {
                                                        MarkSheetId = mS.Id,
                                                        MarkSheeDetailId = mSD.Id,
                                                        mS.SubjectGroupDetailId,
                                                        mSD.ExamGroupDetailId,
                                                        mSD.ExamMark,
                                                    };

                            StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                            studentMarkViewModel.AcademicYearId = academicYearId;
                            studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                            studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                            studentMarkViewModel.IDNo = studentPromotion.IDNo;
                            studentMarkViewModel.FullName = studentPromotion.FullName;
                            studentMarkViewModel.Gender = studentPromotion.Gender;
                            studentMarkViewModel.SectionName = studentPromotion.SectionName;

                            List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                            foreach (var subject in subjectList)
                            {
                                //add to student subjects list
                                var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                                SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                                subjectMarkViewModel.SubjectGroupDetailId = subject.SubjectGroupDetailId;

                                //subject exams
                                //adding all subject exams
                                decimal subjectExamsMark = 0;
                                List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                                foreach (var subjectExam in subjectExams)
                                {
                                    subjectExamsMark += subjectExam.ExamMark;

                                    SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                                    subjectExamMarkViewModel.ExamGroupDetailId = subjectExam.ExamGroupDetailId;
                                    ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subjectExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                                    subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                                    subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                                    subjectExamMarkViewModel.ExamMark = subjectExam.ExamMark;

                                    examMarks.Add(subjectExamMarkViewModel);
                                }

                                subjectMarkViewModel.RankFromGrade = 0;//with single subject
                                subjectMarkViewModel.RankFromSection = 0;//with single subject

                                subjectMarkViewModel.ExamMarks = examMarks;
                                subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)
                                subjectMarkViewModels.Add(subjectMarkViewModel);

                                overallMark += subjectExamsMark;
                            }

                            studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                            studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                            studentMarkViewModel.RankFromSection = 0;//with all subjects
                            studentMarkViewModel.RankFromGrade = 0;//with all subjects

                            studentMarkViewModels.Add(studentMarkViewModel);
                        }

                        studentMarkViewModels = studentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                        #region SECTION RANK CALCULATION
                        int count = 1;
                        int similarCount = 0;
                        for (int i = 0; i < studentMarkViewModels.Count; i++)
                        {
                            if (i > 0)
                            {
                                if (studentMarkViewModels[i].OverallMark == studentMarkViewModels[i - 1].OverallMark)
                                {
                                    studentMarkViewModels[i].RankFromSection = count;
                                    similarCount++;
                                }
                                else
                                {
                                    if (similarCount > 0)
                                    {
                                        count = similarCount + count + 1;
                                        studentMarkViewModels[i].RankFromSection = count;
                                        similarCount = 0;
                                    }
                                    else
                                        studentMarkViewModels[i].RankFromSection = ++count;
                                }
                            }
                            else
                            {
                                studentMarkViewModels[i].RankFromSection = count;//taking the first
                            }

                        }
                        #endregion

                        #endregion

                        #region CALCULATING DATA For GRADE RANK
                        //preparing data with viewmodel
                        List<StudentMarkViewModel> gradeStudentMarkViewModels = new List<StudentMarkViewModel>();
                        foreach (var studentPromotion in gradeStudentPromotions)
                        {
                            StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                            studentMarkViewModel.AcademicYearId = academicYearId;
                            studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                            studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                            studentMarkViewModel.IDNo = studentPromotion.IDNo;
                            studentMarkViewModel.FullName = studentPromotion.FullName;
                            studentMarkViewModel.Gender = studentPromotion.Gender;
                            studentMarkViewModel.SectionName = studentPromotion.SectionName;

                            //Calculating total mark
                            decimal overallMark = 0;//all subjects mark
                            var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                                    join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                                    where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                                    select new
                                                    {
                                                        MarkSheetId = mS.Id,
                                                        MarkSheeDetailId = mSD.Id,
                                                        mS.SubjectGroupDetailId,
                                                        mSD.ExamGroupDetailId,
                                                        mSD.ExamMark,
                                                    };

                            List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                            foreach (var subject in subjectList)
                            {
                                SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                                decimal subjectExamsMark = 0;
                                var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                                List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                                foreach (var subExam in subjectExams)
                                {
                                    subjectExamsMark += subExam.ExamMark;

                                    SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                                    subjectExamMarkViewModel.ExamGroupDetailId = subExam.ExamGroupDetailId;
                                    ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                                    subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                                    subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                                    subjectExamMarkViewModel.ExamMark = subExam.ExamMark;

                                    examMarks.Add(subjectExamMarkViewModel);
                                }

                                subjectMarkViewModel.RankFromGrade = 0;//with single subject
                                subjectMarkViewModel.RankFromSection = 0;//with single subject
                                subjectMarkViewModel.ExamMarks = examMarks;
                                subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)

                                subjectMarkViewModels.Add(subjectMarkViewModel);
                                overallMark += subjectExamsMark;
                            }

                            studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                            studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                            studentMarkViewModel.RankFromSection = 0;//with all subjects
                            studentMarkViewModel.RankFromGrade = 0;//with all subjects

                            gradeStudentMarkViewModels.Add(studentMarkViewModel);
                        }

                        gradeStudentMarkViewModels = gradeStudentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                        #region GRADE RANK CALCULATION (FROM ALL SUBJECTS & ALL STUDENTS)
                        int count1 = 1;
                        int similarCount1 = 0;
                        for (int i = 0; i < gradeStudentMarkViewModels.Count; i++)
                        {
                            if (i > 0)
                            {
                                if (gradeStudentMarkViewModels[i].OverallMark == gradeStudentMarkViewModels[i - 1].OverallMark)
                                {
                                    gradeStudentMarkViewModels[i].RankFromGrade = count1;
                                    similarCount1++;
                                }
                                else
                                {
                                    if (similarCount1 > 0)
                                    {
                                        count1 = similarCount1 + count1 + 1;
                                        gradeStudentMarkViewModels[i].RankFromGrade = count1;
                                        similarCount1 = 0;
                                    }
                                    else
                                        gradeStudentMarkViewModels[i].RankFromGrade = ++count1;
                                }
                            }
                            else
                            {
                                gradeStudentMarkViewModels[i].RankFromGrade = count1;//taking the first
                            }
                        }
                        #endregion

                        //Clearing old records
                        foreach (var sms in semesterMarkSummaries) {
                            var smSummaries = _context.SMSummaryDetails.Where(a => a.SMSummaryId == sms.Id).ToList();
                            _context.SMSummaryDetails.RemoveRange(smSummaries);
                        }

                        _context.SMSummaries.RemoveRange(semesterMarkSummaries);
                        _context.SaveChanges();

                        SMSummary semesterMarkSummary;
                        foreach (var sectionStudentMark in studentMarkViewModels)
                        {
                            foreach (var gradeStudentMark in gradeStudentMarkViewModels)
                            {
                                if (gradeStudentMark.StudentPromotionId == sectionStudentMark.StudentPromotionId)
                                {
                                    sectionStudentMark.RankFromGrade = gradeStudentMark.RankFromGrade;

                                    //saving to database
                                    semesterMarkSummary = new SMSummary();
                                    semesterMarkSummary.AcademicYearId = academicYearId;
                                    semesterMarkSummary.AcademicYearSectionId = academicYearSectionId;
                                    semesterMarkSummary.StudentPromotionId = sectionStudentMark.StudentPromotionId;
                                    semesterMarkSummary.IDNo = sectionStudentMark.IDNo;
                                    semesterMarkSummary.FullName = sectionStudentMark.FullName;
                                    semesterMarkSummary.Gender = sectionStudentMark.Gender;
                                    semesterMarkSummary.SectionName = sectionStudentMark.SectionName;
                                    semesterMarkSummary.TotalMark = sectionStudentMark.OverallMark;
                                    semesterMarkSummary.OutOf = sectionStudentMark.SubjectMarks.Count * 100; ;
                                    semesterMarkSummary.Average = sectionStudentMark.OverallMark / (sectionStudentMark.SubjectMarks.Count * 100);
                                    semesterMarkSummary.RankFromSection = sectionStudentMark.RankFromSection;
                                    semesterMarkSummary.RankFromGrade = sectionStudentMark.RankFromGrade;
                                    semesterMarkSummary.RankFromSchool = 1;//to be done later
                                    semesterMarkSummary.IsPassed = ((sectionStudentMark.OverallMark / (sectionStudentMark.SubjectMarks.Count * 100)) >= (sectionStudentMark.OverallMark / 2) ? true : false);
                                    semesterMarkSummary.Remark = "Saved to Database";

                                    _context.SMSummaries.Add(semesterMarkSummary);
                                    int pass = _context.SaveChanges();

                                    //saving summary details
                                    if (pass > 0)
                                    {
                                        SMSummaryDetail sMSummaryDetail;
                                        foreach (var sMark in sectionStudentMark.SubjectMarks)
                                        {
                                            sMSummaryDetail = new SMSummaryDetail();
                                            sMSummaryDetail.SMSummaryId = semesterMarkSummary.Id;
                                            sMSummaryDetail.SubjectGroupDetailId = sMark.SubjectGroupDetailId;
                                            sMSummaryDetail.SubjectTotal = sMark.TotalMark;
                                            sMSummaryDetail.Average = sMark.TotalMark / sectionStudentMark.SubjectMarks.Count;
                                            sMSummaryDetail.RankFromSection = sMark.RankFromSection;
                                            sMSummaryDetail.RankFromGrade = sMark.RankFromGrade;
                                            sMSummaryDetail.IsPassed = (sMark.TotalMark >= 50 ? true : false);
                                            sMSummaryDetail.Remark = "Saved to Database";

                                            _context.SMSummaryDetails.Add(sMSummaryDetail);
                                            _context.SaveChanges();
                                        }
                                    }

                                    break;
                                }
                            }
                        }

                        #endregion
                    }

                    var semesterMarkSummaries2 = _context.SMSummaries.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();

                    ViewData["exams"] = examList;
                    ViewData["subjects"] = subjectList;
                    ViewData["queryResult"] = semesterMarkSummaries2;
                    ViewData["sectionStudents"] = sectionStudentPromotions;
                    ViewData["academicYearId"] = academicYearId;
                    ViewData["academicYearSectionId"] = academicYearSectionId;
                    ViewData["academicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
                    return View();
                }
                else 
                {
                    #region PREPARING DATA For SECTION RANK
                    //preparing data with viewmodel
                    List<StudentMarkViewModel> studentMarkViewModels = new List<StudentMarkViewModel>();
                    foreach (var studentPromotion in sectionStudentPromotions)
                    {
                        //Calculating total mark
                        decimal overallMark = 0;//all subjects mark
                        var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                                join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                                where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                                select new
                                                {
                                                    MarkSheetId = mS.Id,
                                                    MarkSheeDetailId = mSD.Id,
                                                    mS.SubjectGroupDetailId,
                                                    mSD.ExamGroupDetailId,
                                                    mSD.ExamMark,
                                                };

                        StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                        studentMarkViewModel.AcademicYearId = academicYearId;
                        studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                        studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                        studentMarkViewModel.IDNo = studentPromotion.IDNo;
                        studentMarkViewModel.FullName = studentPromotion.FullName;
                        studentMarkViewModel.Gender = studentPromotion.Gender;
                        studentMarkViewModel.SectionName = studentPromotion.SectionName;

                        List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                        foreach (var subject in subjectList)
                        {
                            //add to student subjects list
                            var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                            SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                            subjectMarkViewModel.SubjectGroupDetailId = subject.SubjectGroupDetailId;

                            //subject exams
                            //adding all subject exams
                            decimal subjectExamsMark = 0;
                            List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                            foreach (var subjectExam in subjectExams)
                            {
                                subjectExamsMark += subjectExam.ExamMark;

                                SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                                subjectExamMarkViewModel.ExamGroupDetailId = subjectExam.ExamGroupDetailId;
                                ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subjectExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                                subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                                subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                                subjectExamMarkViewModel.ExamMark = subjectExam.ExamMark;

                                examMarks.Add(subjectExamMarkViewModel);
                            }

                            subjectMarkViewModel.RankFromGrade = 0;//with single subject
                            subjectMarkViewModel.RankFromSection = 0;//with single subject

                            subjectMarkViewModel.ExamMarks = examMarks;
                            subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)
                            subjectMarkViewModels.Add(subjectMarkViewModel);

                            overallMark += subjectExamsMark;
                        }

                        studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                        studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                        studentMarkViewModel.RankFromSection = 0;//with all subjects
                        studentMarkViewModel.RankFromGrade = 0;//with all subjects

                        studentMarkViewModels.Add(studentMarkViewModel);
                    }

                    studentMarkViewModels = studentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                    #region SECTION RANK CALCULATION
                    int count = 1;
                    int similarCount = 0;
                    for (int i = 0; i < studentMarkViewModels.Count; i++)
                    {
                        if (i > 0)
                        {
                            if (studentMarkViewModels[i].OverallMark == studentMarkViewModels[i - 1].OverallMark)
                            {
                                studentMarkViewModels[i].RankFromSection = count;
                                similarCount++;
                            }
                            else
                            {
                                if (similarCount > 0)
                                {
                                    count = similarCount + count + 1;
                                    studentMarkViewModels[i].RankFromSection = count;
                                    similarCount = 0;
                                }
                                else
                                    studentMarkViewModels[i].RankFromSection = ++count;
                            }
                        }
                        else
                        {
                            studentMarkViewModels[i].RankFromSection = count;//taking the first
                        }

                    }
                    #endregion

                    #endregion

                    #region PREPARING DATA FOR RANK PER GRADE
                    //preparing data with viewmodel
                    List<StudentMarkViewModel> gradeStudentMarkViewModels = new List<StudentMarkViewModel>();
                    foreach (var studentPromotion in gradeStudentPromotions)
                    {
                        StudentMarkViewModel studentMarkViewModel = new StudentMarkViewModel();
                        studentMarkViewModel.AcademicYearId = academicYearId;
                        studentMarkViewModel.AcademicYearSectionId = academicYearSectionId;
                        studentMarkViewModel.StudentPromotionId = studentPromotion.StudentPromotionId;
                        studentMarkViewModel.IDNo = studentPromotion.IDNo;
                        studentMarkViewModel.FullName = studentPromotion.FullName;
                        studentMarkViewModel.Gender = studentPromotion.Gender;
                        studentMarkViewModel.SectionName = studentPromotion.SectionName;

                        //Calculating total mark
                        decimal overallMark = 0;//all subjects mark
                        var marksSheetDetails = from mSD in _context.MarksSheetDetails
                                                join mS in _context.MarksSheets on mSD.MarkSheetId equals mS.Id
                                                where mS.StudentPromotionId == studentPromotion.StudentPromotionId
                                                select new
                                                {
                                                    MarkSheetId = mS.Id,
                                                    MarkSheeDetailId = mSD.Id,
                                                    mS.SubjectGroupDetailId,
                                                    mSD.ExamGroupDetailId,
                                                    mSD.ExamMark,
                                                };

                        List<SubjectMarkViewModel> subjectMarkViewModels = new List<SubjectMarkViewModel>();
                        foreach (var subject in subjectList)
                        {
                            SubjectMarkViewModel subjectMarkViewModel = new SubjectMarkViewModel();
                            decimal subjectExamsMark = 0;
                            var subjectExams = marksSheetDetails.Where(a => a.SubjectGroupDetailId == subject.SubjectGroupDetailId).ToList();
                            List<SubjectExamMarkViewModel> examMarks = new List<SubjectExamMarkViewModel>();
                            foreach (var subExam in subjectExams)
                            {
                                subjectExamsMark += subExam.ExamMark;

                                SubjectExamMarkViewModel subjectExamMarkViewModel = new SubjectExamMarkViewModel();
                                subjectExamMarkViewModel.ExamGroupDetailId = subExam.ExamGroupDetailId;
                                ExamGroupDetail examGroupDetail = _context.ExamGroupDetails.Include(a => a.Exam).Where(a => a.Id.Equals(subExam.ExamGroupDetailId)).OrderByDescending(a => a.Id).ToList()[0];
                                subjectExamMarkViewModel.ExamName = examGroupDetail.Exam.Name;
                                subjectExamMarkViewModel.ExamWeight = examGroupDetail.Exam.Weight;
                                subjectExamMarkViewModel.ExamMark = subExam.ExamMark;

                                examMarks.Add(subjectExamMarkViewModel);
                            }

                            subjectMarkViewModel.RankFromGrade = 0;//with single subject
                            subjectMarkViewModel.RankFromSection = 0;//with single subject
                            subjectMarkViewModel.ExamMarks = examMarks;
                            subjectMarkViewModel.TotalMark = subjectExamsMark;//total exams mark (for single subject)

                            subjectMarkViewModels.Add(subjectMarkViewModel);
                            overallMark += subjectExamsMark;
                        }

                        studentMarkViewModel.SubjectMarks = subjectMarkViewModels;
                        studentMarkViewModel.OverallMark = overallMark;//total subjects mark
                        studentMarkViewModel.RankFromSection = 0;//with all subjects
                        studentMarkViewModel.RankFromGrade = 0;//with all subjects

                        gradeStudentMarkViewModels.Add(studentMarkViewModel);
                    }

                    gradeStudentMarkViewModels = gradeStudentMarkViewModels.OrderByDescending(a => a.OverallMark).ToList();

                    #region GRADE RANK CALCULATION (FROM ALL SUBJECTS & ALL STUDENTS)
                    int count1 = 1;
                    int similarCount1 = 0;
                    for (int i = 0; i < gradeStudentMarkViewModels.Count; i++)
                    {
                        if (i > 0)
                        {
                            if (gradeStudentMarkViewModels[i].OverallMark == gradeStudentMarkViewModels[i - 1].OverallMark)
                            {
                                gradeStudentMarkViewModels[i].RankFromGrade = count1;
                                similarCount1++;
                            }
                            else
                            {
                                if (similarCount1 > 0)
                                {
                                    count1 = similarCount1 + count1 + 1;
                                    gradeStudentMarkViewModels[i].RankFromGrade = count1;
                                    similarCount1 = 0;
                                }
                                else
                                    gradeStudentMarkViewModels[i].RankFromGrade = ++count1;
                            }
                        }
                        else
                        {
                            gradeStudentMarkViewModels[i].RankFromGrade = count1;//taking the first
                        }
                    }
                    #endregion

                    SMSummary semesterMarkSummary;
                    foreach (var sectionStudentMark in studentMarkViewModels)
                    {
                        foreach (var gradeStudentMark in gradeStudentMarkViewModels)
                        {
                            if (gradeStudentMark.StudentPromotionId == sectionStudentMark.StudentPromotionId)
                            {
                                sectionStudentMark.RankFromGrade = gradeStudentMark.RankFromGrade;

                                //saving to database
                                semesterMarkSummary = new SMSummary();
                                semesterMarkSummary.AcademicYearId = academicYearId;
                                semesterMarkSummary.AcademicYearSectionId = academicYearSectionId;
                                semesterMarkSummary.StudentPromotionId = sectionStudentMark.StudentPromotionId;
                                semesterMarkSummary.IDNo = sectionStudentMark.IDNo;
                                semesterMarkSummary.FullName = sectionStudentMark.FullName;
                                semesterMarkSummary.Gender = sectionStudentMark.Gender;
                                semesterMarkSummary.SectionName = sectionStudentMark.SectionName;
                                semesterMarkSummary.TotalMark = sectionStudentMark.OverallMark;
                                semesterMarkSummary.OutOf = sectionStudentMark.SubjectMarks.Count * 100; ;
                                semesterMarkSummary.Average = sectionStudentMark.OverallMark / (sectionStudentMark.SubjectMarks.Count * 100);
                                semesterMarkSummary.RankFromSection = sectionStudentMark.RankFromSection;
                                semesterMarkSummary.RankFromGrade = sectionStudentMark.RankFromGrade;
                                semesterMarkSummary.RankFromSchool = 1;//to be done later
                                semesterMarkSummary.IsPassed = ((sectionStudentMark.OverallMark / (sectionStudentMark.SubjectMarks.Count * 100)) >= (sectionStudentMark.OverallMark / 2) ? true : false);
                                semesterMarkSummary.Remark = "Saved to Database";

                                _context.SMSummaries.Add(semesterMarkSummary);
                                int pass = _context.SaveChanges();

                                //saving summary details
                                if (pass > 0) 
                                {
                                    SMSummaryDetail sMSummaryDetail;
                                    foreach (var sMark in sectionStudentMark.SubjectMarks)
                                    {
                                        sMSummaryDetail = new SMSummaryDetail();
                                        sMSummaryDetail.SMSummaryId = semesterMarkSummary.Id;
                                        sMSummaryDetail.SubjectGroupDetailId = sMark.SubjectGroupDetailId;
                                        sMSummaryDetail.SubjectTotal = sMark.TotalMark;
                                        sMSummaryDetail.Average = sMark.TotalMark/sectionStudentMark.SubjectMarks.Count;
                                        sMSummaryDetail.RankFromSection = sMark.RankFromSection;
                                        sMSummaryDetail.RankFromGrade = sMark.RankFromGrade;
                                        sMSummaryDetail.IsPassed = (sMark.TotalMark  >= 50 ? true: false);
                                        sMSummaryDetail.Remark = "Saved to Database";

                                        _context.SMSummaryDetails.Add(sMSummaryDetail);
                                        _context.SaveChanges();
                                    }
                                }

                                break;
                            }
                        }
                    }

                    #endregion

                    var sMSummaries = _context.SMSummaries.Where(a => a.AcademicYearSectionId == academicYearSectionId).ToList();

                    ViewData["exams"] = examList;
                    ViewData["subjects"] = subjectList;
                    ViewData["queryResult"] = sMSummaries;
                    ViewData["sectionStudents"] = sectionStudentPromotions;
                    ViewData["academicYearId"] = academicYearId;
                    ViewData["academicYearSectionId"] = academicYearSectionId;
                    ViewData["academicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
                    return View();
                }
            }

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return View();
        }
        public IActionResult GenerateSemesterGradeReport(IFormCollection iFormCollection)
        {
            var academicYears = (from aY in _context.AcademicYears.Include(a => a.Session).Include(a => a.Class).Include(a => a.Semester)
                                 select new
                                 {
                                     AcademicYearId = aY.Id,
                                     aY.ClassId,
                                     RosterName = aY.Session.Year + " " + aY.Session.SchoolType + " Grade " + aY.Class.Name + " (Semester: " + aY.Semester.Name + ")",
                                 }).ToList();

            if ((!string.IsNullOrEmpty(iFormCollection["AcademicYearIdd"]) && Convert.ToInt32(iFormCollection["AcademicYearIdd"]) > 0) && !string.IsNullOrEmpty(iFormCollection["AcademicYearSectionIdd"]))
            {
                var academicYearId = Convert.ToInt32(iFormCollection["AcademicYearIdd"]);
                var academicYearSectionId = Convert.ToInt32(iFormCollection["AcademicYearSectionIdd"]);

                var smSummaries = (from sms in _context.SMSummaries
                                   join smsD in _context.SMSummaryDetails on sms.Id equals smsD.SMSummaryId
                                   join sgD in _context.SubjectGroupDetails.Include(a => a.Subject) on smsD.SubjectGroupDetailId equals sgD.Id
                                   join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sms.AcademicYearId equals aY.Id
                                   join aYS in _context.AcademicYearSections.Include(a => a.Section) on sms.AcademicYearSectionId equals aYS.Id
                                   where sms.AcademicYearSectionId == academicYearSectionId
                                   select new
                                   {
                                       smsId = sms.Id,
                                       sms.IDNo,
                                       sms.FullName,
                                       ClassName = aY.Class.Name,
                                       SchoolName = aY.Session.SchoolType,
                                       ExamCenter = aY.Session.SchoolType,
                                       SubjectName = sgD.Subject.Name + '('+ sgD.Subject.Code + ')',
                                       SubjectCode = sgD.Subject.Code,
                                       SubjectCount = _context.SMSummaryDetails.Where(a => a.SMSummaryId == sms.Id).ToList().Count,
                                       smsD.SubjectTotal,
                                       OverallMark = sms.TotalMark,
                                       sms.Average,
                                       SessionName = aY.Session.Year + '-' + aY.Session.SchoolType,
                                       SectionName = aYS.Section.Name,
                                       OverallMessage = smsD.IsPassed ? "PASSED" : "FAILED",
                                       HomeRoomTeacherName = "Abebe",
                                       sms.RankFromSection,
                                       sms.RankFromGrade,
                                       sms.RankFromSchool
                                   });

                string mimetype = "";
                int extension = 1;
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rptSemesterGradeReport.rdlc";

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("rpt1", "Header");
                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("rptSemesterReportDataSet", smSummaries);
                var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

                return File(result.MainStream, "application/pdf");
                //return View();
            }

            ViewData["AcademicYears"] = new SelectList(academicYears, "AcademicYearId", "RosterName");
            return View();
        }

        public IActionResult GenerateAnnualRoster(string SessionId, string ClassId, string AcademicYearSectionId, string Overwrite)
        {
            var sessions = (from s in _context.Sessions
                           select new
                           {
                               SessionId = s.Id,
                               SessionName = s.Year.ToString() + ' ' + s.SchoolType.ToString(),
                           }).ToList();

            var classes = (from c in _context.Classes
                                 select new
                                 {
                                     ClassId = c.Id,
                                     ClassName = c.Name,
                                 }).ToList();

            if ((!string.IsNullOrEmpty(SessionId) && Convert.ToInt32(SessionId) > 0) && (!string.IsNullOrEmpty(ClassId) && Convert.ToInt32(ClassId) > 0) && (!string.IsNullOrEmpty(AcademicYearSectionId) && Convert.ToInt32(AcademicYearSectionId) > 0))
            {
                var sessionId = Convert.ToInt32(SessionId);
                var classId = Convert.ToInt32(ClassId);
                var academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);

                #region LOADING DATA
                var academicYears = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester)
                                     where aY.ClassId == classId && aY.SessionId == sessionId
                                     select new
                                     {
                                         aYId = aY.Id,
                                         RosterName = aY.Session.Year.ToString() + "-" + aY.Class.Name + " ("
                                     }).ToList();

                var sectionStudentPromotions = (from sP in _context.StudentPromotions.Include(a => a.Student).Include(a => a.AcademicYearSection)
                                              join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                              where aY.SessionId == sessionId && aY.ClassId == classId && sP.AcademicYearSectionId == academicYearSectionId
                                              select new
                                              {
                                                  SessionId = aY.Session.Id,
                                                  AcademicYearId = aY.Id,
                                                  SemesterId = aY.Semester.Id,
                                                  StudentPromotionId = sP.Id,
                                                  sP.AcademicYearSectionId,
                                                  aY.ClassId,
                                                  sP.Student.IDNo,
                                                  FullName = sP.Student.FirstName + " " + sP.Student.MiddleName,
                                                  sP.Student.Gender,
                                                  Semester = aY.Semester.Name,
                                                  ClassName = aY.Class.Name,
                                                  SectionName = aY.Class.Name + " (" + sP.AcademicYearSection.Section.Name + ")",
                                                  aY.ExamGroupId,
                                                  aY.SubjectGroupId,
                                                  aY.Session.Year,
                                              }).ToList();
                #endregion

                if (academicYears.Count > 1)
                {
                    //check if annual summaries found
                    var annualMarkSummaries = _context.SMSummaries.ToList();
                    if (annualMarkSummaries.Count > 0)
                    {
                        ViewData["queryResult"] = annualMarkSummaries;
                        ViewData["academicYears"] = academicYears;
                        ViewData["sectionStudents"] = sectionStudentPromotions;
                        ViewData["sessionId"] = sessionId;
                        ViewData["classId"] = classId;
                        ViewData["academicYearSectionId"] = academicYearSectionId;
                        ViewData["sessions"] = new SelectList(sessions, "SessionId", "SessionName");
                        ViewData["classes"] = new SelectList(classes, "ClassId", "ClassName");
                        return View();
                    }
                    else
                    {
                        ViewData["Message"] = "No Semester Summaries Found!! Please do semester summaries first!";
                    }
                }
                else 
                {
                    ViewData["Message"] = "Second Semester Summaries NOT Found!! Please add second semester data first!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["sessions"] = new SelectList(sessions, "SessionId", "SessionName");
            ViewData["classes"] = new SelectList(classes, "ClassId", "ClassName");
            return View();
        }
        public IActionResult GenerateAnnualGradeReport(string SessionId, string ClassId, string AcademicYearSectionId, IFormCollection iFormCollection)
        {
            var sessions = (from s in _context.Sessions
                            select new
                            {
                                SessionId = s.Id,
                                SessionName = s.Year.ToString() + ' ' + s.SchoolType.ToString(),
                            }).ToList();

            var classes = (from c in _context.Classes
                           select new
                           {
                               ClassId = c.Id,
                               ClassName = c.Name,
                           }).ToList();

            if ((!string.IsNullOrEmpty(SessionId) && Convert.ToInt32(SessionId) > 0) && (!string.IsNullOrEmpty(ClassId) && Convert.ToInt32(ClassId) > 0) && (!string.IsNullOrEmpty(AcademicYearSectionId) && Convert.ToInt32(AcademicYearSectionId) > 0))
            {
                var sessionId = Convert.ToInt32(SessionId);
                var classId = Convert.ToInt32(ClassId);
                var academicYearSectionId = Convert.ToInt32(AcademicYearSectionId);

                #region LOADING DATA
                var academicYears = (from aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester)
                                     where aY.ClassId == classId && aY.SessionId == sessionId
                                     select new
                                     {
                                         aYId = aY.Id,
                                         RosterName = aY.Session.Year.ToString() + "-" + aY.Class.Name + " ("
                                     }).ToList();
                var sectionStudentPromotions = (from sP in _context.StudentPromotions.Include(a => a.Student).Include(a => a.AcademicYearSection)
                                                join aY in _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Include(a => a.Semester) on sP.AcademicYearSection.AcademicYearId equals aY.Id
                                                where aY.SessionId == sessionId && aY.ClassId == classId && sP.AcademicYearSectionId == academicYearSectionId
                                                select new
                                                {
                                                    SessionId = aY.Session.Id,
                                                    AcademicYearId = aY.Id,
                                                    SemesterId = aY.Semester.Id,
                                                    StudentPromotionId = sP.Id,
                                                    sP.AcademicYearSectionId,
                                                    aY.ClassId,
                                                    sP.Student.IDNo,
                                                    FullName = sP.Student.FirstName + " " + sP.Student.MiddleName,
                                                    sP.Student.Gender,
                                                    Semester = aY.Semester.Name,
                                                    ClassName = aY.Class.Name,
                                                    SectionName = aY.Class.Name + " (" + sP.AcademicYearSection.Section.Name + ")",
                                                    aY.ExamGroupId,
                                                    aY.SubjectGroupId,
                                                    aY.Session.Year,
                                                }).ToList();
                #endregion

                //check if annual summaries found
                var annualMarkSummaries = _context.SMSummaries.ToList();
                DataTable dt = new DataTable();
                dt.Columns.Add("IIDNo");
                dt.Columns.Add("IFullName");
                dt.Columns.Add("ClassName");
                dt.Columns.Add("SchoolName");
                dt.Columns.Add("Average");
                dt.Columns.Add("SemesterSubjectAverage");
                dt.Columns.Add("SemesterSubjectTotal");
                //dt.Columns.Add("SemesterAverageTotal");
                dt.Columns.Add("ExamCenter");
                dt.Columns.Add("ISubjectName");
                dt.Columns.Add("ISubjectCount");
                dt.Columns.Add("ISubjectTotal");
                dt.Columns.Add("IOverallMark");
                dt.Columns.Add("OverallMark");
                dt.Columns.Add("IAverage");
                dt.Columns.Add("ISessionName");
                dt.Columns.Add("ISectionName");
                dt.Columns.Add("IOverallMessage");
                dt.Columns.Add("IHomeRoomTeacherName");
                dt.Columns.Add("IRankFromSection");
                dt.Columns.Add("IRankFromGrade");
                dt.Columns.Add("IRankFromSchool");
                dt.Columns.Add("RankFromSection");
                dt.Columns.Add("RankFromGrade");
                dt.Columns.Add("RankFromSchool");
                dt.Columns.Add("IISubjectName");
                dt.Columns.Add("IISubjectCount");
                dt.Columns.Add("IISubjectTotal");
                dt.Columns.Add("IIOverallMark");
                dt.Columns.Add("IIAverage");
                dt.Columns.Add("IISessionName");
                dt.Columns.Add("IISectionName");
                dt.Columns.Add("IIOverallMessage");
                dt.Columns.Add("IIHomeRoomTeacherName");
                dt.Columns.Add("IIRankFromSection");
                dt.Columns.Add("IIRankFromGrade");
                dt.Columns.Add("IIRankFromSchool");

                if (annualMarkSummaries.Count > 0)
                {
                    if (academicYears.Count > 1)
                    {
                        var semesterI = academicYears[0].aYId;
                        var semesterII = academicYears[1].aYId;

                        var semesterISummaries = annualMarkSummaries.Where(a => a.AcademicYearId == semesterI).OrderBy(a => a.IDNo).ToList();
                        var semesterIISummaries = annualMarkSummaries.Where(a => a.AcademicYearId == semesterII).OrderBy(a => a.IDNo).ToList();

                        for (int i = 0; i < semesterISummaries.Count; i++)
                        {
                            var semesterSummary = semesterISummaries[i];
                            var semesterIISummary = semesterIISummaries[i];
                            var semesterSummaryDetails = _context.SMSummaryDetails.Where(a => a.SMSummaryId == semesterSummary.Id).ToList();
                            var semesterIISummaryDetails = _context.SMSummaryDetails.Where(a => a.SMSummaryId == semesterIISummary.Id).ToList();

                            DataRow row;
                            for (int j = 0; j < semesterSummaryDetails.Count; j++)
                            {
                                //if (semesterSummaryDetails[j].SubjectGroupDetailId == semesterIISummaryDetails[j].SubjectGroupDetailId) 
                                //{
                                var academicYear = _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Where(a => a.Id == semesterSummary.AcademicYearId).ToList();
                                var academicYearII = _context.AcademicYears.Include(a => a.Class).Include(a => a.Session).Where(a => a.Id == semesterIISummary.AcademicYearId).ToList();
                                var subject = _context.SubjectGroupDetails.Include(a => a.Subject).Where(a => a.Id == semesterSummaryDetails[j].SubjectGroupDetailId).ToList()[0];
                                var subjectII = _context.SubjectGroupDetails.Include(a => a.Subject).Where(a => a.Id == semesterIISummaryDetails[j].SubjectGroupDetailId).ToList()[0];

                                row = dt.NewRow();
                                row["IIDNo"] = semesterSummary.IDNo;
                                row["IFullName"] = semesterSummary.FullName;
                                row["ClassName"] = academicYear[0].Class.Name;
                                row["SchoolName"] = academicYear[0].Session.SchoolType.ToString();
                                row["ExamCenter"] = academicYear[0].Session.SchoolType.ToString();
                                row["Average"] = (semesterSummary.Average + semesterIISummary.Average) / 2;
                                row["SemesterSubjectAverage"] = (semesterSummaryDetails[j].SubjectTotal + semesterIISummaryDetails[j].SubjectTotal) / 2;
                                row["SemesterSubjectTotal"] = semesterSummaryDetails[j].SubjectTotal + semesterIISummaryDetails[j].SubjectTotal;
                                //row["SemesterAverageTotal"] = (semesterSummary.TotalMark / semesterSummaryDetails.Count) + (semesterIISummary.TotalMark / semesterIISummaryDetails.Count);
                                row["ISubjectName"] = subject.Subject.Name + "(" + subject.Subject.Code + ")";
                                row["ISubjectCount"] = semesterSummaryDetails.Count;
                                row["ISubjectTotal"] = semesterSummaryDetails[j].SubjectTotal;
                                row["IOverallMark"] = semesterSummary.TotalMark;
                                row["OverallMark"] = semesterSummary.TotalMark + semesterIISummary.TotalMark;
                                row["IAverage"] = semesterSummary.TotalMark / semesterSummaryDetails.Count;
                                row["ISessionName"] = academicYear[0].Session.Year.ToString() + '-' + academicYear[0].Session.SchoolType.ToString();
                                row["ISectionName"] = semesterSummary.SectionName;
                                row["IOverallMessage"] = semesterSummaryDetails[j].SubjectTotal >= 50 ? "PASSED" : "FAILED";
                                row["IHomeRoomTeacherName"] = semesterSummary.AcademicYearId;
                                row["IRankFromSection"] = semesterSummary.RankFromSection;
                                row["IRankFromGrade"] = semesterSummary.RankFromGrade;
                                row["IRankFromSchool"] = semesterSummary.RankFromSchool;
                                row["RankFromSection"] = Math.Floor((decimal)(semesterSummary.RankFromSection + semesterIISummary.RankFromSection)/2);
                                row["RankFromGrade"] = Math.Floor((decimal)(semesterSummary.RankFromGrade + semesterIISummary.RankFromGrade) / 2);
                                row["RankFromSchool"] = Math.Floor((decimal)(semesterSummary.RankFromSchool + semesterIISummary.RankFromSchool) / 2);
                                row["IISubjectName"] = subjectII.Subject.Name + "(" + subjectII.Subject.Code + ")";
                                row["IISubjectCount"] = semesterIISummaryDetails.Count;
                                row["IISubjectTotal"] = semesterIISummaryDetails[j].SubjectTotal;
                                row["IIOverallMark"] = semesterIISummary.TotalMark;
                                row["IIAverage"] = semesterIISummary.TotalMark / semesterIISummaryDetails.Count;
                                row["IISessionName"] = academicYearII[0].Session.Year.ToString() + '-' + academicYearII[0].Session.SchoolType.ToString();
                                row["IISectionName"] = academicYearII[0].Session.SchoolType.ToString();
                                row["IIOverallMessage"] = semesterIISummaryDetails[j].SubjectTotal >= 50 ? "PASSED" : "FAILED";
                                row["IIHomeRoomTeacherName"] = semesterIISummary.AcademicYearId;
                                row["IIRankFromSection"] = semesterIISummary.RankFromSection;
                                row["IIRankFromGrade"] = semesterIISummary.RankFromGrade;
                                row["IIRankFromSchool"] = semesterIISummary.RankFromSchool;

                                dt.Rows.Add(row);
                                //}                                
                            }
                        }
                    }
                    string mimetype = "";
                    int extension = 1;
                    var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rptAGReport.rdlc";

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("rpt1", "Header");
                    LocalReport localReport = new LocalReport(path);
                    localReport.AddDataSource("rptAGRDataSet", dt);
                    var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

                    return File(result.MainStream, "application/pdf");
                }
                else
                {
                    ViewData["Message"] = "No Semester Summaries Found!! Please do semester summaries first!";
                }
            }

            ViewData["sessions"] = new SelectList(sessions, "SessionId", "SessionName");
            ViewData["classes"] = new SelectList(classes, "ClassId", "ClassName");
            return View();
        }
        
        #endregion

        // GET: Registrar/MarkSheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MarksSheets == null)
            {
                return NotFound();
            }

            var markSheet = await _context.MarksSheets.FindAsync(id);
            if (markSheet == null)
            {
                return NotFound();
            }
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", markSheet.StudentPromotionId);
            return View(markSheet);
        }

        // POST: Registrar/MarkSheets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentPromotionId,SubjectGroupDetailId,Average,RegisteredBy,RegisteredAt,ModifiedBy,ModifiedAt,ApprovedBy,ApprovedAt")] MarkSheet markSheet)
        {
            if (id != markSheet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(markSheet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkSheetExists(markSheet.Id))
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
            ViewData["StudentPromotionId"] = new SelectList(_context.StudentPromotions, "Id", "Id", markSheet.StudentPromotionId);
            return View(markSheet);
        }

        // GET: Registrar/MarkSheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MarksSheets == null)
            {
                return NotFound();
            }

            var markSheet = await _context.MarksSheets
                .Include(m => m.StudentPromotion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markSheet == null)
            {
                return NotFound();
            }

            return View(markSheet);
        }

        // POST: Registrar/MarkSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MarksSheets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MarksSheets'  is null.");
            }
            var markSheet = await _context.MarksSheets.FindAsync(id);
            if (markSheet != null)
            {
                _context.MarksSheets.Remove(markSheet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkSheetExists(int id)
        {
            return _context.MarksSheets.Any(e => e.Id == id);
        }
    }
}
