using EmployeeManagement.Domain.DTOs;

namespace EmployeeManagement.DAL.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}