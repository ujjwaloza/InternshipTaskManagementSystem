using System.ComponentModel.DataAnnotations;
using InternshipTaskManagementSystem.Models;
namespace InternshipTaskManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public bool IsFirstLogin { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 🔥 ADD THESE
        public ICollection<Project>? StudentProjects { get; set; }
        public ICollection<Project>? MentorProjects { get; set; }

        public ICollection<TaskModel>? Tasks { get; set; }
        public ICollection<WeeklyReport>? WeeklyReports { get; set; }
    }
}
