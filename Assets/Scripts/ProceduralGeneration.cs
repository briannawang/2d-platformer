using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] TileBase tile;
    [SerializeField] Tilemap tileMap;
    [SerializeField] float seed;
    [SerializeField] int requiredFloorPercent;

    private void Start()
    {
        int[,] map = GenerateArray(width, height, false);
        map = CaveGeneration.TunnelCave(map, seed);
        map = CaveGeneration.RandomWalkCave(map, seed, requiredFloorPercent);
        RenderMap(map, tileMap, tile, width, height);
    }


    // procedural generation code -----------------------------------------

    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }

    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile, int width, int height)
    {
        int centerX = width / 2;
        int centerY = height / 2;

        //Clear the map (ensures we dont overlap)
        tilemap.ClearAllTiles();
        //Loop through the width of the map
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            //Loop through the height of the map
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                // 1 = tile, 0 = no tile
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x - centerX, y - centerY, 0), tile);
                }
            }
        }
    }

    public static void UpdateMap(int[,] map, Tilemap tilemap) //Takes in our map and tilemap, setting null tiles where needed
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                //We are only going to update the map, rather than rendering again
                //This is because it uses less resources to update tiles to null
                //As opposed to re-drawing every single tile (and collision data)
                if (map[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    // debug -----------------------------------------

    public static void ConsolePrint(int[,] map) 
    {
        string toPrint = "";

        for (int y = map.GetUpperBound(1) - 1; y >= 0 ; y--)
        {
            for (int x = map.GetUpperBound(0) - 1; x >= 0; x--)
            {
                if (map[x, y] == 0)
                {
                    toPrint += "  ";
                }
                else
                {
                    toPrint += "# ";
                }
            }
            toPrint += "\n";
        }
        print(toPrint);
    }
}
