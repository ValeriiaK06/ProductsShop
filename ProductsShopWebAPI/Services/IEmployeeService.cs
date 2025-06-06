//using ProductsShopWebAPI.Models;

//namespace ProductsShopWebAPI.Services
//{
//    public interface IEmployeeService
//    {
//        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
//        Task<Employee?> GetEmployeeByIdAsync(int id);
//        Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request);
//        Task<Employee?> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
//        Task<bool> DeleteEmployeeAsync(int id);
//        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);
//    }
//}

using ProductsShopWebAPI.Models;

namespace ProductsShopWebAPI.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string department);

    
        Task<IEnumerable<Gender>> GetGendersAsync();
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<IEnumerable<Position>> GetPositionsAsync();
    }
}