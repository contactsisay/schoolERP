using Microsoft.AspNetCore.Mvc;

namespace SchoolERP.Areas.Academic.Controllers
{
    [Area("Academic")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
