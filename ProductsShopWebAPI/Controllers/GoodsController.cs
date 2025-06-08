using Microsoft.AspNetCore.Mvc;
using ProductsShopWebAPI.Models;
using ProductsShopWebAPI.Services;

namespace ProductsShopWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsController : ControllerBase
    {
        private readonly IGoodsService _goodsService;
        private readonly ILogger<GoodsController> _logger;

        public GoodsController(IGoodsService goodsService, ILogger<GoodsController> logger)
        {
            _goodsService = goodsService;
            _logger = logger;
        }

        /// <summary>
        /// Get all goods with their department and country information
        /// </summary>
        /// <returns>List of goods with full details</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoodsDto>>> GetAllGoods()
        {
            try
            {
                _logger.LogInformation("Getting all goods");
                var goods = await _goodsService.GetAllGoodsAsync();
                return Ok(goods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all goods");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get goods by ID with full details
        /// </summary>
        /// <param name="id">Goods ID</param>
        /// <returns>Goods details with related information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GoodsDto>> GetGoodsById(int id)
        {
            try
            {
                _logger.LogInformation("Getting goods with ID: {GoodsId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid goods ID");
                }

                var goods = await _goodsService.GetGoodsByIdAsync(id);

                if (goods == null)
                {
                    return NotFound($"Goods with ID {id} not found");
                }

                return Ok(goods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting goods with ID: {GoodsId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create new goods
        /// </summary>
        /// <param name="request">Goods creation request</param>
        /// <returns>Created goods with full details</returns>
        [HttpPost]
        public async Task<ActionResult<GoodsDto>> CreateGoods([FromBody] CreateGoodsRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new goods: {ProductName}", request.ProductName);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Только базовая проверка на null/empty - всю остальную валидацию делают триггеры
                if (string.IsNullOrWhiteSpace(request.ProductName) ||
                    string.IsNullOrWhiteSpace(request.Manufacturer))
                {
                    return BadRequest("ProductName and Manufacturer are required");
                }

                var goods = await _goodsService.CreateGoodsAsync(request);

                return CreatedAtAction(
                    nameof(GetGoodsById),
                    new { id = goods.ID },
                    goods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating goods");

                // Передаем сообщение от триггера клиенту
                if (ex.Message.Contains("Temperature must be") ||
                    ex.Message.Contains("Term must be") ||
                    ex.Message.Contains("Department") ||
                    ex.Message.Contains("Country") ||
                    ex.Message.Contains("Term cannot be specified"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update existing goods
        /// </summary>
        /// <param name="id">Goods ID</param>
        /// <param name="request">Goods update request</param>
        /// <returns>Updated goods with full details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<GoodsDto>> UpdateGoods(int id, [FromBody] UpdateGoodsRequest request)
        {
            try
            {
                _logger.LogInformation("Updating goods with ID: {GoodsId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid goods ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Только базовая проверка на null/empty - всю остальную валидацию делают триггеры
                if (string.IsNullOrWhiteSpace(request.ProductName) ||
                    string.IsNullOrWhiteSpace(request.Manufacturer))
                {
                    return BadRequest("ProductName and Manufacturer are required");
                }

                var goods = await _goodsService.UpdateGoodsAsync(id, request);

                if (goods == null)
                {
                    return NotFound($"Goods with ID {id} not found");
                }

                return Ok(goods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating goods with ID: {GoodsId}", id);

                // Передаем сообщение от триггера клиенту
                if (ex.Message.Contains("Temperature must be") ||
                    ex.Message.Contains("Term must be") ||
                    ex.Message.Contains("Department") ||
                    ex.Message.Contains("Country") ||
                    ex.Message.Contains("Term cannot be specified"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete goods
        /// </summary>
        /// <param name="id">Goods ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGoods(int id)
        {
            try
            {
                _logger.LogInformation("Deleting goods with ID: {GoodsId}", id);

                if (id <= 0)
                {
                    return BadRequest("Invalid goods ID");
                }

                var result = await _goodsService.DeleteGoodsAsync(id);

                if (!result)
                {
                    return NotFound($"Goods with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting goods with ID: {GoodsId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get goods with expiration term
        /// </summary>
        /// <returns>List of goods that have expiration term</returns>
        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<GoodsDto>>> GetExpiringGoods()
        {
            try
            {
                _logger.LogInformation("Getting goods with expiration term");
                var goods = await _goodsService.GetExpiringGoodsAsync();
                return Ok(goods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting expiring goods");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get goods without expiration term
        /// </summary>
        /// <returns>List of goods that do not have expiration term</returns>
        [HttpGet("non-expiring")]
        public async Task<ActionResult<IEnumerable<GoodsDto>>> GetNonExpiringGoods()
        {
            try
            {
                _logger.LogInformation("Getting goods without expiration term");
                var goods = await _goodsService.GetNonExpiringGoodsAsync();
                return Ok(goods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting non-expiring goods");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get department products analysis
        /// </summary>
        /// <param name="departmentName">Department name</param>
        /// <returns>Department analysis with product statistics</returns>
        [HttpGet("department-analysis/{departmentName}")]
        public async Task<ActionResult<DepartmentAnalysisDto>> GetDepartmentAnalysis(string departmentName)
        {
            try
            {
                _logger.LogInformation("Getting department analysis for: {DepartmentName}", departmentName);

                if (string.IsNullOrWhiteSpace(departmentName))
                {
                    return BadRequest("Department name is required");
                }

                var analysis = await _goodsService.GetDepartmentAnalysisAsync(departmentName);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting department analysis for: {DepartmentName}", departmentName);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all available departments for goods
        /// </summary>
        /// <returns>List of departments</returns>
        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            try
            {
                _logger.LogInformation("Getting all departments");
                var departments = await _goodsService.GetDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting departments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all available countries
        /// </summary>
        /// <returns>List of countries</returns>
        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            try
            {
                _logger.LogInformation("Getting all countries");
                var countries = await _goodsService.GetCountriesAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting countries");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}