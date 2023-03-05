using Microsoft.AspNetCore.Mvc;
using SchoolERP.Models;

namespace SchoolERP.Areas.Purchase.Controllers
{
    [Area("Purchase")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Purchase Home";
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            return View();
        }
    }
}
