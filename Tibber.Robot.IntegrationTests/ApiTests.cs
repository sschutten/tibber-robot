using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Tibber.Robot.Api.Data;

namespace Tibber.Robot.IntegrationTests;

public class ApiTests : IAsyncLifetime
{
    private HttpClient _httpClient = default!;
    private RobotDbContext _dbContext = default!;
    private DistributedApplication? _app;

    public async Task DisposeAsync()
    {
        if (_app != null)
            await _app.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        // Initialize the app host and wait for the API to be running
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Tibber_Robot_AppHost>();

        _app = await appHost.BuildAsync();

        var resourceNotificationService = _app.Services
            .GetRequiredService<ResourceNotificationService>();

        await _app.StartAsync();

        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

        // Create an HTTP client for calling the API
        // and a DbContext for verifying the data in the database
        _httpClient = _app.CreateHttpClient("api");

        var dbConnectionString = await _app.GetConnectionStringAsync("db");
        var options = new DbContextOptionsBuilder<RobotDbContext>()
            .UseNpgsql(dbConnectionString)
            .Options;
        _dbContext = new RobotDbContext(options);
    }

    /// <summary>
    /// Ensures the enter-path endpoint returns the execution and stores it in the database.
    /// </summary>
    [Fact]
    public async Task Enter_path_stores_and_returns_execution()
    {
        // Arrange
        var request = new
        {
            start = new { x = 0, y = 0 },
            commands = new[]
            {
                new { direction = "north", step = 1 },
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/tibber-developer-test/enter-path", request);

        // Assert

        // Scrub duration because each test it's slightly different
        // also per verification the casing is a bit different, hence we're scrubbing 'duration' and 'Duration'
        var settings = new VerifySettings();
        settings.ScrubMembers("duration", "Duration");

        // Verify the API response is what we expect
        await Verify(response, settings).UseMethodName($"{CurrentMethodName()}_HttpResponseMessage");

        // Verify the data in the database is what we expect
        var execution = await _dbContext.Executions.FirstAsync();
        await Verify(execution, settings).UseMethodName($"{CurrentMethodName()}_Execution");
    }

    private static string CurrentMethodName(
        [CallerMemberName] string methodName = default!)
        => methodName;
}