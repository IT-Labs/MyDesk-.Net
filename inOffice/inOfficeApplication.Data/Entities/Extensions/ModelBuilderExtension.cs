using Microsoft.EntityFrameworkCore;

namespace inOfficeApplication.Data.Entities.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void SeedAdminEmployee(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(new Employee()
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Employee",
                JobTitle = "admin",
                Email = "admin@it-labs.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
                IsAdmin = true,
                IsDeleted = false
            });
        }
    }
}
