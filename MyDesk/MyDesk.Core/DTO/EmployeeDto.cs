namespace MyDesk.Core.DTO
{
    public class EmployeeDto
    {
        public int? Id { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsSSOAccount { get; set; }
    }
}
