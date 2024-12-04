using Tibber.Robot.Api.Models;
using Tibber.Robot.Api.Robots;

namespace Tibber.Robot.UnitTests.RobotFactories;

internal class OptimizedBentleyOttmannRobotFactory : RobotFactory<OptimizedBentleyOttmannRobot>
{
    public override AbstractRobot Create(Point start) => new OptimizedBentleyOttmannRobot(start);
}
