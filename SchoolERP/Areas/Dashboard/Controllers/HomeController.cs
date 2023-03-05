using AspNetCore.Reporting;
using BALibrary.Academic;
using BALibrary.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Data;
using SchoolERP.Models;
using System.Linq;

namespace SchoolERP.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            int userRoleId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserRoleId));
            var roleModules = _context.RoleModules.Where(rm => rm.RoleId == userRoleId).ToList();

            ViewData["UserRoleModules"] = roleModules;
            return View();
        }

        public IActionResult SalesSummary()
        {
            ViewData["Title"] = "Dashboard";
            int userRoleId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserRoleId));
            var roleModules = _context.RoleModules.Where(rm => rm.RoleId == userRoleId).ToList();

            #region Sales Data
            //Sales data
            var queryResult = from inv in _context.Invoices.Include(a => a.InvoiceType).Include(a => a.Customer).Where(a => a.Status != 0)
                              join invD in _context.InvoiceDetails on inv.Id equals invD.InvoiceId
                              join pb in _context.ProductBatches.Include(pb => pb.Product) on invD.ProductBatchId equals pb.Id
                              join pc in _context.ProductCategories on pb.Product.ProductCategoryId equals pc.Id
                              join e in _context.Employees on inv.EmployeeId equals e.Id
                              select new
                              {
                                  pb.ProductId,
                                  ProductCategoryId = pc.Id,
                                  inv.InvoiceNo,
                                  InvoiceTypeName = inv.InvoiceType.Name,
                                  ProductName = pb.Product.Name,
                                  ProductTypeName = pc.Name,
                                  ProductCode = pb.Product.Code,
                                  CustomerTINNo = inv.Customer.TINNo,
                                  CustomerName = inv.Customer.Name,
                                  ProductBatchNo = pb.BatchNo,
                                  inv.InvoiceDate,
                                  EmployeeFullName = e.FirstName + e.MiddleName + e.LastName,
                                  e.Gender,
                                  ItemQuantity = invD.Quantity,
                                  ItemSellingPrice = invD.SellingPrice,
                                  InvoiceRowTotal = invD.RowTotal
                              };
            #endregion

            string productCategorySales = string.Empty;
            var productCategories = _context.ProductCategories;
            foreach (var pc in productCategories)
            {
                decimal pSales = 0;
                foreach (var item in queryResult)
                {
                    if (item.ProductCategoryId == pc.Id)
                    {
                        pSales += item.InvoiceRowTotal;
                    }
                }

                productCategorySales += pc.Name + "#" + pSales + ",";
            }

            ViewData["UserRoleModules"] = roleModules;
            ViewData["SalesData"] = productCategorySales;
            return View();
        }
        public IActionResult ViewSummary2()
        {
            #region Sales Data (queryResult 0)
            //Reports compilation
            var queryResult0 = from inv in _context.Invoices.Include(a => a.InvoiceType).Include(a => a.Customer).Where(a => a.Status != 0)
                               join invD in _context.InvoiceDetails on inv.Id equals invD.InvoiceId
                               join pb in _context.ProductBatches.Include(pb => pb.Product) on invD.ProductBatchId equals pb.Id
                               join pc in _context.ProductCategories on pb.Product.ProductCategoryId equals pc.Id
                               join e in _context.Employees on inv.EmployeeId equals e.Id
                               select new
                               {
                                   pb.ProductId,
                                   ProductCategoryId = pc.Id,
                                   inv.InvoiceNo,
                                   InvoiceTypeName = inv.InvoiceType.Name,
                                   ProductName = pb.Product.Name,
                                   ProductTypeName = pc.Name,
                                   ProductCode = pb.Product.Code,
                                   CustomerTINNo = inv.Customer.TINNo,
                                   CustomerName = inv.Customer.Name,
                                   ProductBatchNo = pb.BatchNo,
                                   inv.InvoiceDate,
                                   EmployeeFullName = e.FirstName + e.MiddleName + e.LastName,
                                   e.Gender,
                                   ItemQuantity = invD.Quantity,
                                   ItemSellingPrice = invD.SellingPrice,
                                   InvoiceRowTotal = invD.RowTotal
                               };
            #endregion

            #region Products data (queryResult 1)
            var queryResult1 = from p in _context.Products.Include(p => p.ProductCategory).Include(p => p.Uom)
                               join pb in _context.ProductBatches on p.Id equals pb.ProductId
                               join s in _context.Stocks on pb.Id equals s.ProductBatchId
                               join e in _context.Employees on pb.EmployeeId equals e.Id
                               select new
                               {
                                   ProductBatchNo = s.ProductBatch.BatchNo,
                                   ProductName = p.Name,
                                   ProductTypeName = p.ProductCategory.Name,
                                   ProductCode = p.Code,
                                   ProductUomNme = p.Uom.Name,
                                   EmployeeFullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                   s.InitialQuantity,
                                   s.SoldQuantity,
                                   StockBalance = s.CurrentQuantity,
                                   s.ActionTaken,
                                   s.Description,
                                   StockUpdatedAt = s.UpdatedAt,
                                   p.MinimumOrderLevel,
                                   UnderStock = (s.CurrentQuantity <= p.MinimumOrderLevel ? "YES" : "NO"),
                                   pb.ExpirationDate,
                                   pb.BestBefore,
                                   pb.ManufacturedDate,
                               };
            #endregion

            #region Product Batches Data (queryResult2
            var queryResult2 = from pb in _context.ProductBatches
                               join p in _context.Products.Include(p => p.ProductCategory).Include(p => p.Uom) on pb.ProductId equals p.Id
                               select new
                               {
                                   ProductBatchId = pb.Id,
                                   ProductName = p.Name,
                                   ProductTypeName = p.ProductCategory.Name,
                                   ProductCode = p.Code,
                                   ProductUomNme = p.Uom.Name,
                                   p.MinimumOrderLevel,
                               };

            #endregion

            var dailySales = queryResult0.Where(i => i.InvoiceDate.Equals(DateTime.Now));

            //get dates in current week
            List<DateTime> dates = Common.GetWeekDatesFromDate(DateTime.Today);
            DateTime FirstDateOfWeek = dates[0];
            DateTime LastDateOfWeek = dates[6];

            var weeklySales = queryResult0.Where(i => i.InvoiceDate >= FirstDateOfWeek).Where(i => i.InvoiceDate <= DateTime.Today);

            decimal dailySalesAmt = 0;
            foreach (var item in dailySales)
            {
                dailySalesAmt += Convert.ToDecimal(item.InvoiceRowTotal);
            }

            decimal weeklySalesAmt = 0;
            foreach (var item in weeklySales)
            {
                weeklySalesAmt += Convert.ToDecimal(item.InvoiceRowTotal);
            }

            DateTime currentDate = DateTime.Now.Date;
            DateTime nextTwoMonths = currentDate.AddDays(62);

            var itemsToExpire = queryResult1.Where(p => p.ExpirationDate <= nextTwoMonths);
            var ItemsToOrder = queryResult1.Where(p => p.MinimumOrderLevel <= 10);

            string productCategorySales = string.Empty;
            var productCategories = _context.ProductCategories;
            foreach (var pc in productCategories)
            {
                decimal pSales = 0;
                foreach (var item in queryResult0)
                {
                    if (item.ProductCategoryId == pc.Id)
                    {
                        pSales += item.InvoiceRowTotal;
                    }
                }

                productCategorySales += pc.Name + "#" + pSales + ",";
            }

            //get months list (current and previous)
            List<int> previousMonths = Common.GetPreviousMonths(currentDate);
            string prevMonths = string.Empty;
            string salesDataPerMonth = string.Empty;
            string salesCategoryDataPerMonth = string.Empty;
            foreach (int month in previousMonths)
            {
                decimal monthlySales = 0;
                prevMonths += Common.GetMonthText(month) + "#";
                foreach (var item in queryResult0)
                {
                    if (item.InvoiceDate.Month == month)
                    {
                        monthlySales += item.InvoiceRowTotal;
                        salesCategoryDataPerMonth += Common.GetMonthText(month) + "#" + item.InvoiceRowTotal + "#" + item.ProductTypeName + ",";
                    }
                }
                salesDataPerMonth += Common.GetMonthText(month) + "#" + monthlySales + ",";
            }

            var latestProducts = queryResult2.OrderByDescending(pb => pb.ProductBatchId);

            ViewData["ItemsToExpire"] = itemsToExpire;
            ViewData["ItemsToOrder"] = ItemsToOrder;
            ViewData["ItemsToExpireCount"] = itemsToExpire.Count();
            ViewData["ItemsToOrderCount"] = ItemsToOrder.Count();
            ViewData["LatestProducts"] = latestProducts;
            ViewData["LatestProductCount"] = latestProducts.Count();
            ViewData["DailySalesAmt"] = dailySalesAmt;
            ViewData["WeeklySalesAmt"] = weeklySalesAmt;
            ViewData["DailySales"] = dailySales;
            ViewData["WeeklySales"] = weeklySales;
            ViewData["Title"] = "View Summary";
            HttpContext.Session.SetString("CategorySalesData", productCategorySales);
            HttpContext.Session.SetString("SalesDataPerMonth", salesDataPerMonth);
            HttpContext.Session.SetString("SalesCategoryDataPerMonth", salesCategoryDataPerMonth);
            HttpContext.Session.SetString("PreviousMonths", prevMonths);
            return View();
        }
        public IActionResult ViewSummary()
        {
            #region Sales Data (queryResult 0)
            //Reports compilation
            var queryResult0 = from inv in _context.Invoices.Include(a => a.InvoiceType).Include(a => a.Customer).Where(a => a.Status != 0)
                               join invD in _context.InvoiceDetails on inv.Id equals invD.InvoiceId
                               join pb in _context.ProductBatches.Include(pb => pb.Product) on invD.ProductBatchId equals pb.Id
                               join pc in _context.ProductCategories on pb.Product.ProductCategoryId equals pc.Id
                               join e in _context.Employees on inv.EmployeeId equals e.Id
                               select new
                               {
                                   pb.ProductId,
                                   ProductCategoryId = pc.Id,
                                   inv.InvoiceNo,
                                   InvoiceTypeName = inv.InvoiceType.Name,
                                   ProductName = pb.Product.Name,
                                   ProductTypeName = pc.Name,
                                   ProductCode = pb.Product.Code,
                                   CustomerTINNo = inv.Customer.TINNo,
                                   CustomerName = inv.Customer.Name,
                                   ProductBatchNo = pb.BatchNo,
                                   inv.InvoiceDate,
                                   EmployeeFullName = e.FirstName + e.MiddleName + e.LastName,
                                   e.Gender,
                                   ItemQuantity = invD.Quantity,
                                   ItemSellingPrice = invD.SellingPrice,
                                   InvoiceRowTotal = invD.RowTotal
                               };
            #endregion

            #region Products data (queryResult 1)
            var queryResult1 = from p in _context.Products.Include(p => p.ProductCategory).Include(p => p.Uom)
                               join pb in _context.ProductBatches on p.Id equals pb.ProductId
                               join s in _context.Stocks on pb.Id equals s.ProductBatchId
                               join e in _context.Employees on pb.EmployeeId equals e.Id
                               select new
                               {
                                   ProductBatchNo = s.ProductBatch.BatchNo,
                                   ProductName = p.Name,
                                   ProductTypeName = p.ProductCategory.Name,
                                   ProductCode = p.Code,
                                   ProductUomNme = p.Uom.Name,
                                   EmployeeFullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                   s.InitialQuantity,
                                   s.SoldQuantity,
                                   StockBalance = s.CurrentQuantity,
                                   s.ActionTaken,
                                   s.Description,
                                   StockUpdatedAt = s.UpdatedAt,
                                   p.MinimumOrderLevel,
                                   UnderStock = (s.CurrentQuantity <= p.MinimumOrderLevel ? "YES" : "NO"),
                                   pb.ExpirationDate,
                                   pb.BestBefore,
                                   pb.ManufacturedDate,
                               };
            #endregion

            #region Product Batches Data (queryResult2
            var queryResult2 = from pb in _context.ProductBatches
                               join p in _context.Products.Include(p => p.ProductCategory).Include(p => p.Uom) on pb.ProductId equals p.Id
                               select new
                               {
                                   ProductBatchId = pb.Id,
                                   ProductName = p.Name,
                                   ProductTypeName = p.ProductCategory.Name,
                                   ProductCode = p.Code,
                                   ProductUomNme = p.Uom.Name,
                                   p.MinimumOrderLevel,
                               };

            #endregion

            var dailySales = queryResult0.Where(i => i.InvoiceDate.Equals(DateTime.Now));

            //get dates in current week
            List<DateTime> dates = Common.GetWeekDatesFromDate(DateTime.Today);
            DateTime FirstDateOfWeek = dates[0];
            DateTime LastDateOfWeek = dates[6];

            var weeklySales = queryResult0.Where(i => i.InvoiceDate >= FirstDateOfWeek).Where(i => i.InvoiceDate <= DateTime.Today);

            decimal dailySalesAmt = 0;
            foreach (var item in dailySales)
            {
                dailySalesAmt += Convert.ToDecimal(item.InvoiceRowTotal);
            }

            decimal weeklySalesAmt = 0;
            foreach (var item in weeklySales)
            {
                weeklySalesAmt += Convert.ToDecimal(item.InvoiceRowTotal);
            }

            DateTime currentDate = DateTime.Now.Date;
            DateTime nextTwoMonths = currentDate.AddDays(62);

            var itemsToExpire = queryResult1.Where(p => p.ExpirationDate <= nextTwoMonths);
            var ItemsToOrder = queryResult1.Where(p => p.MinimumOrderLevel <= 10);

            var latestProducts = queryResult2.OrderByDescending(pb => pb.ProductBatchId);

            var subjectList = (from s in _context.Subjects
                                select new
                               {
                                   s.Id,
                                   SubjectName = s.Name + "(" + s.Code + ")",
                               }).ToList();

            var smSummaries = (from smS in _context.SMSummaries
                               join smsDetail in _context.SMSummaryDetails on smS.Id equals smsDetail.SMSummaryId
                               join sGD in _context.SubjectGroupDetails.Include(a => a.Subject) on smsDetail.SubjectGroupDetailId equals sGD.Id
                               join aY in _context.AcademicYears.Include(a => a.Session) on smS.AcademicYearId equals aY.Id
                               where aY.Session.Year == DateTime.Now.Year
                               select new
                               {
                                   smS.Id,
                                   sGD.SubjectId,
                                   aY.Session.Year,
                                   smsDetail.SubjectGroupDetailId,
                                   smS.AcademicYearId,
                                   smS.AcademicYearSectionId,
                                   smS.FullName,
                                   smS.IDNo,
                                   smS.Gender,
                                   smS.SectionName,
                                   smS.OutOf,
                                   smS.TotalMark,
                                   smS.RankFromSection,
                                   smS.RankFromGrade,
                                   smS.RankFromSchool,
                                   SubjectName = sGD.Subject.Name + "(" + sGD.Subject.Code + ")",
                                   smsDetail.SubjectTotal,
                                   smsDetail.IsPassed,
                                   SubjectRankFromSection = smsDetail.RankFromSection,
                                   SubjectRankFromGrade = smsDetail.RankFromGrade,
                               }).ToList();

            var failedStudents = smSummaries.Where(a => a.SubjectTotal < 50).ToList();//all subjects
            var passedStudents = smSummaries.Where(a => a.SubjectTotal >= 50).ToList();
            var over75Students = smSummaries.Where(a => a.SubjectTotal >= 75).ToList();
            var firstRankFromSection = smSummaries.Where(a => a.RankFromSection == 1).ToList();
            var firstRankFromGrade = smSummaries.Where(a => a.RankFromGrade == 1).ToList();

            string markCategoryStatus = string.Empty;
            markCategoryStatus += "ABOVE 75%" + "#" + over75Students.Count +"#"+ "28a745" + ",";
            markCategoryStatus += "ABOVE 50%" + "#" + passedStudents.Count + "#" + "17a2b8" + ",";
            markCategoryStatus += "BELOW 50%" + "#" + failedStudents.Count + "#" + "dc3545" + ",";

            //get subject list
            string subjectLists = string.Empty;
            string salesDataPerMonth = string.Empty;
            string productCategorySales = string.Empty;

            string markCategoryDataPerSubject = string.Empty;
            foreach (var s in subjectList)
            {
                var subjectFailedStudents = failedStudents.Where(a => a.SubjectId == s.Id).ToList();
                var subjectPassedStudents = passedStudents.Where(a => a.SubjectId == s.Id).ToList();

                if (subjectFailedStudents.Count > 0 && subjectPassedStudents.Count > 0) 
                {
                    subjectLists += s.SubjectName + "#";
                    markCategoryDataPerSubject += s.SubjectName + "#" + subjectFailedStudents.Count + "#" + subjectPassedStudents.Count + ",";
                }
            }

            //ViewData["UserRoleModules"] = roleModules;
            ViewData["ItemsToExpire"] = itemsToExpire;
            ViewData["ItemsToOrder"] = ItemsToOrder;
            ViewData["ItemsToExpireCount"] = itemsToExpire.Count();
            ViewData["ItemsToOrderCount"] = ItemsToOrder.Count();
            ViewData["LatestProducts"] = latestProducts;
            ViewData["LatestProductCount"] = latestProducts.Count();
            ViewData["DailySalesAmt"] = dailySalesAmt;
            ViewData["WeeklySalesAmt"] = weeklySalesAmt;
            ViewData["DailySales"] = dailySales;
            ViewData["WeeklySales"] = weeklySales;
            ViewData["Title"] = "View Summary";
            HttpContext.Session.SetString("SalesDataPerMonth", salesDataPerMonth);
            HttpContext.Session.SetString("CategorySalesData", productCategorySales);
            HttpContext.Session.SetString("MarkCategoryDataPerSubject", markCategoryDataPerSubject);
            HttpContext.Session.SetString("MarkCategoryStatus", markCategoryStatus);
            HttpContext.Session.SetString("AllSubjects", subjectLists);
            ViewData["queryResult"] = smSummaries;
            ViewData["FailedStudents"] = failedStudents;
            ViewData["PassedStudents"] = passedStudents;
            ViewData["Over75Students"] = over75Students;
            ViewData["FirstRankFromSection"] = firstRankFromSection;
            ViewData["FirstRankFromGrade"] = firstRankFromGrade;
            return View();
        }
        public IActionResult SummaryDetail(string id) 
        {
            var smSummaries = (from smS in _context.SMSummaries
                               join smsDetail in _context.SMSummaryDetails on smS.Id equals smsDetail.SMSummaryId
                               join sGD in _context.SubjectGroupDetails.Include(a => a.Subject) on smsDetail.SubjectGroupDetailId equals sGD.Id
                               join aY in _context.AcademicYears.Include(a => a.Session) on smS.AcademicYearId equals aY.Id
                               where aY.Session.Year == DateTime.Now.Year
                               select new
                               {
                                   smS.Id,
                                   sGD.SubjectId,
                                   aY.Session.Year,
                                   smsDetail.SubjectGroupDetailId,
                                   smS.AcademicYearId,
                                   smS.AcademicYearSectionId,
                                   smS.FullName,
                                   smS.IDNo,
                                   smS.Gender,
                                   smS.SectionName,
                                   smS.OutOf,
                                   smS.TotalMark,
                                   smS.RankFromSection,
                                   smS.RankFromGrade,
                                   smS.RankFromSchool,
                                   SubjectName = sGD.Subject.Name + "(" + sGD.Subject.Code + ")",
                                   smsDetail.SubjectTotal,
                                   smsDetail.IsPassed,
                                   SubjectRankFromSection = smsDetail.RankFromSection,
                                   SubjectRankFromGrade = smsDetail.RankFromGrade,
                               }).ToList();
            if (id == "75")
                smSummaries = smSummaries.Where(a => a.SubjectTotal >= 75).ToList();
            else if(id == "50")
                smSummaries = smSummaries.Where(a => a.SubjectTotal >= 50).ToList();
            else if (id == "49")
                smSummaries = smSummaries.Where(a => a.SubjectTotal < 50).ToList();

            ViewData["id"] = id;
            ViewData["queryResult"] = smSummaries;
            return View();
        }
        public IActionResult SummaryToPdf(string id) {
            var smSummaries = (from smS in _context.SMSummaries
                               join smsDetail in _context.SMSummaryDetails on smS.Id equals smsDetail.SMSummaryId
                               join sGD in _context.SubjectGroupDetails.Include(a => a.Subject) on smsDetail.SubjectGroupDetailId equals sGD.Id
                               join aY in _context.AcademicYears.Include(a => a.Session) on smS.AcademicYearId equals aY.Id
                               where aY.Session.Year == DateTime.Now.Year
                               select new
                               {
                                   smS.Id,
                                   sGD.SubjectId,
                                   aY.Session.Year,
                                   smsDetail.SubjectGroupDetailId,
                                   smS.AcademicYearId,
                                   smS.AcademicYearSectionId,
                                   smS.FullName,
                                   smS.IDNo,
                                   smS.Gender,
                                   smS.SectionName,
                                   smS.OutOf,
                                   smS.TotalMark,
                                   smS.RankFromSection,
                                   smS.RankFromGrade,
                                   smS.RankFromSchool,
                                   SubjectName = sGD.Subject.Name + "(" + sGD.Subject.Code + ")",
                                   smsDetail.SubjectTotal,
                                   smsDetail.IsPassed,
                                   SubjectRankFromSection = smsDetail.RankFromSection,
                                   SubjectRankFromGrade = smsDetail.RankFromGrade,
                               }).ToList();

            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rptMarkSummary.rdlc";
            var rpt1 = string.Empty;

            if (id == "75")
            {
                smSummaries = smSummaries.Where(a => a.SubjectTotal >= 75).ToList();
                rpt1 = "ABOVE 75%";
            }
            else if (id == "50")
            {
                smSummaries = smSummaries.Where(a => a.SubjectTotal >= 50).ToList();
                rpt1 = "ABOVE 50%";
            }
            else if (id == "49")
            {
                smSummaries = smSummaries.Where(a => a.SubjectTotal < 50).ToList();
                rpt1 = "BELOW 50%";
            }
            
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("rpt1", rpt1);
            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("DataSet1", smSummaries);
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

            return File(result.MainStream, "application/pdf");
        }

        public IActionResult NotAuthorized()
        {
            int userRoleId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserRoleId));
            var roleModules = _context.RoleModules.Where(rm => rm.RoleId == userRoleId).ToList();
            ViewData["UserRoleModules"] = roleModules;
            return View();
        }

    }
}
