using Microsoft.AspNetCore.Mvc;
using ProductsShopWebAPI.Models;
using ProductsShopWebAPI.Services;

namespace ProductsShopWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active employees
        /// </summary>
        /// <returns>List of employees</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Getting all employees");
                var employees = await _employeeService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all employees");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            try
            {
                _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                var employee = await _employeeService.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting employee with ID: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="request">Employee creation request</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new employee: {FirstName} {LastName}", request.FirstName, request.LastName);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Basic validation
                if (string.IsNullOrWhiteSpace(request.FirstName) ||
                    string.IsNullOrWhiteSpace(request.LastName) ||
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("FirstName, LastName, and Email are required");
                }

                if (request.Salary < 0)
                {
                    return BadRequest("Salary cannot be negative");
                }

                var employee = await _employeeService.CreateEmployeeAsync(request);

                return CreatedAtAction(
                    nameof(GetEmployeeById),
                    new { id = employee.Id },
                    employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="request">Employee update request</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Basic validation
                if (string.IsNullOrWhiteSpace(request.FirstName) ||
                    string.IsNullOrWhiteSpace(request.LastName) ||
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("FirstName, LastName, and Email are required");
                }

                if (request.Salary < 0)
                {
                    return BadRequest("Salary cannot be negative");
                }

                var employee = await _employeeService.UpdateEmployeeAsync(id, request);

                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete an employee (soft delete)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            try
            {
                _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                var result = await _employeeService.DeleteEmployeeAsync(id);

                if (!result)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get employees by department
        /// </summary>
        /// <param name="department">Department name</param>
        /// <returns>List of employees in the department</returns>
        [HttpGet("department/{department}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByDepartment(string department)
        {
            try
            {
                _logger.LogInformation("Getting employees for department: {Department}", department);

                if (string.IsNullOrWhiteSpace(department))
                {
                    return BadRequest("Department name is required");
                }

                var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting employees for department: {Department}", department);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}