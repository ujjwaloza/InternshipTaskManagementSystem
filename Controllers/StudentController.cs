using InternshipTaskManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            return View();
        }
        public IActionResult StartTask(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Tasks SET Status='In Progress' WHERE TaskId=@TaskId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TaskId", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("ViewTasks");
        }
        public IActionResult CompleteTask(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Tasks SET Status='Completed' WHERE TaskId=@TaskId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TaskId", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("ViewTasks");
        }


        public IActionResult ViewTasks()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            int studentId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            List<TaskModel> tasks = new();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT TaskId, Title, Description, Status, ProjectId
            FROM Tasks
            WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", studentId);

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



        [HttpPost]
        public IActionResult UpdateTaskStatus(int taskId, string status)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            int studentId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE Tasks
            SET Status = @Status
            WHERE TaskId = @TaskId AND UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@TaskId", taskId);
                cmd.Parameters.AddWithValue("@UserId", studentId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("ViewTasks");
        }


        public IActionResult SubmitReport()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
                return RedirectToAction("Login", "Account");

            return View();
        }

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
