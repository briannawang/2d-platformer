using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public static void RandomPlaceItems(int[,] map, float seed, int itemCount, GameObject item, Transform player, Transform enemy)
    {
        //Seed our random
        System.Random rand = new System.Random(seed.GetHashCode());

        int itemX = rand.Next(1, map.GetUpperBound(0) - 1);
        int itemY = rand.Next(1, map.GetUpperBound(1) - 1);

        // player should have a 2 x 2 space to spawn
        while (map[itemX, itemY] != -1 || map[itemX + 1, itemY + 1] == 1 || map[itemX - 1, itemY + 1] == 1)
        {
            itemX = rand.Next(1, map.GetUpperBound(0) - 1);
            itemY = rand.Next(1, map.GetUpperBound(1) - 1);
        }
        player.position = new Vector3(itemX + 0.5f, itemY + 0.5f, 0f); // TODO: to use prefabs directly, may need: gameObject.transform.localPosition

        // enemy should also have 2 x 2 place to spawn
        itemX = rand.Next(1, map.GetUpperBound(0) - 1);
        itemY = rand.Next(1, map.GetUpperBound(1) - 1);
        while (map[itemX, itemY] != -1 || map[itemX + 1, itemY + 1] == 1 || map[itemX - 1, itemY + 1] == 1)
        {
            itemX = rand.Next(1, map.GetUpperBound(0) - 1);
            itemY = rand.Next(1, map.GetUpperBound(1) - 1);
        }
        enemy.position = new Vector3(itemX + 0.5f, itemY + 0.5f, 0f);

        int items = 0;

        while (items < itemCount)
        {
            itemX = rand.Next(1, map.GetUpperBound(0) - 1);
            itemY = rand.Next(1, map.GetUpperBound(1) - 1);

            // if tile is free to place item
            if (map[itemX, itemY] != 1)
            {
                // generate item at tile location
                Instantiate(item, new Vector3(itemX + 0.5f, itemY + 0.5f, 0f), Quaternion.identity);
                items++;
            }
        }
    }
}