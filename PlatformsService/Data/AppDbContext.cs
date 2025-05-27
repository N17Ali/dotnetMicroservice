using Microsoft.EntityFrameworkCore;
using PlatformsService.Models;

namespace PlatformsService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>().HasIndex(p => p.Name).IsUnique();

        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Platform> Platforms { get; set; }
}