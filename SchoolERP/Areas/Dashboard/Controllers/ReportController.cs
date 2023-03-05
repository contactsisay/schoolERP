using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Models;
using System.Data;

namespace SchoolERP.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Print()
        //{
            //var dt = new DataTable();
            ////dt = GetEmployeeList();
            

            //string mimetype = "";
            //int extension = 1;
            //var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\rptSummaryBySubject.rdlc";

            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("rp1", "Welcome to RDLC");
            //LocalReport localReport = new LocalReport(path);
            //localReport.AddDataSource("rptDataSet", dt);
            
            //var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

            //return File(result.MainStream, "application/pdf");
        //}
    }
}
