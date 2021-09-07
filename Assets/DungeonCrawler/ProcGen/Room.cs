using System;
using System.Collections.Generic;
using UnityEngine;

public struct Room
{
    GridPolygon outline;
    List<ConnectionArea> doorZones;

	public Room(int width, int height) {
        outline = new GridPolygon(new Vector2Int[] {
            new Vector2Int(0, 0),
            new Vector2Int(0, height),
            new Vector2Int(width, height),
            new Vector2Int(width, 0)
        });

        doorZones = new List<ConnectionArea>() {
            new ConnectionArea(new OrthogonalLine(new Vector2Int(0, 1), new Vector2Int(0, height-1)), ConnectionArea.Direction.LEFT),
            new ConnectionArea(new OrthogonalLine(new Vector2Int(1, height), new Vector2Int(width-1, height)), ConnectionArea.Direction.UP),
            new ConnectionArea(new OrthogonalLine(new Vector2Int(width, 1), new Vector2Int(width, height-1)), ConnectionArea.Direction.RIGHT),
            new ConnectionArea(new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(width-1, 0)), ConnectionArea.Direction.DOWN)
        };
    }

    public IReadOnlyCollection<ConnectionArea> GetConnections() {
        return doorZones.AsReadOnly();
    }

    public GridPolygon GetPolygon() {
        return outline;
    }

    public TileType GetTile(Vector2Int pos) {
        foreach(ConnectionArea conn in doorZones) {
            if(conn.connectionLine.Contains(pos))
                return TileType.DOOR;
        }

        if(outline.GetPoints().Contains(pos)) {
            return TileType.WALL;
        }

        if(outline.IsInside(pos))
            return TileType.GROUND;
    }
}
