using Microsoft.EntityFrameworkCore;
using InternshipTaskManagementSystem.Models;
using AspNetCoreGeneratedDocument;
namespace InternshipTaskManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
       public DbSet<WeeklyReport> WeeklyReports{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project → Student
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(u => u.StudentProjects)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project → Mentor
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Mentor)
                .WithMany(u => u.MentorProjects)
                .HasForeignKey(p => p.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

        

            // WeeklyReport → Project
            modelBuilder.Entity<WeeklyReport>()
                .HasOne(w => w.Project)
                .WithMany(p => p.WeeklyReports)
                .HasForeignKey(w => w.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WeeklyReport>()
        .HasOne(w => w.Student)
        .WithMany(u => u.WeeklyReports)
        .HasForeignKey(w => w.StudentId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
       .HasOne<User>()
       .WithMany()
       .HasForeignKey(t => t.StudentId)
       .OnDelete(DeleteBehavior.Restrict);

            // Task → Mentor
            modelBuilder.Entity<TaskModel>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
    
 }
