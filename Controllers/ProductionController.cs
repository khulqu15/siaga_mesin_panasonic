﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using panasonic_machine_checker.data;
using panasonic_machine_checker.Models;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace panasonic_machine_checker.Controllers
{
    public class ProductionController : Controller
    {
        private DbPanasonicContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public ProductionController(ILogger<ProductionController> logger, DbPanasonicContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet("/Production/Dashboard/{id}")]
        public IActionResult Dashboard(int id)
        {
            ViewData["UserRole"] = "Production Leader";
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
                    Vendor = item.Vendor,
                    Reason = item.Reason,
                });
            }

            casesModel.TotalItems = count;
            casesModel.CurrentPage = 1;
            casesModel.PageSize = 10;

            return View("~/Views/Production/Leader/Chart.cshtml", casesModel);
        }

        [HttpGet("/Production/ManagerDashboard/{id}")]
        public IActionResult ManagerDashboard(int id)
        {
            ViewData["UserRole"] = "Production Manager";
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

            return View("~/Views/Production/Manager/Chart.cshtml", casesModel);
        }

        [HttpGet("/Production/UserData/{id}")]
        public JsonResult getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true, data = user });
        }

        [HttpGet("/Production/LeaderProfile")]
        public IActionResult LeaderProfile()
        {
            ViewData["UserRole"] = "Production Leader";
            return View("~/Views/Production/Profile.cshtml");
        }

        [HttpGet("/Production/ManagerProfile")]
        public IActionResult ManagerProfile()
        {
            ViewData["UserRole"] = "Production Manager";
            return View("~/Views/Production/Profile.cshtml");
        }

        [HttpGet("/Production/Leader/{id}")]
        public IActionResult Leader(int id, int page = 1, string sortOrder = "newest", string statusOrder = "Pending")
        {
            ViewData["UserRole"] = "Production Leader";
            int pageSize = 10;
            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();

            MachinesModel machinesModel = new MachinesModel();
            machinesModel.MachinesList = new List<Machines>();
            var machines = _context.Machines
                .Include(m => m.MachineLiniId)
                .Where(m => m.MachineLiniId.LeaderId == id && m.IsUsed != 1)
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
                .Where(c => c.Machine.MachineLiniId.LeaderId == id)
                .AsQueryable();

            var checkPendingCase = _context.Cases
                .Include(c => c.ReportedByNavigation)
                .Include(c => c.Status)
                .Include(c => c.RepairSchedules)
                .Include(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .Where(c => c.Machine.MachineLiniId.LeaderId == id && c.Status.Name == "Pending")
                .Count();

            query = query.Where(c => c.Status.Name == statusOrder);
            query = query.Where(c => c.ReportedByNavigation.Id == id);
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
            ViewBag.CountPendingCase = checkPendingCase;
            return View("~/Views/Production/Leader/Index.cshtml", caseModel);
        }


        [HttpPost("/Production/CreateCase")]
        public JsonResult CreateCase([FromBody] Case data)
        {
            _context.Cases.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, case_data = data });
        }

        [HttpPost("/Production/UpdateMachines/{id}")]
        public JsonResult UpdateMachines(int id, [FromBody] Machine data)
        {
            var machines = _context.Machines.Find(id);
            if(machines != null)
            {
                machines.IsUsed = data.IsUsed;
                _context.SaveChanges();
                return Json(new { success = true, data = machines });
            }
            return Json(new { success = false, message = "Machines not found." });

        }

        [HttpPost("/Production/UpdateCase/{id}")]
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
                return Json(new { success = true, case_data = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpDelete("/Production/DeleteCase/{id}")]
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


        [HttpGet("/Production/LeaderJobOrder/{id}")]
        public IActionResult LeaderJobOrder(int id, int page = 1, string sortOrder = "newest")
        {
            ViewData["UserRole"] = "Production Leader";
            int pageSize = 10;
            JobOrdersModel model = new JobOrdersModel();
            model.JobOrdersList = new List<JobOrders>();

            CasesModel caseModel = new CasesModel();
            caseModel.CasesList = new List<Cases>();
            var cases = _context.Cases.Include(c => c.Machine).Include(c => c.ReportedByNavigation).Include(c => c.Status).ToList();

            StatusJobOrdersModel statusModel = new StatusJobOrdersModel();
            statusModel.StatusList = new List<StatusJobOrders>();
            var status = _context.StatusJoborders.ToList();

            var query = _context.JobOrders
                .Include(item => item.ScheduledByNavigation)
                .Include(item => item.Status)
                .Include(item => item.Case)
                .ThenInclude(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .ThenInclude(l => l.BULiniId)
                .Where(item => item.Case.Machine.MachineLiniId.LeaderId == id)
                .AsQueryable();
            query = query.Where(c => c.ScheduledByNavigation.Id == id);
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
                    StatusId = item.StatusId
                });
            }

            model.TotalItems = itemCount;
            model.CurrentPage = page;
            model.PageSize = pageSize;

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

            foreach (var item in status)
            {
                statusModel.StatusList.Add(new StatusJobOrders
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            ViewBag.CaseList = caseModel.CasesList;
            ViewBag.StatusList = statusModel.StatusList;
            return View("~/Views/Production/Leader/JobOrders.cshtml", model);
        }
        [HttpGet("/Production/LeaderMachineRepairs/{id}")]
        public IActionResult LeaderMachineRepairs(int id, int page = 1, string sortOrder = "newest")
        {
            ViewData["UserRole"] = "Production Leader";
            int pageSize = 10;
            MachineRepairsModel model = new MachineRepairsModel();
            model.MachineRepairsList = new List<MachineRepairs>();

            RepairSchedulesModel caseModel = new RepairSchedulesModel();
            caseModel.RepairSchedulesList = new List<RepairSchedules>();
            var cases = _context.RepairSchedules.Include(c => c.Case).Include(c => c.ScheduledByNavigation).Include(c => c.Status).ToList();


            StatusMachineRepairsModel statusModel = new StatusMachineRepairsModel();
            statusModel.StatusList = new List<StatusMachineRepairs>();
            var status = _context.StatusMachinerepairs.ToList();

            var query = _context.MachineRepairs
                .Include(item => item.ReviewedByNavigation)
                .Include(item => item.Status)
                .Include(item => item.Schedule)
                .ThenInclude(schedule => schedule.Case)
                .ThenInclude(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .Where(item => item.Schedule.Case.Machine.MachineLiniId.LeaderId == id)
                .AsQueryable();
            
            // query = query.Where(item => item.ReviewedByNavigation.Id == id);

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

            foreach (var item in status)
            {
                statusModel.StatusList.Add(new StatusMachineRepairs
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }
            ViewBag.StatusList = statusModel.StatusList;

            return View("~/Views/Production/Leader/MachineRepairs.cshtml", model);
        }
        
        [HttpPost("/Production/UpdateMachineRepairs/{id}")]
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

        [HttpDelete("/Production/DeleteMachineRepairs/{id}")]
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

        [HttpDelete("/Production/DeleteJobOrder/{id}")]
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

        [HttpGet("/Production/ManagerReportedCase/{id}")]
        public IActionResult ManagerReportedCase(int id, int page = 1, string sortOrder = "newest", string statusOrder = "Pending")
        {
            ViewData["UserRole"] = "Production Manager";
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
                .Where(c => c.Machine.MachineLiniId.LeaderId == id)
                .AsQueryable();

            query = query.Where(c => c.Status.Name == statusOrder);
            query = query.Where(c => c.ReportedByNavigation.Id == id);
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
            return View("~/Views/Production/Manager/ReportedCase.cshtml", caseModel);
        }

        [HttpGet("/Production/Manager/{id}")]
        public IActionResult Manager(int id, int page = 1, string sortOrder = "newest", string statusOrder = "Pending")
        {
            ViewData["UserRole"] = "Production Manager";
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

            var query = _context.JobOrders
                .Include(item => item.ScheduledByNavigation)
                .Include(c => c.Status)
                .Include(item => item.Case)
                .ThenInclude(c => c.Machine)
                .ThenInclude(m => m.MachineLiniId)
                .ThenInclude(l => l.BULiniId)
                .Include(item => item.Case.Status)
                .Where(item => item.Case.Machine.MachineLiniId.BULiniId.ManagerId == id)
                .AsQueryable();

            query = query.Where(item => item.Case.Status.Name == statusOrder);

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
                    StatusId = item.StatusId
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
            return View("~/Views/Production/Manager/Index.cshtml", model);
        }

        [HttpPost("/Production/CreateJobOrder")]
        public JsonResult CreateJobOrder([FromBody] JobOrder data)
        {
            _context.JobOrders.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }

        [HttpPost("/Production/UpdateJobOrder/{id}")]
        public JsonResult UpdateJobOrder(int id, [FromBody] JobOrder data)
        {
            var case_data = _context.JobOrders.Find(id);
            if (case_data != null)
            {
                case_data.ScheduledBy = data.ScheduledBy;
                case_data.ScheduledDate = data.ScheduledDate;
                case_data.CaseId = data.CaseId;
                case_data.StatusId = data.StatusId;
                _context.SaveChanges();
                return Json(new { success = true, job_order = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpPut("/Production/UpdateManagerCase/{id}")]
        public JsonResult UpdateManagerCase(int id, [FromBody] Case data)
        {
            var case_data = _context.Cases.Find(id);
            if (case_data != null)
            {
                if(data.MachineId.HasValue)
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

                if (data.IsScheduled.HasValue)
                    case_data.IsScheduled = data.IsScheduled;

                if (!string.IsNullOrWhiteSpace(data.Vendor))
                    case_data.Vendor = data.Vendor;

                if (!string.IsNullOrWhiteSpace(data.Reason))
                    case_data.Reason = data.Reason;

                _context.SaveChanges();
                return Json(new { success = true, machine = case_data });
            }
            return Json(new { success = false, message = "Machine not found." });
        }

        [HttpGet("/Production/ManagerMachineRepairs/{id}")]
        public IActionResult ManagerMachineRepairs(int id, int page = 1, string sortOrder = "newest")
        {
            ViewData["UserRole"] = "Production Manager";
            int pageSize = 10;
            MachineRepairsModel model = new MachineRepairsModel();
            model.MachineRepairsList = new List<MachineRepairs>();

            RepairSchedulesModel caseModel = new RepairSchedulesModel();
            caseModel.RepairSchedulesList = new List<RepairSchedules>();
            var cases = _context.RepairSchedules.Include(c => c.Case).Include(c => c.ScheduledByNavigation).Include(c => c.Status).ToList();


            StatusMachineRepairsModel statusModel = new StatusMachineRepairsModel();
            statusModel.StatusList = new List<StatusMachineRepairs>();
            var status = _context.StatusMachinerepairs.ToList();

            var query = _context.MachineRepairs
                .Include(item => item.ReviewedByNavigation)
                .Include(item => item.Status)
                .Include(item => item.Schedule)
                    .ThenInclude(s => s.Case)
                        .ThenInclude(c => c.Machine)
                            .ThenInclude(m => m.MachineLiniId)
                                .ThenInclude(l => l.BULiniId)
                .Include(item => item.Schedule.Case.ReportedByNavigation)
                .Where(item => item.Schedule.Case.Machine.MachineLiniId.BULiniId.ManagerId == id)
                .AsQueryable();


            // query = query.Where(item => item.ReviewedByNavigation.Id == id);

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

            foreach (var item in status)
            {
                statusModel.StatusList.Add(new StatusMachineRepairs
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }
            ViewBag.StatusList = statusModel.StatusList;

            return View("~/Views/Production/Manager/MachineRepairs.cshtml", model);
        }

        [HttpPost("/Production/CreateRepairSchedule")]
        public JsonResult CreateRepairSchedule([FromBody] RepairSchedule data)
        {
            _context.RepairSchedules.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, schedule = data });
        }

        [HttpPost("/Production/CreateMachineRepairs")]
        public JsonResult ProductionMachineRepairs([FromBody] MachineRepair data)
        {
            _context.MachineRepairs.Add(data);
            _context.SaveChanges();
            return Json(new { success = true, machine = data });
        }
    }
}
