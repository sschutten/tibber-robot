using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tibber.Robot.Api;
using Tibber.Robot.Api.Models;

IConfig? config = null;
#if DEBUG
config = new DebugInProcessConfig();
#endif
var summary = BenchmarkRunner.Run<RobotBenchmarks>(config);

public class RobotBenchmarks
{
    private readonly PathRequest _lightRequest;
    private readonly PathRequest _heavyRequest;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public RobotBenchmarks()
    {
        _lightRequest = DehydrateRequest("robotcleanerpathlight.json");
        _heavyRequest = DehydrateRequest("robotcleanerpathheavy.json");
    }

    //[Benchmark]
    //public void Light() => EnterPath(_lightRequest);

    [Benchmark]
    public void Heavy() => EnterPath(_heavyRequest);

    private static void EnterPath(PathRequest request)
    {
        var robot = new Robot(request.Start.X, request.Start.Y);

        foreach (var command in request.Commands)
        {
            robot.Move(command.Direction, command.Steps);
        }

        robot.GetTotalUniqueCleanedPlaces();
    }

    private static PathRequest DehydrateRequest(string filename)
    {
        var assembly = typeof(RobotBenchmarks).Assembly;
        using var stream = assembly.GetManifestResourceStream($"Tibber.Robot.Benchmarks.{filename}") ?? throw new Exception("Can't load file");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<PathRequest>(json, _jsonSerializerOptions) ?? throw new Exception("Response could not be deserialized to an Execution");
    }
}


