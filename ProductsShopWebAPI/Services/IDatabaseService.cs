using Microsoft.Data.SqlClient;

namespace ProductsShopWebAPI.Services
{
    public interface IDatabaseService
    {
        SqlConnection CreateConnection();
        Task<SqlConnection> CreateConnectionAsync();
        Task<bool> TestConnectionAsync();
    }
}