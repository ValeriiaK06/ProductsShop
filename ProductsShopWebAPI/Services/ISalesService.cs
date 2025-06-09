using ProductsShopWebAPI.Models;

namespace ProductsShopWebAPI.Services
{
    public interface ISalesService
    {
        // Основные CRUD операции
        Task<IEnumerable<SalesDto>> GetAllSalesAsync();
        Task<SalesDto?> GetSalesByIdAsync(int id);
        Task<SalesDto> CreateSalesAsync(CreateSalesRequest request);
        Task<SalesDto?> UpdateSalesAsync(int id, UpdateSalesRequest request);
        Task<bool> DeleteSalesAsync(int id);

        // Аналитические методы
        Task<IEnumerable<EmployeeSalesAnalysisDto>> GetEmployeeSalesAnalysisAsync();
        Task<IEnumerable<SalesPeriodAnalysisDto>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate);

        // Работа с хранимыми процедурами
        Task<IEnumerable<GoodsDto>> GetProductsByDepartmentAndCountryAsync(int departmentId, int? countryId = null);
        Task<DepartmentWorkingStatusDto> CheckDepartmentWorkingStatusAsync(int departmentId, TimeSpan checkTime);

        // Фильтрация
        Task<IEnumerable<SalesDto>> GetSalesByEmployeeAsync(int employeeId);
        Task<IEnumerable<SalesDto>> GetSalesByProductAsync(int productId);
    }
}