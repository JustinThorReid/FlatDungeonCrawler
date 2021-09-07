using System;
using UnityEngine;

public struct GridRectangle {
    public readonly Vector2Int BL;
    public readonly Vector2Int TR;

    public int width
    {
        get {
            return TR.x - BL.x;
        }
    }
    public int height
    {
        get {
            return TR.y - BL.y;
        }
    }

    public GridRectangle(Vector2Int bl, Vector2Int tr) {
        Debug.Assert(bl.x <= tr.x && bl.y <= tr.y, "GridRectangle: BL must be smaller than TR");

        BL = bl;
        TR = tr;
    }

    public static GridRectangle operator +(GridRectangle rect, Vector2Int point) {
        return new GridRectangle(rect.BL + point, rect.TR + point);
    }

    /// <summary>
    /// Combine 2 rectanges by returning a rectangle that contains both
    /// </summary>
    /// <param name="line"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static GridRectangle operator +(GridRectangle a, GridRectangle b) {
        Vector2Int bl = new Vector2Int(Math.Min(a.BL.x, b.BL.x), Math.Min(a.BL.y, b.BL.y));
        Vector2Int tr = new Vector2Int(Math.Max(a.TR.x, b.TR.x), Math.Max(a.TR.y, b.TR.y));
        return new GridRectangle(bl, tr);
    }
}