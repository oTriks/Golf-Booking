using Microsoft.EntityFrameworkCore;
using Golf_Booking.Models;

public class BookingContext : DbContext
{
    public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

    public DbSet<GolfClub> GolfClubs { get; set; }
    public DbSet<GolfCourse> GolfCourses { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One Golf Club -> Many Courses
        modelBuilder.Entity<GolfCourse>()
            .HasOne(c => c.GolfClub)
            .WithMany(club => club.Courses)
            .HasForeignKey(c => c.GolfClubId)
            .OnDelete(DeleteBehavior.Cascade); // This line configures cascade delete


        // One Golf Course -> Many Bookings
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.GolfCourse)
            .WithMany()
            .HasForeignKey(b => b.GolfCourseId);

        base.OnModelCreating(modelBuilder);
    }

}
