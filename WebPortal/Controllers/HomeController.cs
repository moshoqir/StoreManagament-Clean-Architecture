using Microsoft.AspNetCore.Mvc;

namespace WebPortal.Controllers
{
    public sealed class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Account");
        }
    }
}
