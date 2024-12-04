using Tibber.Robot.Api.Models;
using Tibber.Robot.Api.Robots;

namespace Tibber.Robot.UnitTests.RobotFactories;

internal class HashRobotFactory : RobotFactory<HashRobot>
{
    public override AbstractRobot Create(Point start) => new HashRobot(start);
}
