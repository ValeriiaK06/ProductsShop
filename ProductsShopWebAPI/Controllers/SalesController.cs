using Microsoft.AspNetCore.Mvc;
using ProductsShopWebAPI.Models;
using ProductsShopWebAPI.Services;

namespace ProductsShopWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesService salesService, ILogger<SalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// Get all sales with employee and product information
        /// </summary>
        /// <returns>List of sales with full details</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesDto>>> GetAllSales()
        {
            try
            {
                _logger.LogInformation("Getting all sales");
                var sales = await _salesService.GetAllSalesAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all sales");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get sales by ID with full details
        /// </summary>
        /// <param name="id">Sales ID</param>
        /// <returns>Sales details with related information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesDto>> GetSalesById(int id)
        {
            try
            {
                _logger.LogInformation("Getting sales with ID: {SalesId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid sales ID");
                }

                var sales = await _salesService.GetSalesByIdAsync(id);

                if (sales == null)
                {
                    return NotFound($"Sales with ID {id} not found");
                }

                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sales with ID: {SalesId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new sales record
        /// </summary>
        /// <param name="request">Sales creation request</param>
        /// <returns>Created sales record with full details</returns>
        [HttpPost]
        public async Task<ActionResult<SalesDto>> CreateSales([FromBody] CreateSalesRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new sales record");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Только базовая проверка на валидные значения - всю остальную валидацию делают триггеры
                if (request.Employee <= 0 || request.Product <= 0)
                {
                    return BadRequest("Employee and Product IDs must be valid");
                }

                var sales = await _salesService.CreateSalesAsync(request);

                return CreatedAtAction(
                    nameof(GetSalesById),
                    new { id = sales.ID },
                    sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating sales");

                // Передаем сообщение от триггера клиенту
                if (ex.Message.Contains("Quantity of sold products must be greater than zero!") ||
                    ex.Message.Contains("Product price must be greater than zero!") ||
                    ex.Message.Contains("Sale date cannot be in the future!") ||
                    ex.Message.Contains("Referenced employee does not exist!") ||
                    ex.Message.Contains("Referenced product does not exist!") ||
                    ex.Message.Contains("Employee can only sell products from their own department!") ||
                    ex.Message.Contains("Product price must be between 0.01 and 999999.99!") ||
                    ex.Message.Contains("Quantity cannot exceed 10000 units per sale!"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing sales record
        /// </summary>
        /// <param name="id">Sales ID</param>
        /// <param name="request">Sales update request</param>
        /// <returns>Updated sales record with full details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<SalesDto>> UpdateSales(int id, [FromBody] UpdateSalesRequest request)
        {
            try
            {
                _logger.LogInformation("Updating sales with ID: {SalesId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid sales ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Только базовая проверка - всю остальную валидацию делают триггеры
                if (request.Employee <= 0 || request.Product <= 0)
                {
                    return BadRequest("Employee and Product IDs must be valid");
                }

                var sales = await _salesService.UpdateSalesAsync(id, request);

                if (sales == null)
                {
                    return NotFound($"Sales with ID {id} not found");
                }

                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating sales with ID: {SalesId}", id);

                // Передаем сообщение от триггера клиенту
                if (ex.Message.Contains("Quantity of sold products must be greater than zero!") ||
                    ex.Message.Contains("Product price must be greater than zero!") ||
                    ex.Message.Contains("Sale date cannot be in the future!") ||
                    ex.Message.Contains("Referenced employee does not exist!") ||
                    ex.Message.Contains("Referenced product does not exist!") ||
                    ex.Message.Contains("Employee can only sell products from their own department!") ||
                    ex.Message.Contains("Product price must be between 0.01 and 999999.99!") ||
                    ex.Message.Contains("Quantity cannot exceed 10000 units per sale!"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a sales record
        /// </summary>
        /// <param name="id">Sales ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSales(int id)
        {
            try
            {
                _logger.LogInformation("Deleting sales with ID: {SalesId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid sales ID");
                }

                var result = await _salesService.DeleteSalesAsync(id);

                if (!result)
                {
                    return NotFound($"Sales with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting sales with ID: {SalesId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get sales by employee ID
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <returns>List of sales for the employee</returns>
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<SalesDto>>> GetSalesByEmployee(int employeeId)
        {
            try
            {
                _logger.LogInformation("Getting sales for employee: {EmployeeId}", employeeId);

                if (employeeId <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                var sales = await _salesService.GetSalesByEmployeeAsync(employeeId);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sales for employee: {EmployeeId}", employeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get sales by product ID
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>List of sales for the product</returns>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<SalesDto>>> GetSalesByProduct(int productId)
        {
            try
            {
                _logger.LogInformation("Getting sales for product: {ProductId}", productId);

                if (productId <= 0)
                {
                    return BadRequest("Invalid product ID");
                }

                var sales = await _salesService.GetSalesByProductAsync(productId);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sales for product: {ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get employee sales analysis
        /// </summary>
        /// <returns>Analysis of sales by employees</returns>
        [HttpGet("analysis/employees")]
        public async Task<ActionResult<IEnumerable<EmployeeSalesAnalysisDto>>> GetEmployeeSalesAnalysis()
        {
            try
            {
                _logger.LogInformation("Getting employee sales analysis");
                var analysis = await _salesService.GetEmployeeSalesAnalysisAsync();
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting employee sales analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get sales analysis for a specific period
        /// </summary>
        /// <param name="startDate">Start date (YYYY-MM-DD)</param>
        /// <param name="endDate">End date (YYYY-MM-DD)</param>
        /// <returns>Sales analysis for the period</returns>
        [HttpGet("analysis/period")]
        public async Task<ActionResult<IEnumerable<SalesPeriodAnalysisDto>>> GetSalesByPeriod(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting sales analysis for period: {StartDate} to {EndDate}", startDate, endDate);

                if (startDate > endDate)
                {
                    return BadRequest("Start date cannot be later than end date");
                }

                if (startDate > DateTime.Today)
                {
                    return BadRequest("Start date cannot be in the future");
                }

                var analysis = await _salesService.GetSalesByPeriodAsync(startDate, endDate);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sales period analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get products by department and optionally by country (using stored procedure)
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <param name="countryId">Country ID (optional)</param>
        /// <returns>List of products matching criteria</returns>
        [HttpGet("products/department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<GoodsDto>>> GetProductsByDepartmentAndCountry(
            int departmentId,
            [FromQuery] int? countryId = null)
        {
            try
            {
                _logger.LogInformation("Getting products for department {DepartmentId} and country {CountryId}",
                    departmentId, countryId);

                if (departmentId <= 0)
                {
                    return BadRequest("Invalid department ID");
                }

                var products = await _salesService.GetProductsByDepartmentAndCountryAsync(departmentId, countryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products by department and country");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if department is working at specific time (using stored procedure)
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <param name="checkTime">Time to check (HH:mm format)</param>
        /// <returns>Department working status</returns>
        [HttpGet("department/{departmentId}/working-status")]
        public async Task<ActionResult<DepartmentWorkingStatusDto>> CheckDepartmentWorkingStatus(
            int departmentId,
            [FromQuery] string checkTime)
        {
            try
            {
                _logger.LogInformation("Checking working status for department {DepartmentId} at time {CheckTime}",
                    departmentId, checkTime);

                if (departmentId <= 0)
                {
                    return BadRequest("Invalid department ID");
                }

                if (string.IsNullOrWhiteSpace(checkTime))
                {
                    return BadRequest("Check time is required (format: HH:mm)");
                }

                if (!TimeSpan.TryParse(checkTime, out var timeSpan))
                {
                    return BadRequest("Invalid time format. Use HH:mm format (e.g., 14:30)");
                }

                var status = await _salesService.CheckDepartmentWorkingStatusAsync(departmentId, timeSpan);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking department working status");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}