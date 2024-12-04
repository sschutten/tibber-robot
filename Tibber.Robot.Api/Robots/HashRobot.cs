using System.Diagnostics.CodeAnalysis;
using Tibber.Robot.Api.Models;

namespace Tibber.Robot.Api.Robots;

/// <summary>
/// This robot calculates the number of unique places by using a hash set.
/// The hashset is an easy way to store unique values, but it is not the most efficient.
/// </summary>
/// <param name="start"></param>
public class HashRobot : AbstractRobot
{
    private readonly HashSet<Point> _uniquePlacesCleaned = new(new PointComparer());

    public HashRobot(Point start) : base(start)
    {
        _uniquePlacesCleaned = [AbsolutePosition];
    }

    /// <inheritdoc />
    public override void Move(Direction direction, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            switch (direction)
            {
                case Direction.North:
                    AbsolutePosition.Y += 1;
                    break;
                case Direction.East:
                    AbsolutePosition.X += 1;
                    break;
                case Direction.South:
                    AbsolutePosition.Y -= 1;
                    break;
                case Direction.West:
                    AbsolutePosition.X -= 1;
                    break;
            }

            _uniquePlacesCleaned.Add(new(AbsolutePosition.X, AbsolutePosition.Y));
        }
    }

    /// <inheritdoc />
    public override long GetTotalUniqueCleanedPlaces()
    {
        return _uniquePlacesCleaned.Count;
    }

    private class PointComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point? x, Point? y)
        {
            return x?.X == y?.X && x?.Y == y?.Y;
        }

        public int GetHashCode([DisallowNull] Point obj)
        {
            return (obj.X << 16) ^ obj.Y.GetHashCode();
        }
    }
}
