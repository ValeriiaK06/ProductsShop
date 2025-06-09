using Microsoft.AspNetCore.Mvc;
using ProductsShopWebAPI.Models;
using ProductsShopWebAPI.Services;

namespace ProductsShopWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all departments with full details
        /// </summary>
        /// <returns>List of departments with full information</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
        {
            try
            {
                _logger.LogInformation("Getting all departments");
                var departments = await _departmentService.GetAllDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all departments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get department by ID with full details
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            try
            {
                _logger.LogInformation("Getting department with ID: {DepartmentId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid department ID");
                }

                var department = await _departmentService.GetDepartmentByIdAsync(id);

                if (department == null)
                {
                    return NotFound($"Department with ID {id} not found");
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department with ID: {DepartmentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing department
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="request">Department update request</param>
        /// <returns>Updated department</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                _logger.LogInformation("Updating department with ID: {DepartmentId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid department ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Базовая валидация
                if (string.IsNullOrWhiteSpace(request.DepartmentName))
                {
                    return BadRequest("Department name is required");
                }

                if (request.LocationRow < 0)
                {
                    return BadRequest("Location row must be non-negative");
                }

                if (request.Counters < 0)
                {
                    return BadRequest("Counters must be non-negative");
                }

                if (request.StartTime >= request.EndTime)
                {
                    return BadRequest("Start time must be earlier than end time");
                }

                var department = await _departmentService.UpdateDepartmentAsync(id, request);

                if (department == null)
                {
                    return NotFound($"Department with ID {id} not found");
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department with ID: {DepartmentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}