using Microsoft.Data.SqlClient;
using ProductsShopWebAPI.Models;
using System.Data;

namespace ProductsShopWebAPI.Services
{
    public class GoodsService : IGoodsService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<GoodsService> _logger;

        public GoodsService(IDatabaseService databaseService, ILogger<GoodsService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<IEnumerable<GoodsDto>> GetAllGoodsAsync()
        {
            var goods = new List<GoodsDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        g.ID,
                        g.ProductName,
                        d.Department as DepartmentName,
                        c.Country as CountryName,
                        g.Manufacturer,
                        g.Conditions_Temperature,
                        g.Is_Expiring,
                        g.Term
                    FROM Goods g
                    INNER JOIN Departments d ON g.Department = d.ID
                    INNER JOIN Countries c ON g.Country = c.ID
                    ORDER BY g.ProductName";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    goods.Add(MapToGoodsDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all goods");
                throw;
            }

            return goods;
        }

        public async Task<GoodsDto?> GetGoodsByIdAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        g.ID,
                        g.ProductName,
                        d.Department as DepartmentName,
                        c.Country as CountryName,
                        g.Manufacturer,
                        g.Conditions_Temperature,
                        g.Is_Expiring,
                        g.Term
                    FROM Goods g
                    INNER JOIN Departments d ON g.Department = d.ID
                    INNER JOIN Countries c ON g.Country = c.ID
                    WHERE g.ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToGoodsDto(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting goods by ID: {GoodsId}", id);
                throw;
            }
        }

        public async Task<GoodsDto> CreateGoodsAsync(CreateGoodsRequest request)
        {
            try
            {
                _logger.LogInformation("Creating goods: {@Request}", request);

                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    INSERT INTO Goods (ProductName, Department, Country, Manufacturer, 
                                     Conditions_Temperature, Is_Expiring, Term)
                    VALUES (@ProductName, @Department, @Country, @Manufacturer, 
                           @Conditions_Temperature, @Is_Expiring, @Term)";

                using var command = new SqlCommand(sql, connection);
                AddGoodsParameters(command, request);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to insert goods");
                }

                // Находим созданный товар по уникальным данным
                var goods = await GetAllGoodsAsync();
                var createdGoods = goods
                    .Where(g => g.ProductName == request.ProductName &&
                               g.Manufacturer == request.Manufacturer &&
                               g.Conditions_Temperature == request.Conditions_Temperature)
                    .OrderByDescending(g => g.ID)
                    .FirstOrDefault();

                if (createdGoods == null)
                {
                    throw new Exception("Created goods not found");
                }

                return createdGoods;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goods");
                throw;
            }
        }

        public async Task<GoodsDto?> UpdateGoodsAsync(int id, UpdateGoodsRequest request)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    UPDATE Goods 
                    SET ProductName = @ProductName,
                        Department = @Department,
                        Country = @Country,
                        Manufacturer = @Manufacturer,
                        Conditions_Temperature = @Conditions_Temperature,
                        Is_Expiring = @Is_Expiring,
                        Term = @Term
                    WHERE ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                AddGoodsParameters(command, request);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                    return null;

                return await GetGoodsByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goods with ID: {GoodsId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteGoodsAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = "DELETE FROM Goods WHERE ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting goods with ID: {GoodsId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<GoodsDto>> GetExpiringGoodsAsync()
        {
            var goods = new List<GoodsDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        g.ID,
                        g.ProductName,
                        d.Department as DepartmentName,
                        c.Country as CountryName,
                        g.Manufacturer,
                        g.Conditions_Temperature,
                        g.Is_Expiring,
                        g.Term
                    FROM Goods g
                    INNER JOIN Departments d ON g.Department = d.ID
                    INNER JOIN Countries c ON g.Country = c.ID
                    WHERE g.Is_Expiring = 1
                    ORDER BY g.Term ASC, g.ProductName";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    goods.Add(MapToGoodsDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring goods");
                throw;
            }

            return goods;
        }

        public async Task<IEnumerable<GoodsDto>> GetNonExpiringGoodsAsync()
        {
            var goods = new List<GoodsDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        g.ID,
                        g.ProductName,
                        d.Department as DepartmentName,
                        c.Country as CountryName,
                        g.Manufacturer,
                        g.Conditions_Temperature,
                        g.Is_Expiring,
                        g.Term
                    FROM Goods g
                    INNER JOIN Departments d ON g.Department = d.ID
                    INNER JOIN Countries c ON g.Country = c.ID
                    WHERE g.Is_Expiring = 0
                    ORDER BY g.ProductName";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    goods.Add(MapToGoodsDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting non-expiring goods");
                throw;
            }

            return goods;
        }

        public async Task<DepartmentAnalysisDto> GetDepartmentAnalysisAsync(string departmentName)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                using var command = new SqlCommand("GetDepartmentProductsInfo", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Входные параметры
                command.Parameters.AddWithValue("@DepartmentName", departmentName);

                // Выходные параметры
                var productCountParam = new SqlParameter("@ProductCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var expiringProductCountParam = new SqlParameter("@ExpiringProductCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var avgStorageTempParam = new SqlParameter("@AvgStorageTemp", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 10, Scale = 2 };
                var maxTermDaysParam = new SqlParameter("@MaxTermDays", SqlDbType.Int) { Direction = ParameterDirection.Output };

                command.Parameters.Add(productCountParam);
                command.Parameters.Add(expiringProductCountParam);
                command.Parameters.Add(avgStorageTempParam);
                command.Parameters.Add(maxTermDaysParam);

                // Выполняем процедуру
                await command.ExecuteNonQueryAsync();

                // Получаем результаты
                var result = new DepartmentAnalysisDto
                {
                    DepartmentName = departmentName,
                    ProductCount = productCountParam.Value != DBNull.Value ? (int)productCountParam.Value : 0,
                    ExpiringProductCount = expiringProductCountParam.Value != DBNull.Value ? (int)expiringProductCountParam.Value : 0,
                    AvgStorageTemp = avgStorageTempParam.Value != DBNull.Value ? (decimal)avgStorageTempParam.Value : 0,
                    MaxTermDays = maxTermDaysParam.Value != DBNull.Value ? (int)maxTermDaysParam.Value : 0
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department analysis for: {DepartmentName}", departmentName);
                throw;
            }
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            var departments = new List<Department>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();
                var sql = @"SELECT ID, Department as DepartmentName, LocationRow, Sales, 
                           Counters, StartTime, EndTime FROM Departments ORDER BY Department";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    departments.Add(new Department
                    {
                        ID = reader.GetInt32("ID"),
                        DepartmentName = reader.GetString("DepartmentName"),
                        LocationRow = reader.GetInt32("LocationRow"),
                        Sales = reader.GetBoolean("Sales"),
                        Counters = reader.GetInt32("Counters"),
                        StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                        EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime"))
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting departments");
                throw;
            }

            return departments;
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            var countries = new List<Country>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();
                var sql = "SELECT ID, Country as CountryName FROM Countries ORDER BY Country";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    countries.Add(new Country
                    {
                        ID = reader.GetInt32("ID"),
                        CountryName = reader.GetString("CountryName")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting countries");
                throw;
            }

            return countries;
        }

        private static GoodsDto MapToGoodsDto(SqlDataReader reader)
        {
            return new GoodsDto
            {
                ID = reader.GetInt32("ID"),
                ProductName = reader.GetString("ProductName"),
                DepartmentName = reader.GetString("DepartmentName"),
                CountryName = reader.GetString("CountryName"),
                Manufacturer = reader.GetString("Manufacturer"),
                Conditions_Temperature = reader.GetDecimal("Conditions_Temperature"),
                Is_Expiring = reader.GetBoolean("Is_Expiring"),
                Term = reader.IsDBNull("Term") ? null : reader.GetInt32("Term")
            };
        }

        private static void AddGoodsParameters(SqlCommand command, CreateGoodsRequest request)
        {
            command.Parameters.AddWithValue("@ProductName", request.ProductName);
            command.Parameters.AddWithValue("@Department", request.Department);
            command.Parameters.AddWithValue("@Country", request.Country);
            command.Parameters.AddWithValue("@Manufacturer", request.Manufacturer);
            command.Parameters.AddWithValue("@Conditions_Temperature", request.Conditions_Temperature);
            command.Parameters.AddWithValue("@Is_Expiring", request.Is_Expiring);
            command.Parameters.AddWithValue("@Term", (object?)request.Term ?? DBNull.Value);
        }

        private static void AddGoodsParameters(SqlCommand command, UpdateGoodsRequest request)
        {
            command.Parameters.AddWithValue("@ProductName", request.ProductName);
            command.Parameters.AddWithValue("@Department", request.Department);
            command.Parameters.AddWithValue("@Country", request.Country);
            command.Parameters.AddWithValue("@Manufacturer", request.Manufacturer);
            command.Parameters.AddWithValue("@Conditions_Temperature", request.Conditions_Temperature);
            command.Parameters.AddWithValue("@Is_Expiring", request.Is_Expiring);
            command.Parameters.AddWithValue("@Term", (object?)request.Term ?? DBNull.Value);
        }
    }
}