using Tibber.Robot.Api.Models;
using Tibber.Robot.Api.Robots;

namespace Tibber.Robot.UnitTests.RobotFactories;

internal class VectorRobotFactory : RobotFactory<VectorRobot>
{
    public override AbstractRobot Create(Point start) => new VectorRobot(start);
}
