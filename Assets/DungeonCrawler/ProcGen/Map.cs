using System;

public class Map
{
    public readonly int width;
    public readonly int height;
    public readonly TileType[,] map;

	public Map(int width, int height)
	{
        this.width = width;
        this.height = height;
        map = new TileType[width, height];
	}
}
