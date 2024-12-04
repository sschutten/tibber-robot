using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tibber.Robot.Api.Robots;
using Tibber.Robot.Api.Models;

IConfig? config = null;
#if DEBUG
config = new DebugInProcessConfig();
#endif

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);

[MemoryDiagnoser]
public class RobotBenchmarks
{
    private readonly PathRequest _lightRequest;
    private readonly PathRequest _mediumRequest;
    private readonly PathRequest _heavyRequest;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public RobotBenchmarks()
    {
        _lightRequest = DehydrateRequest("robotcleanerpathlight.json");
        _mediumRequest = DehydrateRequest("robotcleanerpathmedium.json");
        _heavyRequest = DehydrateRequest("robotcleanerpathheavy.json");
    }

    [Benchmark]
    public void LightBentleyOttmann() => EnterPath(new BentleyOttmannRobot(_lightRequest.Start), _lightRequest.Commands);

    [Benchmark]
    public void LightOptimizedBentleyOttmann() => EnterPath(new OptimizedBentleyOttmannRobot(_lightRequest.Start), _lightRequest.Commands);
    
    [Benchmark]
    public void LightHashRobot() => EnterPath(new HashRobot(_lightRequest.Start), _lightRequest.Commands);

    [Benchmark]
    public void LightSegmentedRobot() => EnterPath(new SegmentedRobot(_lightRequest.Start, 100), _lightRequest.Commands);

    [Benchmark]
    public void LightVectorRobot() => EnterPath(new VectorRobot(_lightRequest.Start), _lightRequest.Commands);

    [Benchmark]
    public void MediumBentleyOttmann() => EnterPath(new BentleyOttmannRobot(_mediumRequest.Start), _mediumRequest.Commands);

    [Benchmark]
    public void MediumOptimizedBentleyOttmann() => EnterPath(new OptimizedBentleyOttmannRobot(_mediumRequest.Start), _mediumRequest.Commands);
    
    [Benchmark]
    public void MediumHashRobot() => EnterPath(new HashRobot(_mediumRequest.Start), _mediumRequest.Commands);

    [Benchmark]
    public void MediumSegmentedRobot() => EnterPath(new SegmentedRobot(_mediumRequest.Start, 100), _mediumRequest.Commands);

    [Benchmark]
    public void MediumVectorRobot() => EnterPath(new VectorRobot(_mediumRequest.Start), _mediumRequest.Commands);

    [Benchmark]
    public void HeavyBentleyOttmann() => EnterPath(new BentleyOttmannRobot(_heavyRequest.Start), _heavyRequest.Commands);

    [Benchmark]
    public void HeavyOptimizedBentleyOttmann() => EnterPath(new OptimizedBentleyOttmannRobot(_heavyRequest.Start), _heavyRequest.Commands);

    private static void EnterPath(AbstractRobot robot, Command[] commands)
    {
        foreach (var command in commands)
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