using Microsoft.EntityFrameworkCore;
using BadmintonCourtBookingSystem.Models;

namespace BadmintonCourtBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Court> Courts => Set<Court>();
        public DbSet<CourtImage> CourtImages => Set<CourtImage>();
        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Court>().ToTable("Courts");
            modelBuilder.Entity<CourtImage>().ToTable("CourtImages");
            modelBuilder.Entity<TimeSlot>().ToTable("TimeSlots");
            modelBuilder.Entity<Booking>().ToTable("Bookings");
            modelBuilder.Entity<Review>().ToTable("Reviews");
            modelBuilder.Entity<Notification>().ToTable("Notifications");

            modelBuilder.Entity<Booking>()
    .Property(b => b.Amount)
    .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Court>()
                .Property(c => c.HourlyRate)
                .HasColumnType("decimal(18,2)");

            

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName)
                .HasColumnName("RoleName");

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new
                {
                    b.CourtId,
                    b.BookingDate,
                    b.TimeSlotId
                })
                .IsUnique();

            modelBuilder.Entity<CourtImage>(entity =>
            {
                entity.ToTable("CourtImages");

                entity.HasKey(e => e.ImageId);
            });
        }
    }


}