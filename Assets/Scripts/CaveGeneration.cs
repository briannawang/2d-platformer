using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGeneration : MonoBehaviour
{
    public static int[,] RandomWalkCave(int[,] map, float seed, int requiredFloorPercent)
    {
        //Seed our random
        System.Random rand = new System.Random(seed.GetHashCode());

        //Define our start x position
        int floorX = rand.Next(1, map.GetUpperBound(0) - 1);
        //Define our start y position
        int floorY = rand.Next(1, map.GetUpperBound(1) - 1);
        //Determine our required floorAmount
        int reqFloorAmount = ((map.GetUpperBound(1) * map.GetUpperBound(0)) * requiredFloorPercent) / 100;
        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
        int floorCount = 0;

        //Set our start position to not be a tile (0 = no tile, 1 = tile)
        map[floorX, floorY] = 0;
        //Increase our floor count
        floorCount++;

        while (floorCount < reqFloorAmount)
        {
            //Determine our next direction
            int randDir = rand.Next(4);

            switch (randDir)
            {
                //Up
                case 0:
                    //Ensure that the edges are still tiles
                    if ((floorY + 1) < map.GetUpperBound(1) - 1)
                    {
                        //Move the y up one
                        floorY++;

                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase floor count
                            floorCount++;
                        }
                    }
                    break;
                //Down
                case 1:
                    //Ensure that the edges are still tiles
                    if ((floorY - 1) > 1)
                    {
                        //Move the y down one
                        floorY--;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                //Right
                case 2:
                    //Ensure that the edges are still tiles
                    if ((floorX + 1) < map.GetUpperBound(0) - 1)
                    {
                        //Move the x to the right
                        floorX++;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                //Left
                case 3:
                    //Ensure that the edges are still tiles
                    if ((floorX - 1) > 1)
                    {
                        //Move the x to the left
                        floorX--;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
            }
        }
        //Return the updated map
        return map;
    }
}