using System.ComponentModel.DataAnnotations;
namespace InternshipTaskManagementSystem.Models
{
    public class TaskModel
    {
        [Key]
            public int TaskId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
            public int ProjectId { get; set; }
        public int StudentId { get; set; }   // 👈 who will do task
        public int MentorId { get; set; }
        public DateTime CreatedAt { get; set; }
      
        public Project project { get; set; }


    }
}
