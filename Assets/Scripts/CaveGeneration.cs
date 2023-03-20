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
        int nextInt = 6;

        while (floorCount < reqFloorAmount)
        {
            //Determine our next direction
            int randDir = rand.Next(nextInt);

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
                        nextInt = 6;
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
                        nextInt = 6;
                    }
                    break;
                //Right
                case 2 or 3:
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
                        nextInt = 6;
                    }
                    else
                    {
                        nextInt = 2;
                    }
                    break;
                //Left
                case 4 or 5:
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
                    else
                    {
                        nextInt = 2;
                    }
                    break;
            }
        }
        //Return the updated map
        return map;
    }

    public static int[,] TunnelCave(int[,] map, float seed)
    {
        //Seed our random
        System.Random rand = new System.Random((seed + 13).GetHashCode());

        //Define our start x position
        int floorX = rand.Next(1, map.GetUpperBound(0) / 8);
        //Define our start y position
        int floorY = rand.Next(1, map.GetUpperBound(1) / 8);
        int boundX = map.GetUpperBound(0) - 1;
        int boundY = map.GetUpperBound(1) - 1;
        int countX = 3;

        //Set our start position to not be a tile (0 = no tile, 1 = tile)
        map[floorX, floorY] = 0;
        for (int i = floorY; i < floorY + 3; i++)
        {
            for (int j = floorX; j < floorX + 3; j++)
            {
                //Check if that piece is currently still a tile
                if (map[j, i] == 1)
                {
                    //Change it to not a tile
                    map[j, i] = 0;
                }
            }
        }

        while (floorY + 6 < boundY)
        {
            //Determine our next direction
            int randDir;
            if ((float)countX / boundX < 0.3f)
            {
                randDir = rand.Next(1, 7);
            } else
            {
                randDir = rand.Next(7);
            }

            switch (randDir)
            {
                //Up
                case 0:
                    for (int i = floorY + 3; i < floorY + 9; i++)
                    {
                        for (int j = floorX; j < floorX + 3; j++)
                        {
                            //Check if that piece is currently still a tile
                            if (map[j, i] == 1)
                            {
                                //Change it to not a tile
                                map[j, i] = 0;
                            }
                        }
                    }
                    floorY += 6;
                    countX = 3;
                    break;
                //Right
                case 1 or 2 or 5:
                    if (floorX + 9 < boundX)
                    {
                        int newSpace = 0;
                        for (int j = floorX + 3; j < floorX + 9; j++)
                        {
                            //Check if that piece is currently still a tile
                            if (map[j, floorY] == 1)
                            {
                                //Change it to not a tile
                                map[j, floorY] = 0;
                                newSpace++;
                            }
                            map[j, floorY + 1] = 0;
                            map[j, floorY + 2] = 0;
                        }
                        floorX += 6;
                        countX += newSpace;
                    }
                    break;
                    //Left
                case 3 or 4 or 6:
                    if (floorX - 6 > 1)
                    {
                        int newSpace = 0;
                        for (int j = floorX - 1; j >= floorX - 6; j--)
                        {
                            //Check if that piece is currently still a tile
                            if (map[j, floorY] == 1)
                            {
                                //Change it to not a tile
                                map[j, floorY] = 0;
                                newSpace++;
                            }
                            map[j, floorY + 1] = 0;
                            map[j, floorY + 2] = 0;
                        }
                        floorX -= 6;
                        countX += newSpace;
                    }
                    break;
            }
        }
        //Return the updated map
        return map;
    }
}