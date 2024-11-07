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
        Assert.Equal(2, robot.UniquePlacesCleaned.Count);
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
        Assert.Equal(3, robot.UniquePlacesCleaned.Count);
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
        Assert.Equal(4, robot.UniquePlacesCleaned.Count);
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
        Assert.Equal(2, robot.UniquePlacesCleaned.Count);
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
        Assert.Equal(10001, robot.UniquePlacesCleaned.Count);
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
        Assert.Equal(10001, robot.UniquePlacesCleaned.Count);
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
        Assert.Single(robot.UniquePlacesCleaned);
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
        Assert.Equal(2, robot.UniquePlacesCleaned.Count);
    }
}
