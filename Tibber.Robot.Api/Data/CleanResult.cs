namespace Tibber.Robot.Api.Data;

public class CleanResult
{
    public int Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int Commands { get; set; }
    public int Result { get; set; }
    public TimeSpan Duration { get; set; }
}
