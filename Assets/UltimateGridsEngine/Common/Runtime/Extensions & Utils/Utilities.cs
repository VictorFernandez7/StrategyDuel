using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    // Manhattan distance used for rectangle grids
    public static float Heuristic(GridTile a, GridTile b)
    {
        return Mathf.Abs(a.m_GridPosition.x - b.m_GridPosition.x) + Mathf.Abs(a.m_GridPosition.y - b.m_GridPosition.y);
    }

    // Distance calculation based on cube coordinates used in hexagon grids
    public static float HexCubeDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
    }

    // Turn R offset coordinates into cube coordinates used in hexagon grids
    public static Vector3Int ROffsetToCubeCoordinates(Vector2Int gridPosition)
    {
        int x = gridPosition.x - ((gridPosition.y - (gridPosition.y & 1)) / 2);
        int y = gridPosition.y;
        int z = -x - y;
        return new Vector3Int(x, y, z);
    }

    // Takes two offset coordinates converts them to cube and then calculates the distance used for hexagong rids.
    public static float HexDistance(GridTile a, GridTile b)
    {
        var aCube = ROffsetToCubeCoordinates(a.m_GridPosition);
        var bCube = ROffsetToCubeCoordinates(b.m_GridPosition);
        return HexCubeDistance(aCube, bCube);
    }

    public static Color ColorFromRGB(int r, int g, int b)
    {
        return new Color((float)r / 256, (float)g / 256, (float)b / 256);
    }
}
