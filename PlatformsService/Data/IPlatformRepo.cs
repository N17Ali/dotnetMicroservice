using PlatformsService.Models;

namespace PlatformsService.Data;

public interface IPlatformRepo
{
    bool SaveChanges();
    IEnumerable<Platform> GetAllPlatforms();
    Platform GetPlatformById(int id);
    Platform GetPlatformByName(string name);
    void CreatePlatform(Platform platform);
}