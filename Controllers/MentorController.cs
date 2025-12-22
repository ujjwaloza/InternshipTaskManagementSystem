using InternshipTaskManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InternshipTaskManagementSystem.Controllers
{
    public class MentorController : Controller
    {
        private readonly string _connectionString;

        public MentorController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Mentor")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ================= VIEW ASSIGNED STUDENTS =================
        public IActionResult AssignedStudents()
        {
            int mentorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<User> students = new();

            using SqlConnection con = new(_connectionString);
            string query = @"
                SELECT DISTINCT u.UserId, u.FullName, u.Email, u.Role
                FROM Projects p
                JOIN Users u ON p.StudentId = u.UserId
                WHERE p.MentorId = @MentorId";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@MentorId", mentorId);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                students.Add(new User
                {
                    UserId = (int)reader["UserId"],
                    FullName = reader["FullName"].ToString(),
                    Email = reader["Email"].ToString(),
                    Role = reader["Role"].ToString()
                });
            }

            return View(students);
        }

        // ================= STUDENT PROJECTS =================
        public IActionResult StudentProjects(int studentId)
        {
            List<Project> projects = new();

            using SqlConnection con = new(_connectionString);
            string query = "SELECT * FROM Projects WHERE StudentId = @StudentId";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                projects.Add(new Project
                {
                    ProjectId = (int)reader["ProjectId"],
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString()
                });
            }

            return View(projects);
        }

        // ================= STUDENT TASKS =================
        public IActionResult StudentTasks(int studentId)
        {
            List<TaskModel> tasks = new();

            using SqlConnection con = new(_connectionString);
            string query = @"
                SELECT t.*
                FROM Tasks t
                JOIN Projects p ON t.ProjectId = p.ProjectId
                WHERE p.StudentId = @StudentId";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

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

            return View(tasks);
        }

        // ================= ASSIGN TASK (GET) =================
        public IActionResult AssignTask(int projectId)
        {
            ViewBag.ProjectId = projectId;
            return View(new TaskModel { ProjectId = projectId });
        }

        // ================= ASSIGN TASK (POST) =================
        [HttpPost]
        public IActionResult AssignTask(TaskModel task)
        {
            if (task.ProjectId <= 0)
            {
                ModelState.AddModelError("", "Invalid Project");
                return View(task);
            }

            using SqlConnection con = new(_connectionString);

            // ✅ VALIDATE PROJECT EXISTS
            SqlCommand check = new(
                "SELECT COUNT(*) FROM Projects WHERE ProjectId=@ProjectId", con);
            check.Parameters.AddWithValue("@ProjectId", task.ProjectId);

            con.Open();
            int exists = (int)check.ExecuteScalar();

            if (exists == 0)
            {
                ModelState.AddModelError("", "Project does not exist");
                return View(task);
            }

            SqlCommand cmd = new(@"
                INSERT INTO Tasks (Title, Description, Status, ProjectId)
                VALUES (@Title, @Description, 'ToDo', @ProjectId)", con);

            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description ?? "");
            cmd.Parameters.AddWithValue("@ProjectId", task.ProjectId);

            cmd.ExecuteNonQuery();

            return RedirectToAction("Dashboard");
        }

        // ================= WEEKLY REPORTS =================
        public IActionResult WeeklyReports()
        {
            int mentorId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<WeeklyReport> reports = new();

            using SqlConnection con = new(_connectionString);
            string query = @"
                SELECT wr.*
                FROM WeeklyReports wr
                JOIN Projects p ON wr.StudentId = p.StudentId
                WHERE p.MentorId = @MentorId";

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
                    SubmittedOn = (DateTime)reader["SubmittedOn"]
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
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
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
            }

            return RedirectToAction("WeeklyReports");
        }

    }
}
