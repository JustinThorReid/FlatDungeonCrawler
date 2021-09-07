using System;
using UnityEngine;

public class ConnectionArea
{
    public enum Direction {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public readonly OrthogonalLine connectionLine;
    public readonly Direction connectionDirection;
    public ConnectionArea otherSide;

	public ConnectionArea(OrthogonalLine connectionLine, Direction connectionDirection)
	{
        this.connectionLine = connectionLine;
        this.connectionDirection = connectionDirection;
	}

    public Vector2Int StartPos() {
        if(connectionDirection == Direction.UP || connectionDirection == Direction.DOWN) {
            return connectionLine.From.x < connectionLine.To.x ? connectionLine.From : connectionLine.To;
        } else {
            return connectionLine.From.y < connectionLine.To.y ? connectionLine.From : connectionLine.To;
        }
    }

    public Vector2Int EndPos() {
        if(connectionDirection == Direction.UP || connectionDirection == Direction.DOWN) {
            return connectionLine.From.x > connectionLine.To.x ? connectionLine.From : connectionLine.To;
        } else {
            return connectionLine.From.y > connectionLine.To.y ? connectionLine.From : connectionLine.To;
        }
    }

    public Vector2Int GetStartOverlap(ConnectionArea other) {
        return other.StartPos() - EndPos();
    }
    public Vector2Int GetEndOverlap(ConnectionArea other) {
        return other.EndPos() - StartPos();
    }
}

public static class DirectionExtensions {
    public static bool IsOpposite(this ConnectionArea.Direction thisDir, ConnectionArea.Direction otherDir) {
        if(thisDir == ConnectionArea.Direction.UP && otherDir == ConnectionArea.Direction.DOWN)
            return true;
        if(thisDir == ConnectionArea.Direction.DOWN && otherDir == ConnectionArea.Direction.UP)
            return true;
        if(thisDir == ConnectionArea.Direction.LEFT && otherDir == ConnectionArea.Direction.RIGHT)
            return true;
        if(thisDir == ConnectionArea.Direction.RIGHT && otherDir == ConnectionArea.Direction.LEFT)
            return true;

        return false;
    }
}
