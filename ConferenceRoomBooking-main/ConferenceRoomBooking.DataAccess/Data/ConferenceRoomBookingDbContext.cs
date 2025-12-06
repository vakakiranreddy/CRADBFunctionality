using ConferenceRoomBooking.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.DataAccess.Data
{
    public class ConferenceRoomBookingDbContext : DbContext
    {
        public ConferenceRoomBookingDbContext(DbContextOptions<ConferenceRoomBookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserOtpVerification> UserOtpVerifications { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingCheckIn> BookingCheckIns { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRSVP> EventRSVPs { get; set; }
        public DbSet<BroadcastNotification> BroadcastNotifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserBookingStats> UserBookingStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.EmployeeId).IsUnique();
            });

            modelBuilder.Entity<UserBookingStats>(entity =>
            {
                entity.HasOne(s => s.User)
                    .WithOne(u => u.BookingStats)
                    .HasForeignKey<UserBookingStats>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => s.UserId).IsUnique();
            });
            // OTP
            modelBuilder.Entity<UserOtpVerification>(entity =>
            {
                entity.HasIndex(o => new { o.UserId, o.IsUsed });
            });

            // Location Hierarchy
            modelBuilder.Entity<Floor>(entity =>
            {
                entity.HasOne(f => f.Location)
                    .WithMany()
                    .HasForeignKey(f => f.LocationId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.Building)
                    .WithMany(b => b.Floors)
                    .HasForeignKey(f => f.BuildingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Resource
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasOne(r => r.Location)
                    .WithMany(l => l.Resources)
                    .HasForeignKey(r => r.LocationId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Building)
                    .WithMany(b => b.Resources)
                    .HasForeignKey(r => r.BuildingId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Floor)
                    .WithMany(f => f.Resources)
                    .HasForeignKey(r => r.FloorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Resource)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(b => b.StartTime);
                entity.HasIndex(b => b.SessionStatus);
                entity.HasIndex(b => new { b.ResourceId, b.StartTime, b.SessionStatus });
            });

            // Event
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasOne(e => e.Location)
                    .WithMany()
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Building)
                    .WithMany()
                    .HasForeignKey(e => e.BuildingId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Floor)
                    .WithMany()
                    .HasForeignKey(e => e.FloorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.Date);
            });

            // EventRSVP
            modelBuilder.Entity<EventRSVP>(entity =>
            {
                entity.HasOne(r => r.Event)
                    .WithMany(e => e.RSVPs)
                    .HasForeignKey(r => r.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => new { r.EventId, r.UserId }).IsUnique();
            });

            // Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(d => d.DepartmentName).IsUnique();
            });

            // User-Department relationship
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Department)
                    .WithMany(d => d.Users)
                    .HasForeignKey(u => u.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Notifications
            modelBuilder.Entity<BroadcastNotification>(entity =>
            {
                entity.HasIndex(n => n.CreatedAt);
                entity.HasIndex(n => n.Status);
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.HasIndex(n => n.UserId);
                entity.HasIndex(n => n.Status);
                entity.HasIndex(n => n.CreatedAt);
            });
        }
    }
}








