using PlatformsService.Dtos;

namespace PlatformsService.SyncDataService.Http;

public interface ICommandDataClient
{
    // This interface is used to communicate with the command service
    Task SendPlatformToCommand(PlatformReadDto platform);
}