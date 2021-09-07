using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacedRoom
{
    public readonly Vector2Int location;
    public readonly Room room;

    public PlacedRoom[] connections;

	public PlacedRoom(Room room, Vector2Int location)
	{
        this.location = location;
        this.room = room;
        this.connections = new PlacedRoom[room.GetConnections().Count];
	}

    public PlacedRoom(Room room, Vector2Int location, int thisConnection, PlacedRoom otherRoom, int otherConnection) {
        this.location = location;
        this.room = room;
        this.connections = new PlacedRoom[room.GetConnections().Count];

        this.connections[thisConnection] = otherRoom;
        otherRoom.connections[otherConnection] = this;
    }

    public ConnectionArea GetConnectionArea(int index) {
        ConnectionArea connection = room.GetConnections().ElementAt(index);
        return new ConnectionArea(connection.connectionLine, connection.connectionDirection);
    }

    public GridRectangle GetBounds() {
        return room.GetPolygon().BoundingRectangle + location;
    }

    public void PlaceTiles(Map map) {
        GridRectangle rect = room.GetPolygon().BoundingRectangle;
        Vector2Int pos = new Vector2Int();
        for(pos.x = rect.BL.x; pos.x <= rect.TR.x; pos.x++) {
            for(pos.y = rect.BL.y; pos.y <= rect.TR.y; pos.y++) {
                TileType tile = room.GetTile(pos);
                Vector2Int mapPos = pos + location;
                if(map.map[mapPos.x, mapPos.y] == TileType.NONE)
                    map.map[mapPos.x, mapPos.y] = tile;
            }
        }
    }
}
