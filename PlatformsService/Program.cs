using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PlatformsService.Data;
using PlatformsService.Dtos.Validation;
using PlatformsService.Endpoints;
using PlatformsService.Middleware;
using PlatformsService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandsDataClient, HttpCommandsDataClient>();
builder.Services.AddValidatorsFromAssemblyContaining<PlatformCreateDtoValidator>();

var app = builder.Build();

// get the Commands Service URL from configuration.
// This can be set in appsettings.json or as an environment variable.
// in kubernetes, it can be set as a ConfigMap or Secret.
app.Logger.LogInformation("Commands Service Endpoint: {Endpoint}", builder.Configuration["CommandsService"]);

PrepDb.PrepPopulation(app);

PlatformsEndpoints.MapEndpoints(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCustomErrorHandling();

app.Run();