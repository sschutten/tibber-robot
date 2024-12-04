using Tibber.Robot.Api.Models;
using Tibber.Robot.Api.Robots;
using Xunit.Abstractions;

namespace Tibber.Robot.UnitTests.RobotFactories;

internal class SegmentedRobotFactory : RobotFactory<SegmentedRobot>
{
    private byte _segmentedSideLength;

    public SegmentedRobotFactory(byte segmentedSideLength)
    {
        _segmentedSideLength = segmentedSideLength;
    }

    public SegmentedRobotFactory()
    {
    }

    public override AbstractRobot Create(Point start) => new SegmentedRobot(start, _segmentedSideLength);

    public override void Deserialize(IXunitSerializationInfo info)
    {
        _segmentedSideLength = info.GetValue<byte>(nameof(_segmentedSideLength));
    }

    public override void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(_segmentedSideLength), _segmentedSideLength);
    }
}
