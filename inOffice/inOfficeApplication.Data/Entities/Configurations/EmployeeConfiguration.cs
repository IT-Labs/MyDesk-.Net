using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inOfficeApplication.Data.Entities.Configurations
{
    public partial class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> entity)
        {
            entity.ToTable("Employee");

            entity.Property(e => e.Email).IsRequired();

            entity.Property(e => e.FirstName).IsRequired();

            entity.Property(e => e.IsAdmin)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(1)))");

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            entity.Property(e => e.LastName).IsRequired();

            entity.Property(e => e.Password).IsRequired();

            entity.HasIndex(e => e.Email, "IX_Employee_Email").IsUnique();

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Employee> entity);
    }
}
