using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Serialization;
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

builder.AddNpgsqlDbContext<RobotDbContext>("db");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/tibber-developer-test/enter-path", async (PathRequest request, [FromServices] RobotDbContext dbContext) =>
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Move the robot
        var robot = new Tibber.Robot.Api.Robots.HashRobot(request.Start);
        foreach (var command in request.Commands)
        {
            robot.Move(command.Direction, command.Steps);
        }

        stopwatch.Stop();

        // Save the result
        var result = new Execution
        {
            Commands = request.Commands.Length,
            Result = robot.GetTotalUniqueCleanedPlaces(),
            Duration = stopwatch.Elapsed,
            Timestamp = DateTimeOffset.UtcNow,
        };

        await dbContext.Executions.AddAsync(result);
        await dbContext.SaveChangesAsync();

        return result;
    })
    .WithName("EnterPath")
    .WithOpenApi()
    .Produces<Execution>();

app.Run();
