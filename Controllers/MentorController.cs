using InternshipTaskManagementSystem.Data;
using InternshipTaskManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InternshipTaskManagementSystem.Controllers
{
    public class MentorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public MentorController(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Mentor")
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult AssignedStudents()
        {
            int mentorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<Project> data = new();

            using SqlConnection con = new(_connectionString);

            string query = @"
                SELECT p.ProjectId, p.Title, p.Description,
                       u.UserId, u.FullName, u.Email
                FROM Projects p
                JOIN Users u ON p.StudentId = u.UserId
                WHERE p.MentorId = @MentorId";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@MentorId", mentorId);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                data.Add(new Project
                {
                    ProjectId = (int)reader["ProjectId"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),

                    Student = new User
                    {
                        UserId = (int)reader["UserId"],
                        FullName = reader["FullName"].ToString(),
                        Email = reader["Email"].ToString()
                    }
                });
            }

            return View(data);
        }

        public IActionResult StudentTasks(int projectId)
        {
            int mentorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            List<TaskModel> tasks = new();

            using SqlConnection con = new(_connectionString);

            string query = @"
                SELECT * FROM Tasks
                WHERE ProjectId = @ProjectId AND MentorId = @MentorId";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ProjectId", projectId);
            cmd.Parameters.AddWithValue("@MentorId", mentorId);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(new TaskModel
                {
                    TaskId = (int)reader["TaskId"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    Status = reader["Status"].ToString(),
                    ProjectId = (int)reader["ProjectId"]
                });
            }

            ViewBag.ProjectId = projectId;

            return View(tasks);
        }

        public IActionResult AssignTask(int projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        [HttpPost]
        public IActionResult AssignTask(TaskModel task)
        {
            var mentorIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(mentorIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            int mentorId = Convert.ToInt32(mentorIdStr);

            using SqlConnection con = new(_connectionString);

            SqlCommand getStudent = new(
                "SELECT StudentId FROM Projects WHERE ProjectId=@ProjectId", con);
            getStudent.Parameters.AddWithValue("@ProjectId", task.ProjectId);

            con.Open();

            object result = getStudent.ExecuteScalar();

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid Project");
                return View(task);
            }

            int studentId = (int)result;

            SqlCommand cmd = new(@"
                INSERT INTO Tasks (Title, Description, Status, ProjectId, StudentId, MentorId, CreatedAt, DueDate)
                VALUES (@Title, @Description, 'Pending', @ProjectId, @StudentId, @MentorId, GETDATE(), @DueDate)", con);

            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description ?? "");
            cmd.Parameters.AddWithValue("@ProjectId", task.ProjectId);
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@MentorId", mentorId);
            cmd.Parameters.AddWithValue("@DueDate",
    task.DueDate ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            return RedirectToAction("StudentTasks", new { projectId = task.ProjectId });
        }

        public IActionResult WeeklyReports()
        {
            int mentorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<WeeklyReport> reports = new();

            using SqlConnection con = new(_connectionString);

            string query = @"
                SELECT wr.*, u.FullName
                FROM WeeklyReports wr
                JOIN Users u ON wr.StudentId = u.UserId
                JOIN Projects p ON wr.ProjectId = p.ProjectId
                WHERE p.MentorId = @MentorId
                ORDER BY wr.SubmittedOn DESC";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@MentorId", mentorId);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                reports.Add(new WeeklyReport
                {
                    ReportId = (int)reader["ReportId"],
                    StudentId = (int)reader["StudentId"],
                    WeekNumber = (int)reader["WeekNumber"],
                    Content = reader["Content"].ToString(),
                    SubmittedOn = (DateTime)reader["SubmittedOn"],
                    Student = new User
                    {
                        FullName = reader["FullName"].ToString()
                    }
                });
            }

            return View(reports);
        }

        public IActionResult Feedback(int reportId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Mentor")
                return RedirectToAction("Login", "Account");

            ViewBag.ReportId = reportId;
            return View();
        }

        [HttpPost]
        public IActionResult Feedback(int ReportId, string Feedback, string Status)
        {
            using SqlConnection con = new(_connectionString);

            string query = @"
                UPDATE WeeklyReports
                SET MentorFeedback = @Feedback, Status = @Status
                WHERE ReportId = @ReportId";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Feedback", Feedback);
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.AddWithValue("@ReportId", ReportId);

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("WeeklyReports");
        }
        public IActionResult StudentProgress()
        {
            int mentorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            var data = _context.Projects
                .Where(p => p.MentorId == mentorId)
                .SelectMany(p => p.Tasks.Select(t => new
                {
                    StudentName = p.Student.FullName,
                    ProjectTitle = p.Title,
                    TaskTitle = t.Title,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    StudentId = p.StudentId,
                    ProjectId = p.ProjectId
                }))
                .ToList();

            return View(data);
        }
        public IActionResult Reply(int studentId)
        {
            ViewBag.StudentId = studentId;
            return View();
        }

        [HttpPost]
        public IActionResult Reply(int studentId, string message)
        {
           
            TempData["Msg"] = "Reply sent to student!";
            return RedirectToAction("StudentProgress");
        }
    }
}
