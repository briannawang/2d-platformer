using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private PathNode[,] map;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public void PathfindingCtor(int[,] cellMap)
    {
        map = new PathNode[cellMap.GetUpperBound(0), cellMap.GetUpperBound(1)];

        for (int x = 0; x < cellMap.GetUpperBound(0); x++)
        {
            for (int y = 0; y < cellMap.GetUpperBound(1); y++)
            {
                // when generated, cellmap has a flag on whether it is traversable by the enemy - only walls/ceilings and open vertical spaces
                // so when pathfinding it looks natural
                map[x, y] = new PathNode(x, y, cellMap[x, y] < 0, cellMap[x, y] == -1); // only traversable paths
            }
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        endX = Mathf.Min(map.GetUpperBound(0) - 1, Mathf.Max(0, endX));
        endY = Mathf.Min(map.GetUpperBound(1) - 1, Mathf.Max(0, endY));

        PathNode startNode = map[startX, startY];
        PathNode endNode = map[endX, endY];

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        if (!endNode.endable)
        {
            return closedList;
            //endX += (endX - startX > 0) ? -1 : 1;
            //endX = Mathf.Min(map.GetUpperBound(0) - 1, Mathf.Max(0, endX));
            //endY += (endY - startY > 0) ? -1 : 1;
            //endY = Mathf.Min(map.GetUpperBound(1) - 1, Mathf.Max(0, endY));
            //endNode = map[endX, endY];
        }

        // initialize map w/ gcosts, fcosts, prevNode
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                PathNode pathNode = map[x, y];
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.prevNode = null;
            }
        }

        // initialize startNode vals
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        // go through openlist until found endNode
        while (openList.Count > 0)
        {
            PathNode currNode = GetLowestFCostNode(openList);
            if (currNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currNode); // curr node has been searched
            closedList.Add(currNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.traversable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currNode.gCost + CalculateDistanceCost(currNode, neighbourNode);

                // if current tentative path to neighbour is better, replace it
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.prevNode = currNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // couldnt find path
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currNode.x - 1 >= 0)
        {
            // left
            neighbourList.Add(GetNode(currNode.x - 1, currNode.y));

            // left-down
            if (currNode.y - 1 >= 0) neighbourList.Add(GetNode(currNode.x - 1, currNode.y - 1));

            // left-up
            if (currNode.y + 1 < map.GetUpperBound(1)) neighbourList.Add(GetNode(currNode.x - 1, currNode.y +1));
        }

        if(currNode.x + 1 < map.GetUpperBound(0))
        {
            // right
            neighbourList.Add(GetNode(currNode.x + 1, currNode.y));

            // right-down
            if (currNode.y - 1 >= 0) neighbourList.Add(GetNode(currNode.x + 1, currNode.y - 1));

            // right-up
            if (currNode.y + 1 < map.GetUpperBound(1)) neighbourList.Add(GetNode(currNode.x + 1, currNode.y + 1));
        }

        // down
        if (currNode.y - 1 >= 0) neighbourList.Add(GetNode(currNode.x, currNode.y - 1));

        // up
        if (currNode.y + 1 < map.GetUpperBound(1)) neighbourList.Add(GetNode(currNode.x, currNode.y + 1));

        return neighbourList;
    }

    private PathNode GetNode(int x, int y)
    {
        return map[x, y];
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);

        PathNode currNode = endNode;

        // startnode has prev is null
        while (currNode.prevNode != null)
        {
            path.Add(currNode.prevNode);
            currNode = currNode.prevNode;
        }

        path.Reverse();

        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFNode.fCost)
            {
                lowestFNode = pathNodeList[i];
            }
        }

        return lowestFNode;
    }
}
