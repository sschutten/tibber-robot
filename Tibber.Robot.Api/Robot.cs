using Tibber.Robot.Api.Models;

namespace Tibber.Robot.Api;

/// <summary>
/// The robot simulated moving in an office space and cleans the places it visits.
/// The path of the robot's movement is described by the starting coordinates and move commands.
/// After the cleaning has been done, the unique places cleaned can be retrieved.
/// </summary>
public class Robot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Robot"/> class at the position designated by the x and y coordinates.
    /// </summary>
    /// <param name="x">The initial x coordinate</param>
    /// <param name="y">The initial y coordinate</param>
    public Robot(int x, int y)
    {
        Position = (x, y);
        UniquePlacesCleaned = [Position];
    }

    /// <summary>
    /// The current position of the robot.
    /// </summary>
    public (int X, int Y) Position { get; private set; }

    /// <summary>
    /// The unique places cleaned by the robot.
    /// </summary>
    public HashSet<(int X, int Y)> UniquePlacesCleaned { get; } = [];

    /// <summary>
    /// Moves the robot in the specified direction for the specified number of steps.
    /// </summary>
    /// <param name="direction">The direction for the robot to move in</param>
    /// <param name="steps">The number of steps to take into the direction</param>
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
