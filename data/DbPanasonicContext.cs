using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace panasonic_machine_checker.data;

public partial class DbPanasonicContext : DbContext
{
    public DbPanasonicContext()
    {
    }

    public DbPanasonicContext(DbContextOptions<DbPanasonicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Case> Cases { get; set; }

    public virtual DbSet<JobOrder> JobOrders { get; set; }

    public virtual DbSet<Kytform> Kytforms { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<MachineRepair> MachineRepairs { get; set; }

    public virtual DbSet<RepairSchedule> RepairSchedules { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StatusCase> StatusCases { get; set; }

    public virtual DbSet<StatusJoborder> StatusJoborders { get; set; }

    public virtual DbSet<StatusMachinerepair> StatusMachinerepairs { get; set; }

    public virtual DbSet<StatusRepairschedule> StatusRepairschedules { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<BU> BUs { get; set; }

    public virtual DbSet<Lini> Linis { get; set; }

    public virtual DbSet<KytMember> KytMembers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-2EGHJVD\\SQLEXPRESS;Database=db_panasonic;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Case>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cases__3214EC0769DC0362");

            entity.ToTable("cases");

            entity.Property(e => e.Id);
            entity.Property(e => e.DateCompleted)
                .HasColumnType("datetime")
                .HasColumnName("date_completed");
            entity.Property(e => e.DateReported)
                .HasColumnType("datetime")
                .HasColumnName("date_reported");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.ReportedById).HasColumnName("reported_by");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.IsApproved).HasColumnName("is_approved");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Machine).WithMany(p => p.Cases)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_cases_machines");

            entity.HasOne(d => d.ReportedByNavigation).WithMany(p => p.Cases)
                .HasForeignKey(d => d.ReportedById)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_cases_users");

            entity.HasOne(d => d.Status).WithMany(p => p.Cases)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_cases_status");
        });

        modelBuilder.Entity<JobOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__job_orde__3214EC07A1BB962C");

            entity.ToTable("job_orders");

            entity.Property(e => e.Id);
            entity.Property(e => e.CaseId).HasColumnName("case_id");
            entity.Property(e => e.ScheduledBy).HasColumnName("scheduled_by");
            entity.Property(e => e.ScheduledDate)
                .HasColumnType("datetime")
                .HasColumnName("scheduled_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Description).HasColumnName("description");

            entity.HasOne(d => d.Case).WithMany(p => p.JobOrders)
                .HasForeignKey(d => d.CaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_joborders_cases");

            entity.HasOne(d => d.ScheduledByNavigation).WithMany(p => p.JobOrders)
                .HasForeignKey(d => d.ScheduledBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_joborders_users");

            entity.HasOne(d => d.Status).WithMany(p => p.JobOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_joborders_status");
        });

        modelBuilder.Entity<Kytform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__kytforms__3214EC0735B75197");

            entity.ToTable("kytforms");

            entity.Property(e => e.Id);
            entity.Property(e => e.CaseId).HasColumnName("case_id");
            entity.Property(e => e.FilledBy).HasColumnName("filled_by");
            entity.Property(e => e.Approval).HasColumnName("approval");
            entity.Property(e => e.PotentialHazard)
                .HasColumnType("text")
                .HasColumnName("potential_hazard");

            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");

            entity.Property(e => e.Action).HasColumnType("text").HasColumnName("action");
            entity.Property(e => e.DangerousMode).HasColumnType("text").HasColumnName("dangerous_mode");
            entity.Property(e => e.PrepareProcess).HasColumnType("text").HasColumnName("prepare_process");
            entity.Property(e => e.PreparePrediction).HasColumnType("text").HasColumnName("prepare_prediction");
            entity.Property(e => e.MainProcess).HasColumnType("text").HasColumnName("main_process");
            entity.Property(e => e.MainPrediction).HasColumnType("text").HasColumnName("main_prediction");
            entity.Property(e => e.ConfirmProcess).HasColumnType("text").HasColumnName("confirm_process");
            entity.Property(e => e.ConfirmPrediction).HasColumnType("text").HasColumnName("confirm_prediction");
            entity.Property(e => e.ApproveAt).HasColumnName("approve_at");

            entity.HasOne(d => d.Case).WithMany(p => p.Kytforms)
                .HasForeignKey(d => d.CaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_kytforms_cases");

            entity.HasOne(d => d.FilledByNavigation).WithMany(p => p.Kytforms)
                .HasForeignKey(d => d.FilledBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_kytforms_users");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__machines__3214EC076A78C8B9");

            entity.ToTable("machines");

            entity.Property(e => e.Id);
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.LiniId).HasColumnName("lini_id");

            entity.HasOne(e => e.MachineLiniId).WithMany(p => p.Machines)
                .HasForeignKey(d => d.LiniId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_machine_linies");
        });

        modelBuilder.Entity<KytMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_kyt_member");
            entity.ToTable("kyt_member");
            entity.Property(e => e.Id);
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.KytId).HasColumnName("kyt_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.UserMember).WithMany(p => p.Member)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_kyt_member_users");

            entity.HasOne(d => d.Kyform).WithMany(p => p.Member)
                .HasForeignKey(d => d.KytId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_kyt_member_kytforms");
        });

        modelBuilder.Entity<MachineRepair>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__machine___3214EC070C6B7673");

            entity.ToTable("machine_repairs");

            entity.Property(e => e.Id);
            entity.Property(e => e.Corrections)
                .HasColumnType("text")
                .HasColumnName("corrections");
            entity.Property(e => e.RepairDate)
                .HasColumnType("datetime")
                .HasColumnName("repair_date");
            entity.Property(e => e.Result)
                .HasColumnType("text")
                .HasColumnName("result");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.MachineRepairs)
                .HasForeignKey(d => d.ReviewedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_machinerepairs_users");

            entity.HasOne(d => d.Schedule).WithMany(p => p.MachineRepairs)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_machinerepairs_schedule");

            entity.HasOne(d => d.Status).WithMany(p => p.MachineRepairs)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_machinerepairs_status");
        });

        modelBuilder.Entity<RepairSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__repair_s__3214EC075330E993");

            entity.ToTable("repair_schedules");

            entity.Property(e => e.Id);
            entity.Property(e => e.CallDate)
                .HasColumnType("datetime")
                .HasColumnName("call_date");
            entity.Property(e => e.CaseId).HasColumnName("case_id");
            entity.Property(e => e.RepairDate)
                .HasColumnType("datetime")
                .HasColumnName("repair_date");
            entity.Property(e => e.ScheduledBy).HasColumnName("scheduled_by");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Case).WithMany(p => p.RepairSchedules)
                .HasForeignKey(d => d.CaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_repairschedules_cases");

            entity.HasOne(d => d.ScheduledByNavigation).WithMany(p => p.RepairSchedules)
                .HasForeignKey(d => d.ScheduledBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_repairschedules_users");

            entity.HasOne(d => d.Status).WithMany(p => p.RepairSchedules)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_repairschedules_status");
        });

        modelBuilder.Entity<BU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_bu");
            entity.ToTable("bu");
            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ManagerUserId).WithMany(p => p.BUs)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_bus_users");
        });

        modelBuilder.Entity<Lini>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_linies");
            entity.ToTable("linies");
            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.LeaderId).HasColumnName("leader_id");
            entity.Property(e => e.BUId).HasColumnName("bu_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.BULiniId).WithMany(p => p.Lini)
                .HasForeignKey(d => d.BUId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_linies_bu");

            entity.HasOne(d => d.LeaderLiniId).WithMany(p => p.Lini)
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_linies_users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3214EC070F0C6DE6");

            entity.ToTable("roles");

            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusCase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status_c__3214EC07169034D7");

            entity.ToTable("status_cases");

            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusJoborder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status_j__3214EC07B2EC33FC");

            entity.ToTable("status_joborders");

            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusMachinerepair>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status_m__3214EC07B69E8ABA");

            entity.ToTable("status_machinerepairs");

            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusRepairschedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status_r__3214EC0746B9D567");

            entity.ToTable("status_repairschedules");

            entity.Property(e => e.Id);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Table__3214EC0774F4A716");

            entity.ToTable("Table");

            entity.Property(e => e.Id);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC0761329034");

            entity.ToTable("users");

            entity.Property(e => e.Id);
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.VerifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("verified_at");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_users_roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
