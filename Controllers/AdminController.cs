using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using panasonic_machine_checker.data;
using panasonic_machine_checker.Models;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure;
using System.Drawing.Printing;

namespace panasonic_machine_checker.Controllers
{
    public class AdminController : Controller
    {
        private DbPanasonicContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, DbPanasonicContext context)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet("/Admin/UserData/{id}")]
        public JsonResult getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true, data = user });
        }

        public IActionResult Index()
        {
            CasesModel casesModel = new CasesModel();
            casesModel.CasesList = new List<Cases>();

            var query = _context.Cases
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .AsQueryable();
            var count = query.Count();
            var data = query.ToList();

            foreach(var item in data)
            {
                casesModel.CasesList.Add(new Cases
                {
                    Id = item.Id,
                    Description = item.Description,
                    MachineId = item.MachineId,
                    ReportedById = item.ReportedById,
                    StatusId = item.StatusId,
                    DateCompleted = item.DateCompleted,
                    DateReported = item.DateReported,
                    Machine = item.Machine,
                    Status = item.Status,
                    ReportedByNavigation = item.ReportedByNavigation,
                    IsApproved = item.IsApproved,
                    ApprovedAt = item.ApprovedAt,
                    CreatedAt = item.CreatedAt,
                    Lini = item.Machine.MachineLiniId,
                });
            }

            casesModel.TotalItems = count;
            casesModel.CurrentPage = 1;
            casesModel.PageSize = 10;

            return View(casesModel);
        }

        [HttpGet("/Admin/Lini")]
        public IActionResult Lini(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;

            LiniesModel model = new LiniesModel();
            model.LiniesList = new List<Linies>();
            var query = _context.Linis.Include(c => c.LeaderLiniId).Include(c => c.BULiniId).AsQueryable();
            switch (sortOrder.ToLower())
            {
                case "newest":
                    query = query.OrderByDescending(m => m.Id);
                    break;
                case "oldest":
                    query = query.OrderBy(m => m.Id);
                    break;
            }
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).Where(u => u.RoleNavigation.Name == "Manager Production").ToList();

            BUsModel BUsModel = new BUsModel();
            BUsModel.BUsList = new List<BUs>();
            var bus = _context.BUs.Include(b => b.ManagerUserId).ToList();

            var itemCount = query.Count();

            foreach (var item in data)
            {
                model.LiniesList.Add(new Linies
                {
                    Id = item.Id,
                    Name = item.Name,
                    BuId = item.BUId,
                    BU = item.BULiniId,
                    LeaderId = item.LeaderId,
                    UserLeaderId = item.LeaderLiniId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                });
            }

            model.PageSize = pageSize;
            model.TotalItems = itemCount;
            model.CurrentPage = page;

            foreach (var item in users)
            {
                usersModel.UserList.Add(new Users
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Password = item.Password,
                    RoleNavigation = item.RoleNavigation,
                    Avatar = item.Avatar,
                    Phone = item.Phone,
                    VerifiedAt = item.VerifiedAt,
                    RoleId = item.Role
                });
            }
            ViewBag.UserList = usersModel.UserList;

            foreach (var item in bus)
            {
                BUsModel.BUsList.Add(new BUs
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ManagerId = item.ManagerId,
                    UserManagerId = item.ManagerUserId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                });
            }
            ViewBag.BUsList = BUsModel.BUsList;

            return View("~/Views/Admin/Lini.cshtml", model);
        }

        [HttpPost("/Admin/CreateLini")]
        public JsonResult CreateLini([FromBody] Lini data)
        {
            _context.Linis.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateLini/{id}")]
        public JsonResult UpdateLini(int id, [FromBody] Lini data)
        {
            var item = _context.Linis.Find(id);
            if (item != null)
            {
                item.Name = data.Name;
                item.LeaderId = data.LeaderId;
                item.BUId = data.BUId;
                item.CreatedAt = data.CreatedAt;
                item.UpdatedAt = data.UpdatedAt;
                _context.SaveChanges();
                return Json(new { success = true, item = item });
            }
            return Json(new { success = false, message = "BU's not found." });
        }

        [HttpDelete("/Admin/DeleteLini/{id}")]
        public JsonResult DeleteLini(int id)
        {
            var data = _context.Linis.Find(id);
            if (data != null)
            {
                _context.Linis.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet("/Admin/BU")]
        public IActionResult BU(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;

            BUsModel model = new BUsModel();
            model.BUsList = new List<BUs>();
            var query = _context.BUs.Include(c => c.ManagerUserId).Include(c => c.Lini).AsQueryable();
            switch (sortOrder.ToLower())
            {
                case "newest":
                    query = query.OrderByDescending(m => m.Id);
                    break;
                case "oldest":
                    query = query.OrderBy(m => m.Id);
                    break;
            }
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).Where(u => u.RoleNavigation.Name == "Manager Production").ToList();

            var itemCount = query.Count();
            
            foreach (var item in data)
            {
                model.BUsList.Add(new BUs
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ManagerId = item.ManagerId,
                    UserManagerId = item.ManagerUserId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                });
            }

            model.PageSize = pageSize;
            model.TotalItems = itemCount;
            model.CurrentPage = page;

            foreach (var item in users)
            {
                usersModel.UserList.Add(new Users
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Password = item.Password,
                    RoleNavigation = item.RoleNavigation,
                    Avatar = item.Avatar,
                    Phone = item.Phone,
                    VerifiedAt = item.VerifiedAt,
                    RoleId = item.Role
                });
            }

            ViewBag.UserList = usersModel.UserList;
            return View("~/Views/Admin/BU.cshtml", model);
        }


        [HttpPost("/Admin/CreateBU")]
        public JsonResult CreateBU([FromBody] BU data)
        {
            _context.BUs.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateBU/{id}")]
        public JsonResult UpdateBU(int id, [FromBody] BU data)
        {
            var item = _context.BUs.Find(id);
            if (item != null)
            {
                item.Name = data.Name;
                item.Description = data.Description;
                item.ManagerId = data.ManagerId;
                item.CreatedAt = data.CreatedAt;
                item.UpdatedAt = data.UpdatedAt;
                _context.SaveChanges();
                return Json(new { success = true, item = item });
            }
            return Json(new { success = false, message = "BU's not found." });
        }

        [HttpDelete("/Admin/DeleteBU/{id}")]
        public JsonResult DeleteBU(int id)
        {
            var data = _context.BUs.Find(id);
            if (data != null)
            {
                _context.BUs.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult Cases(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();

            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();
            var machines = _context.Machines.ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.ToList();

            StatusModel statusModel = new StatusModel();
            statusModel.StatusList = new List<StatusCases>();
            var status = _context.StatusCases.ToList();

            var query = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).AsQueryable();
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
                caseModel.CasesList.Add(new Cases
                {
                    Id = item.Id,
                    Description = item.Description,
                    MachineId = item.MachineId,
                    ReportedById = item.ReportedById,
                    StatusId = item.StatusId,
                    DateCompleted = item.DateCompleted,
                    DateReported = item.DateReported,
                    Machine = item.Machine,
                    Status = item.Status,
                    ReportedByNavigation = item.ReportedByNavigation,
                    IsApproved = item.IsApproved,
                    ApprovedAt = item.ApprovedAt,
                    CreatedAt = item.CreatedAt,
                });
            }
            caseModel.TotalItems = itemCount;
            caseModel.CurrentPage = page;
            caseModel.PageSize = pageSize;

            foreach(var machine in machines)
            {
                machinesModel.MachinesList.Add(new Machines
                {
                    Id = machine.Id,
                    Name = machine.Name,
                    Type = machine.Type,
                    Location = machine.Location
                });
            }

            foreach (var item in users)
            {
                usersModel.UserList.Add(new Users
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Password = item.Password,
                    RoleNavigation = item.RoleNavigation,
                    Avatar = item.Avatar,
                    Phone = item.Phone,
                    VerifiedAt = item.VerifiedAt,
                    RoleId = item.Role
                });
            }

            foreach (var item in status)
            {
                statusModel.StatusList.Add(new StatusCases
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            ViewBag.MachineList = machinesModel.MachinesList;
            ViewBag.UserList = usersModel.UserList;
            ViewBag.StatusList = statusModel.StatusList;
            return View(caseModel);
        }


        [HttpPost("/Admin/CreateCase")]
        public JsonResult CreateCase([FromBody] Case data)
        {
            _context.Cases.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateCase/{id}")]
        public JsonResult UpdateCase(int id, [FromBody] Case data)
        {
            var case_data = _context.Cases.Find(id);
            if (case_data != null)
            {
                case_data.MachineId = data.MachineId;
                case_data.ReportedById = data.ReportedById;
                case_data.Description = data.Description;
                case_data.StatusId = data.StatusId;
                case_data.DateCompleted = data.DateCompleted;
                case_data.DateReported = data.DateReported;
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteCase/{id}")]
        public JsonResult DeleteCase(int id)
        {
            var data = _context.Cases.Find(id);
            if (data != null)
            {
                _context.Cases.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult Machines(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();

            var query = _context.Machines.Include(m => m.MachineLiniId).ThenInclude(l => l.BULiniId).AsQueryable();

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
                machinesModel.MachinesList.Add(new Machines
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Location = item.Location,
                    LiniId = item.LiniId,
                    MachineLiniId = item.MachineLiniId,
                });
            }

            machinesModel.TotalItems = itemCount;
            machinesModel.CurrentPage = page;
            machinesModel.PageSize = pageSize;

            LiniesModel model = new LiniesModel();
            model.LiniesList = new List<Linies>();
            var linies = _context.Linis.Include(c => c.LeaderLiniId).Include(c => c.BULiniId).ToList();

            foreach (var item in linies)
            {
                model.LiniesList.Add(new Linies
                {
                    Id = item.Id,
                    Name = item.Name,
                    BuId = item.BUId,
                    BU = item.BULiniId,
                    LeaderId = item.LeaderId,
                    UserLeaderId = item.LeaderLiniId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                });
            }

            ViewBag.LiniesList = model.LiniesList;

            return View(machinesModel);
        }

        [HttpPost("/Admin/CreateMachine")]
        public JsonResult CreateMachine([FromBody] Machine machine)
        {
            _context.Machines.Add(machine);
            _context.SaveChanges();
            return Json(new { success = true, machine = machine });
        }

        [HttpPost("/Admin/UpdateMachine/{id}")]
        public JsonResult UpdateMachine(int id, [FromBody] Machine machine)
        {
            var machineData = _context.Machines.Find(id);
            if (machineData != null)
            {
                machineData.Name = machine.Name;
                machineData.Type = machine.Type;
                machineData.Location = machine.Location;
                machineData.LiniId = machine.LiniId;
                _context.SaveChanges();
                return Json(new { success = true, machine = machine });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteMachine/{id}")]
        public JsonResult DeleteMachine(int id)
        {
            var machine = _context.Machines.Find(id);
            if(machine != null) {
                _context.Machines.Remove(machine);
                _context.SaveChanges();
                return Json(new {success=true});
            }
            return Json(new {success=false});
        }

        public IActionResult MachineRepairs(int page = 1, string sortOrder = "newest") 
        {
            int pageSize = 10;
            MachineRepairsModel model = new MachineRepairsModel();
            model.MachineRepairsList = new List<MachineRepairs>();

            RepairSchedulesModel caseModel = new RepairSchedulesModel();
            caseModel.RepairSchedulesList = new List<RepairSchedules>();
            var cases = _context.RepairSchedules.Include(c => c.Case).Include(c => c.ScheduledByNavigation).Include(c => c.Status).ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).ToList();

            StatusMachineRepairsModel statusModel = new StatusMachineRepairsModel();
            statusModel.StatusList = new List<StatusMachineRepairs>();
            var status = _context.StatusMachinerepairs.ToList();

            var query = _context.MachineRepairs.Include(item => item.Schedule).Include(item => item.ReviewedByNavigation).Include(item => item.Status).AsQueryable();
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
                model.MachineRepairsList.Add(new MachineRepairs
                {
                    Id = item.Id,
                    ScheduledById = item.ScheduleId,
                    ReviewedById = item.ReviewedBy,
                    RepairDate = item.RepairDate,
                    Corrections = item.Corrections,
                    Result = item.Result,
                    Status = item.Status,
                    ReviewsBy = item.ReviewedByNavigation,
                    ScheduledBy = item.Schedule,
                    StatusId = item.StatusId,
                });
            }

            model.CurrentPage = page;
            model.PageSize = 10;
            model.TotalItems = itemCount;

            foreach (var item in cases)
            {
                caseModel.RepairSchedulesList.Add(new RepairSchedules
                {
                    StatusId = item.StatusId,
                    Status = item.Status,
                    Id = item.Id,
                    CallDate = item.CallDate,
                    RepairDate = item.RepairDate,
                    Case = item.Case,
                    ScheduledById = item.ScheduledBy,
                    ScheduledByNavigation = item.ScheduledByNavigation,
                });
            }
            ViewBag.CaseList = caseModel.RepairSchedulesList;

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

            foreach (var item in status)
            {
                statusModel.StatusList.Add(new StatusMachineRepairs
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }
            ViewBag.StatusList = statusModel.StatusList;

            return View(model);
        }

        [HttpPost("/Admin/CreateMachineRepairs")]
        public JsonResult CreateMachineRepairs([FromBody] MachineRepair data)
        {
            _context.MachineRepairs.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateMachineRepairs/{id}")]
        public JsonResult UpdateMachineRepairs(int id, [FromBody] MachineRepair data)
        {
            var case_data = _context.MachineRepairs.Find(id);
            if (case_data != null)
            {
                case_data.ScheduleId = data.ScheduleId;
                case_data.ReviewedBy = data.ReviewedBy;
                case_data.StatusId = data.StatusId;
                case_data.RepairDate = data.RepairDate;
                case_data.Corrections = data.Corrections;
                case_data.Result = data.Result;
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteMachineRepairs/{id}")]
        public JsonResult DeleteMachineRepairs(int id)
        {
            var data = _context.MachineRepairs.Find(id);
            if (data != null)
            {
                _context.MachineRepairs.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult JobOrders(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            JobOrdersModel model = new JobOrdersModel();
            model.JobOrdersList = new List<JobOrders>();

            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();
            var cases = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).ToList();

            StatusJobOrdersModel statusModel = new StatusJobOrdersModel();
            statusModel.StatusList = new List<StatusJobOrders>();
            var status = _context.StatusJoborders.ToList();

            var query = _context.JobOrders.Include(item => item.Case).Include(item => item.ScheduledByNavigation).Include(c => c.Status).AsQueryable();
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
                model.JobOrdersList.Add(new JobOrders
                {
                    Id = item.Id,
                    CaseId = item.CaseId,
                    Case = item.Case,
                    ScheduledById = item.ScheduledBy,
                    ScheduledBy = item.ScheduledByNavigation,
                    ScheduledDate = item.ScheduledDate,
                    Status = item.Status,
                    StatusId = item.StatusId,
                    Description = item.Description,
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

            foreach (var item in status)
            {
                statusModel.StatusList.Add(new StatusJobOrders
                {
                    Name = item.Name,
                    Id = item.Id,
                });
            }
            ViewBag.StatusList = statusModel.StatusList;

            return View(model);
        }

        [HttpPost("/Admin/CreateJobOrder")]
        public JsonResult CreateJobOrder([FromBody] JobOrder data)
        {
            _context.JobOrders.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateJobOrder/{id}")]
        public JsonResult UpdateJobOrder(int id, [FromBody] JobOrder data)
        {
            var case_data = _context.JobOrders.Find(id);
            if (case_data != null)
            {
                case_data.ScheduledBy = data.ScheduledBy;
                case_data.ScheduledDate = data.ScheduledDate;
                case_data.CaseId = data.CaseId;
                case_data.StatusId = data.StatusId;
                case_data.Description = data.Description;
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteJobOrder/{id}")]
        public JsonResult DeleteJobOrder(int id)
        {
            var data = _context.JobOrders.Find(id);
            if (data != null)
            {
                _context.JobOrders.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult KYTForms(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            KytFormsModel model = new KytFormsModel();
            model.KytFormsList = new List<KytForms>();

            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();
            var cases = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).ToList();

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

            return View(model);
        }

        [HttpPost("/Admin/CreateKYTForms")]
        public JsonResult CreateKYTForms([FromBody] Kytform data)
        {
            _context.Kytforms.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateKYTForms/{id}")]
        public JsonResult UpdateKYTForms(int id, [FromBody] Kytform data)
        {
            var case_data = _context.Kytforms.Find(id);
            if (case_data != null)
            {
                case_data.FilledBy = data.FilledBy;
                case_data.CaseId = data.CaseId;
                case_data.PotentialHazard = data.PotentialHazard;
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteKYTForms/{id}")]
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

        public IActionResult RepairSchedules(int page = 1, string sortOrder = "newest") {
            int pageSize = 10;
            RepairSchedulesModel model = new RepairSchedulesModel();
            model.RepairSchedulesList = new List<RepairSchedules>();

            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();
            var cases = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.Include(u => u.RoleNavigation).ToList();

            StatusRepairSchedulesModel statusModel = new StatusRepairSchedulesModel();
            statusModel.StatusList = new List<StatusRepairSchedules>();
            var status = _context.StatusRepairschedules.ToList();

            var query = _context.RepairSchedules.Include(item => item.ScheduledByNavigation).Include(item => item.Case).Include(item => item.Status).AsQueryable();
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
            
            foreach(var item in data)
            {
                model.RepairSchedulesList.Add(new RepairSchedules
                {
                    Id = item.Id,
                    ScheduledById = item.ScheduledBy,
                    CaseId = item.CaseId,
                    CallDate = item.CallDate,
                    Case = item.Case,
                    RepairDate = item.RepairDate,
                    ScheduledByNavigation = item.ScheduledByNavigation,
                    Status = item.Status,
                    StatusId = item.StatusId,
                });
            }

            model.CurrentPage = page;
            model.PageSize = 10;
            model.TotalItems = itemCount;

            foreach(var item in cases)
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

            foreach(var item in users)
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

            foreach(var item in status)
            {
                statusModel.StatusList.Add(new StatusRepairSchedules
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }
            ViewBag.StatusList = statusModel.StatusList;

            return View(model);
        }

        [HttpPost("/Admin/CreateRepairSchedule")]
        public JsonResult CreateRepairSchedule([FromBody] RepairSchedule data)
        {
            _context.RepairSchedules.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Admin/UpdateRepairSchedule/{id}")]
        public JsonResult UpdateRepairSchedule(int id, [FromBody] RepairSchedule data)
        {
            var case_data = _context.RepairSchedules.Find(id);
            if (case_data != null)
            {
                case_data.StatusId = data.StatusId;
                case_data.RepairDate = data.RepairDate;
                case_data.ScheduledBy = data.ScheduledBy;
                case_data.CaseId = data.CaseId;
                case_data.CallDate = data.CallDate;
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteRepairSchedule/{id}")]
        public JsonResult DeleteRepairSchedule(int id)
        {
            var data = _context.RepairSchedules.Find(id);
            if (data != null)
            {
                _context.RepairSchedules.Remove(data);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult Status(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            StatusModel statusModel = new StatusModel();
            statusModel.StatusList = new List<StatusCases>();

            var query = _context.StatusCases.AsQueryable();

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
                statusModel.StatusList.Add(new StatusCases
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            statusModel.TotalItems = itemCount;
            statusModel.CurrentPage = page;
            statusModel.PageSize = pageSize;

            return View(statusModel);
        }

        [HttpPost("/Admin/CreateStatus")]
        public JsonResult CreateStatus([FromBody] StatusCase status)
        {
            _context.StatusCases.Add(status);
            _context.SaveChanges();
            return Json(new { success = true, machine = status });
        }

        [HttpPost("/Admin/UpdateStatus/{id}")]
        public JsonResult UpdateStatus(int id, [FromBody] StatusCase status)
        {
            var roleData = _context.StatusCases.Find(id);
            if (roleData != null)
            {
                roleData.Name = status.Name;
                _context.SaveChanges();
                return Json(new { success = true, role = status });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteStatus/{id}")]
        public JsonResult DeleteStatus(int id)
        {
            var status = _context.StatusCases.Find(id);
            if (status != null)
            {
                _context.StatusCases.Remove(status);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult Users(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            UsersModel model = new UsersModel();
            model.UserList = new List<Users>();

            RolesModels roleModel = new RolesModels();
            roleModel.RolesList = new List<Roles>();
            
            var query = _context.Users.Include(u => u.RoleNavigation).AsQueryable();
            var role = _context.Roles.ToList();

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
                model.UserList.Add(new Users
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Password = item.Password,
                    RoleNavigation = item.RoleNavigation,
                    Avatar = item.Avatar,
                    Phone = item.Phone,
                    VerifiedAt = item.VerifiedAt,
                    RoleId = item.Role
                });
            }

            model.TotalItems = itemCount;
            model.CurrentPage = page;
            model.PageSize = pageSize;

            foreach(var item in role)
            {
                roleModel.RolesList.Add(new Roles
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            ViewBag.RoleList = roleModel.RolesList;

            return View(model);
        }

        [HttpPost("/Admin/CreateUser")]
        public async Task<JsonResult> CreateUser([FromForm] IFormFile avatar, [FromForm] string name, [FromForm] string email, [FromForm] string phone, [FromForm] string password, [FromForm] int role)
        {
            try
            {
                var user = new User
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Password = _passwordHasher.HashPassword(null, password),
                    Role = role
                };

                if (avatar != null && avatar.Length > 0)
                {
                    var uploadPath = Path.Combine("wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, avatar.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(stream);
                    }
                    user.Avatar = filePath;
                }
                _context.Users.Add(user);
                _context.SaveChanges();
                return Json(new { success = true, user = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("/Admin/UpdateUser/{id}")]
        public async Task<JsonResult> UpdateUser(int id, [FromForm] IFormFile avatar, [FromForm] string name, [FromForm] string email, [FromForm] string phone, [FromForm] string password, [FromForm] int role)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                user.Name = name;
                user.Email = email;
                user.Phone = phone;
                user.Password = _passwordHasher.HashPassword(user, password);
                user.Role = role;

                if (avatar != null && avatar.Length > 0)
                {
                    var uploadPath = Path.Combine("wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, avatar.FileName);
                    if (user.Avatar != null)
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), user.Avatar);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(stream);
                    }
                    user.Avatar = filePath;  // Update with the new file path
                }

                _context.SaveChanges();
                return Json(new { success = true, user = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("/Admin/DeleteUser/{id}")]
        public JsonResult DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                if (user.Avatar != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), user.Avatar);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Roles(int page = 1, string sortOrder = "newest")
        {
            int pageSize = 10;
            RolesModels model = new RolesModels();
            model.RolesList = new List<Roles>();

            var query = _context.Roles.AsQueryable();

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
                model.RolesList.Add(new Roles
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            model.TotalItems = itemCount;
            model.CurrentPage = page;
            model.PageSize = pageSize;

            return View(model);
        }

        [HttpPost("/Admin/CreateRole")]
        public JsonResult CreateRole([FromBody] Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return Json(new { success = true, machine = role });
        }

        [HttpPost("/Admin/UpdateRole/{id}")]
        public JsonResult UpdateRole(int id, [FromBody] Role role)
        {
            var roleData = _context.Roles.Find(id);
            if (roleData != null)
            {
                roleData.Name = role.Name;
                _context.SaveChanges();
                return Json(new { success = true, role = role });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Admin/DeleteRole/{id}")]
        public JsonResult DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
