using ProductsShopWebAPI.Models;

namespace ProductsShopWebAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private static List<Employee> _employees = new()
        {
            new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@company.com",
                Department = "Engineering",
                Position = "Senior Developer",
                Salary = 85000,
                HireDate = DateTime.Now.AddYears(-3),
                IsActive = true
            },
            new Employee
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@company.com",
                Department = "Marketing",
                Position = "Marketing Manager",
                Salary = 75000,
                HireDate = DateTime.Now.AddYears(-2),
                IsActive = true
            },
            new Employee
            {
                Id = 3,
                FirstName = "Mike",
                LastName = "Johnson",
                Email = "mike.johnson@company.com",
                Department = "Engineering",
                Position = "DevOps Engineer",
                Salary = 80000,
                HireDate = DateTime.Now.AddYears(-1),
                IsActive = true
            },
            new Employee
            {
                Id = 4,
                FirstName = "Sarah",
                LastName = "Williams",
                Email = "sarah.williams@company.com",
                Department = "HR",
                Position = "HR Specialist",
                Salary = 60000,
                HireDate = DateTime.Now.AddMonths(-8),
                IsActive = true
            },
            new Employee
            {
                Id = 5,
                FirstName = "David",
                LastName = "Brown",
                Email = "david.brown@company.com",
                Department = "Finance",
                Position = "Financial Analyst",
                Salary = 70000,
                HireDate = DateTime.Now.AddMonths(-6),
                IsActive = false
            }
        };

        private static int _nextId = 6;

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            // Simulate async operation
            await Task.Delay(10);
            return _employees.Where(e => e.IsActive).ToList();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            await Task.Delay(10);
            return _employees.FirstOrDefault(e => e.Id == id && e.IsActive);
        }

        public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            await Task.Delay(10);

            var employee = new Employee
            {
                Id = _nextId++,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Department = request.Department,
                Position = request.Position,
                Salary = request.Salary,
                HireDate = DateTime.Now,
                IsActive = true
            };

            _employees.Add(employee);
            return employee;
        }

        public async Task<Employee?> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
        {
            await Task.Delay(10);

            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return null;

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Department = request.Department;
            employee.Position = request.Position;
            employee.Salary = request.Salary;
            employee.IsActive = request.IsActive;

            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            await Task.Delay(10);

            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return false;

            // Soft delete - just mark as inactive
            employee.IsActive = false;
            return true;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            await Task.Delay(10);
            return _employees.Where(e => e.IsActive &&
                                   e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                            .ToList();
        }
    }
}