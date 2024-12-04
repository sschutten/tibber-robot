using Tibber.Robot.Api.Models;

namespace Tibber.Robot.Api.Robots;

/// <inheritdoc />
public class SegmentedRobot : AbstractRobot
{
    private const int _minX = -100_000;
    private const int _maxX = 100_000;
    private const int _minY = -100_000;
    private const int _maxY = 100_000;
    private const int _maxHorizontal = _maxX + 1 - _minX; // Add 1 because 0 is also a valid coordinate
    private const int _maxVertical = _maxY + 1 - _minY; // Add 1 because 0 is also a valid coordinate

    /// <summary>
    /// The segments are stored in a 2D array.
    /// </summary>
    private readonly Segment[,] _segments;
    private int _left;
    private int _bottom;
    private readonly byte _segmentSideLength;

    public SegmentedRobot(Point start, byte segmentSideLength) : base(start)
    {
        _segmentSideLength = segmentSideLength;
        var width = _maxHorizontal / segmentSideLength;
        var height = _maxVertical / segmentSideLength;
        _segments = new Segment[width, height];
        
        _left = start.X - _minX;
        _bottom = start.Y - _minY;

        CleanCurrentPosition();
    }

    /// <inheritdoc />
    public override void Move(Direction direction, int steps)
    {
        int dX = 0, dY = 0;
        switch (direction)
        {
            case Direction.North:
                dY = 1;
                break;
            case Direction.East:
                dX = 1;
                break;
            case Direction.South:
                dY = -1;
                break;
            case Direction.West:
                dX = -1;
                break;
        }

        for (int i = 0; i < steps; i++)
        {
            _left += dX;
            _bottom += dY;
            AbsolutePosition.X += dX;
            AbsolutePosition.Y += dY;
            CleanCurrentPosition();
        }
    }

    private void CleanCurrentPosition()
    {
        var segmentX = _left / _segmentSideLength;
        var segmentY = _bottom / _segmentSideLength;

        // Create the segment if it doesn't exist
        if (_segments[segmentX, segmentY] == null)
            _segments[segmentX, segmentY] = new Segment(_segmentSideLength);

        _segments[segmentX, segmentY].Clean((byte)(AbsolutePosition.X % _segmentSideLength), (byte)(AbsolutePosition.Y % _segmentSideLength));
    }

    /// <inheritdoc />
    public override long GetTotalUniqueCleanedPlaces()
    {
        return _segments.Cast<Segment>()
            .Where(x => x != null)
            .Sum(x => x.Count);
    }

    private class Segment(byte sideLength)
    {
        public int Count { get; private set; }

        private bool _cleaned = false;
        private readonly int _capacity = sideLength * sideLength;

        private HashSet<int> _places = [];

        public void Clean(byte x, byte y)
        {
            // Return immediately if the segment is already cleaned
            if (_cleaned) return;

            // Add the hash of the place and increment the count if it's unique
            if (_places.Add(x << 8 ^ y))
                Count++;

            // Once the segment is full, mark it as cleaned
            // and clear the places to save memory
            if (Count == _capacity)
            {
                _cleaned = true;
                _places = [];
            }
        }
    }
}
