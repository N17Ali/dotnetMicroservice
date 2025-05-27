using PlatformsService.Exceptions;
using PlatformsService.Models;

namespace PlatformsService.Data;

public class PlatformRepo(AppDbContext context) : IPlatformRepo
{
    private readonly AppDbContext _context = context;

    public void CreatePlatform(Platform platform)
    {
        if (_context.Platforms.Any(p => p.Name == platform.Name))
        {
            throw new DuplicateResourceException($"Platform with name {platform.Name} already exists. ");
        }
        _context.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return [.. _context.Platforms];
    }

    public Platform GetPlatformById(int id)
    {
        return _context.Platforms.FirstOrDefault(p => p.Id == id) ?? throw new ResourceNotFoundException($"Platform with id {id} not found.");
    }

    public Platform GetPlatformByName(string name)
    {
        return _context.Platforms.FirstOrDefault(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) ?? throw new ResourceNotFoundException($"Platform with name {name} not found.");
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}