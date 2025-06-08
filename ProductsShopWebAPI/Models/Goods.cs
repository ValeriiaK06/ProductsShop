namespace ProductsShopWebAPI.Models
{
    public class Goods
    {
        public int ID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Department { get; set; }
        public int Country { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public decimal Conditions_Temperature { get; set; }
        public bool Is_Expiring { get; set; }
        public int? Term { get; set; }
    }

    public class GoodsDto
    {
        public int ID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public decimal Conditions_Temperature { get; set; }
        public bool Is_Expiring { get; set; }
        public int? Term { get; set; }
        public string ExpirationStatus => Is_Expiring
            ? $"Так ({Term} днів)"
            : "Ні";
        public string TemperatureDisplay => $"{Conditions_Temperature}°C";
    }

    public class CreateGoodsRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public int Department { get; set; }
        public int Country { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public decimal Conditions_Temperature { get; set; }
        public bool Is_Expiring { get; set; }
        public int? Term { get; set; }
    }

    public class UpdateGoodsRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public int Department { get; set; }
        public int Country { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public decimal Conditions_Temperature { get; set; }
        public bool Is_Expiring { get; set; }
        public int? Term { get; set; }
    }

    public class DepartmentAnalysisDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int ExpiringProductCount { get; set; }
        public decimal AvgStorageTemp { get; set; }
        public int MaxTermDays { get; set; }
        public string Summary => ProductCount > 0
            ? $"Відділ {DepartmentName}: {ProductCount} товарів, {ExpiringProductCount} з терміном годності, середня температура {AvgStorageTemp:F1}°C, максимальний термін {MaxTermDays} днів"
            : $"Відділ {DepartmentName} не знайдено або немає товарів";
    }
}