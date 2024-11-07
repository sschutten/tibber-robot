namespace Tibber.Robot.Api.Models;

public record PathRequest(Start Start)
{
    public Commmand[] Commmands { get; set; } = [];
}

public record Start(int X, int Y);

public record Commmand(Direction Direction, int Steps);
