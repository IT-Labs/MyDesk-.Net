using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyDesk.Data.Entities.Configurations
{
    public partial class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> entity)
        {
            entity.ToTable("Reservation");

            entity.HasIndex(e => e.ConferenceRoomId, "IX_Reservations_ConferenceRoomId");

            entity.HasIndex(e => e.DeskId, "IX_Reservations_DeskId");

            entity.HasIndex(e => e.EmployeeId, "IX_Reservations_EmployeeId");

            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.Property(e => e.EndDate).HasColumnType("date").HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            entity.HasOne(d => d.ConferenceRoom)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ConferenceRoomId);

            entity.HasOne(d => d.Desk)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.DeskId)
                .HasConstraintName("FK_Reservations_Desks_DeskId");

            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Reservations_Employees_EmployeeId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Reservation> entity);
    }
}
