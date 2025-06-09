namespace ProductsShopWebAPI.Models
{
    public class DepartmentDto
    {
        public int ID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int LocationRow { get; set; }
        public bool Sales { get; set; }
        public int Counters { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string FormattedStartTime => StartTime.ToString(@"hh\:mm");
        public string FormattedEndTime => EndTime.ToString(@"hh\:mm");
        public string SalesStatus => Sales ? "Так" : "Ні";
        public string WorkingHours => $"{FormattedStartTime} - {FormattedEndTime}";
    }

    public class UpdateDepartmentRequest
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int LocationRow { get; set; }
        public bool Sales { get; set; }
        public int Counters { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}