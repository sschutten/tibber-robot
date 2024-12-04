using Tibber.Robot.Api.Models;
using Tibber.Robot.Api.Robots;
using Xunit.Abstractions;

namespace Tibber.Robot.UnitTests.RobotFactories;

public interface IRobotFactory : IXunitSerializable
{
    AbstractRobot Create(Point start);
}

internal abstract class RobotFactory<TRobot> : IRobotFactory
    where TRobot : AbstractRobot
{
    public RobotFactory()
    {
    }

    // Override ToString for better test naming
    public override string ToString()
    {
        return typeof(TRobot).Name;
    }

    public abstract AbstractRobot Create(Point start);

    public virtual void Deserialize(IXunitSerializationInfo info)
    {
    }

    public virtual void Serialize(IXunitSerializationInfo info)
    {
    }
}
