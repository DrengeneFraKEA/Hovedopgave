using Hovedopgave.Server.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Teams> Teams { get; set; }
    public DbSet<Organizations> Organizations { get; set; }
    public DbSet<Competitions> Competitions{ get; set; }
    public DbSet<User_Game_Profiles> User_Game_Profiles { get; set; }
    public DbSet<League_of_Legends_Profiles> League_of_Legends_Profiles { get; set; }
    public DbSet<Valorant_Profiles> Valorant_Profiles { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>().ToTable("users");
        modelBuilder.Entity<Teams>().ToTable("teams");
        modelBuilder.Entity<Organizations>().ToTable("organizations");
        modelBuilder.Entity<Competitions>().ToTable("competitions");
        modelBuilder.Entity<User_Game_Profiles>().ToTable("user_game_profiles");
        modelBuilder.Entity<League_of_Legends_Profiles>().ToTable("league_of_legends_profiles");
        modelBuilder.Entity<Valorant_Profiles>().ToTable("valorant_profiles");

    }
}
