using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Domain.DTOs;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.Service.Interfaces;

namespace EmployeeManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Department>>>> GetAll()
        {
            var data = (await _service.GetAllAsync()).ToList();
            return Ok(ApiResponse<List<Department>>.Ok(data, "Departments retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<Department>>> GetById(int id)
        {
            var dep = await _service.GetByIdAsync(id);
            if (dep == null)
                return NotFound(ApiResponse<Department>.Fail("Department not found."));

            return Ok(ApiResponse<Department>.Ok(dep, "Department retrieved successfully."));
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Department>>> Create([FromBody] DepartmentCreateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                int userId = int.Parse(userIdClaim ?? "0");

                var created = await _service.InsertAsync(dto, userId);
                return Ok(ApiResponse<Department>.Ok(created, "Created successfully."));
            }
            catch (Exception ex)
            {
                // This sends the "A department with this name already exists" message to the UI
                return BadRequest(ApiResponse<Department>.Fail(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<Department>>> Update(int id, [FromBody] DepartmentUpdateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim))
                    return Unauthorized(ApiResponse<Department>.Fail("Invalid token."));

                int userId = int.Parse(userIdClaim);

                // FIX: Added 'userId' as the third argument here
                var updated = await _service.UpdateAsync(id, dto, userId);

                return Ok(ApiResponse<Department>.Ok(updated, "Department updated successfully."));
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new
                {
                    success = false,
                    message = message
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                // FIX: Your Repository returns a 'bool', ensure Service does too.
                bool deleted = await _service.DeleteAsync(id);
                if (!deleted) return BadRequest(ApiResponse<object>.Fail("Delete failed."));

                return Ok(ApiResponse<object>.Ok(null, "Deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}