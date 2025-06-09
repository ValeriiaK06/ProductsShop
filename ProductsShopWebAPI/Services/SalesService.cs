using Microsoft.Data.SqlClient;
using ProductsShopWebAPI.Models;
using System.Data;

namespace ProductsShopWebAPI.Services
{
    public class SalesService : ISalesService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<SalesService> _logger;

        public SalesService(IDatabaseService databaseService, ILogger<SalesService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<IEnumerable<SalesDto>> GetAllSalesAsync()
        {
            var sales = new List<SalesDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        s.ID,
                        CONCAT(e.LastName, ' ', e.Name, ISNULL(' ' + e.Surname, '')) as EmployeeFullName,
                        CONCAT(e.Name, ' ', e.LastName) as EmployeeName,
                        g.ProductName,
                        d.Department as DepartmentName,
                        s.Date,
                        s.Quantity,
                        s.Price
                    FROM Sales s
                    INNER JOIN Employees e ON s.Employee = e.Id
                    INNER JOIN Goods g ON s.Product = g.ID
                    INNER JOIN Departments d ON e.Department = d.ID
                    ORDER BY s.Date DESC, s.ID DESC";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    sales.Add(MapToSalesDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sales");
                throw;
            }

            return sales;
        }

        public async Task<SalesDto?> GetSalesByIdAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        s.ID,
                        CONCAT(e.LastName, ' ', e.Name, ISNULL(' ' + e.Surname, '')) as EmployeeFullName,
                        CONCAT(e.Name, ' ', e.LastName) as EmployeeName,
                        g.ProductName,
                        d.Department as DepartmentName,
                        s.Date,
                        s.Quantity,
                        s.Price
                    FROM Sales s
                    INNER JOIN Employees e ON s.Employee = e.Id
                    INNER JOIN Goods g ON s.Product = g.ID
                    INNER JOIN Departments d ON e.Department = d.ID
                    WHERE s.ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToSalesDto(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales by ID: {SalesId}", id);
                throw;
            }
        }

        public async Task<SalesDto> CreateSalesAsync(CreateSalesRequest request)
        {
            try
            {
                _logger.LogInformation("Creating sales: {@Request}", request);

                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    INSERT INTO Sales (Employee, Product, Date, Quantity, Price)
                    VALUES (@Employee, @Product, @Date, @Quantity, @Price)";

                using var command = new SqlCommand(sql, connection);
                AddSalesParameters(command, request);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to insert sales");
                }

                // Находим созданную продажу
                var allSales = await GetAllSalesAsync();
                var createdSales = allSales
                    .Where(s => s.Date.Date == request.Date.Date &&
                               Math.Abs(s.Quantity - request.Quantity) < 0.001m &&
                               Math.Abs(s.Price - request.Price) < 0.01m)
                    .OrderByDescending(s => s.ID)
                    .FirstOrDefault();

                if (createdSales == null)
                {
                    throw new Exception("Created sales not found");
                }

                return createdSales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sales");
                throw;
            }
        }

        public async Task<SalesDto?> UpdateSalesAsync(int id, UpdateSalesRequest request)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    UPDATE Sales 
                    SET Employee = @Employee,
                        Product = @Product,
                        Date = @Date,
                        Quantity = @Quantity,
                        Price = @Price
                    WHERE ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                AddSalesParameters(command, request);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                    return null;

                return await GetSalesByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sales with ID: {SalesId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteSalesAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = "DELETE FROM Sales WHERE ID = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sales with ID: {SalesId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeSalesAnalysisDto>> GetEmployeeSalesAnalysisAsync()
        {
            var analysis = new List<EmployeeSalesAnalysisDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        e.Id as EmployeeId,
                        CONCAT(e.LastName, ' ', e.Name, ISNULL(' ' + e.Surname, '')) as EmployeeName,
                        d.Department as DepartmentName,
                        COUNT(s.ID) as TotalSales,
                        SUM(s.Quantity) as TotalQuantity,
                        SUM(s.Quantity * s.Price) as TotalAmount,
                        AVG(s.Price) as AveragePrice
                    FROM Employees e
                    INNER JOIN Departments d ON e.Department = d.ID
                    LEFT JOIN Sales s ON e.Id = s.Employee
                    GROUP BY e.Id, e.LastName, e.Name, e.Surname, d.Department
                    HAVING COUNT(s.ID) > 0
                    ORDER BY TotalAmount DESC";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    analysis.Add(new EmployeeSalesAnalysisDto
                    {
                        EmployeeId = reader.GetInt32("EmployeeId"),
                        EmployeeName = reader.GetString("EmployeeName"),
                        DepartmentName = reader.GetString("DepartmentName"),
                        TotalSales = reader.GetInt32("TotalSales"),
                        TotalQuantity = reader.IsDBNull("TotalQuantity") ? 0 : reader.GetDecimal("TotalQuantity"),
                        TotalAmount = reader.IsDBNull("TotalAmount") ? 0 : reader.GetDecimal("TotalAmount"),
                        AveragePrice = reader.IsDBNull("AveragePrice") ? 0 : reader.GetDecimal("AveragePrice")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee sales analysis");
                throw;
            }

            return analysis;
        }

        public async Task<IEnumerable<SalesPeriodAnalysisDto>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var analysis = new List<SalesPeriodAnalysisDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        CAST(s.Date AS DATE) as Date,
                        COUNT(s.ID) as TotalSales,
                        SUM(s.Quantity) as TotalQuantity,
                        SUM(s.Quantity * s.Price) as TotalAmount
                    FROM Sales s
                    WHERE CAST(s.Date AS DATE) BETWEEN @StartDate AND @EndDate
                    GROUP BY CAST(s.Date AS DATE)
                    ORDER BY Date DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@StartDate", startDate.Date);
                command.Parameters.AddWithValue("@EndDate", endDate.Date);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    analysis.Add(new SalesPeriodAnalysisDto
                    {
                        Date = reader.GetDateTime("Date"),
                        TotalSales = reader.GetInt32("TotalSales"),
                        TotalQuantity = reader.IsDBNull("TotalQuantity") ? 0 : reader.GetDecimal("TotalQuantity"),
                        TotalAmount = reader.IsDBNull("TotalAmount") ? 0 : reader.GetDecimal("TotalAmount")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales by period analysis");
                throw;
            }

            return analysis;
        }

        public async Task<IEnumerable<GoodsDto>> GetProductsByDepartmentAndCountryAsync(int departmentId, int? countryId = null)
        {
            var products = new List<GoodsDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                using var command = new SqlCommand("GetProductsByDepartmentAndCountry", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@DepartmentId", departmentId);
                if (countryId.HasValue)
                {
                    command.Parameters.AddWithValue("@CountryId", countryId.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@CountryId", DBNull.Value);
                }

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new GoodsDto
                    {
                        ID = reader.GetInt32("ID"),
                        ProductName = reader.GetString("ProductName"),
                        DepartmentName = reader.GetString("Department"),
                        CountryName = reader.GetString("Country"),
                        Manufacturer = reader.GetString("Manufacturer"),
                        Conditions_Temperature = reader.GetDecimal("Conditions_Temperature"),
                        Is_Expiring = reader.GetBoolean("Is_Expiring"),
                        Term = reader.IsDBNull("Term") ? null : reader.GetInt32("Term")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by department and country");
                throw;
            }

            return products;
        }

        public async Task<DepartmentWorkingStatusDto> CheckDepartmentWorkingStatusAsync(int departmentId, TimeSpan checkTime)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                using var command = new SqlCommand("IsDepartmentWorkingAtTime", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@DepartmentId", departmentId);
                command.Parameters.AddWithValue("@CheckTime", checkTime);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new DepartmentWorkingStatusDto
                    {
                        DepartmentID = reader.GetInt32("DepartmentID"),
                        DepartmentName = reader.IsDBNull("DepartmentName") ? "" : reader.GetString("DepartmentName"),
                        OpeningTime = reader.IsDBNull("OpeningTime") ? "" : reader.GetString("OpeningTime"),
                        ClosingTime = reader.IsDBNull("ClosingTime") ? "" : reader.GetString("ClosingTime"),
                        CheckedTime = reader.IsDBNull("CheckedTime") ? "" : reader.GetString("CheckedTime"),
                        WorkingStatus = reader.IsDBNull("WorkingStatus") ? "" : reader.GetString("WorkingStatus")
                    };
                }

                // Fallback if no data returned
                return new DepartmentWorkingStatusDto
                {
                    DepartmentID = departmentId,
                    DepartmentName = "Unknown",
                    WorkingStatus = "Department not found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking department working status");
                throw;
            }
        }

        public async Task<IEnumerable<SalesDto>> GetSalesByEmployeeAsync(int employeeId)
        {
            var sales = new List<SalesDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        s.ID,
                        CONCAT(e.LastName, ' ', e.Name, ISNULL(' ' + e.Surname, '')) as EmployeeFullName,
                        CONCAT(e.Name, ' ', e.LastName) as EmployeeName,
                        g.ProductName,
                        d.Department as DepartmentName,
                        s.Date,
                        s.Quantity,
                        s.Price
                    FROM Sales s
                    INNER JOIN Employees e ON s.Employee = e.Id
                    INNER JOIN Goods g ON s.Product = g.ID
                    INNER JOIN Departments d ON e.Department = d.ID
                    WHERE s.Employee = @EmployeeId
                    ORDER BY s.Date DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    sales.Add(MapToSalesDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales by employee: {EmployeeId}", employeeId);
                throw;
            }

            return sales;
        }

        public async Task<IEnumerable<SalesDto>> GetSalesByProductAsync(int productId)
        {
            var sales = new List<SalesDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        s.ID,
                        CONCAT(e.LastName, ' ', e.Name, ISNULL(' ' + e.Surname, '')) as EmployeeFullName,
                        CONCAT(e.Name, ' ', e.LastName) as EmployeeName,
                        g.ProductName,
                        d.Department as DepartmentName,
                        s.Date,
                        s.Quantity,
                        s.Price
                    FROM Sales s
                    INNER JOIN Employees e ON s.Employee = e.Id
                    INNER JOIN Goods g ON s.Product = g.ID
                    INNER JOIN Departments d ON e.Department = d.ID
                    WHERE s.Product = @ProductId
                    ORDER BY s.Date DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    sales.Add(MapToSalesDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales by product: {ProductId}", productId);
                throw;
            }

            return sales;
        }

        private static SalesDto MapToSalesDto(SqlDataReader reader)
        {
            return new SalesDto
            {
                ID = reader.GetInt32("ID"),
                EmployeeFullName = reader.GetString("EmployeeFullName"),
                EmployeeName = reader.GetString("EmployeeName"),
                ProductName = reader.GetString("ProductName"),
                DepartmentName = reader.GetString("DepartmentName"),
                Date = reader.GetDateTime("Date"),
                Quantity = reader.GetDecimal("Quantity"),
                Price = reader.GetDecimal("Price")
            };
        }

        private static void AddSalesParameters(SqlCommand command, CreateSalesRequest request)
        {
            command.Parameters.AddWithValue("@Employee", request.Employee);
            command.Parameters.AddWithValue("@Product", request.Product);
            command.Parameters.AddWithValue("@Date", request.Date);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);
            command.Parameters.AddWithValue("@Price", request.Price);
        }

        private static void AddSalesParameters(SqlCommand command, UpdateSalesRequest request)
        {
            command.Parameters.AddWithValue("@Employee", request.Employee);
            command.Parameters.AddWithValue("@Product", request.Product);
            command.Parameters.AddWithValue("@Date", request.Date);
            command.Parameters.AddWithValue("@Quantity", request.Quantity);
            command.Parameters.AddWithValue("@Price", request.Price);
        }
    }
}