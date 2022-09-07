﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using inOfficeApplication.Data;

#nullable disable

namespace inOfficeApplication.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool?>("DoubleMonitor")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool?>("NearWindow")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<bool?>("SingleMonitor")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<bool?>("Unavailable")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "DoubleMonitor", "NearWindow", "SingleMonitor", "IsDeleted", "Unavailable" }, "IX_Categories");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.ConferenceRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<int?>("IndexForOffice")
                        .HasColumnType("int");

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<int>("OfficeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "OfficeId" }, "IX_ConferenceRooms_OfficeId");

                    b.ToTable("ConferenceRoom", (string)null);
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Desk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CategorieId")
                        .HasColumnType("int");

                    b.Property<string>("Categories")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IndexForOffice")
                        .HasColumnType("int");

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<int>("OfficeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategorieId");

                    b.HasIndex(new[] { "OfficeId" }, "IX_Desks_OfficeId");

                    b.ToTable("Desk", (string)null);
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsAdmin")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(1)))");

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<string>("JobTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Employee", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@it-labs.com",
                            FirstName = "Admin",
                            IsAdmin = true,
                            IsDeleted = false,
                            JobTitle = "admin",
                            LastName = "Employee",
                            Password = "$2a$11$cIqMEmkIaW/xe7w3ORbbj.tWNWcKHOVXpGGz7ryksaAHVelzfG5sW"
                        });
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Office", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OfficeImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Office", (string)null);
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("ConferenceRoomId")
                        .HasColumnType("int");

                    b.Property<int?>("DeskId")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ConferenceRoomId" }, "IX_Reservations_ConferenceRoomId");

                    b.HasIndex(new[] { "DeskId" }, "IX_Reservations_DeskId");

                    b.HasIndex(new[] { "EmployeeId" }, "IX_Reservations_EmployeeId");

                    b.ToTable("Reservation", (string)null);
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool?>("IsDeleted")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<int>("ReservationId")
                        .HasColumnType("int");

                    b.Property<string>("ReviewOutput")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reviews")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ReservationId" }, "IX_Reviews_ReservationId");

                    b.ToTable("Review", (string)null);
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.ConferenceRoom", b =>
                {
                    b.HasOne("inOfficeApplication.Data.Entities.Office", "Office")
                        .WithMany("ConferenceRooms")
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Office");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Desk", b =>
                {
                    b.HasOne("inOfficeApplication.Data.Entities.Category", "Categorie")
                        .WithMany("Desks")
                        .HasForeignKey("CategorieId")
                        .HasConstraintName("FK_Desks_Category_CategorieId");

                    b.HasOne("inOfficeApplication.Data.Entities.Office", "Office")
                        .WithMany("Desks")
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Desks_Offices_OfficeId");

                    b.Navigation("Categorie");

                    b.Navigation("Office");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Reservation", b =>
                {
                    b.HasOne("inOfficeApplication.Data.Entities.ConferenceRoom", "ConferenceRoom")
                        .WithMany("Reservations")
                        .HasForeignKey("ConferenceRoomId");

                    b.HasOne("inOfficeApplication.Data.Entities.Desk", "Desk")
                        .WithMany("Reservations")
                        .HasForeignKey("DeskId")
                        .HasConstraintName("FK_Reservations_Desks_DeskId");

                    b.HasOne("inOfficeApplication.Data.Entities.Employee", "Employee")
                        .WithMany("Reservations")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Reservations_Employees_EmployeeId");

                    b.Navigation("ConferenceRoom");

                    b.Navigation("Desk");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Review", b =>
                {
                    b.HasOne("inOfficeApplication.Data.Entities.Reservation", "Reservation")
                        .WithMany("Reviews")
                        .HasForeignKey("ReservationId")
                        .IsRequired()
                        .HasConstraintName("FK_Reviews_Reservations_ReservationId");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Category", b =>
                {
                    b.Navigation("Desks");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.ConferenceRoom", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Desk", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Employee", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Office", b =>
                {
                    b.Navigation("ConferenceRooms");

                    b.Navigation("Desks");
                });

            modelBuilder.Entity("inOfficeApplication.Data.Entities.Reservation", b =>
                {
                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
