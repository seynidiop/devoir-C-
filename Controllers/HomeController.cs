using Microsoft.AspNetCore.Mvc;

namespace GestionApprovisionnements.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Approvisionnements");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
