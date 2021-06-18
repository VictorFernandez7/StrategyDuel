using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class RangeAlgorithms
{
    public static List<GridTile> SearchByParameters(GridTile start, RangeParameters rangeParameters)
    {
        switch (rangeParameters.m_RangeSearchType)
        {
            case RangeSearchType.RectangleByGridPosition:
            default:
                return RangeAlgorithms.SearchByGridPosition(start, rangeParameters.m_MaxReach, rangeParameters.m_WalkableTilesOnly, rangeParameters.m_UnOccupiedTilesOnly, rangeParameters.m_SquareRange, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
            case RangeSearchType.RectangleByMovement:
                return RangeAlgorithms.SearchByMovement(start, rangeParameters.m_MaxReach, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
            case RangeSearchType.HexagonByGridPosition:
                return RangeAlgorithms.HexagonSearchByGridPosition(start, rangeParameters.m_MaxReach, rangeParameters.m_WalkableTilesOnly, rangeParameters.m_UnOccupiedTilesOnly, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
            case RangeSearchType.HexagonByMovement:
                return RangeAlgorithms.HexagonSearchByMovement(start, rangeParameters.m_MaxReach, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
        }
    }

    public static List<GridTile> SearchByGridPosition(GridTile start, int maxReach, bool WalkableTilesOnly = true, bool unoccupiedTilesOnly = true, bool square = true, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
    {
        List<GridTile> range = new List<GridTile>();

        // Start is goal
        if (maxReach <= 0)
        {
            range.Add(start);
            return range;
        }
        if (maxReach < MinReach)
        {
            return range;
        }

        Dictionary<GridTile, float> cost_so_far = new Dictionary<GridTile, float>();
        SimplePriorityQueue<GridTile> frontier = new SimplePriorityQueue<GridTile>();
        frontier.Enqueue(start, 0);
        cost_so_far.Add(start, 0);

        GridTile current = start;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (cost_so_far[current] <= maxReach)
            {
                var neighbors = WalkableTilesOnly == true ? GridManager.Instance.WalkableNeighbors(current, ignoreHeight, unoccupiedTilesOnly, null, GridManager.defaultRectangle8Directions) : GridManager.Instance.Neighbors(current, ignoreHeight, GridManager.defaultRectangle8Directions);
                foreach (GridTile next in neighbors)
                {
                    float new_cost = cost_so_far[current] + (square == true ? 1 : Utilities.Heuristic(current, next));
                    if (!cost_so_far.ContainsKey(next))
                    {
                        cost_so_far[next] = new_cost;
                        float priority = new_cost;
                        frontier.Enqueue(next, priority);

                        if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                        {
                            range.Add(next);
                        }
                    }
                }
            }
        }

        // remove the starting tile if required
        if (!includeStartingTile)
        {
            if (range.Contains(start))
            {
                range.Remove(start);
            }
        }

        return range;
    }


    public static List<GridTile> SearchByMovement(GridTile start, int maxReach, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
    {
        List<GridTile> range = new List<GridTile>();

        // Start is goal
        if (maxReach == 0)
        {
            range.Add(start);
            return range;
        }
        if (maxReach < MinReach)
        {
            return range;
        }

        Dictionary<GridTile, float> cost_so_far = new Dictionary<GridTile, float>();
        SimplePriorityQueue<GridTile> frontier = new SimplePriorityQueue<GridTile>();
        frontier.Enqueue(start, 0);
        cost_so_far.Add(start, 0);

        GridTile current = start;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (cost_so_far[current] <= maxReach)
            {
                foreach (GridTile next in GridManager.Instance.WalkableNeighbors(current, ignoreHeight))
                {
                    float new_cost = cost_so_far[current] + next.Cost();
                    if (!cost_so_far.ContainsKey(next))
                    {
                        cost_so_far[next] = new_cost;
                        float priority = new_cost;
                        frontier.Enqueue(next, priority);

                        if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                        {
                            range.Add(next);
                        }
                    }
                }
            }
        }

        // remove the starting tile if required
        if (!includeStartingTile)
        {
            if (range.Contains(start))
            {
                range.Remove(start);
            }
        }

        return range;
    }

    public static List<GridTile> HexagonSearchByGridPosition(GridTile start, int maxReach, bool WalkableTilesOnly = true, bool unoccupiedTilesOnly = true, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
    {
        List<GridTile> range = new List<GridTile>();

        // Start is goal
        if (maxReach == 0)
        {
            range.Add(start);
            return range;
        }
        if (maxReach < MinReach)
        {
            return range;
        }

        Dictionary<GridTile, float> cost_so_far = new Dictionary<GridTile, float>();
        SimplePriorityQueue<GridTile> frontier = new SimplePriorityQueue<GridTile>();
        frontier.Enqueue(start, 0);
        cost_so_far.Add(start, 0);

        GridTile current = start;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (cost_so_far[current] <= maxReach)
            {
                var neighbors = WalkableTilesOnly == true ? GridManager.Instance.WalkableNeighbors(current, ignoreHeight, unoccupiedTilesOnly, null) : GridManager.Instance.Neighbors(current, ignoreHeight);
                foreach (GridTile next in neighbors)
                {
                    float new_cost = Utilities.HexDistance(next, start);
                    if (!cost_so_far.ContainsKey(next))
                    {
                        cost_so_far[next] = new_cost;
                        float priority = new_cost;
                        frontier.Enqueue(next, priority);

                        if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                        {
                            range.Add(next);
                        }
                    }
                }
            }
        }

        // remove the starting tile if required
        if (!includeStartingTile)
        {
            if (range.Contains(start))
            {
                range.Remove(start);
            }
        }

        return range;
    }

    public static List<GridTile> HexagonSearchByMovement(GridTile start, int maxReach, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
    {
        List<GridTile> range = new List<GridTile>();

        // Start is goal
        if (maxReach == 0)
        {
            range.Add(start);
            return range;
        }
        if (maxReach < MinReach)
        {
            return range;
        }

        Dictionary<GridTile, float> cost_so_far = new Dictionary<GridTile, float>();
        SimplePriorityQueue<GridTile> frontier = new SimplePriorityQueue<GridTile>();
        frontier.Enqueue(start, 0);
        cost_so_far.Add(start, 0);

        GridTile current = start;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (cost_so_far[current] <= maxReach)
            {
                foreach (GridTile next in GridManager.Instance.WalkableNeighbors(current, ignoreHeight, true, null))
                {
                    float new_cost = cost_so_far[current] + next.Cost();
                    if (!cost_so_far.ContainsKey(next))
                    {
                        cost_so_far[next] = new_cost;
                        float priority = new_cost;
                        frontier.Enqueue(next, priority);

                        if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                        {
                            range.Add(next);
                        }
                    }
                }
            }
        }

        // remove the starting tile if required
        if (!includeStartingTile)
        {
            if (range.Contains(start))
            {
                range.Remove(start);
            }
        }

        return range;
    }
}