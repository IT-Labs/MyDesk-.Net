using MyDesk.Data.Entities;
using MyDesk.Data.Entities.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MyDesk.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(GetOptions(options, httpContextAccessor))
        {
        }

        private static DbContextOptions<ApplicationDbContext> GetOptions(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        {

            // TODO: This code should be reviwed, as we cannot pass connection string on HEADER.
            // There should be a mapping between Tenant name and Connection String
            // The Authorization Middleware that was previosly used was addressing this issue, but it was removed.

            //string tenant = httpContextAccessor?.HttpContext?.Items["tenant"]?.ToString();
            //if (!string.IsNullOrEmpty(tenant))
            //{
            //    DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //    dbContextOptionsBuilder.UseSqlServer(tenant);

            //    return dbContextOptionsBuilder.Options;
            //}

            return options;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public virtual DbSet<Desk> Desks { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ConferenceRoomConfiguration());
            modelBuilder.ApplyConfiguration(new DeskConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new OfficeConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
