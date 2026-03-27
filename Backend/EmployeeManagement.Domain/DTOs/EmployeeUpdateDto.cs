namespace EmployeeManagement.Domain.DTOs
{
    public class EmployeeUpdateDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
       
    }
}
