using PlatformsService.Dtos;

namespace PlatformsService.SyncDataService.Http;

public interface ICommandsDataClient
{
    // This interface is used to communicate with the command service
    Task SendPlatformToCommands(PlatformReadDto platform);
}