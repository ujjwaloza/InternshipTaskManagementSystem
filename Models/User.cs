using System;//needed for datetime without this it gives error
using System.ComponentModel.DataAnnotations;//used for data annotations like [Key], [Required], etc.

namespace InternshipTaskManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin, Student, Mentor

        public DateTime CreatedAt { get; set; }=DateTime.Now;
    }
}
//how ef will use this user class: DbSet in applicationDbContext.cs 
//public DbSet<User> Users { get; set; } tell ef core that create a table named Users Model