
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Railway_Reservation_Sys.Models;

namespace Railway_Reservation_Sys
{
    public class RailwayReservationDbContext : DbContext
    {
        public RailwayReservationDbContext(DbContextOptions<RailwayReservationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ReservationHeader> ReservationHeaders { get; set; }
        public DbSet<ReservationDetails> ReservationDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Train Relationships
            modelBuilder.Entity<Train>()
                .HasOne(t => t.SourceStation)
                .WithMany()
                .HasForeignKey(t => t.SourceID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Train>()
                .HasOne(t => t.DestinationStation)
                .WithMany()
                .HasForeignKey(t => t.DestinationID)
                .OnDelete(DeleteBehavior.NoAction);

            // ReservationHeader Relationships
            modelBuilder.Entity<ReservationHeader>()
                .HasOne(rh => rh.User)
                .WithMany(u => u.ReservationHeaders)
                .HasForeignKey(rh => rh.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationHeader>()
                .HasOne(rh => rh.Train)
                .WithMany(t => t.ReservationHeaders)
                .HasForeignKey(rh => rh.TrainID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationHeader>()
                .HasOne(rh => rh.Schedule)
                .WithMany(s => s.ReservationHeaders)
                .HasForeignKey(rh => rh.ScheduleID)
                .OnDelete(DeleteBehavior.Restrict);

            // ReservationDetails Relationships
            modelBuilder.Entity<ReservationDetails>()
                .HasOne(rd => rd.ReservationHeader)
                .WithMany(rh => rh.ReservationDetails)
                .HasForeignKey(rd => rd.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationDetails>()
                .HasOne(rd => rd.Passenger)
                .WithMany(p => p.ReservationDetails)
                .HasForeignKey(rd => rd.PassengerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationDetails>()
                .HasOne(rd => rd.Seat)
                .WithMany()
                .HasForeignKey(rd => rd.SeatID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationDetails>()
                .HasOne(rd => rd.Coach)
                .WithMany(rd=>rd.ReservationDetails)
                .HasForeignKey(rd => rd.CoachID)
                .OnDelete(DeleteBehavior.Restrict);

        }


    }
}





