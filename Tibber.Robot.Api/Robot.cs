using Tibber.Robot.Api.Models;

namespace Tibber.Robot.Api;

public class Robot
{

    public Robot(int x, int y)
    {
        Position = (x, y);
        UniquePlacesCleaned = [Position];
    }

    public (int X, int Y) Position { get; private set; }
    public HashSet<(int X, int Y)> UniquePlacesCleaned { get; } = [];

    public void Move(Direction direction, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            switch (direction)
            {
                case Direction.North:
                    Position = (Position.X, Position.Y + 1);
                    break;
                case Direction.East:
                    Position = (Position.X + 1, Position.Y);
                    break;
                case Direction.South:
                    Position = (Position.X, Position.Y - 1);
                    break;
                case Direction.West:
                    Position = (Position.X - 1, Position.Y);
                    break;
            }
            UniquePlacesCleaned.Add(Position);
        }
    }
}
