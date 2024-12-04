using Tibber.Robot.Api.Models;

namespace Tibber.Robot.Api.Robots;

/// <summary>
/// The robot simulated moving in an office space and cleans the places it visits.
/// The path of the robot's movement is described by the starting coordinates and move commands.
/// After the cleaning has been done, the unique places cleaned can be retrieved.
/// </summary>
public abstract class AbstractRobot(Point start)
{
    private const int _offset = 100_000;

    /// <summary>
    /// The current position of the robot, offset to be a positive number
    /// </summary>
    protected Point AbsolutePosition { get; } = new Point(start.X + _offset, start.Y + _offset);

    /// <summary>
    /// The current position of the robot.
    /// </summary>
    public Point Position => new(AbsolutePosition.X - _offset, AbsolutePosition.Y - _offset);

    /// <summary>
    /// Moves the robot in the specified direction for the specified number of steps.
    /// </summary>
    /// <param name="direction">The direction for the robot to move in</param>
    /// <param name="steps">The number of steps to take into the direction</param>
    public abstract void Move(Direction direction, int steps);

    /// <summary>
    /// Gets the total number of unique places cleaned by the robot.
    /// </summary>
    /// <returns>Total number of unique places cleaned</returns>
    public abstract long GetTotalUniqueCleanedPlaces();
}
