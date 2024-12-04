using Tibber.Robot.Api.Models;
using Tibber.Robot.UnitTests.RobotFactories;
using static Tibber.Robot.Api.Models.Direction;

namespace Tibber.Robot.UnitTests;

public class RobotTests
{
    public class RobotFactories : TheoryData<IRobotFactory>
    {
        public RobotFactories()
        {
            Add(new BentleyOttmannRobotFactory());
            Add(new OptimizedBentleyOttmannRobotFactory());
            Add(new HashRobotFactory());
            Add(new SegmentedRobotFactory(100));
            Add(new VectorRobotFactory());
        }
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_NorthByOneStep_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 1);

        // Assert
        Assert.Equal(new Point(1, 2), robot.Position);
        Assert.Equal(2, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_NorthByOneStepAndEastByOneStep_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 1);
        robot.Move(East, 1);

        // Assert
        Assert.Equal(new Point(2, 2), robot.Position);
        Assert.Equal(3, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_InSquarePattern_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 1);
        robot.Move(East, 1);
        robot.Move(South, 1);
        robot.Move(West, 1);

        // Assert
        Assert.Equal(new Point(1, 1), robot.Position);
        Assert.Equal(4, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_InSpiralPattern_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(-2, -2));

        // Act
        robot.Move(North, 4);
        robot.Move(East, 4);
        robot.Move(South, 4);
        robot.Move(West, 3);
        robot.Move(North, 3);
        robot.Move(East, 2);
        robot.Move(South, 2);
        robot.Move(West, 1);
        robot.Move(North, 1);

        // Assert
        Assert.Equal(new Point(0, 0), robot.Position);
        Assert.Equal(25, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_BackAndForth_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 1);
        robot.Move(South, 1);

        // Assert
        Assert.Equal(new Point(1, 1), robot.Position);
        Assert.Equal(2, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_LargeStepsInOneDirection_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 10000);

        // Assert
        Assert.Equal(new Point(1, 10001), robot.Position);
        Assert.Equal(10001, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_LargeStepsInMultipleDirections_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 5000);
        robot.Move(East, 5000);

        // Assert
        Assert.Equal(new Point(5001, 5001), robot.Position);
        Assert.Equal(10001, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void NoMovement_InitialPositionAndUniqueVerticesCountIsOne(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        // No movement

        // Assert
        Assert.Equal(new Point(1, 1), robot.Position);
        Assert.Equal(1, robot.GetTotalUniqueCleanedPlaces());
    }

    [Theory, ClassData(typeof(RobotFactories))]
    public void Move_RevisitsSamePosition_UpdatesPositionAndUniqueVerticesCount(IRobotFactory robotFactory)
    {
        // Arrange
        var robot = robotFactory.Create(new Point(1, 1));

        // Act
        robot.Move(North, 1);
        robot.Move(South, 1);
        robot.Move(North, 1);

        // Assert
        Assert.Equal(new Point(1, 2), robot.Position);
        Assert.Equal(2, robot.GetTotalUniqueCleanedPlaces());
    }
}
