//using Microsoft.AspNetCore.Mvc;
//using ProductsShopWebAPI.Models;
//using ProductsShopWebAPI.Services;

//namespace ProductsShopWebAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class EmployeesController : ControllerBase
//    {
//        private readonly IEmployeeService _employeeService;
//        private readonly ILogger<EmployeesController> _logger;

//        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
//        {
//            _employeeService = employeeService;
//            _logger = logger;
//        }

//        /// <summary>
//        /// Get all active employees
//        /// </summary>
//        /// <returns>List of employees</returns>
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
//        {
//            try
//            {
//                _logger.LogInformation("Getting all employees");
//                var employees = await _employeeService.GetAllEmployeesAsync();
//                return Ok(employees);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting all employees");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get employee by ID
//        /// </summary>
//        /// <param name="id">Employee ID</param>
//        /// <returns>Employee details</returns>
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
//        {
//            try
//            {
//                _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);

//                if (id <= 0)
//                {
//                    return BadRequest("Invalid employee ID");
//                }

//                var employee = await _employeeService.GetEmployeeByIdAsync(id);

//                if (employee == null)
//                {
//                    return NotFound($"Employee with ID {id} not found");
//                }

//                return Ok(employee);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting employee with ID: {EmployeeId}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Create a new employee
//        /// </summary>
//        /// <param name="request">Employee creation request</param>
//        /// <returns>Created employee</returns>
//        [HttpPost]
//        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] CreateEmployeeRequest request)
//        {
//            try
//            {
//                _logger.LogInformation("Creating new employee: {FirstName} {LastName}", request.FirstName, request.LastName);

//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                // Basic validation
//                if (string.IsNullOrWhiteSpace(request.FirstName) ||
//                    string.IsNullOrWhiteSpace(request.LastName) ||
//                    string.IsNullOrWhiteSpace(request.Email))
//                {
//                    return BadRequest("FirstName, LastName, and Email are required");
//                }

//                if (request.Salary < 0)
//                {
//                    return BadRequest("Salary cannot be negative");
//                }

//                var employee = await _employeeService.CreateEmployeeAsync(request);

//                return CreatedAtAction(
//                    nameof(GetEmployeeById),
//                    new { id = employee.Id },
//                    employee);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while creating employee");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Update an existing employee
//        /// </summary>
//        /// <param name="id">Employee ID</param>
//        /// <param name="request">Employee update request</param>
//        /// <returns>Updated employee</returns>
//        [HttpPut("{id}")]
//        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
//        {
//            try
//            {
//                _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

//                if (id <= 0)
//                {
//                    return BadRequest("Invalid employee ID");
//                }

//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                // Basic validation
//                if (string.IsNullOrWhiteSpace(request.FirstName) ||
//                    string.IsNullOrWhiteSpace(request.LastName) ||
//                    string.IsNullOrWhiteSpace(request.Email))
//                {
//                    return BadRequest("FirstName, LastName, and Email are required");
//                }

//                if (request.Salary < 0)
//                {
//                    return BadRequest("Salary cannot be negative");
//                }

//                var employee = await _employeeService.UpdateEmployeeAsync(id, request);

//                if (employee == null)
//                {
//                    return NotFound($"Employee with ID {id} not found");
//                }

//                return Ok(employee);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Delete an employee (soft delete)
//        /// </summary>
//        /// <param name="id">Employee ID</param>
//        /// <returns>Success status</returns>
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> DeleteEmployee(int id)
//        {
//            try
//            {
//                _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

//                if (id <= 0)
//                {
//                    return BadRequest("Invalid employee ID");
//                }

//                var result = await _employeeService.DeleteEmployeeAsync(id);

//                if (!result)
//                {
//                    return NotFound($"Employee with ID {id} not found");
//                }

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get employees by department
//        /// </summary>
//        /// <param name="department">Department name</param>
//        /// <returns>List of employees in the department</returns>
//        [HttpGet("department/{department}")]
//        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByDepartment(string department)
//        {
//            try
//            {
//                _logger.LogInformation("Getting employees for department: {Department}", department);

//                if (string.IsNullOrWhiteSpace(department))
//                {
//                    return BadRequest("Department name is required");
//                }

//                var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
//                return Ok(employees);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting employees for department: {Department}", department);
//                return StatusCode(500, "Internal server error");
//            }
//        }
//    }
//}

//using Microsoft.AspNetCore.Mvc;
//using ProductsShopWebAPI.Models;
//using ProductsShopWebAPI.Services;

