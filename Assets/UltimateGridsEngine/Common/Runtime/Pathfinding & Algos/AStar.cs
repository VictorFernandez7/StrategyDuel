using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class AStar
{
    /// <summary>
    /// Returns the best path as a List of Nodes
    /// </summary>
    public static List<GridTile> Search(GridTile start, GridTile goal)
    {
        // Start is goal
        if (start == goal)
        {
            return null;
        }

        Dictionary<GridTile, GridTile> came_from = new Dictionary<GridTile, GridTile>();
        Dictionary<GridTile, float> cost_so_far = new Dictionary<GridTile, float>();

        List<GridTile> path = new List<GridTile>();

        SimplePriorityQueue<GridTile> frontier = new SimplePriorityQueue<GridTile>();
        frontier.Enqueue(start, 0);

        came_from.Add(start, start);
        cost_so_far.Add(start, 0);

        GridTile current = start;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (current == goal) break; // Early exit

            foreach (GridTile next in GridManager.Instance.WalkableNeighbors(current, false, false, goal))
            {
                float new_cost = cost_so_far[current] + next.Cost();
                if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                {
                    cost_so_far[next] = new_cost;
                    came_from[next] = current;
                    float priority = new_cost + Utilities.Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    next.Priority = new_cost;
                }
            }
        }

        while (current && current != start)
        {
            path.Add(current);
            current = came_from[current];
        }
        path.Reverse();

        // If we were not able to reach the destination target tile just return an empty/null path
        //if (!path.Contains(goal))
        //    path.Clear();

        return path;
    }

    /// <summary>
    /// Returns the best path as a List of Nodes
    /// </summary>
    public static List<GridTile> Search(GridTile start, Vector2Int goalPosition)
    {
        var goalTile = GridManager.Instance.GetGridTileAtPosition(goalPosition);
        return goalTile != null ? Search(start, goalTile) : null;
    }
}