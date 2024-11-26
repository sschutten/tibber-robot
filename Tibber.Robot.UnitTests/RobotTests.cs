using static Tibber.Robot.Api.Models.Direction;

namespace Tibber.Robot.UnitTests;

public class RobotTests
{
    [Fact]
    public void Move_NorthByOneStep_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 1);

        // Assert
        Assert.Equal((1, 2), robot.Position);
        Assert.Equal(2, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_NorthByOneStepAndEastByOneStep_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 1);
        robot.Move(East, 1);

        // Assert
        Assert.Equal((2, 2), robot.Position);
        Assert.Equal(3, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_InSquarePattern_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 1);
        robot.Move(East, 1);
        robot.Move(South, 1);
        robot.Move(West, 1);

        // Assert
        Assert.Equal((1, 1), robot.Position);
        Assert.Equal(4, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_InSpiralPattern_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(-2, -2);

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
        Assert.Equal((0, 0), robot.Position);
        Assert.Equal(25, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_BackAndForth_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 1);
        robot.Move(South, 1);

        // Assert
        Assert.Equal((1, 1), robot.Position);
        Assert.Equal(2, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_LargeStepsInOneDirection_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 10000);

        // Assert
        Assert.Equal((1, 10001), robot.Position);
        Assert.Equal(10001, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_LargeStepsInMultipleDirections_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 5000);
        robot.Move(East, 5000);

        // Assert
        Assert.Equal((5001, 5001), robot.Position);
        Assert.Equal(10001, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void NoMovement_InitialPositionAndUniqueVerticesCountIsOne()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        // No movement

        // Assert
        Assert.Equal((1, 1), robot.Position);
        Assert.Equal(1, robot.GetTotalUniqueCleanedPlaces());
    }

    [Fact]
    public void Move_RevisitsSamePosition_UpdatesPositionAndUniqueVerticesCount()
    {
        // Arrange
        var robot = new Api.Robot(1, 1);

        // Act
        robot.Move(North, 1);
        robot.Move(South, 1);
        robot.Move(North, 1);

        // Assert
        Assert.Equal((1, 2), robot.Position);
        Assert.Equal(2, robot.GetTotalUniqueCleanedPlaces());
    }
}
