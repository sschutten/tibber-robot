using Tibber.Robot.Api.Models;
using Tibber.Robot.Api.Robots;

namespace Tibber.Robot.UnitTests.RobotFactories;

internal class BentleyOttmannRobotFactory : RobotFactory<BentleyOttmannRobot>
{
    public override AbstractRobot Create(Point start) => new BentleyOttmannRobot(start);
}
