using Microsoft.AspNetCore.Mvc;

namespace panasonic_machine_checker.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cases()
        {
            return View();
        }

        public IActionResult Machines() {
            return View();
        }

        public IActionResult KYTForms()
        {
            return View();
        }

        public IActionResult RepairSchedules() { 
            return View();
        }

        public IActionResult Status()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}
