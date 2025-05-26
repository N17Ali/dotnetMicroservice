using PlatformsService.Models;

namespace PlatformsService.Data;

public class PlatformRepo(AppDbContext context) : IPlatformRepo
{
    private readonly AppDbContext _context = context;

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        _context.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Platform GetPlatformById(int id)
    {
        return _context.Platforms.FirstOrDefault(p => p.Id == id);
    }

    public Platform GetPlatformByName(string name)
    {
        return _context.Platforms.FirstOrDefault(p => p.Name == name);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}