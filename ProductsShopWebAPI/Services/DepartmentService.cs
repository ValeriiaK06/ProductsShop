// Создать новый файл Services/DepartmentService.cs

using System.Data;
using Microsoft.Data.SqlClient;
using ProductsShopWebAPI.Models;

namespace ProductsShopWebAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDatabaseService databaseService, ILogger<DepartmentService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = new List<DepartmentDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        ID,
                        Department as DepartmentName,
                        LocationRow,
                        Sales,
                        Counters,
                        StartTime,
                        EndTime
                    FROM Departments
                    ORDER BY DepartmentName";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    departments.Add(MapToDepartmentDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all departments");
                throw;
            }

            return departments;
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        ID,
                        Department as DepartmentName,
                        LocationRow,
                        Sales,
                        Counters,
                        StartTime,
                        EndTime
                    FROM Departments
                    WHERE ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToDepartmentDto(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department by ID: {DepartmentId}", id);
                throw;
            }
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    UPDATE Departments 
                    SET Department = @DepartmentName,
                        LocationRow = @LocationRow,
                        Sales = @Sales,
                        Counters = @Counters,
                        StartTime = @StartTime,
                        EndTime = @EndTime
                    WHERE ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@DepartmentName", request.DepartmentName);
                command.Parameters.AddWithValue("@LocationRow", request.LocationRow);
                command.Parameters.AddWithValue("@Sales", request.Sales);
                command.Parameters.AddWithValue("@Counters", request.Counters);
                command.Parameters.AddWithValue("@StartTime", request.StartTime);
                command.Parameters.AddWithValue("@EndTime", request.EndTime);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                    return null;

                return await GetDepartmentByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department with ID: {DepartmentId}", id);
                throw;
            }
        }

        private static DepartmentDto MapToDepartmentDto(SqlDataReader reader)
        {
            return new DepartmentDto
            {
                ID = reader.GetInt32("ID"),
                DepartmentName = reader.GetString("DepartmentName"),
                LocationRow = reader.GetInt32("LocationRow"),
                Sales = reader.GetBoolean("Sales"),
                Counters = reader.GetInt32("Counters"),
                StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime"))
            };
        }
    }
}