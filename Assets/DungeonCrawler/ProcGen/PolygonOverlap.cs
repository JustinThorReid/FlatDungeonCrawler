using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonOverlap
{
    /// <inheritdoc />
    public bool DoOverlap(GridPolygon polygon1, Vector2Int position1, GridPolygon polygon2, Vector2Int position2) {
        // Polygons cannot overlap if their bounding rectangles do not overlap
        if(!DoOverlap(GetBoundingRectangle(polygon1) + position1, GetBoundingRectangle(polygon2) + position2))
            return false;

        var decomposition1 = GetDecomposition(polygon1).Select(x => x + position1).ToList();
        var decomposition2 = GetDecomposition(polygon2).Select(x => x + position2).ToList();

        foreach(var r1 in decomposition1) {
            foreach(var r2 in decomposition2) {
                if(DoOverlap(r1, r2)) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if two rectangles overlap.
    /// </summary>
    /// <param name="rectangle1"></param>
    /// <param name="rectangle2"></param>
    /// <returns></returns>
    public bool DoOverlap(GridRectangle rectangle1, GridRectangle rectangle2) {
        return rectangle1.BL.x < rectangle2.TR.x && rectangle1.TR.x > rectangle2.BL.x && rectangle1.BL.y < rectangle2.TR.y && rectangle1.TR.y > rectangle2.BL.y;
    }

    /// <inheritdoc />
    public int OverlapArea(GridPolygon polygon1, Vector2Int position1, GridPolygon polygon2, Vector2Int position2) {
        // Polygons cannot overlap if their bounding rectangles do not overlap
        if(!DoOverlap(GetBoundingRectangle(polygon1) + position1, GetBoundingRectangle(polygon2) + position2))
            return 0;

        var decomposition1 = GetDecomposition(polygon1).Select(x => x + position1).ToList();
        var decomposition2 = GetDecomposition(polygon2).Select(x => x + position2).ToList();
        var area = 0;

        foreach(var r1 in decomposition1) {
            foreach(var r2 in decomposition2) {
                var overlapX = Math.Max(0, Math.Min(r1.TR.x, r2.TR.x) - Math.Max(r1.BL.x, r2.BL.x));
                var overlapY = Math.Max(0, Math.Min(r1.TR.y, r2.TR.y) - Math.Max(r1.BL.y, r2.BL.y));
                area += overlapX * overlapY;
            }
        }

        return area;
    }

    /// <inheritdoc />
    public bool DoTouch(GridPolygon polygon1, Vector2Int position1, GridPolygon polygon2, Vector2Int position2, int minimumLength = 0) {
        if(minimumLength < 0)
            throw new ArgumentException("The minimum length must by at least 0.", nameof(minimumLength));

        var bounding1 = GetBoundingRectangle(polygon1) + position1;
        var bounding2 = GetBoundingRectangle(polygon2) + position2;

        if(!DoOverlap(bounding1, bounding2) && !DoTouch(bounding1, bounding2, minimumLength)) {
            return false;
        }

        var decomposition1 = GetDecomposition(polygon1).Select(x => x + position1);
        var decomposition2 = GetDecomposition(polygon2).Select(x => x + position2);

        foreach(var r1 in decomposition1) {
            foreach(var r2 in decomposition2) {
                if(DoTouch(r1, r2, minimumLength)) {
                    return true;
                }
            }
        }

        return false;
    }

    protected bool DoTouch(GridRectangle rectangle1, GridRectangle rectangle2, int minimumLength) {
        var overlapX = Math.Max(-1, Math.Min(rectangle1.TR.x, rectangle2.TR.x) - Math.Max(rectangle1.BL.x, rectangle2.BL.x));
        var overlapY = Math.Max(-1, Math.Min(rectangle1.TR.y, rectangle2.TR.y) - Math.Max(rectangle1.BL.y, rectangle2.BL.y));

        if((overlapX == 0 && overlapY >= minimumLength) || (overlapY == 0 && overlapX >= minimumLength)) {
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    //public IList<Tuple<Vector2Int, bool>> OverlapAlongLine(GridPolygon movingPolygon, GridPolygon fixedPolygon, OrthogonalLine line) {
    //    var reverse = line.GetDirection() == OrthogonalLine.Direction.Bottom || line.GetDirection() == OrthogonalLine.Direction.Left;

    //    if(reverse) {
    //        line = line.SwitchOrientation();
    //    }

    //    var rotation = line.ComputeRotation();
    //    var rotatedLine = line.Rotate(rotation);

    //    var movingDecomposition = GetDecomposition(movingPolygon).Select(x => x.Rotate(rotation)).ToList();
    //    var fixedDecomposition = GetDecomposition(fixedPolygon).Select(x => x.Rotate(rotation)).ToList();

    //    var smallestX = movingDecomposition.Min(x => x.BL.x);
    //    var events = new List<Tuple<Vector2Int, bool>>();

    //    // Compute the overlap for every rectangle in the decomposition of the moving polygon
    //    foreach(var movingRectangle in movingDecomposition) {
    //        var newEvents = OverlapAlongLine(movingRectangle, fixedDecomposition, rotatedLine, movingRectangle.BL.x - smallestX);
    //        events = MergeEvents(events, newEvents, rotatedLine);
    //    }

    //    if(reverse) {
    //        events = ReverseEvents(events, rotatedLine);
    //    }

    //    return events.Select(x => Tuple.Create(x.Item1.RotateAroundCenter(-rotation), x.Item2)).ToList();
    //}

    /// <inheritdoc />
    public int GetDistance(GridPolygon polygon1, Vector2Int position1, GridPolygon polygon2, Vector2Int position2) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the overlap along a line of a given moving rectangle and a set o fixed rectangles.
    /// </summary>
    /// <param name="movingRectangle"></param>
    /// <param name="fixedRectangles"></param>
    /// <param name="line"></param>
    /// <param name="movingRectangleOffset">Specifies the X-axis offset of a given moving rectangle.</param>
    /// <returns></returns>
    //protected List<Tuple<Vector2Int, bool>> OverlapAlongLine(GridRectangle movingRectangle, IList<GridRectangle> fixedRectangles, OrthogonalLine line, int movingRectangleOffset = 0) {
    //    if(line.GetDirection() != OrthogonalLine.Direction.Right)
    //        throw new ArgumentException();

    //    var events = new List<Tuple<Vector2Int, bool>>();

    //    foreach(var fixedRectangle in fixedRectangles) {
    //        var newEvents = OverlapAlongLine(movingRectangle, fixedRectangle, line, movingRectangleOffset);
    //        events = MergeEvents(events, newEvents, line);
    //    }

    //    return events;
    //}

    /// <summary>
    /// Computes the overlap along a line of a given moving rectangle and a fixed rectangle.
    /// </summary>
    /// <param name="movingRectangle"></param>
    /// <param name="fixedRectangle"></param>
    /// <param name="line"></param>
    /// <param name="movingRectangleOffset"></param>
    /// <returns></returns>
    //protected List<Tuple<Vector2Int, bool>> OverlapAlongLine(GridRectangle movingRectangle, GridRectangle fixedRectangle, OrthogonalLine line, int movingRectangleOffset = 0) {
    //    if(line.GetDirection() != OrthogonalLine.Direction.Right)
    //        throw new ArgumentException();

    //    // The smallest rectangle that covers both the first and the last position on the line of the moving rectangle
    //    var boundingRectangle = new GridRectangle(movingRectangle.A + line.From, movingRectangle.B + line.To);

    //    // They cannot overlap if the bounding rectangle does not overlap with the fixed one
    //    if(!DoOverlap(boundingRectangle, fixedRectangle)) {
    //        return new List<Tuple<Vector2Int, bool>>();
    //    }

    //    var events = new List<Tuple<Vector2Int, bool>>();

    //    if(fixedRectangle.BL.x - movingRectangle.Width - movingRectangleOffset <= line.From.x) {
    //        events.Add(Tuple.Create(line.From, true));
    //    }

    //    if(fixedRectangle.BL.x > line.From.x + movingRectangle.Width + movingRectangleOffset) {
    //        events.Add(Tuple.Create(new Vector2Int(fixedRectangle.BL.x - movingRectangle.Width + 1 - movingRectangleOffset, line.From.y), true));
    //    }

    //    if(fixedRectangle.TR.x - movingRectangleOffset < line.To.x) {
    //        events.Add(Tuple.Create(new Vector2Int(fixedRectangle.TR.x - movingRectangleOffset, line.From.y), false));
    //    }

    //    return events;
    //}

    /// <summary>
    /// Reverses a given events list in a way that the line has the opposite direction.
    /// </summary>
    /// <param name="events"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    //protected List<Tuple<Vector2Int, bool>> ReverseEvents(List<Tuple<Vector2Int, bool>> events, OrthogonalLine line) {
    //    var eventsCopy = new List<Tuple<Vector2Int, bool>>(events);

    //    if(events.Count == 0)
    //        return events;

    //    eventsCopy.Reverse();
    //    var newEvents = new List<Tuple<Vector2Int, bool>>();

    //    if(events.Last().Item2) {
    //        newEvents.Add(Tuple.Create(line.To, true));
    //    }

    //    foreach(var @event in eventsCopy) {
    //        if(!(@event.Item1 == line.From && @event.Item2 == true)) {
    //            newEvents.Add(Tuple.Create(@event.Item1 - line.GetDirectionVector(), !@event.Item2));
    //        }
    //    }

    //    return newEvents;
    //}

    /// <summary>
    /// Merges two lists of events.
    /// </summary>
    /// <param name="events1"></param>
    /// <param name="events2"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    protected List<Tuple<Vector2Int, bool>> MergeEvents(List<Tuple<Vector2Int, bool>> events1, List<Tuple<Vector2Int, bool>> events2, OrthogonalLine line) {
        if(events1.Count == 0)
            return events2;

        if(events2.Count == 0)
            return events1;

        var merged = new List<Tuple<Vector2Int, bool>>();

        var counter1 = 0;
        var counter2 = 0;

        var lastOverlap = false;
        var overlap1 = false;
        var overlap2 = false;

        // Run the main loop while both lists still have elements
        while(counter1 < events1.Count && counter2 < events2.Count) {
            var pair1 = events1[counter1];
            int pos1 = line.IndexOf(pair1.Item1);

            var pair2 = events2[counter2];
            int pos2 = line.IndexOf(pair2.Item1);

            if(pos1 <= pos2) {
                overlap1 = pair1.Item2;
                counter1++;
            }

            if(pos1 >= pos2) {
                overlap2 = pair2.Item2;
                counter2++;
            }

            var overlap = overlap1 || overlap2;

            if(overlap != lastOverlap) {
                if(pos1 < pos2) {
                    merged.Add(Tuple.Create(pair1.Item1, overlap));
                } else {
                    merged.Add(Tuple.Create(pair2.Item1, overlap));
                }
            }

            lastOverlap = overlap;
        }

        // Add remaining elements from the first list
        if(events2.Last().Item2 != true) {
            while(counter1 < events1.Count) {
                var pair = events1[counter1];

                if(merged.Last().Item2 != pair.Item2) {
                    merged.Add(pair);
                }

                counter1++;
            }
        }

        // Add remaining elements from the second list
        if(events1.Last().Item2 != true) {
            while(counter2 < events2.Count) {
                var pair = events2[counter2];

                if(merged.Last().Item2 != pair.Item2) {
                    merged.Add(pair);
                }

                counter2++;
            }
        }

        return merged;
    }

    private readonly GridPolygonPartitioning polygonPartitioning = new GridPolygonPartitioning();
    private readonly Dictionary<GridPolygon, List<GridRectangle>> partitions = new Dictionary<GridPolygon, List<GridRectangle>>();

    protected List<GridRectangle> GetDecomposition(GridPolygon polygon) {
        if(partitions.TryGetValue(polygon, out var p)) {
            return p;
        }

        var ps = polygonPartitioning.GetPartitions(polygon);
        partitions.Add(polygon, ps);

        return ps;
    }

    protected GridRectangle GetBoundingRectangle(GridPolygon polygon) {
        return polygon.BoundingRectangle;
    }
}

