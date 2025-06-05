using Microsoft.AspNetCore.Mvc;
using ProductsShopWebAPI.Services;

namespace ProductsShopWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IDatabaseService databaseService, ILogger<HealthController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// Check if the database connection is working
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet("database")]
        public async Task<ActionResult> CheckDatabaseHealth()
        {
            try
            {
                var isHealthy = await _databaseService.TestConnectionAsync();

                if (isHealthy)
                {
                    return Ok(new { Status = "Healthy", Message = "Database connection is working" });
                }
                else
                {
                    return StatusCode(503, new { Status = "Unhealthy", Message = "Database connection failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new { Status = "Unhealthy", Message = "Health check failed", Error = ex.Message });
            }
        }
    }
}