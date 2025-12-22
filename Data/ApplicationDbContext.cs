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
    }
}
