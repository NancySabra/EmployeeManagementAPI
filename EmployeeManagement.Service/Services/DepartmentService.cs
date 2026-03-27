using EmployeeManagement.DAL.Interfaces;
using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Service.Interfaces;

namespace EmployeeManagement.Service.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentService(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        // FIX 1: Add 'int userId' to match the Interface and pass it to Repo
        public async Task<Department> InsertAsync(DepartmentCreateDto dto, int userId)
        {
            return await _repo.InsertAsync(dto, userId);
        }

        // FIX 2: Add 'int userId' to match the Interface and pass it to Repo
        public async Task<Department> UpdateAsync(int id, DepartmentUpdateDto dto, int userId)
        {
            return await _repo.UpdateAsync(id, dto, userId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}