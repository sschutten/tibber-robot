namespace Tibber.Robot.Api.Models;

public record PathRequest(Start Start)
{
    public Command[] Commands { get; set; } = [];
}

public record Start : Point;

public record Command(Direction Direction, int Steps);
