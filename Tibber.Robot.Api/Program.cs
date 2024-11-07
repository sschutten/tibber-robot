using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Tibber.Robot.Api;
using Tibber.Robot.Api.Data;
using Tibber.Robot.Api.Models;
using Tibber.Robot.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHostedService<MigrationWorker>();

builder.AddNpgsqlDbContext<RobotDbContext>(connectionName: "db");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/tibber-developer-test/enter-path", (PathRequest request, [FromServices] RobotDbContext dbContext) =>
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var robot = new Robot(request.Start.X, request.Start.Y);
        foreach (var command in request.Commmands)
        {
            robot.Move(command.Direction, command.Steps);
        }

        stopwatch.Stop();

        return new CleanResult
        {
            Commands = request.Commmands.Length,
            Result = robot.UniquePlacesCleaned.Count,
            Duration = stopwatch.Elapsed,
            Timestamp = DateTimeOffset.UtcNow,
        };
    })
    .WithName("EnterPath")
    .WithOpenApi()
    .Produces<CleanResult>();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
