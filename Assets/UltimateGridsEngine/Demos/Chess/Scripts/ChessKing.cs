using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessKing : ChessPiece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        List<Vector2Int> directions = new List<Vector2Int>(BishopDirections);
        directions.AddRange(RookDirections);

        // Movement
        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextGridPoint = new Vector2Int(gridPoint.x + dir.x, gridPoint.y + dir.y);
            if (GridManager.Instance.GetGridTileAtPosition(nextGridPoint))
            {
                if (!GridManager.Instance.GetGridObjectAtPosition(nextGridPoint))
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
