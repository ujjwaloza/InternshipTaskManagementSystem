using InternshipTaskManagementSystem.Models;
public class ChangePasswordDto
{
    public int UserId { get; set; }

    public string NewPassword { get; set; }

    public string ConfirmPassword { get; set; }
}