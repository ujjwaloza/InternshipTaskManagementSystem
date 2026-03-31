using System.ComponentModel.DataAnnotations;
using InternshipTaskManagementSystem.Models;
namespace InternshipTaskManagementSystem.Models
{
    public class TaskModel
    {
        [Key]
        public int TaskId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public int ProjectId { get; set; }

        public int? StudentId { get; set; }
        public int? MentorId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Project? project { get; set; }

        // OPTIONAL (remove if not needed)
        // public User? Student { get; set; }
        // public User? Mentor { get; set; }
    }
}
