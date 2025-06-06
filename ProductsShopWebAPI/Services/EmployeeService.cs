//using ProductsShopWebAPI.Models;

//namespace ProductsShopWebAPI.Services
//{
//    public class EmployeeService : IEmployeeService
//    {
//        private static List<Employee> _employees = new()
//        {
//            new Employee
//            {
//                Id = 1,
//                FirstName = "John",
//                LastName = "Doe",
//                Email = "john.doe@company.com",
//                Department = "Engineering",
//                Position = "Senior Developer",
//                Salary = 85000,
//                HireDate = DateTime.Now.AddYears(-3),
//                IsActive = true
//            },
//            new Employee
//            {
//                Id = 2,
//                FirstName = "Jane",
//                LastName = "Smith",
//                Email = "jane.smith@company.com",
//                Department = "Marketing",
//                Position = "Marketing Manager",
//                Salary = 75000,
//                HireDate = DateTime.Now.AddYears(-2),
//                IsActive = true
//            },
//            new Employee
//            {
//                Id = 3,
//                FirstName = "Mike",
//                LastName = "Johnson",
//                Email = "mike.johnson@company.com",
//                Department = "Engineering",
//                Position = "DevOps Engineer",
//                Salary = 80000,
//                HireDate = DateTime.Now.AddYears(-1),
//                IsActive = true
//            },
//            new Employee
//            {
//                Id = 4,
//                FirstName = "Sarah",
//                LastName = "Williams",
//                Email = "sarah.williams@company.com",
//                Department = "HR",
//                Position = "HR Specialist",
//                Salary = 60000,
//                HireDate = DateTime.Now.AddMonths(-8),
//                IsActive = true
//            },
//            new Employee
//            {
//                Id = 5,
//                FirstName = "David",
//                LastName = "Brown",
//                Email = "david.brown@company.com",
//                Department = "Finance",
//                Position = "Financial Analyst",
//                Salary = 70000,
//                HireDate = DateTime.Now.AddMonths(-6),
//                IsActive = false
//            }
//        };

//        private static int _nextId = 6;

//        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
//        {
//            // Simulate async operation
//            await Task.Delay(10);
//            return _employees.Where(e => e.IsActive).ToList();
//        }

//        public async Task<Employee?> GetEmployeeByIdAsync(int id)
//        {
//            await Task.Delay(10);
//            return _employees.FirstOrDefault(e => e.Id == id && e.IsActive);
//        }

//        public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request)
//        {
//            await Task.Delay(10);

//            var employee = new Employee
//            {
//                Id = _nextId++,
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                Email = request.Email,
//                Department = request.Department,
//                Position = request.Position,
//                Salary = request.Salary,
//                HireDate = DateTime.Now,
//                IsActive = true
//            };

//            _employees.Add(employee);
//            return employee;
//        }

//        public async Task<Employee?> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
//        {
//            await Task.Delay(10);

//            var employee = _employees.FirstOrDefault(e => e.Id == id);
//            if (employee == null)
//                return null;

//            employee.FirstName = request.FirstName;
//            employee.LastName = request.LastName;
//            employee.Email = request.Email;
//            employee.Department = request.Department;
//            employee.Position = request.Position;
//            employee.Salary = request.Salary;
//            employee.IsActive = request.IsActive;

//            return employee;
//        }

//        public async Task<bool> DeleteEmployeeAsync(int id)
//        {
//            await Task.Delay(10);

//            var employee = _employees.FirstOrDefault(e => e.Id == id);
//            if (employee == null)
//                return false;

//            // Soft delete - just mark as inactive
//            employee.IsActive = false;
//            return true;
//        }

//        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
//        {
//            await Task.Delay(10);
//            return _employees.Where(e => e.IsActive &&
//                                   e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
//                            .ToList();
//        }
//    }
//}

using Microsoft.Data.SqlClient;
using ProductsShopWebAPI.Models;
using System.Data;

