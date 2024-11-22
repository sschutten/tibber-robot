namespace Tibber.Robot.Api.Models;

public record PathRequest(Start Start)
{
    public Command[] Commands { get; set; } = [];
}

public record Start(int X, int Y);

public record Command(Direction Direction, int Steps);
