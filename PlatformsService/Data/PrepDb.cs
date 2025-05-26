using PlatformsService.Models;

namespace PlatformsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }

    private static void SeedData(AppDbContext context)
    {
        if (!context.Platforms.Any())
        {
            Console.WriteLine("Seeding data...");

            context.Platforms.AddRange(
                new Platform("DotNet", "Microsoft", "Free"),
                new Platform("SQL Server Express", "Microsoft", "Free"),
                new Platform("Kubernetes", "Cloud Native Computing Foundation", "Free")
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("We already have data.");
        }
    }
}