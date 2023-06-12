using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    private Rigidbody2D enemyRb;

    Pathfinding pathfinding;

    private void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        pathfinding = GameObject.Find("ProceduralGeneration").GetComponent<Pathfinding>();

        InvokeRepeating("EnemyPathfind", 5.0f, 1.5f);
    }

    private void EnemyPathfind()
    {
        Vector2 playerPos = playerRb.position;
        Vector2 enemyPos = enemyRb.position;

        List<PathNode> moveList = pathfinding.FindPath(Mathf.FloorToInt(enemyPos.x), Mathf.FloorToInt(enemyPos.y), Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));

        StartCoroutine(Chase(moveList));
    }

    private IEnumerator Chase(List<PathNode> moveList)
    {
        foreach (PathNode n in moveList)
        {
            yield return new WaitForSeconds(0.12f);
            enemyRb.position = new Vector3(n.x, n.y + 1, 0f);

        }
    }
}



// TODO:
// https://www.youtube.com/watch?v=alU04hvz6L4 behaviour
// https://www.youtube.com/watch?v=HVC9Med7hGw pathfind
// https://pavcreations.com/pathfinding-with-a-star-algorithm-in-unity-small-game-project/
// https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
// consider optimizations: https://github.com/Epicguru/FastAStar/blob/master/FastAStar/Pathfinding/Pathfinding.cs