//namespace ProductsShopWebAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class EmployeesController : ControllerBase
//    {
//        private readonly IEmployeeService _employeeService;
//        private readonly ILogger<EmployeesController> _logger;

//        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
//        {
//            _employeeService = employeeService;
//            _logger = logger;
//        }

//        /// <summary>
//        /// Get all employees with their department, position, and gender information
//        /// </summary>
//        /// <returns>List of employees with full details</returns>
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
//        {
//            try
//            {
//                _logger.LogInformation("Getting all employees");
//                var employees = await _employeeService.GetAllEmployeesAsync();
//                return Ok(employees);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting all employees");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get employee by ID with full details
//        /// </summary>
//        /// <param name="id">Employee ID</param>
//        /// <returns>Employee details with related information</returns>
//        [HttpGet("{id}")]
//        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
//        {
//            try
//            {
//                _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);

//                if (id <= 0)
//                {
//                    return BadRequest("Invalid employee ID");
//                }

//                var employee = await _employeeService.GetEmployeeByIdAsync(id);

//                if (employee == null)
//                {
//                    return NotFound($"Employee with ID {id} not found");
//                }

//                return Ok(employee);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting employee with ID: {EmployeeId}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Create a new employee
//        /// </summary>
//        /// <param name="request">Employee creation request</param>
//        /// <returns>Created employee with full details</returns>
//        [HttpPost]
//        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
//        {
//            try
//            {
//                _logger.LogInformation("Creating new employee: {FirstName} {LastName}", request.Name, request.LastName);

//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                // Validate required fields
//                if (string.IsNullOrWhiteSpace(request.Name) ||
//                    string.IsNullOrWhiteSpace(request.LastName) ||
//                    string.IsNullOrWhiteSpace(request.PhoneNumber))
//                {
//                    return BadRequest("Name, LastName, and PhoneNumber are required");
//                }

//                // Validate age (must be at least 18)
//                var age = DateTime.Now.Year - request.DateOfBirth.Year;
//                if (DateTime.Now.DayOfYear < request.DateOfBirth.DayOfYear) age--;

//                if (age < 18)
//                {
//                    return BadRequest("Employee must be at least 18 years old");
//                }

//                // Validate experience vs age
//                if (request.ExperienceYear > age - 18)
//                {
//                    return BadRequest("Work experience cannot exceed employee age minus 18 years");
//                }

//                // Validate phone number format (Ukrainian format)
//                if (!request.PhoneNumber.StartsWith("380") || request.PhoneNumber.Length != 12)
//                {
//                    return BadRequest("Phone number must start with 380 and be 12 digits long");
//                }

//                var employee = await _employeeService.CreateEmployeeAsync(request);

//                return CreatedAtAction(
//                    nameof(GetEmployeeById),
//                    new { id = employee.Id },
//                    employee);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while creating employee");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Update an existing employee
//        /// </summary>
//        /// <param name="id">Employee ID</param>
//        /// <param name="request">Employee update request</param>
//        /// <returns>Updated employee with full details</returns>
//        [HttpPut("{id}")]
//        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
//        {
//            try
//            {
//                _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

//                if (id <= 0)
//                {
//                    return BadRequest("Invalid employee ID");
//                }

//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                // Same validation as in Create
//                if (string.IsNullOrWhiteSpace(request.Name) ||
//                    string.IsNullOrWhiteSpace(request.LastName) ||
//                    string.IsNullOrWhiteSpace(request.PhoneNumber))
//                {
//                    return BadRequest("Name, LastName, and PhoneNumber are required");
//                }

//                var age = DateTime.Now.Year - request.DateOfBirth.Year;
//                if (DateTime.Now.DayOfYear < request.DateOfBirth.DayOfYear) age--;

//                if (age < 18)
//                {
//                    return BadRequest("Employee must be at least 18 years old");
//                }

//                if (request.ExperienceYear > age - 18)
//                {
//                    return BadRequest("Work experience cannot exceed employee age minus 18 years");
//                }

//                if (!request.PhoneNumber.StartsWith("380") || request.PhoneNumber.Length != 12)
//                {
//                    return BadRequest("Phone number must start with 380 and be 12 digits long");
//                }

//                var employee = await _employeeService.UpdateEmployeeAsync(id, request);

//                if (employee == null)
//                {
//                    return NotFound($"Employee with ID {id} not found");
//                }

//                return Ok(employee);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Delete an employee
//        /// </summary>
//        /// <param name="id">Employee ID</param>
//        /// <returns>Success status</returns>
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> DeleteEmployee(int id)
//        {
//            try
//            {
//                _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

