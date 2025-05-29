namespace CommandsService.Endpoints;

public static class PlatformEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("api/c/platforms", () =>
        {
            return Results.Ok("Inbound Test from platforms endpoint");
        });
    }
}