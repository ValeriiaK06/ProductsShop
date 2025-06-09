using ProductsShopWebAPI.Models;

namespace ProductsShopWebAPI.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);

        Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentRequest request);
    }
}
