namespace MyDesk.Core.DTO
{
    public class PaginationDto<T> where T : class
    {
        public List<T> Values { get; set; }
        public int TotalCount { get; set; }
    }
}
