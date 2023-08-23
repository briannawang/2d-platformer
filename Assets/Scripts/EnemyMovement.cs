using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    private Rigidbody2D enemyRb;

    Pathfinding pathfinding;
    private bool chasing = false;
    private bool chaseCalc = false;

    private void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        pathfinding = GameObject.Find("ProceduralGeneration").GetComponent<Pathfinding>();
    }

    private void Update()
    {
        Vector2 playerPos = playerRb.position;
        Vector2 enemyPos = enemyRb.position;

        // if not chasing, then start chase
        if (Utils.withinRange(playerPos, enemyPos, 13))
        {
            if (!chasing)
            {
                InvokeRepeating("EnemyChase", 0.0f, 0.5f); // (method, time to start up, time to repeat)
                chasing = true;
            }

        }
        // if currently chasing and gets out of range, stop chase
        else
        {
            if (chasing)
            {
                CancelInvoke("EnemyChase");
                chasing = false;
            }
            InvokeRepeating("EnemyWander", 0.0f, 3.5f); // (method, time to start up, time to repeat)
        }
    }

    private void EnemyWander()
    {
        System.Random rand = new System.Random(0.GetHashCode());
        int targX = rand.Next(0, 30) - 15;
        int targY = rand.Next(0, 30) - 15;
        Vector2 enemyPos = enemyRb.position;
        List<PathNode> moveList = pathfinding.FindPath(Mathf.FloorToInt(enemyPos.x), Mathf.FloorToInt(enemyPos.y), Mathf.FloorToInt(enemyPos.x) + targX, Mathf.FloorToInt(enemyPos.y) + targY);
        Pathfind(moveList);
    }

    private void EnemyChase()
    {
        Vector2 playerPos = playerRb.position;
        Vector2 enemyPos = enemyRb.position;
        List<PathNode> moveList = pathfinding.FindPath(Mathf.FloorToInt(enemyPos.x), Mathf.FloorToInt(enemyPos.y), Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));
        Pathfind(moveList);
    }

    async void Pathfind(List<PathNode> moveList)
    {
        if (!chaseCalc)
        {
            chaseCalc = true;
            foreach (PathNode n in moveList)
            {
                await Task.Delay(130); // time between each movement
                enemyRb.position = new Vector3(n.x + 0.5f, n.y + 0.5f, 0f);
            }
            chaseCalc = false;
        }
    }
}



// TODO:
// https://www.youtube.com/watch?v=alU04hvz6L4 behaviour
// https://www.youtube.com/watch?v=HVC9Med7hGw pathfind
// https://pavcreations.com/pathfinding-with-a-star-algorithm-in-unity-small-game-project/
// https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
// consider optimizations: https://github.com/Epicguru/FastAStar/blob/master/FastAStar/Pathfinding/Pathfinding.cs

