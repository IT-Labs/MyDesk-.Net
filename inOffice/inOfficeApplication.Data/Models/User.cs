using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class User  : BaseEntity
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Password { get; set; }

    }
}
