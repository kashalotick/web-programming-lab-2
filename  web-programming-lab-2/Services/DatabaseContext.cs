using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;


namespace web_programming_lab_2.Services;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("ck_positive_price", "price >= 0");
                t.HasCheckConstraint("ck_positive_capacity", "capacity >= 0");
            });


        modelBuilder.Entity<Reservation>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("ck_checkout_after_checkin", "check_out > check_in");
            });

        modelBuilder.Entity<Reservation>()
            .Property(r => r.CreatedAt)
            .HasDefaultValueSql("NOW()");
        modelBuilder.Entity<Reservation>()
            .Property(r => r.IsActive)
            .HasDefaultValue(true);


        modelBuilder.Entity<Reservation>()
            .HasOne(res => res.Room)
            .WithMany(room => room.Reservations)
            .HasForeignKey(res => res.RoomId);
    }
}