namespace ProductsShopWebAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IDatabaseService databaseService, ILogger<EmployeeService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = new List<EmployeeDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.LastName,
                        e.Surname,
                        g.Gender,
                        e.DateOfBirth,
                        e.PhoneNumber,
                        e.Address,
                        e.City,
                        d.Department as DepartmentName,
                        e.ExperienceYear,
                        p.JobTitleName,
                        p.Salary,
                        p.WorkingTime,
                        e.YearOfEmployment
                    FROM Employees e
                    INNER JOIN Genders g ON e.Gender = g.Id
                    INNER JOIN Departments d ON e.Department = d.ID
                    INNER JOIN Positions p ON e.JobTitle = p.Id
                    ORDER BY e.LastName, e.Name";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapToEmployeeDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                throw;
            }

            return employees;
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.LastName,
                        e.Surname,
                        g.Gender,
                        e.DateOfBirth,
                        e.PhoneNumber,
                        e.Address,
                        e.City,
                        d.Department as DepartmentName,
                        e.ExperienceYear,
                        p.JobTitleName,
                        p.Salary,
                        p.WorkingTime,
                        e.YearOfEmployment
                    FROM Employees e
                    INNER JOIN Genders g ON e.Gender = g.Id
                    INNER JOIN Departments d ON e.Department = d.ID
                    INNER JOIN Positions p ON e.JobTitle = p.Id
                    WHERE e.Id = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToEmployeeDto(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by ID: {EmployeeId}", id);
                throw;
            }
        }

        //public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
        //{
        //    try
        //    {
        //        using var connection = await _databaseService.CreateConnectionAsync();

        //        var sql = @"
        //            INSERT INTO Employees (Name, LastName, Surname, Gender, DateOfBirth, PhoneNumber, 
        //                                 Address, City, Department, ExperienceYear, JobTitle, YearOfEmployment)
        //            VALUES (@Name, @LastName, @Surname, @Gender, @DateOfBirth, @PhoneNumber, 
        //                   @Address, @City, @Department, @ExperienceYear, @JobTitle, @YearOfEmployment);
        //            SELECT SCOPE_IDENTITY();";

        //        using var command = new SqlCommand(sql, connection);
        //        AddEmployeeParameters(command, request);
        //        command.Parameters.AddWithValue("@YearOfEmployment", DateTime.Now.Date);

        //        var newId = Convert.ToInt32(await command.ExecuteScalarAsync());

        //        // Получаем созданного сотрудника с полной информацией
        //        var createdEmployee = await GetEmployeeByIdAsync(newId);
        //        return createdEmployee!;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating employee");
        //        throw;
        //    }
        //}
        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("Creating employee: {@Request}", request);

                using var connection = await _databaseService.CreateConnectionAsync();

                // Убираем SCOPE_IDENTITY() - используем простую вставку
                var sql = @"
            INSERT INTO Employees (Name, LastName, Surname, Gender, DateOfBirth, PhoneNumber, 
                                 Address, City, Department, ExperienceYear, JobTitle, YearOfEmployment)
            VALUES (@Name, @LastName, @Surname, @Gender, @DateOfBirth, @PhoneNumber, 
                   @Address, @City, @Department, @ExperienceYear, @JobTitle, @YearOfEmployment)";

                using var command = new SqlCommand(sql, connection);
                AddEmployeeParameters(command, request);
                command.Parameters.AddWithValue("@YearOfEmployment", DateTime.Now.Date);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to insert employee");
                }

                // Находим созданного сотрудника по уникальным данным
                var employees = await GetAllEmployeesAsync();
                var createdEmployee = employees
                    .Where(e => e.Name == request.Name &&
                               e.LastName == request.LastName &&
                               e.PhoneNumber == request.PhoneNumber)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefault();

                if (createdEmployee == null)
                {
                    throw new Exception("Created employee not found");
                }

                return createdEmployee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                throw;
            }
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    UPDATE Employees 
                    SET Name = @Name, 
                        LastName = @LastName, 
                        Surname = @Surname,
                        Gender = @Gender,
                        DateOfBirth = @DateOfBirth,
                        PhoneNumber = @PhoneNumber,
                        Address = @Address,
                        City = @City,
                        Department = @Department,
                        ExperienceYear = @ExperienceYear,
                        JobTitle = @JobTitle,
                        YearOfEmployment = @YearOfEmployment
                    WHERE Id = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                AddEmployeeParameters(command, request);
                command.Parameters.AddWithValue("@YearOfEmployment", request.YearOfEmployment);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                    return null;

                return await GetEmployeeByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = "DELETE FROM Employees WHERE Id = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string departmentName)
        {
            var employees = new List<EmployeeDto>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();

                var sql = @"
                    SELECT 
                        e.Id,
                        e.Name,
                        e.LastName,
                        e.Surname,
                        g.Gender,
                        e.DateOfBirth,
                        e.PhoneNumber,
                        e.Address,
                        e.City,
                        d.Department as DepartmentName,
                        e.ExperienceYear,
                        p.JobTitleName,
                        p.Salary,
                        p.WorkingTime,
                        e.YearOfEmployment
                    FROM Employees e
                    INNER JOIN Genders g ON e.Gender = g.Id
                    INNER JOIN Departments d ON e.Department = d.ID
                    INNER JOIN Positions p ON e.JobTitle = p.Id
                    WHERE d.Department LIKE @DepartmentName
                    ORDER BY e.LastName, e.Name";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DepartmentName", $"%{departmentName}%");

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapToEmployeeDto(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees by department: {Department}", departmentName);
                throw;
            }

            return employees;
        }

        // Вспомогательные методы для получения справочных данных
        public async Task<IEnumerable<Gender>> GetGendersAsync()
        {
            var genders = new List<Gender>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();
                var sql = "SELECT Id, Gender as GenderName FROM Genders ORDER BY Gender";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    genders.Add(new Gender
                    {
                        Id = reader.GetInt32("Id"),
                        GenderName = reader.GetString("GenderName")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genders");
                throw;
            }

            return genders;
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

        public async Task<IEnumerable<Position>> GetPositionsAsync()
        {
            var positions = new List<Position>();

            try
            {
                using var connection = await _databaseService.CreateConnectionAsync();
                var sql = @"SELECT Id, JobTitleName, Salary, WorkingTime 
                           FROM Positions ORDER BY JobTitleName";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    positions.Add(new Position
                    {
                        Id = reader.GetInt32("Id"),
                        JobTitleName = reader.GetString("JobTitleName"),
                        Salary = reader.GetDecimal("Salary"),
                        WorkingTime = reader.GetInt32("WorkingTime")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting positions");
                throw;
            }

            return positions;
        }

        private static EmployeeDto MapToEmployeeDto(SqlDataReader reader)
        {
            return new EmployeeDto
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                LastName = reader.GetString("LastName"),
                Surname = reader.IsDBNull("Surname") ? null : reader.GetString("Surname"),
                Gender = reader.GetString("Gender"),
                DateOfBirth = reader.GetDateTime("DateOfBirth"),
                PhoneNumber = reader.GetString("PhoneNumber"),
                Address = reader.GetString("Address"),
                City = reader.GetString("City"),
                DepartmentName = reader.GetString("DepartmentName"),
                ExperienceYear = reader.GetInt32("ExperienceYear"),
                JobTitleName = reader.GetString("JobTitleName"),
                Salary = reader.GetDecimal("Salary"),
                WorkingTime = reader.GetInt32("WorkingTime"),
                YearOfEmployment = reader.GetDateTime("YearOfEmployment")
            };
        }

        private static void AddEmployeeParameters(SqlCommand command, CreateEmployeeRequest request)
        {
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@LastName", request.LastName);
            command.Parameters.AddWithValue("@Surname", (object?)request.Surname ?? DBNull.Value);
            command.Parameters.AddWithValue("@Gender", request.Gender);
            command.Parameters.AddWithValue("@DateOfBirth", request.DateOfBirth);
            command.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber);
            command.Parameters.AddWithValue("@Address", request.Address);
            command.Parameters.AddWithValue("@City", request.City);
            command.Parameters.AddWithValue("@Department", request.Department);
            command.Parameters.AddWithValue("@ExperienceYear", request.ExperienceYear);
            command.Parameters.AddWithValue("@JobTitle", request.JobTitle);
        }

        private static void AddEmployeeParameters(SqlCommand command, UpdateEmployeeRequest request)
        {
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@LastName", request.LastName);
            command.Parameters.AddWithValue("@Surname", (object?)request.Surname ?? DBNull.Value);
            command.Parameters.AddWithValue("@Gender", request.Gender);
            command.Parameters.AddWithValue("@DateOfBirth", request.DateOfBirth);
            command.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber);
            command.Parameters.AddWithValue("@Address", request.Address);
            command.Parameters.AddWithValue("@City", request.City);
            command.Parameters.AddWithValue("@Department", request.Department);
            command.Parameters.AddWithValue("@ExperienceYear", request.ExperienceYear);
            command.Parameters.AddWithValue("@JobTitle", request.JobTitle);
        }
    }
}