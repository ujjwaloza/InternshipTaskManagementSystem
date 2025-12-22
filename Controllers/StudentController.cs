using InternshipTaskManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InternshipTaskManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _connectionString;

        public StudentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ================= VIEW TASKS =================
        public IActionResult ViewTasks()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            int studentId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<TaskModel> tasks = new();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT t.*
                    FROM Tasks t
                    JOIN Projects p ON t.ProjectId = p.ProjectId
                    WHERE p.StudentId = @StudentId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tasks.Add(new TaskModel
                    {
                        TaskId = Convert.ToInt32(reader["TaskId"]),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ProjectId = Convert.ToInt32(reader["ProjectId"])
                    });
                }
            }

            return View(tasks);
        }

        // ================= UPDATE TASK STATUS =================
        public IActionResult UpdateTaskStatus(int taskId, string status)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            using SqlConnection con = new SqlConnection(_connectionString);
            string query = "UPDATE Tasks SET Status=@Status WHERE TaskId=@TaskId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@TaskId", taskId);

            con.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("ViewTasks");
        }

        // ================= SUBMIT WEEKLY REPORT (GET) =================
        public IActionResult SubmitReport()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ================= SUBMIT WEEKLY REPORT (POST) =================
        [HttpPost]
        public IActionResult SubmitReport(int WeekNumber, string Content)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            int studentId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO WeeklyReports (StudentId, WeekNumber, Content, SubmittedOn)
                    VALUES (@StudentId, @WeekNumber, @Content, GETDATE())";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                cmd.Parameters.AddWithValue("@WeekNumber", WeekNumber);
                cmd.Parameters.AddWithValue("@Content", Content);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Dashboard");
        }

        // ================= VIEW MY REPORTS =================
        public IActionResult MyReports()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            int studentId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<WeeklyReport> reports = new();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT ReportId, StudentId, WeekNumber, Content, SubmittedOn
                    FROM WeeklyReports
                    WHERE StudentId = @StudentId
                    ORDER BY SubmittedOn DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reports.Add(new WeeklyReport
                    {
                        ReportId = Convert.ToInt32(reader["ReportId"]),
                        StudentId = Convert.ToInt32(reader["StudentId"]),
                        WeekNumber = Convert.ToInt32(reader["WeekNumber"]),
                        Content = reader["Content"].ToString(),
                        SubmittedOn = Convert.ToDateTime(reader["SubmittedOn"])
                    });
                }
            }

            return View(reports);
        }
    }
}
