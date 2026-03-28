using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Service.Interfaces;
using EmployeeManagement.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // FIX 1: Path changed from "ByDepartment/{id}" to "department/{id}" 
        // This stops the 404 error when filtering departments.
        [HttpGet("department/{departmentId:int}")]
        public async Task<ActionResult<ApiResponse<List<Employee>>>> GetByDepartment(int departmentId)
        {
            try
            {
                var data = await _service.GetByDepartmentAsync(departmentId);
                return Ok(ApiResponse<List<Employee>>.Ok(data.ToList(), "Employees retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Employee>>.Fail(ex.Message));
            }
        }

        // FIX 2: Added the missing Search endpoint.
        // This fixes the 404 error when searching by name, email, or phone.
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<Employee>>>> Search([FromQuery] string searchTerm)
        {
            try
            {
                // We need to ensure your IEmployeeService has a SearchAsync method.
                // If it doesn't, you can temporarily filter the GetAll results here:
                var allEmployees = await _service.GetAllAsync();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Ok(ApiResponse<List<Employee>>.Ok(allEmployees.ToList(), "All employees retrieved."));
                }

                var filtered = allEmployees.Where(e =>
                    (e.FirstName != null && e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (e.LastName != null && e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (e.Email != null && e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (e.PhoneNumber != null && e.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                return Ok(ApiResponse<List<Employee>>.Ok(filtered, "Search results retrieved."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Employee>>.Fail(ex.Message));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Employee>>>> GetAll()
        {
            try
            {
                var data = await _service.GetAllAsync();
                return Ok(ApiResponse<List<Employee>>.Ok(data.ToList(), "Employees retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Employee>>.Fail(ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<Employee>>> GetById(int id)
        {
            try
            {
                var emp = await _service.GetByIdAsync(id);
                if (emp == null)
                    return NotFound(ApiResponse<Employee>.Fail("Employee not found."));

                return Ok(ApiResponse<Employee>.Ok(emp, "Employee retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Employee>.Fail(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Employee>>> Create([FromBody] EmployeeCreateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                int userId = int.Parse(userIdClaim ?? "1"); 

                var created = await _service.InsertAsync(dto, userId);
                return Ok(ApiResponse<Employee>.Ok(created, "Employee created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Employee>.Fail(ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                int userId = int.Parse(userIdClaim ?? "1");

                var updated = await _service.UpdateAsync(id, dto, userId);
                return Ok(ApiResponse<Employee>.Ok(updated, "Success"));

            }
            catch (Exception ex)
            {
                // Check if the error message contains your SQL custom error text
                if (ex.Message.Contains("Employee email must be unique") || ex.InnerException?.Message.Contains("unique") == true)
                {
                    return BadRequest(new { success = false, message = "Email already exists" });
                }

                // Generic fallback for other errors
                return StatusCode(500, new { success = false, message = "An internal error occurred" });
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim))
                    return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

                int userId = int.Parse(userIdClaim);
                var msg = await _service.DeleteAsync(id);

                return Ok(ApiResponse<object>.Ok(null!));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}