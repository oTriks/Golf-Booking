using Microsoft.EntityFrameworkCore;
using GolfBookingAPI.Models;

public class BookingContext : DbContext
{
    public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

    public DbSet<GolfClub> GolfClubs { get; set; }
    public DbSet<GolfCourse> GolfCourses { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<WaitingList> WaitingLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GolfCourse>()
            .HasOne(c => c.GolfClub)
            .WithMany(club => club.Courses)
            .HasForeignKey(c => c.GolfClubId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.GolfCourse)
            .WithMany()
            .HasForeignKey(b => b.GolfCourseId);

        base.OnModelCreating(modelBuilder);
    }
}
