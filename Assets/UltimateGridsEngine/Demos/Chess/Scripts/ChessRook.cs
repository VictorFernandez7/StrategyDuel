using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessRook : ChessPiece
{

    public int m_MovementRange = 10;

    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        // Movement
        foreach (Vector2Int dir in RookDirections)
        {
            for (int i = 1; i <= m_MovementRange; i++)
            {
                Vector2Int nextGridPoint = new Vector2Int(gridPoint.x + i * dir.x, gridPoint.y + i * dir.y);
                if (GridManager.Instance.GetGridTileAtPosition(nextGridPoint) == null || GridManager.Instance.GetGridObjectAtPosition(nextGridPoint))
                {
                    break;
                }
                else
                {
                    locations.Add(nextGridPoint);
                }
            }
        }

        // Attack
        if (m_Attack != null)
        {
            var tempPositions = m_Attack.GetAllTargetPositions();
            foreach (Vector2Int pos in tempPositions)
            {
                if (GridManager.Instance.GetGridTileAtPosition(pos))
                {
                    if (GridManager.Instance.GetGridObjectAtPosition(pos))
                    {
                        if (!locations.Contains(pos))
                        {
                            locations.Add(pos);
                        }
                    }
                }
            }
        }

        return locations;
    }
}
