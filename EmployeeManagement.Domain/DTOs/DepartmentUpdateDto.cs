namespace EmployeeManagement.Domain.DTOs
{
    public class DepartmentUpdateDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "";
        public string? Description { get; set; }

    }
}
