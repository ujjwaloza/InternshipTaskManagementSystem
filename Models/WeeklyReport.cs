using InternshipTaskManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
namespace InternshipTaskManagementSystem.Models
{
    public class WeeklyReport
    {
        [Key]
        public int ReportId { get; set; }

        public int? StudentId { get; set; }
        public int? MentorId { get; set; }
        public int ProjectId { get; set; }

        public int WeekNumber { get; set; }
        public string? Content { get; set; }
        public DateTime SubmittedOn { get; set; }

        public User? Student { get; set; }
       // public User? Mentor { get; set; }
        public Project Project { get; set; }
    }
}

    