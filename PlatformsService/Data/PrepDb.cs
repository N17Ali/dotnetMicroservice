using PlatformsService.Models;

namespace PlatformsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), logger);
    }

    private static void SeedData(AppDbContext context, ILogger<Program> logger)
    {
        if (!context.Platforms.Any())
        {
            logger.LogInformation("Seeding data...");

            context.Platforms.AddRange(
                new Platform() { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );

            context.SaveChanges();
        }
        else
        {
            logger.LogInformation("Data already exists. No seeding required.");
        }
    }
}