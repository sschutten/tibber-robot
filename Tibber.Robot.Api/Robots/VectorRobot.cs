using Tibber.Robot.Api.Models;
using Vertex = (int Start, int End);

namespace Tibber.Robot.Api.Robots;

/// <summary>
/// A robot that uses a vector-based approach to keep track of cleaned places.
/// The robot keeps track of all horizontal lines cleaned by the robot.
/// For counting the total unique places cleaned, the robot merges overlapping lines
/// and them sums the length of all lines.
/// </summary>
public class VectorRobot : AbstractRobot
{
    public Dictionary<int, List<Vertex>> Vertices { get; } = [];

    public VectorRobot(Point start) : base(start)
    {
        AddVertex(AbsolutePosition.Y, AbsolutePosition.X, AbsolutePosition.X);
    }

    /// <inheritdoc />
    public override void Move(Direction direction, int steps)
    {
        if (direction == Direction.West)
        {
            AddVertex(AbsolutePosition.Y, AbsolutePosition.X - steps, AbsolutePosition.X);
            AbsolutePosition.X -= steps;
        }
        else if (direction == Direction.East)
        {
            AddVertex(AbsolutePosition.Y, AbsolutePosition.X, AbsolutePosition.X + steps);
            AbsolutePosition.X += steps;
        }
        else
        {
            var dY = direction switch
            {
                Direction.North => 1,
                Direction.South => -1,
                _ => throw new NotImplementedException(),
            };

            for (int i = 1; i <= steps; i++)
                AddVertex(AbsolutePosition.Y + i * dY, AbsolutePosition.X, AbsolutePosition.X);

            AbsolutePosition.Y += steps * dY;
        }
    }

    private void AddVertex(int row, int start, int end)
    {
        if (Vertices.TryGetValue(row, out List<Vertex>? values))
        {
            values.Add(new(start, end));
        }
        else
        {
            Vertices[row] = [new(start, end)];
        }
    }

    /// <inheritdoc />
    public override long GetTotalUniqueCleanedPlaces()
    {
        // Merge vertices
        foreach (var row in Vertices.Values)
        {
            row.Sort((source, other) => source.Start - other.Start);

            for (int i = row.Count - 1; i > 0; i--)
            {
                var current = row[i];
                var previous = row[i - 1];
                if (current.Start <= previous.End + 1)
                {
                    row.RemoveAt(i);
                    row[i - 1] = (previous.Start, current.End);
                }
            }
        }

        // Sum the length of all vertices
        return Vertices.Values
            .SelectMany(v => v)
            .Sum(v => v.End - v.Start + 1);
    }
}
