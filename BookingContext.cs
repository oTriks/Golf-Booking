// BookingContext.cs

using Microsoft.EntityFrameworkCore;

public class BookingContext : DbContext
{
    public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

    public DbSet<GolfBooking> GolfBookings { get; set; }
}
