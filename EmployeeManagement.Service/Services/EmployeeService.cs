using EmployeeManagement.DAL.Interfaces;
using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Service.Interfaces;

namespace EmployeeManagement.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        public EmployeeService(IEmployeeRepository repo) => _repo = repo;

       public async Task<Employee> InsertAsync(EmployeeCreateDto dto, int userId)
{
    return await _repo.InsertAsync(dto, userId);
}
        public async Task<Employee> UpdateAsync(int id, EmployeeUpdateDto dto, int userId) => await _repo.UpdateAsync(id, dto, userId);
        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<IEnumerable<Employee>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<Employee?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId) => await _repo.GetByDepartmentAsync(departmentId);
    }
}