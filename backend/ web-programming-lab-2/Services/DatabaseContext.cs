using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Guests;
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
    public DbSet<Guest> Guests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // rooms
        modelBuilder.Entity<Room>()
            .Property(r => r.Name)
            .HasColumnType("varchar(255)");
        modelBuilder.Entity<Room>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("ck_positive_price", "price >= 0");
                t.HasCheckConstraint("ck_positive_capacity", "capacity > 0");
            });


        // guests
        modelBuilder.Entity<Guest>()
            .Property(r => r.Name)
            .HasColumnType("varchar(255)");
        modelBuilder.Entity<Guest>()
            .HasIndex(g => g.Email)
            .IsUnique();
        modelBuilder.Entity<Guest>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("ck_email", "email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$'");
            });

        // reservations
        modelBuilder.Entity<Reservation>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("ck_checkout_after_checkin", "check_out > check_in");
                t.HasCheckConstraint("ck_positive_grand_total", "grand_total >= 0");
                t.HasCheckConstraint("ck_positive_guest_count", "guest_count > 0");
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

        modelBuilder.Entity<Reservation>()
            .HasOne(res => res.Guest)
            .WithMany(guest => guest.Reservations)
            .HasForeignKey(res => res.GuestId);
    }
}