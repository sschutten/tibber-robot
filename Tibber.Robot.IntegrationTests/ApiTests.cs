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
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Tibber_Robot_AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync();

        var resourceNotificationService = _app.Services
            .GetRequiredService<ResourceNotificationService>();

        await _app.StartAsync();
        
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

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
    /// <returns></returns>
    [Fact]
    public async Task Enter_path_stores_and_returns_execution()
    {
        // Arrange
        var request = new
        {
            start = new { x = 0, y = 0 },
            commmands = new[]
            {
                new { direction = "North", step = 1 },
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/tibber-developer-test/enter-path", request);

        // Assert
        var settings = new VerifySettings();
        settings.ScrubMembers("duration", "Duration"); // Scrub duration because each test it's slightly different

        await Verify(response, settings).UseMethodName($"{CurrentMethodName()}_HttpResponseMessage");

        var execution = await _dbContext.Executions.FirstAsync();
        await Verify(execution, settings).UseMethodName($"{CurrentMethodName()}_Execution");
    }

    private static string CurrentMethodName(
        [CallerMemberName] string methodName = default!)
        => methodName;
}