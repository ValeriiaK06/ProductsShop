namespace ProductsShopWebAPI.Models
{
    public class Sales
    {
        public int ID { get; set; }
        public int Employee { get; set; }
        public int Product { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }

    public class SalesDto
    {
        public int ID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeFullName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public string FormattedDate => Date.ToString("dd.MM.yyyy");
        public string FormattedTotal => $"{Total:N2} грн";
        public string FormattedQuantity => $"{Quantity:N3}";
        public string FormattedPrice => $"{Price:N2} грн";
    }

    public class CreateSalesRequest
    {
        public int Employee { get; set; }
        public int Product { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateSalesRequest
    {
        public int Employee { get; set; }
        public int Product { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // DTO для аналитики продаж
    public class EmployeeSalesAnalysisDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalSales { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AveragePrice { get; set; }
        public string FormattedTotalAmount => $"{TotalAmount:N2} грн";
        public string FormattedAveragePrice => $"{AveragePrice:N2} грн";
        public string FormattedTotalQuantity => $"{TotalQuantity:N3}";
    }

    public class SalesPeriodAnalysisDto
    {
        public DateTime Date { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string FormattedDate => Date.ToString("dd.MM.yyyy");
        public string FormattedTotalAmount => $"{TotalAmount:N2} грн";
        public string FormattedTotalQuantity => $"{TotalQuantity:N3}";
    }

    // DTO для работы с процедурами
    public class DepartmentWorkingStatusDto
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string OpeningTime { get; set; } = string.Empty;
        public string ClosingTime { get; set; } = string.Empty;
        public string CheckedTime { get; set; } = string.Empty;
        public string WorkingStatus { get; set; } = string.Empty;
        public bool IsWorking => WorkingStatus == "Department works";
    }
}
