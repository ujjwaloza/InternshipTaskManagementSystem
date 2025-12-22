using System.ComponentModel.DataAnnotations;
namespace InternshipTaskManagementSystem.Models

{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public int? StudentId { get; set; }

        public int? MentorId   { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public User? Student { get; set; }
        public User? Mentor { get; set; }
       // public ICollection<TaskModel> Tasks { get; set; }
    }
}
