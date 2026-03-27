using EmployeeManagementAPI.Models;

namespace EmployeeManagement.DAL.Interfaces // Use your project's namespace
{
    public interface IUserRepository
    {
        Task<User> GetByUsername(string username);
    }
}