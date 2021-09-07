using System;
using UnityEngine;

public struct OrthogonalLine {
    public readonly Vector2Int From;
    public readonly Vector2Int To;

    public OrthogonalLine(Vector2Int from, Vector2Int to)
	{
        Debug.Assert(from.x <= to.x && from.y <= to.y, "Orthogonal Line: From must be smaller than To");
        Debug.Assert(from.x == to.x || from.y == to.y, "Orthogonal Line: Must be orthogonal");

        this.From = from;
        this.To = to;
	}

    public static OrthogonalLine operator +(OrthogonalLine line, Vector2Int point) {
        return new OrthogonalLine(line.From + point, line.To + point);
    }

    public bool Contains(Vector2Int pos) {
        if(pos.x == From.x && pos.y >= From.y && pos.y <= To.y)
            return true;
        if(pos.y == From.y && pos.x >= From.x && pos.x <= To.x)
            return true;
        return false;
    }

    /// <summary>
    /// Get the index offset from the start of the line for this point, or -1
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int IndexOf(Vector2Int pos) {
        if(pos.x == From.x && pos.y >= From.y && pos.y <= To.y) {
            return pos.y - From.y;
        }
        if(pos.y == From.y && pos.x >= From.x && pos.x <= To.x) {
            return pos.x - From.x;
        }
        return -1;
    }
}
