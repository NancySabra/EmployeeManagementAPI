using System.Data;
using Dapper;
using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.DAL.Interfaces;

namespace EmployeeManagement.DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnection _db;

        public DepartmentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            var result = await _db.QueryAsync<Department>(
                "dbo.usp_Department_GetAll",
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            var result = await _db.QueryFirstOrDefaultAsync<Department>(
                "dbo.usp_Department_GetById",
                new { DepartmentId = id },
                commandType: CommandType.StoredProcedure);

            return result;
        }


        public async Task<Department> InsertAsync(DepartmentCreateDto dto, int userId)
        {
            // 1. Check if name exists (Ensure table name matches your DB: Department or Departments)
            var exists = await _db.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Department WHERE DepartmentName = @Name",
                new { Name = dto.DepartmentName });

            if (exists > 0)
            {
                // This throws the error that Angular will catch and show in the UI
                throw new Exception("A department with this name already exists.");
            }

            // 2. Proceed with insert if it doesn't exist
            return await _db.QuerySingleAsync<Department>(
                "dbo.usp_Department_Insert",
                new
                {
                    dto.DepartmentName,
                    dto.Description,
                    CreatedBy = userId
                },
                commandType: CommandType.StoredProcedure);
        }


        public async Task<Department> UpdateAsync(int id, DepartmentUpdateDto dto, int userId)
        {
            var result = await _db.QuerySingleAsync<Department>(
                "dbo.usp_Department_Update",
                new
                {
                    DepartmentId = id,
                    DepartmentName = dto.DepartmentName,
                    Description = dto.Description,
                    UpdatedBy = userId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Department WHERE DepartmentId = @Id";
            int rowsAffected = await _db.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
