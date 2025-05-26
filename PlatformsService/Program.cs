using Microsoft.EntityFrameworkCore;
using PlatformsService.Data;
using PlatformsService.Endpoints;
using PlatformsService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// get the Commands Service URL from configuration.
// This can be set in appsettings.json or as an environment variable.
// in kubernetes, it can be set as a ConfigMap or Secret.
Console.WriteLine($"Commands Service Endpoint: {builder.Configuration["CommandsService"]}");

var app = builder.Build();

PrepDb.PrepPopulation(app);

PlatformsEndpoints.MapEndpoints(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();