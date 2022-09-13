using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inOfficeApplication.Data.Entities.Configurations
{
    public partial class DeskConfiguration : IEntityTypeConfiguration<Desk>
    {
        public void Configure(EntityTypeBuilder<Desk> entity)
        {
            entity.ToTable("Desk");

            entity.HasIndex(e => e.OfficeId, "IX_Desks_OfficeId");

            entity.Property(e => e.Categories).IsRequired();

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            entity.HasOne(d => d.Categorie)
                .WithMany(p => p.Desks)
                .HasForeignKey(d => d.CategorieId)
                .HasConstraintName("FK_Desks_Category_CategorieId");

            entity.HasOne(d => d.Office)
                .WithMany(p => p.Desks)
                .HasForeignKey(d => d.OfficeId)
                .HasConstraintName("FK_Desks_Offices_OfficeId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Desk> entity);
    }
}
