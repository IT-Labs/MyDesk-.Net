using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inOfficeApplication.Data.Entities.Configurations
{
    public partial class ConferenceRoomConfiguration : IEntityTypeConfiguration<ConferenceRoom>
    {
        public void Configure(EntityTypeBuilder<ConferenceRoom> entity)
        {
            entity.ToTable("ConferenceRoom");

            entity.HasIndex(e => e.OfficeId, "IX_ConferenceRooms_OfficeId");

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            entity.HasOne(d => d.Office)
                .WithMany(p => p.ConferenceRooms)
                .HasForeignKey(d => d.OfficeId);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<ConferenceRoom> entity);
    }
}
