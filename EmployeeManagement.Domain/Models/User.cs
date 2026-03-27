namespace EmployeeManagementAPI.Models // Make sure this matches your project name
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // This is for your 'admin123'
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}