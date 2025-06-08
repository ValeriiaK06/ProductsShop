using ProductsShopWebAPI.Models;

namespace ProductsShopWebAPI.Services
{
    public interface IGoodsService
    {
        Task<IEnumerable<GoodsDto>> GetAllGoodsAsync();
        Task<GoodsDto?> GetGoodsByIdAsync(int id);
        Task<GoodsDto> CreateGoodsAsync(CreateGoodsRequest request);
        Task<GoodsDto?> UpdateGoodsAsync(int id, UpdateGoodsRequest request);
        Task<bool> DeleteGoodsAsync(int id);

        // Фильтрация по сроку годности
        Task<IEnumerable<GoodsDto>> GetExpiringGoodsAsync();
        Task<IEnumerable<GoodsDto>> GetNonExpiringGoodsAsync();

        // Анализ отдела
        Task<DepartmentAnalysisDto> GetDepartmentAnalysisAsync(string departmentName);

        // Справочные данные
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<IEnumerable<Country>> GetCountriesAsync();
    }
}
