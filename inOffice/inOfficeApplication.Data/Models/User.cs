using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class User
    {
        [Required]
        //[MinLength(8)]
        public string? Email { get; set; }
        [Required]
        //[MinLength(2), MaxLength(25)]
        public string? FirstName { get; set; }
        [Required]
        //[MinLength(2), MaxLength(25)]
        public string? LastName { get; set; }

        [Required]
        //[MinLength(8)]
        public string? Password { get; set; }

    }
}
