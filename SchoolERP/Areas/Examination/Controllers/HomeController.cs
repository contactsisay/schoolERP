using Microsoft.AspNetCore.Mvc;

namespace SchoolERP.Areas.Examination.Controllers
{
    [Area("Examination")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
