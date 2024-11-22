namespace Tibber.Robot.Api.Models;

public class Execution
{
    public int Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required int Commands { get; set; }
    public required int Result { get; set; }
    public required TimeSpan Duration { get; set; }
}
