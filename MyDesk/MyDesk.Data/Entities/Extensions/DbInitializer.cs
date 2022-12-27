namespace MyDesk.Data.Entities.Extensions
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, string adminEmail, string adminPassword)
        {
            if (string.IsNullOrEmpty(adminPassword) || string.IsNullOrEmpty(adminEmail))
            {
                return;
            }

            context.Database.EnsureCreated();

            if (context.Employees.Any(x => x.IsAdmin.HasValue && x.IsAdmin.Value && x.Email == adminEmail))
            {
                return;   // DB has been seeded
            }

            var employee = new Employee
            {
                JobTitle = "admin",
                FirstName = "Admin",
                LastName = "Employee",
                Email = adminEmail,
                Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                IsAdmin = true,
                IsDeleted = false
            };

            context.Employees.Add(employee);
            context.SaveChanges();
        }
    }
}
