using Advanced.Algorithms.Geometry;
using Tibber.Robot.Api.Models;
using Point = Tibber.Robot.Api.Models.Point;
using GeometryPoint = Advanced.Algorithms.Geometry.Point;

namespace Tibber.Robot.Api.Robots;

/// <summary>
/// The Bently Ottman robot uses the Bentley-Ottmann algorithm to find the total unique places cleaned.
/// During the movement, the robot creates a list of lines that represent the path it has taken.
/// When calculating the number of unique places cleaned, the robot calculates the sum of the lengths of all
/// lines and subtracts the number of intersections between the lines. For finding the number of intersections
/// it uses the <see href="https://en.wikipedia.org/wiki/Bentley%E2%80%93Ottmann_algorithm"/>Bentley-Ottmann algorithm</see>.
/// </summary>
public class BentleyOttmannRobot : AbstractRobot
{
    private readonly List<Line> _lines = [];

    public BentleyOttmannRobot(Point start) : base(start)
    {
        _lines.Add(new Line(new GeometryPoint(AbsolutePosition.X, AbsolutePosition.Y), new GeometryPoint(AbsolutePosition.X, AbsolutePosition.Y)));
    }

    /// <inheritdoc />
    public override void Move(Direction direction, int steps)
    {
        int dx = 0, dy = 0;
        switch (direction)
        {
            case Direction.North:
                dy = 1;
                break;
            case Direction.East:
                dx = 1;
                break;
            case Direction.South:
                dy = -1;
                break;
            case Direction.West:
                dx = -1;
                break;
        }

        var start = new GeometryPoint(AbsolutePosition.X + dx, AbsolutePosition.Y + dy);

        AbsolutePosition.X += (dx * steps);
        AbsolutePosition.Y += (dy * steps);

        _lines.Add(new Line(start, new GeometryPoint(AbsolutePosition.X, AbsolutePosition.Y)));
    }

    /// <inheritdoc />
    public override long GetTotalUniqueCleanedPlaces()
    {
        long placeCount = 0;
        foreach (var line in _lines)
        {
            placeCount += 1 + (long)Math.Abs(line.Right.X - line.Left.X) + (long)Math.Abs(line.Right.Y - line.Left.Y);
        }

        var bentleyOttmanAlgorithm = new BentleyOttmann();
        var intersections = bentleyOttmanAlgorithm.FindIntersections(_lines);

        return placeCount - intersections.Values.Sum(i => i.Count - 1);
    }
}