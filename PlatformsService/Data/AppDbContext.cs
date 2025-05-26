using Microsoft.EntityFrameworkCore;
using PlatformsService.Models;

namespace PlatformsService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<Platform> Platforms { get; set; }
}