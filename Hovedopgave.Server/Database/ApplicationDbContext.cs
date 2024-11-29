using Hovedopgave.Server.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Teams> Teams { get; set; }
    public DbSet<Organizations> Organizations { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>().ToTable("users");
        modelBuilder.Entity<Teams>().ToTable("teams");
        modelBuilder.Entity<Organizations>().ToTable("organizations");
    }
}
