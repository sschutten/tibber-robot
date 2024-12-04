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
public class OptimizedBentleyOttmannRobot : AbstractRobot
{
    private record Path(int pos, int start, int end, bool? isHorizontal = false)
    {
        public int Pos { get; set; } = pos;
        public int Start { get; set; } = start;
        public int End { get; set; } = end;
        public bool IsHorizontal { get; set; } = isHorizontal ?? false;
    }
    
    private readonly List<Path> _paths = [];

    public OptimizedBentleyOttmannRobot(Point start) : base(start)
    { 
        _paths.Add(new Path(AbsolutePosition.X, AbsolutePosition.Y, AbsolutePosition.Y));
    }

    /// <inheritdoc />
    public override void Move(Direction direction, int steps)
    {
        Path path = default!;
        switch (direction)
        {
            case Direction.North:
                path = new Path(AbsolutePosition.X, AbsolutePosition.Y, AbsolutePosition.Y + steps, false);
                AbsolutePosition.Y += steps;
                break;
            case Direction.East:
                path = new Path(AbsolutePosition.Y, AbsolutePosition.X, AbsolutePosition.X + steps, true);
                AbsolutePosition.X += steps;
                break;
            case Direction.South:
                path = new Path(AbsolutePosition.X, AbsolutePosition.Y - steps, AbsolutePosition.Y, false);
                AbsolutePosition.Y -= steps;
                break;
            case Direction.West:
                path = new Path(AbsolutePosition.Y, AbsolutePosition.X - steps, AbsolutePosition.X, true);
                AbsolutePosition.X -= steps;
                break;
        }

        _paths.Add(path);
    }

    /// <inheritdoc />
    public override long GetTotalUniqueCleanedPlaces()
    {
        var lines = MergePathsIntoLines();

        long placeCount = 0;
        foreach (var line in lines)
        {
            placeCount += 1 + (long)Math.Abs(line.Right.X - line.Left.X) + (long)Math.Abs(line.Right.Y - line.Left.Y);
        }

        var bentleyOttmanAlgorithm = new BentleyOttmann();
        var intersections = bentleyOttmanAlgorithm.FindIntersections(lines);

        return placeCount - intersections.Values.Sum(i => i.Count - 1);
    }

    private List<Line> MergePathsIntoLines()
    {
        // Divide the paths into horizontal and vertical groups
        var grouped = _paths.GroupBy(x => new { x.Pos, x.IsHorizontal }).ToArray();
        var lines = new List<Line>();

        // Merge the paths at each pos
        foreach (var paths in grouped)
        {
            var sortedPaths = paths.OrderBy(x => x.Start).ToList();

            for (var i = sortedPaths.Count - 1; i > 0; --i)
            {
                var current = sortedPaths[i];
                var previous = sortedPaths[i - 1];
                if (previous.End + 1 >= current.Start)
                {
                    previous.End = current.End;
                    sortedPaths.RemoveAt(i);
                }
            }

            // Add the remaining paths as lines
            foreach (var path in sortedPaths)
            {
                if (path.IsHorizontal)
                {
                    lines.Add(new Line(new GeometryPoint(path.Start, path.Pos), new GeometryPoint(path.End, path.Pos)));
                }
                else
                {
                    lines.Add(new Line(new GeometryPoint(path.Pos, path.Start), new GeometryPoint(path.Pos, path.End)));
                }
            }
        }

        return lines;
    }
}