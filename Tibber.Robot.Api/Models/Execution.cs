namespace Tibber.Robot.Api.Models;

public class Execution
{
    public int Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int Commands { get; set; }
    public int Result { get; set; }
    public TimeSpan Duration { get; set; }
}
