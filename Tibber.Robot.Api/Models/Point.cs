namespace Tibber.Robot.Api.Models;

public record Point
{
    public Point() { }

    public Point(int x, int y) => (X, Y) = (x, y);

    public int X { get; set; }
    public int Y { get; set; }
}
