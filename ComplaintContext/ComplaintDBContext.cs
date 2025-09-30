using Microsoft.EntityFrameworkCore;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.ComplaintContext
{
    public class ComplaintDBContext : DbContext
    {
        public ComplaintDBContext(DbContextOptions<ComplaintDBContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Official> Officials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintAttachment> ComplaintAttachments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<WorkUpdate> WorkUpdates { get; set; }
        public DbSet<OfficialDepartment> OfficialDepartments { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // WORKER (One-to-One with User)
            modelBuilder.Entity<Worker>()
                .HasKey(w => w.WorkerId);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.User)
                .WithOne(u => u.WorkerProfile)
                .HasForeignKey<Worker>(w => w.UserId);

            // DEPARTMENT
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.DepartmentName)
                .IsUnique();

             // CATEGORY → DEPARTMENT (⚡ FIXED: no cascade)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Categories)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); 

            // COMPLAINT
            modelBuilder.Entity<Complaint>()
                .HasIndex(c => c.TicketNo)
                .IsUnique();

            modelBuilder.Entity<Complaint>()
                .Property(c => c.CurrentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Citizen)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Complaints)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.SubCategory)
                .WithMany(sub => sub.Complaints)
                .HasForeignKey(c => c.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);   

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Complaints)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.AssignedWorker)
                .WithMany(w => w.AssignedComplaints)
                .HasForeignKey(c => c.AssignedWorkerId)
                .OnDelete(DeleteBehavior.Restrict);   // FIXED


            // COMPLAINT ATTACHMENTS
            modelBuilder.Entity<ComplaintAttachment>()
                .HasOne(a => a.Complaint)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.ComplaintId);

            modelBuilder.Entity<ComplaintAttachment>()
                .HasOne(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // FEEDBACK
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Complaint)
                .WithOne(c => c.Feedback)
                .HasForeignKey<Feedback>(f => f.ComplaintId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Citizen)
                .WithMany()
                .HasForeignKey(f => f.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            // AUDIT LOGS
            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            //workUpdates
            modelBuilder.Entity<WorkUpdate>()
                .HasOne(wu => wu.Complaint)
                .WithMany(c => c.WorkUpdates)
                .HasForeignKey(wu => wu.ComplaintId);

            //official departments
            modelBuilder.Entity<OfficialDepartment>()
                .HasKey(od => new { od.OfficialId, od.DepartmentId });

            modelBuilder.Entity<OfficialDepartment>()
                .HasOne(od => od.Official)
                .WithMany(o => o.OfficialDepartments)
                .HasForeignKey(od => od.OfficialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OfficialDepartment>()
                .HasOne(od => od.Department)
                .WithMany(d => d.OfficialDepartments)
                .HasForeignKey(od => od.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
