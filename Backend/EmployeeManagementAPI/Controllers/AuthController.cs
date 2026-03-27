using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.DAL.Interfaces;
using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // 1. Declare the tools
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;

        // 2. The Constructor "Injects" them so the errors disappear
        public AuthController(IUserRepository userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        // POST /api/Auth/Login
        [AllowAnonymous]
        [HttpPost("Login")]
public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
{
    // 1. Look for the user in the DB using the procedure we created
    var user = await _userRepository.GetByUsername(loginRequest.Username);

    // 2. Check if user exists and password matches
    // NOTE: In a real app, use BCrypt to check hashed passwords!
    if (user == null || user.PasswordHash != loginRequest.Password) 
    {
        return Unauthorized(new { message = "Invalid username or password" });
    }

    // 3. Use the AuthService to create the token
    var token = _authService.GenerateToken(user);

    // 4. Send it back to Angular
    return Ok(new { 
        success = true, 
        data = new { token = token, username = user.Username } 
    });
}

    }
}