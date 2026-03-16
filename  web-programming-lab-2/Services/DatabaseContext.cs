using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Models;


namespace web_programming_lab_2.Services;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    public DbSet<Room> Rooms { get; set; }

}