using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Entities.Configurations;
using inOfficeApplication.Data.Entities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace inOfficeApplication.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public virtual DbSet<Desk> Desks { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ConferenceRoomConfiguration());
            modelBuilder.ApplyConfiguration(new DeskConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new OfficeConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());

            modelBuilder.SeedAdminEmployee();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
