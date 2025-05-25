using AutoMapper;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Endpoints;

public static class PlatformEndpoints
{
    const string tag = "Platform";
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/platforms", (IPlatformRepo repository, IMapper mapper) =>
        {
            var platforms = repository.GetAllPlatforms();
            return Results.Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        })
        .WithName("GetAllPlatforms")
        .WithTags(tag);

        app.MapGet("/api/platforms/{id}", (int id, IPlatformRepo repository, IMapper mapper) =>
        {
            var platform = repository.GetPlatformById(id);
            return platform is not null
                ? Results.Ok(mapper.Map<PlatformReadDto>(platform))
                : Results.NotFound();
        })
        .WithName("GetPlatformById")
        .WithTags(tag);

        app.MapGet("/api/platforms/byname/{name}", (string name, IPlatformRepo repository, IMapper mapper) =>
        {
            var platform = repository.GetPlatformByName(name);
            return platform is not null
                ? Results.Ok(mapper.Map<PlatformReadDto>(platform))
                : Results.NotFound();
        })
        .WithName("GetPlatformByName")
        .WithTags(tag);

        app.MapPost("/api/platforms", async (PlatformCreateDto platformDto, IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient) =>
        {
            var platform = mapper.Map<Platform>(platformDto);
            repository.CreatePlatform(platform);
            repository.SaveChanges();

            var platformReadDto = mapper.Map<PlatformReadDto>(platform);
            try
            {
                await commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send synchronously to Command Service. Continuing without sending: {e.Message}");
            }
            return Results.CreatedAtRoute("GetPlatformById", new { id = platformReadDto.Id }, platformReadDto);
        })
        .WithName("CreatePlatform")
        .WithTags(tag);
    }
}