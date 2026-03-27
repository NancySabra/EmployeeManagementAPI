using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    // Both files must look like this:
    Task<Department> InsertAsync(DepartmentCreateDto dto, int userId);
    Task<Department> UpdateAsync(int id, DepartmentUpdateDto dto, int userId);
    Task<bool> DeleteAsync(int id);
}