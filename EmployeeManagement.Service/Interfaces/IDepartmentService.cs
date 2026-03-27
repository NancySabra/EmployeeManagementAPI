using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Service.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);

        
        Task<Department> InsertAsync(DepartmentCreateDto dto, int userId);
        Task<Department> UpdateAsync(int id, DepartmentUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id);
    }
}