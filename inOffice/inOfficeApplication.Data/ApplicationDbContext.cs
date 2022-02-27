﻿using inOfficeApplication.Data.Models;
using inOfficeApplication.Models;
using Microsoft.EntityFrameworkCore;


namespace inOfficeApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<Mode> Modes { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Desk> Desks { get; set; }
        public virtual DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public virtual DbSet<ConferenceRoomMode> ConferenceRoomModes { get; set; }
        public virtual DbSet<DeskMode> DeskModes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //ONE TO ONE
            //reservation - review: 1-1
            builder.Entity<Reservation>()
                .HasOne<Review>(z => z.Review)
                .WithOne(z => z.Reservation)
                .HasForeignKey<Review>(z => z.ReservationId);

            //reservation - desk: 1-1
            builder.Entity<Reservation>()
                .HasOne<Desk>(z => z.Desk)
                .WithOne(z => z.Reservation)
                .HasForeignKey<Desk>(z => z.ReservationId);

            //reservation - conferenceroom : 1-1
            builder.Entity<Reservation>()
                .HasOne<ConferenceRoom>(z => z.ConferenceRoom)
                .WithOne(z => z.Reservation)
                .HasForeignKey<ConferenceRoom>(z => z.ReservationId);


            //ONE TO MANY
            //employee - reservation: 1-N
            builder.Entity<Reservation>()
                .HasOne<Employee>(z => z.Employee)
                .WithMany(z => z.Reservations)
                .HasForeignKey(z => z.EmployeeId);

            //office - desk : 1-N
            builder.Entity<Desk>()
                .HasOne<Office>(z => z.Office)
                .WithMany(z => z.Desks)
                .HasForeignKey(z => z.OfficeId);

            //office - mode: 1-N
            builder.Entity<Mode>()
                .HasOne<Office>(z => z.Office)
                .WithMany(z => z.Modes)
                .HasForeignKey(z => z.OfficeId);

            //office - conf room: 1-N
            builder.Entity<ConferenceRoom>()
                .HasOne<Office>(z => z.Office)
                .WithMany(z => z.ConferenceRooms)
                .HasForeignKey(z => z.OfficeId);

            builder.Entity<DeskMode>()
                            .HasKey(z => new { z.DeskId, z.ModeId });

            builder.Entity<DeskMode>()
                           .HasOne(z => z.Desk)
                           .WithMany(z => z.DeskModes)
                           .HasForeignKey(z => z.DeskId)
                           .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<DeskMode>()
                           .HasOne(z => z.Mode)
                           .WithMany(z => z.DeskModes)
                           .HasForeignKey(z => z.ModeId)
                           .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<ConferenceRoomMode>()
                            .HasKey(z => new {z.ConferenceRoomId, z.ModeId});

            builder.Entity<ConferenceRoomMode>()
                            .HasOne(z => z.ConferenceRoom)
                            .WithMany(z => z.ConferenceRoomModes)
                            .HasForeignKey(z => z.ConferenceRoomId)
                            .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<ConferenceRoomMode>()
                            .HasOne(z => z.Mode)
                            .WithMany(z => z.ConferenceRoomModes)
                            .HasForeignKey(z => z.ModeId)
                            .OnDelete(DeleteBehavior.ClientCascade);



            //MANY TO MANY
            //desk-mode : n-m
            /*            builder.Entity<DeskMode>()
                             .HasKey(z => new { z.DeskId, z.ModeId });

                        builder.Entity<DeskMode>()
                            .HasOne(z => z.Desk)
                            .WithMany(z => z.DeskModes)
                            .HasForeignKey(z => z.DeskId);

                        builder.Entity<DeskMode>()
                            .HasOne(z => z.Mode)
                            .WithMany(z => z.DeskModes)
                            .HasForeignKey(z => z.ModeId);


                        //conf room-mode: n-m
                        builder.Entity<ConferenceRoomMode>()
                            .HasKey(z => new { z.ConferenceRoomId, z.ModeId });

                        builder.Entity<ConferenceRoomMode>()
                            .HasOne(z => z.ConferenceRoom)
                            .WithMany(z => z.ConferenceRoomModes)
                            .HasForeignKey(z => z.ConferenceRoomId)
                            .OnDelete(DeleteBehavior.ClientCascade);

                        builder.Entity<ConferenceRoomMode>()
                            .HasOne(z => z.Mode)
                            .WithMany(z => z.ConferenceRoomModes)
                            .HasForeignKey(z => z.ModeId)
                            .OnDelete(DeleteBehavior.ClientCascade);*/



        }
    }
}