using Microsoft.AspNetCore.Mvc;

namespace panasonic_machine_checker.Controllers
{
    public class ProductionController : Controller
    {
        public IActionResult Index()
        {
            ViewData["UserRole"] = "Production Leader";
            return View("~/Views/Production/Leader/Index.cshtml");
        }

        public IActionResult Leader()
        {
            ViewData["UserRole"] = "Production Leader";
            return View("~/Views/Production/Leader/Index.cshtml");
        }

        public IActionResult Manager()
        {
            ViewData["UserRole"] = "Production Manager";
            return View("~/Views/Production/Manager/Index.cshtml");
        }

        public IActionResult ManagerMachineRepairs()
        {
            ViewData["UserRole"] = "Production Manager";
            return View("~/Views/Production/Manager/MachineRepairs.cshtml");
        }
    }
}
