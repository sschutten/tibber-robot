using System.Diagnostics.CodeAnalysis;
using Tibber.Robot.Api.Models;
using Point = (int X, int Y);
using Vertex = (int Start, int End);

namespace Tibber.Robot.Api;

//class VertexComparer : IEqualityComparer<Vertex>
//{
//    public bool Equals(Vertex x, Vertex y)
//    {
//        return x.X == y.X && x.Y == y.Y;
//    }

//    public int GetHashCode([DisallowNull] (int X, int Y) obj)
//    {
//        return obj.GetHashCode();
//    }
//}

public class Robot
{
    private int _x;
    private int _y;

    public Point Position => (X: _x, Y: _y);

    public Dictionary<int, SortedList<int, Vertex>> Vertices { get; } = [];

    /// <summary>
    /// The unique places cleaned by the robot.
    /// </summary>
    public Dictionary<int, HashSet<int>> UniquePlacesCleaned { get; } = [];

    public Robot(int x, int y)
    {
        _x = x;
        _y = y;
        AddVertex(y, x, x);
    }

    public void Move(Direction direction, int steps)
    {
        if (direction == Direction.West)
        {
            AddVertex(_y, _x - steps, _x);
            _x -= steps;
        }
        else if (direction == Direction.East)
        {
            AddVertex(_y, _x, _x + steps);
            _x += steps;
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
                AddVertex(_y + i * dY, _x, _x);

            _y += steps * dY;
        }
    }

    private void AddVertex(int row, int start, int end)
    {
        if (Vertices.TryGetValue(row, out SortedList<int, (int Start, int End)>? list))
        {
            // Remove any vertices that fall completely within the new vertex
            var consumedVertices = list.Where(v => v.Key >= start && v.Key <= end && v.Value.End <= end).ToArray();
            foreach (var vertex in consumedVertices)
                list.Remove(vertex.Key);

            // Merge any vertices that overlap with the new vertex
            var overlappingVertices = list.Where(v => (v.Key <= start && v.Value.End >= start) || (v.Key <= end && v.Value.End >= end)).ToArray();
            foreach (var vertex in overlappingVertices)
            {
                start = Math.Min(start, vertex.Key);
                end = Math.Max(end, vertex.Value.End);
                list.Remove(vertex.Key);
            }
            list.Add(start, (start, end));
        }
        else
        {
            Vertices[row] = new SortedList<int, Vertex>() { { start, (start, end) } };
        }
    }

    public int GetTotalUniqueCleanedPlaces()
    {
        return Vertices.Values
            .SelectMany(v => v.Values)
            .Sum(v => v.End - v.Start + 1);
    }
}

///// <summary>
///// The robot simulated moving in an office space and cleans the places it visits.
///// The path of the robot's movement is described by the starting coordinates and move commands.
///// After the cleaning has been done, the unique places cleaned can be retrieved.
///// </summary>
//public class Robot
//{
//    public Robot(int x, int y) : this((x, y))
//    {
//    }

//    public Robot(Vertex vertex)
//    {
//        Position = vertex;
//    }

//    public Vertex Position { get; private set; }

//    /// <summary>
//    /// The unique places cleaned by the robot.
//    /// </summary>
//    public HashSet<Vertex> UniquePlacesCleaned { get; } = new HashSet<Vertex>(new VertexComparer());

//    /// <summary>
//    /// Moves the robot in the specified direction for the specified number of steps.
//    /// </summary>
//    /// <param name="direction">The direction for the robot to move in</param>
//    /// <param name="steps">The number of steps to take into the direction</param>
//    public void Move(Direction direction, int steps)
//    {
//        int startX = Position.X;
//        int startY = Position.Y;

//        int dx = 0, dy = 0;
//        switch (direction)
//        {
//            case Direction.North:
//                dy = 1;
//                break;
//            case Direction.East:
//                dx = 1;
//                break;
//            case Direction.South:
//                dy = -1;
//                break;
//            case Direction.West:
//                dx = -1;
//                break;
//        }

//        for (int i = 1; i <= steps; i++)
//        {
//            UniquePlacesCleaned.Add((startX + i * dx, startY + i * dy));
//        }

//        Position = (startX + steps * dx, startY + steps * dy);
//    }

// Store ranges of cleaned areas as line segments
//public List<(int StartX, int StartY, int EndX, int EndY)> CleanedSegments { get; } = [];

//public void Move(Direction direction, int steps)
//{
//    int startX = Position.X;
//    int startY = Position.Y;

//    switch (direction)
//    {
//        case Direction.North:
//            Position = (Position.X, Position.Y + steps);
//            CleanedSegments.Add((startX, startY, Position.X, Position.Y));
//            break;
//        case Direction.East:
//            Position = (Position.X + steps, Position.Y);
//            CleanedSegments.Add((startX, startY, Position.X, Position.Y));
//            break;
//        case Direction.South:
//            Position = (Position.X, Position.Y - steps);
//            CleanedSegments.Add((startX, startY, Position.X, Position.Y));
//            break;
//        case Direction.West:
//            Position = (Position.X - steps, Position.Y);
//            CleanedSegments.Add((startX, startY, Position.X, Position.Y));
//            break;
//    }
//}

//public int GetTotalUniqueCleanedPlaces()
//{
//    HashSet<(int X, int Y)> uniquePoints = new();

//    foreach (var segment in CleanedSegments)
//    {
//        if (segment.StartX == segment.EndX) // Vertical segment
//        {
//            int minY = Math.Min(segment.StartY, segment.EndY);
//            int maxY = Math.Max(segment.StartY, segment.EndY);

//            for (int y = minY; y <= maxY; y++)
//            {
//                uniquePoints.Add((segment.StartX, y));
//            }
//        }
//        else if (segment.StartY == segment.EndY) // Horizontal segment
//        {
//            int minX = Math.Min(segment.StartX, segment.EndX);
//            int maxX = Math.Max(segment.StartX, segment.EndX);

//            for (int x = minX; x <= maxX; x++)
//            {
//                uniquePoints.Add((x, segment.StartY));
//            }
//        }
//    }

//    return uniquePoints.Count;
//}

//}
