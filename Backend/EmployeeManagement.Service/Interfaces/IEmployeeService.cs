using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Service.Interfaces { 
    public interface IEmployeeService 
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
        Task<Employee> InsertAsync(EmployeeCreateDto dto, int userId);
        Task<Employee> UpdateAsync(int id, EmployeeUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id);
    }
}