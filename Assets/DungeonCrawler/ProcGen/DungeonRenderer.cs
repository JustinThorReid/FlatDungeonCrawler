using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class DungeonRenderer : MonoBehaviour
{
    Tilemap tilemap;
    public int mapWidth = 100;
    public int mapHeight = 100;

    public Tile floorTile;
    public Tile wallTile;
    public Tile doorTile;

    private Generator generator;

    // Start is called before the first frame update
    void Start()
    {
        generator = new Generator();
        Map map = generator.BuildMap();
        tilemap = GetComponent<Tilemap>();

        Vector3Int pos = new Vector3Int();
        for(pos.x = 0; pos.x < map.width; pos.x++) {
            for(pos.y = 0; pos.y < map.height; pos.y++) {
                if(map.map[pos.x, pos.y] == TileType.GROUND) 
                    tilemap.SetTile(new Vector3Int(pos.x - mapWidth/2, pos.y - mapHeight/2, 0), floorTile);
                if(map.map[pos.x, pos.y] == TileType.DOOR)
                    tilemap.SetTile(new Vector3Int(pos.x - mapWidth / 2, pos.y - mapHeight / 2, 0), doorTile);
                if(map.map[pos.x, pos.y] == TileType.WALL)
                    tilemap.SetTile(new Vector3Int(pos.x - mapWidth / 2, pos.y - mapHeight / 2, 0), wallTile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
