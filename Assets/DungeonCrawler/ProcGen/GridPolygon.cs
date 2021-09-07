using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class GridPolygon
{
    public static readonly int[] PossibleRotations = { 0, 90, 180, 270 };
    private readonly List<Vector2Int> points;
    private readonly int hash;
    public GridRectangle BoundingRectangle { get; }

    /// <summary>
    /// Create a polygon with given points.
    /// </summary>
    /// <param name="points"></param>
    /// <exception cref="ArgumentException">Thrown when invariants do not hold</exception>
    public GridPolygon(IEnumerable<Vector2Int> points) {
        this.points = new List<Vector2Int>(points);

        CheckIntegrity();

        hash = ComputeHash();
        BoundingRectangle = GetBoundingRectabgle();
    }

    public bool IsInside(Vector2Int pos) {
        bool inside = false;
        for(int x = BoundingRectangle.BL.x; x <= pos.x; x++) {
            if(points.Contains(new Vector2Int(x, pos.y)))
                inside = !inside;
        }

        if(!inside)
            return false;

        inside = false;
        for(int y = BoundingRectangle.BL.y; y <= pos.y; y++) {
            if(points.Contains(new Vector2Int(pos.x, y)))
                inside = !inside;
        }

        return !inside;
    }

    private void CheckIntegrity() {
        // Each polygon must have at least 4 vertices
        if(points.Count < 4) {
            throw new ArgumentException("Each polygon must have at least 4 points.");
        }

        // Check if all lines are parallel to axis X or Y
        var previousPoint = points[points.Count - 1];
        foreach(var point in points) {
            if(point == previousPoint)
                throw new ArgumentException("All lines must be parallel to one of the axes.");

            if(point.x != previousPoint.x && point.y != previousPoint.y)
                throw new ArgumentException("All lines must be parallel to one of the axes.");

            previousPoint = point;
        }

        // Check if no two adjacent lines are both horizontal or vertical
        for(var i = 0; i < points.Count; i++) {
            var p1 = points[i];
            var p2 = points[(i + 1) % points.Count];
            var p3 = points[(i + 2) % points.Count];

            if(p1.x == p2.x && p2.x == p3.x)
                throw new ArgumentException("No two adjacent lines can be both horizontal or vertical.");

            if(p1.y == p2.y && p2.y == p3.y)
                throw new ArgumentException("No two adjacent lines can be both horizontal or vertical.");
        }

        if(!IsClockwiseOriented(points))
            throw new ArgumentException("Points must be in a clockwise order.");
    }

    private bool IsClockwiseOriented(IList<Vector2Int> points) {
        var previous = points[points.Count - 1];
        var sum = 0L;

        foreach(var point in points) {
            sum += (point.x - previous.x) * (long)(point.y + previous.y);
            previous = point;
        }

        return sum > 0;
    }

    private GridRectangle GetBoundingRectabgle() {
        var smallestX = points.Min(x => x.x);
        var biggestX = points.Max(x => x.x);
        var smallestY = points.Min(x => x.y);
        var biggestY = points.Max(x => x.y);

        return new GridRectangle(new Vector2Int(smallestX, smallestY), new Vector2Int(biggestX, biggestY));
    }

    private int ComputeHash() {
        unchecked {
            var hash = 17;
            points.ForEach(x => hash = hash * 23 + x.x + x.y);
            return hash;
        }
    }

    /// <summary>
    /// Gets point of the polygon.
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<Vector2Int> GetPoints() {
        return points.AsReadOnly();
    }

    /// <summary>
    /// Gets all lines of the polygon ordered as they appear on the polygon.
    /// </summary>
    /// <returns></returns>
    public List<OrthogonalLine> GetLines() {
        var lines = new List<OrthogonalLine>();
        var x1 = points[points.Count - 1];

        foreach(var point in points) {
            var x2 = x1;
            x1 = point;

            lines.Add(new OrthogonalLine(x2, x1));
        }

        return lines;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) {
        return obj is GridPolygon other && points.SequenceEqual(other.GetPoints());
    }

    /// <inheritdoc />
    public override int GetHashCode() {
        return hash;
    }
}