//                if (id <= 0)
//                {
//                    return BadRequest("Invalid employee ID");
//                }

//                var result = await _employeeService.DeleteEmployeeAsync(id);

//                if (!result)
//                {
//                    return NotFound($"Employee with ID {id} not found");
//                }

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while deleting employee with ID: {EmployeeId}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get employees by department name
//        /// </summary>
//        /// <param name="department">Department name</param>
//        /// <returns>List of employees in the department</returns>
//        [HttpGet("department/{department}")]
//        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(string department)
//        {
//            try
//            {
//                _logger.LogInformation("Getting employees for department: {Department}", department);

//                if (string.IsNullOrWhiteSpace(department))
//                {
//                    return BadRequest("Department name is required");
//                }

//                var employees = await _employeeService.GetEmployeesByDepartmentAsync(department);
//                return Ok(employees);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting employees for department: {Department}", department);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get all available genders
//        /// </summary>
//        /// <returns>List of genders</returns>
//        [HttpGet("genders")]
//        public async Task<ActionResult<IEnumerable<Gender>>> GetGenders()
//        {
//            try
//            {
//                _logger.LogInformation("Getting all genders");
//                var genders = await _employeeService.GetGendersAsync();
//                return Ok(genders);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting genders");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get all available departments
//        /// </summary>
//        /// <returns>List of departments</returns>
//        [HttpGet("departments")]
//        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
//        {
//            try
//            {
//                _logger.LogInformation("Getting all departments");
//                var departments = await _employeeService.GetDepartmentsAsync();
//                return Ok(departments);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting departments");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        /// <summary>
//        /// Get all available positions
//        /// </summary>
//        /// <returns>List of positions</returns>
//        [HttpGet("positions")]
//        public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
//        {
//            try
//            {
//                _logger.LogInformation("Getting all positions");
//                var positions = await _employeeService.GetPositionsAsync();
//                return Ok(positions);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while getting positions");
//                return StatusCode(500, "Internal server error");
//            }
//        }
//    }
//}


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
        /// Get all employees with their department, position, and gender information
        /// </summary>
        /// <returns>List of employees with full details</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
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
        /// Get employee by ID with full details
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details with related information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
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
        /// <returns>Created employee with full details</returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new employee: {FirstName} {LastName}", request.Name, request.LastName);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Только базовая проверка на null/empty - всю остальную валидацию делают триггеры
                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.LastName))
                {
                    return BadRequest("Name and LastName are required");
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

                // Передаем сообщение от триггера клиенту
                if (ex.Message.Contains("Employee must be at least") ||
                    ex.Message.Contains("Work experience cannot exceed") ||
                    ex.Message.Contains("Phone number must start") ||
                    ex.Message.Contains("City name must be") ||
                    ex.Message.Contains("specified") && ex.Message.Contains("does not exist") ||
                    ex.Message.Contains("cannot have zero experience"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="request">Employee update request</param>
        /// <returns>Updated employee with full details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request)
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

                // Только базовая проверка на null/empty - всю остальную валидацию делают триггеры
                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.LastName))
                {
                    return BadRequest("Name and LastName are required");
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

                // Передаем сообщение от триггера клиенту
                if (ex.Message.Contains("Employee must be at least") ||
                    ex.Message.Contains("Work experience cannot exceed") ||
                    ex.Message.Contains("Phone number must start") ||
                    ex.Message.Contains("City name must be") ||
                    ex.Message.Contains("specified") && ex.Message.Contains("does not exist") ||
                    ex.Message.Contains("cannot have zero experience"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete an employee
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
        /// Get employees by department name
        /// </summary>
        /// <param name="department">Department name</param>
        /// <returns>List of employees in the department</returns>
        [HttpGet("department/{department}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(string department)
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

        /// <summary>
        /// Get all available genders
        /// </summary>
        /// <returns>List of genders</returns>
        [HttpGet("genders")]
        public async Task<ActionResult<IEnumerable<Gender>>> GetGenders()
        {
            try
            {
                _logger.LogInformation("Getting all genders");
                var genders = await _employeeService.GetGendersAsync();
                return Ok(genders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting genders");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all available departments
        /// </summary>
        /// <returns>List of departments</returns>
        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            try
            {
                _logger.LogInformation("Getting all departments");
                var departments = await _employeeService.GetDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting departments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all available positions
        /// </summary>
        /// <returns>List of positions</returns>
        [HttpGet("positions")]
        public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
        {
            try
            {
                _logger.LogInformation("Getting all positions");
                var positions = await _employeeService.GetPositionsAsync();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting positions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}