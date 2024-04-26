using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using panasonic_machine_checker.data;
using panasonic_machine_checker.Models;

namespace panasonic_machine_checker.Controllers
{

    public class MtcController : Controller
    {
        private DbPanasonicContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<MtcController> _logger;

        public MtcController(ILogger<MtcController> logger, DbPanasonicContext context)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet("/MTC/LeaderProfile")]
        public IActionResult LeaderProfile()
        {
            ViewData["UserRole"] = "MTC Leader";
            return View("~/Views/Production/Profile.cshtml");
        }

        [HttpGet("/MTC/ManagerProfile")]
        public IActionResult ManagerProfile()
        {
            ViewData["UserRole"] = "MTC Manager";
            return View("~/Views/Production/Profile.cshtml");
        }

        [HttpGet("/MTC/MemberProfile")]
        public IActionResult MemberProfile()
        {
            ViewData["UserRole"] = "MTC Member";
            return View("~/Views/Production/Profile.cshtml");
        }

        [HttpGet("/MTC/Manager/{id}")]
        public IActionResult Manager(int id, int page = 1, string sortOrder = "newest") {
            ViewData["UserRole"] = "MTC Manager";
            ViewData["UserId"] = id;
            int pageSize = 10;
            KytFormsModel model = new KytFormsModel();
            model.KytFormsList = new List<KytForms>();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).ToList();

            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();
            var cases = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).ToList();

            var query = _context.Kytforms.Include(item => item.Case).Include(item => item.FilledByNavigation).AsQueryable();
            switch (sortOrder.ToLower())
            {
                case "newest":
                    query = query.OrderByDescending(m => m.Id);
                    break;
                case "oldest":
                    query = query.OrderBy(m => m.Id);
                    break;
            }
            var itemCount = query.Count();
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            foreach (var item in data)
            {
                model.KytFormsList.Add(new KytForms
                {
                    Id = item.Id,
                    CaseId = item.CaseId,
                    PotentialHazard = item.PotentialHazard,
                    Case = item.Case,
                    FilledById = item.FilledBy,
                    FilledBy = item.FilledByNavigation,
                    Approval = item.Approval,
                });
            }

            model.CurrentPage = page;
            model.PageSize = 10;
            model.TotalItems = itemCount;

            foreach (var item in cases)
            {
                caseModel.CasesList.Add(new Cases
                {
                    Description = item.Description,
                    MachineId = item.MachineId,
                    ReportedById = item.ReportedById,
                    StatusId = item.StatusId,
                    ReportedByNavigation = item.ReportedByNavigation,
                    Status = item.Status,
                    Machine = item.Machine,
                    DateCompleted = item.DateCompleted,
                    DateReported = item.DateReported,
                    Id = item.Id,
                });
            }
            ViewBag.CaseList = caseModel.CasesList;

            foreach (var item in users)
            {
                usersModel.UserList.Add(new Users
                {
                    Name = item.Name,
                    Email = item.Email,
                    Password = item.Password,
                    Phone = item.Phone,
                    RoleId = item.Role,
                    Id = item.Id,
                    RoleNavigation = item.RoleNavigation,
                });
            }
            ViewBag.UserList = usersModel.UserList;

            return View("~/Views/MTC/Manager/Index.cshtml", model);
        }

        [HttpPost("/MTC/CreateKYTForms")]
        public JsonResult CreateKYTForms([FromBody] Kytform data)
        {
            _context.Kytforms.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/MTC/UpdateKYTForms/{id}")]
        public JsonResult UpdateKYTForms(int id, [FromBody] Kytform data)
        {
            var case_data = _context.Kytforms.Find(id);
            if (case_data != null)
            {
                case_data.FilledBy = data.FilledBy;
                case_data.CaseId = data.CaseId;
                case_data.PotentialHazard = data.PotentialHazard;
                case_data.Approval = data.Approval;
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/MTC/DeleteKYTForms/{id}")]
        public JsonResult DeleteKYTForms(int id)
        {
            var data = _context.Kytforms.Find(id);
            if (data != null)
            {
                _context.Kytforms.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet("/MTC/Member/{id}")]
        public IActionResult Member(int id, int page = 1, string sortOrder = "newest")
        {
            ViewData["UserRole"] = "MTC Member";
            ViewData["UserId"] = id;
            int pageSize = 10;
            KytFormsModel model = new KytFormsModel();
            model.KytFormsList = new List<KytForms>();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).ToList();

            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();
            var cases = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).ToList();

            var query = _context.Kytforms.Include(item => item.Case).Include(item => item.FilledByNavigation).AsQueryable();
            switch (sortOrder.ToLower())
            {
                case "newest":
                    query = query.OrderByDescending(m => m.Id);
                    break;
                case "oldest":
                    query = query.OrderBy(m => m.Id);
                    break;
            }
            var itemCount = query.Count();
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            foreach (var item in data)
            {
                model.KytFormsList.Add(new KytForms
                {
                    Id = item.Id,
                    CaseId = item.CaseId,
                    PotentialHazard = item.PotentialHazard,
                    Case = item.Case,
                    FilledById = item.FilledBy,
                    FilledBy = item.FilledByNavigation,
                    Approval = item.Approval,
                });
            }

            model.CurrentPage = page;
            model.PageSize = 10;
            model.TotalItems = itemCount;

            foreach (var item in cases)
            {
                caseModel.CasesList.Add(new Cases
                {
                    Description = item.Description,
                    MachineId = item.MachineId,
                    ReportedById = item.ReportedById,
                    StatusId = item.StatusId,
                    ReportedByNavigation = item.ReportedByNavigation,
                    Status = item.Status,
                    Machine = item.Machine,
                    DateCompleted = item.DateCompleted,
                    DateReported = item.DateReported,
                    Id = item.Id,
                });
            }
            ViewBag.CaseList = caseModel.CasesList;

            foreach (var item in users)
            {
                usersModel.UserList.Add(new Users
                {
                    Name = item.Name,
                    Email = item.Email,
                    Password = item.Password,
                    Phone = item.Phone,
                    RoleId = item.Role,
                    Id = item.Id,
                    RoleNavigation = item.RoleNavigation,
                });
            }
            ViewBag.UserList = usersModel.UserList;

            return View("~/Views/MTC/Member/Index.cshtml", model);
        }
    }
}
