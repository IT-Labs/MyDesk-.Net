#nullable disable

namespace inOfficeApplication.Data.Entities
{
    public partial class Employee
    {
        public Employee()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsSSOAccount { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}