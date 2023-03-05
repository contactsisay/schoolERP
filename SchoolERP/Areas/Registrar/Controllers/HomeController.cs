using Microsoft.AspNetCore.Mvc;

namespace SchoolERP.Areas.Registrar.Controllers
{
    [Area("Registrar")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
