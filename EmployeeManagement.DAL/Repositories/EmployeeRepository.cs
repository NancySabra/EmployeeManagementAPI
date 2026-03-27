using Dapper;
using EmployeeManagement.DAL.Interfaces;
using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using System.Data;

namespace EmployeeManagement.DAL.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _db;

        public EmployeeRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.QueryAsync<Employee>(
                "dbo.usp_Employee_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        {
            
            return await _db.QueryAsync<Employee>(
                "dbo.usp_Employee_GetByDepartment",
                new { DepartmentId = departmentId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Employee>(
                "dbo.usp_Employee_GetById",
                new { EmployeeId = id },
                commandType: CommandType.StoredProcedure);
        }

     

        public async Task<Employee> InsertAsync(EmployeeCreateDto dto, int userId)
        {
            var exists = await _db.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Employee WHERE Email = @Email",
                new { Email = dto.Email });

            if (exists > 0)
            {
                throw new Exception("Employee already exists");
            }

            return await _db.QuerySingleAsync<Employee>(
                "dbo.usp_Employee_Insert",
                new
                {
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.PhoneNumber,
                    dto.HireDate,
                    dto.Salary,
                    dto.DepartmentId,
                    CreatedBy = userId,
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Employee> UpdateAsync(int id, EmployeeUpdateDto dto, int userId)
        {
            return await _db.QuerySingleAsync<Employee>(
                "dbo.usp_Employee_Update",
                new
                {
                    EmployeeId = id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.PhoneNumber,
                    dto.HireDate,
                    dto.Salary,
                    dto.DepartmentId,
                    UpdatedBy = userId // Added for audit consistency
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Employee WHERE EmployeeId = @Id";
            int rowsAffected = await _db.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}