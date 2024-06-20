using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using panasonic_machine_checker.data;
using panasonic_machine_checker.Models;
using System.Dynamic;
using System.Xml.Linq;

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

        [HttpGet("/Mtc/Dashboard/{id}")]
        public IActionResult Dashboard(int id)
        {
            ViewData["UserRole"] = "MTC Leader";
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

            foreach (var item in data)
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

            return View("~/Views/MTC/Leader/Chart.cshtml", casesModel);
        }

        [HttpGet("/Mtc/ManagerDashboard/{id}")]
        public IActionResult ManagerDashboard(int id)
        {
            ViewData["UserRole"] = "MTC Manager";
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

            foreach (var item in data)
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

            return View("~/Views/MTC/Manager/Chart.cshtml", casesModel);
        }

        [HttpGet("/MTC/LeaderProfile")]
        public IActionResult LeaderProfile()
        {
            ViewData["UserRole"] = "MTC Leader";
            return View("~/Views/Production/Profile.cshtml");
        }

        [HttpGet("/MTC/ManagerMtcReportedCase/{id}")]
        public IActionResult ManagerMtcReportedCase(int id, int page = 1, string sortOrder = "newest", string statusOrder = "Pending")
        {
            ViewData["UserRole"] = "MTC Manager";
            int pageSize = 10;
            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();

            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();
            var machines = _context.Machines
                .Include(m => m.MachineLiniId)
                .ThenInclude(l => l.BULiniId)
                .ThenInclude(b => b.ManagerUserId)
                .Where(m => m.MachineLiniId.BULiniId.ManagerUserId.Id == id)
                .ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.ToList();

            StatusModel statusModel = new StatusModel();
            statusModel.StatusList = new List<StatusCases>();
            var status = _context.StatusCases.ToList();

            var query = _context.Cases
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.RepairSchedules)
                .Include(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .ThenInclude(l => l.BULiniId)
                .ThenInclude(b => b.ManagerUserId)
                .AsQueryable();

            query = query.Where(c => c.Status.Name == statusOrder);
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
                    RepairSchedule = item.RepairSchedules,
                    ReportedByNavigation = item.ReportedByNavigation,
                    Vendor = item.Vendor,
                    Reason = item.Reason,
                });
            }
            caseModel.TotalItems = itemCount;
            caseModel.CurrentPage = page;
            caseModel.PageSize = pageSize;


            foreach (var machine in machines)
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
            return View("~/Views/MTC/Manager/ReportedCase.cshtml", caseModel);
        }

        [HttpGet("/MTC/LeaderMtcReportedCase/{id}")]
        public IActionResult LeaderMtcReportedCase(int id, int page = 1, string sortOrder = "newest", string statusOrder = "Pending")
        {
            ViewData["UserRole"] = "MTC Leader";
            int pageSize = 10;
            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();

            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();
            var machines = _context.Machines
                .Include(m => m.MachineLiniId)
                .Where(m => m.MachineLiniId.LeaderId == id)
                .ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.ToList();

            StatusModel statusModel = new StatusModel();
            statusModel.StatusList = new List<StatusCases>();
            var status = _context.StatusCases.ToList();

            var query = _context.Cases
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.RepairSchedules)
                .Include(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .AsQueryable();

            query = query.Where(c => c.Status.Name == statusOrder);
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
                    RepairSchedule = item.RepairSchedules,
                    ReportedByNavigation = item.ReportedByNavigation,
                    Vendor = item.Vendor,
                    Reason = item.Reason,
                });
            }
            caseModel.TotalItems = itemCount;
            caseModel.CurrentPage = page;
            caseModel.PageSize = pageSize;


            foreach (var machine in machines)
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
            return View("~/Views/MTC/Leader/ReportedCase.cshtml", caseModel);
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


        [HttpGet("/MTC/Leader/{id}")]
        public IActionResult Leader(int id, int page = 1, string sortOrder = "newest")
        {
            ViewData["UserRole"] = "MTC Leader";
            int pageSize = 10;
            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();

            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();
            var machines = _context.Machines.ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.ToList();

            UsersModel member = new UsersModel();
            member.UserList = new List<Users>();
            var members = _context.Users.Include(u => u.RoleNavigation).Where(u => u.RoleNavigation.Name == "Member MTC").ToList();

            StatusModel statusModel = new StatusModel();
            statusModel.StatusList = new List<StatusCases>();
            var status = _context.StatusCases.ToList();

            RepairSchedulesModel repairSchedulesModel = new RepairSchedulesModel();
            repairSchedulesModel.RepairSchedulesList = new List<RepairSchedules>();
            var repair_schedules = _context.RepairSchedules.Include(item => item.ScheduledByNavigation).Include(item => item.Case).Include(item => item.Status).ToList();

            var query = _context.Cases
                .Include(c => c.Kytforms)
                    .ThenInclude(k => k.Member)
                        .ThenInclude(m => m.UserMember)
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.RepairSchedules)
                .Include(c => c.Machine)
                    .ThenInclude(m => m.MachineLiniId)
                        .ThenInclude(l => l.BULiniId)
                            .ThenInclude(b => b.BuMaintenance)
                .Where(c => c.DateCompleted != null && c.IsScheduled != 0 && c.IsScheduled != null &&
                            c.Machine.MachineLiniId.BULiniId.BuMaintenance.Any(item => item.ManagerId == id))
                .AsQueryable();

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

            var buData = "";

            foreach (var item in data)
            {
                var lini = item.Machine?.MachineLiniId;
                var bu = lini?.BULiniId;
                var kyt = item.Kytforms.FirstOrDefault();
                var filled = kyt?.Member;

                buData = bu.Name;

                caseModel.CasesList.Add(new Cases
                {
                    Id = item.Id,
                    Description = item.Description,
                    MachineId = item.MachineId,
                    ReportedById = item.ReportedById,
                    StatusId = item.StatusId,
                    DateCompleted = item.DateCompleted,
                    DateReported = item.DateReported,
                    IsApproved = item.IsApproved,
                    ApprovedAt = item.ApprovedAt,
                    Machine = item.Machine,
                    Status = item.Status,
                    ReportedByNavigation = item.ReportedByNavigation,
                    Kytforms = item.Kytforms,
                    Lini = lini,
                    BU = bu,
                    ClosedData = filled,
                });
            }

            ViewBag.BU = buData;

            caseModel.TotalItems = itemCount;
            caseModel.CurrentPage = page;
            caseModel.PageSize = pageSize;

            foreach (var machine in machines)
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

            foreach (var item in members)
            {
                member.UserList.Add(new Users
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

            foreach (var item in repair_schedules)
            {
                repairSchedulesModel.RepairSchedulesList.Add(new RepairSchedules
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

            ViewBag.RepairScheduleList = repairSchedulesModel.RepairSchedulesList;
            ViewBag.MachineList = machinesModel.MachinesList;
            ViewBag.UserList = usersModel.UserList;
            ViewBag.MemberList = member.UserList;
            ViewBag.StatusList = statusModel.StatusList;
            return View("~/Views/MTC/Leader/Index.cshtml", caseModel);
        }

        [HttpGet("/MTC/ManagerCase/{id}")]
        public IActionResult ManagerCase(int id, int page = 1, string sortOrder = "newest")
        {
            ViewData["UserRole"] = "MTC Manager";
            int pageSize = 10;
            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();

            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();
            var machines = _context.Machines.ToList();

            UsersModel usersModel = new UsersModel();
            usersModel.UserList = new List<Users>();
            var users = _context.Users.ToList();

            UsersModel member = new UsersModel();
            member.UserList = new List<Users>();
            var members = _context.Users.Include(u => u.RoleNavigation).Where(u => u.RoleNavigation.Name == "Member MTC").ToList();

            StatusModel statusModel = new StatusModel();
            statusModel.StatusList = new List<StatusCases>();
            var status = _context.StatusCases.ToList();

            RepairSchedulesModel repairSchedulesModel = new RepairSchedulesModel();
            repairSchedulesModel.RepairSchedulesList = new List<RepairSchedules>();
            var repair_schedules = _context.RepairSchedules.Include(item => item.ScheduledByNavigation).Include(item => item.Case).Include(item => item.Status).ToList();

            var query = _context.Cases
                .Include(c => c.Kytforms)
                    .ThenInclude(k => k.Member)
                        .ThenInclude(m => m.UserMember)
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.RepairSchedules)
                .Include(c => c.Machine)
                    .ThenInclude(m => m.MachineLiniId)
                        .ThenInclude(l => l.BULiniId)
                            .ThenInclude(b => b.BuMaintenance)
                .Where(c => c.DateCompleted != null &&
                            c.Machine.MachineLiniId.BULiniId.BuMaintenance.Any(item => item.ManagerId == id))
                .AsQueryable();

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
                var lini = item.Machine?.MachineLiniId;
                var bu = lini?.BULiniId;
                var kyt = item.Kytforms.FirstOrDefault();
                var filled = kyt?.Member;

                caseModel.CasesList.Add(new Cases
                {
                    Id = item.Id,
                    Description = item.Description,
                    MachineId = item.MachineId,
                    ReportedById = item.ReportedById,
                    StatusId = item.StatusId,
                    DateCompleted = item.DateCompleted,
                    DateReported = item.DateReported,
                    IsApproved = item.IsApproved,
                    ApprovedAt = item.ApprovedAt,
                    Machine = item.Machine,
                    Status = item.Status,
                    ReportedByNavigation = item.ReportedByNavigation,
                    Kytforms = item.Kytforms,
                    Lini = lini,
                    BU = bu,
                    ClosedData = filled,
                });
            }
            caseModel.TotalItems = itemCount;
            caseModel.CurrentPage = page;
            caseModel.PageSize = pageSize;

            foreach (var machine in machines)
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

            foreach (var item in members)
            {
                member.UserList.Add(new Users
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

            foreach (var item in repair_schedules)
            {
                repairSchedulesModel.RepairSchedulesList.Add(new RepairSchedules
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

            ViewBag.RepairScheduleList = repairSchedulesModel.RepairSchedulesList;
            ViewBag.MachineList = machinesModel.MachinesList;
            ViewBag.UserList = usersModel.UserList;
            ViewBag.MemberList = member.UserList;
            ViewBag.StatusList = statusModel.StatusList;
            return View("~/Views/MTC/Manager/Case.cshtml", caseModel);
        }

        [HttpPost("/MTC/Leader/CreateRepairSchedule")]
        public JsonResult CreateRepairSchedule([FromBody] RepairSchedule data)
        {
            _context.RepairSchedules.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/MTC/Leader/CreateMachineRepairs")]
        public JsonResult CreateMachineRepairs([FromBody] MachineRepair data)
        {
            _context.MachineRepairs.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/MTC/Manager/UpdateCase/{id}")]
        public JsonResult ManagerUpdateCase(int id, [FromBody] Case data)
        {
            var case_data = _context.Cases.Find(id);
            if (case_data != null)
            {
                if (data.ApprovedAt.HasValue)
                    case_data.ApprovedAt = data.ApprovedAt;

                if (data.IsApproved.HasValue)
                    case_data.IsApproved = data.IsApproved;

                if (data.DecisionManager.HasValue)
                    case_data.DecisionManager = data.DecisionManager;

                _context.Cases.Update(case_data);
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }


        [HttpPost("/MTC/Leader/UpdateCase/{id}")]
        public JsonResult UpdateCase(int id, [FromBody] Case data)
        {
            var case_data = _context.Cases.Find(id);
            if (case_data != null)
            {
                if (data.MachineId.HasValue)
                    case_data.MachineId = data.MachineId;

                if (data.ReportedById.HasValue)
                    case_data.ReportedById = data.ReportedById;

                if (!string.IsNullOrEmpty(data.Description))
                    case_data.Description = data.Description;

                if (data.StatusId.HasValue)
                    case_data.StatusId = data.StatusId;

                if (data.DateCompleted.HasValue)
                    case_data.DateCompleted = data.DateCompleted;

                if (data.DateReported.HasValue)
                    case_data.DateReported = data.DateReported;

                if (data.DecisionManager.HasValue)
                    case_data.DecisionManager = data.DecisionManager;

                _context.Cases.Add(case_data);
                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpPost("/MTC/Leader/CreateKYTForms")]
        public JsonResult LeaderCreateKYTForms([FromBody] Kytform data)
        {
            _context.Kytforms.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/MTC/Leader/CreateKYTMember")]
        public JsonResult LeaderCreateKYTMember([FromBody] KytMember data)
        {
            var userExists = _context.Users.Any(u => u.Id == data.MemberId);
            if (!userExists)
            {
                return Json(new { success = false, message = "MemberId does not exist in the user table.", memberId = data.MemberId });
            }
            var kytExist = _context.Kytforms.Any(k => k.Id == data.KytId);
            if (!kytExist)
            {
                return Json(new { success = false, message = "KytId does not exist in the user table.", kytId = data.KytId });
            }
            _context.KytMembers.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
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
            var cases = _context.Cases
                .Include(c => c.Kytforms)
                    .ThenInclude(k => k.Member)
                        .ThenInclude(m => m.UserMember)
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.RepairSchedules)
                .Include(c => c.Machine)
                    .ThenInclude(m => m.MachineLiniId)
                        .ThenInclude(l => l.BULiniId)
                            .ThenInclude(b => b.BuMaintenance)
                .Where(c => c.DateCompleted != null &&
                            c.Machine.MachineLiniId.BULiniId.BuMaintenance.Any(item => item.ManagerId == id))
                .ToList();

            var now = DateTime.Now;
            var query = _context.Kytforms
                .Include(item => item.FilledByNavigation)
                .Include(item => item.Case)
                .ThenInclude(c => c.Machine)
                    .ThenInclude(m => m.MachineLiniId)
                        .ThenInclude(l => l.BULiniId)
                            .ThenInclude(b => b.BuMaintenance)
                .Where(c => c.Case.Machine.MachineLiniId.BULiniId.BuMaintenance.Any(item => item.ManagerId == id))
                .AsQueryable();

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
