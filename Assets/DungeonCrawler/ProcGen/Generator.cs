using UnityEngine;
using System.Collections;
using SimplexNoise;
using System.Collections.Generic;
using System.Linq;

public enum TileType {
    NONE,
    GROUND,
    WALL,
    DOOR
}

public class Generator {
    System.Random r;
    List<Room> possibleRooms = new List<Room>();
    List<PlacedRoom> layout = new List<PlacedRoom>();

    public Generator() {
        r = new System.Random(99);
        
        possibleRooms.Add(new Room(5, 5));
        possibleRooms.Add(new Room(15, 5));
        possibleRooms.Add(new Room(5, 15));
        possibleRooms.Add(new Room(10, 10));
        possibleRooms.Add(new Room(25, 25));

        // Starting room
        layout.Add(new PlacedRoom(possibleRooms[0], Vector2Int.zero));

        for(int i = 0; i < 5; i++) {
            PlacedRoom lastRoom = layout[layout.Count - 1];
            IList<int> connections = lastRoom.connections.Where(otherRoom => otherRoom == null).Select((_id, index) => { return index; }).ToList();
            int connection = connections.GetRandom(r);

            placeRoom(layout, possibleRooms, lastRoom, connection);
        }
    }

    public Map BuildMap() {
        GridRectangle bounds = new GridRectangle();
        foreach(PlacedRoom placedRoom in layout) {
            bounds += placedRoom.GetBounds();
        }

        Map map = new Map(bounds.width, bounds.height);
        foreach(PlacedRoom placedRoom in layout) {
            placedRoom.PlaceTiles(map);
        }

        return map;
    }

    private bool randomBool() {
        return r.Next() % 2 == 1;
    }

    // Given a list of already placed rooms, a list to choose from, and a location to connect to
    // Return a suggested room and location
    private void placeRoom(List<PlacedRoom> placedRooms, List<Room> possibleRooms, PlacedRoom roomToConnectTo, int connectionToUse) {
        ConnectionArea thisConnection = roomToConnectTo.GetConnectionArea(connectionToUse);
        ConnectionArea.Direction thisDirection = thisConnection.connectionDirection;
        possibleRooms.Shuffle(r);

        foreach(Room room in possibleRooms) {
            List<ConnectionArea> possibleConnections = room.GetConnections().Where(connection => connection.connectionDirection.IsOpposite(thisDirection)).ToList();
            possibleConnections.Shuffle(r);

            foreach(ConnectionArea connection in possibleConnections) {
                Vector2Int startPos = thisConnection.GetStartOverlap(connection) + roomToConnectTo.location;
                Vector2Int endPos = thisConnection.GetEndOverlap(connection) + roomToConnectTo.location;
                Vector2Int testPos = new Vector2Int();

                for(testPos.x = startPos.x; testPos.x < endPos.x; testPos.x++) {
                    for(testPos.y = startPos.y; testPos.y < endPos.y; testPos.y++) {
                        if(IsValid(placedRooms, room, testPos)) {
                            placedRooms.Add(new PlacedRoom(room, testPos));
                            return;
                        }
                    }
                }
            }
        }
    }

    private bool IsValid(List<PlacedRoom> placedRooms, Room testRoom, Vector2Int testLocation) {
        PolygonOverlap overlap = new PolygonOverlap();

        foreach(PlacedRoom placedRoom in placedRooms) {
            if(overlap.DoOverlap(testRoom.GetPolygon(), testLocation, placedRoom.room.GetPolygon(), placedRoom.location))
                return false;
        }

        return true;
    }
}
