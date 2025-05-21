using AutoMapper;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Endpoints;

public static class PlatformEndpoints
{
    const string tag = "Platform";
    public static void MapEndpoints(WebApplication app)
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

        app.MapPost("/api/platforms", (PlatformCreateDto platformDto, IPlatformRepo repository, IMapper mapper) =>
        {
            var platform = mapper.Map<Platform>(platformDto);
            repository.CreatePlatform(platform);
            repository.SaveChanges();

            var platformReadDto = mapper.Map<PlatformReadDto>(platform);
            return Results.CreatedAtRoute("GetPlatformById", new { id = platformReadDto.Id }, platformReadDto);
        })
        .WithName("CreatePlatform")
        .WithTags(tag);
    }
}