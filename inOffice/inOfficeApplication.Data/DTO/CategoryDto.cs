namespace inOfficeApplication.Data.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public bool? DoubleMonitor { get; set; }
        public bool? NearWindow { get; set; }
        public bool? SingleMonitor { get; set; }
        public bool? Unavailable { get; set; }
        public int? DeskId { get; set; }
    }
}
