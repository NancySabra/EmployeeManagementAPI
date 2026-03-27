using EmployeeManagement.DAL.Interfaces;
using EmployeeManagementAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using EmployeeManagement.DAL.Interfaces;

namespace EmployeeManagementAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> GetByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("usp_User_GetByUsername", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", username);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            UserId = (int)reader["UserId"],
                            Username = reader["Username"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Email = reader["Email"].ToString(),
                            Role = reader["Role"].ToString(),
                            IsActive = (bool)reader["IsActive"]
                        };
                    }
                }
            }
            return null; // Return null if admin/manager not found
        }
    }
}