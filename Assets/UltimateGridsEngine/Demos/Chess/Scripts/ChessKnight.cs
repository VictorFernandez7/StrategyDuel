using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessKnight : ChessPiece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPosition)
    {

        var tempLocations = new List<Vector2Int>();
        var locations = new List<Vector2Int>();

        // Movement
        tempLocations.Add(new Vector2Int(gridPosition.x - 1, gridPosition.y + 2));
        tempLocations.Add(new Vector2Int(gridPosition.x + 1, gridPosition.y + 2));
        tempLocations.Add(new Vector2Int(gridPosition.x + 2, gridPosition.y + 1));
        tempLocations.Add(new Vector2Int(gridPosition.x - 2, gridPosition.y + 1));
        tempLocations.Add(new Vector2Int(gridPosition.x + 2, gridPosition.y - 1));
        tempLocations.Add(new Vector2Int(gridPosition.x - 2, gridPosition.y - 1));
        tempLocations.Add(new Vector2Int(gridPosition.x + 1, gridPosition.y - 2));
        tempLocations.Add(new Vector2Int(gridPosition.x - 1, gridPosition.y - 2));

        foreach (Vector2Int loc in tempLocations)
        {
            if (GridManager.Instance.GetGridTileAtPosition(loc))
            {
                if (GridManager.Instance.GetGridObjectAtPosition(loc) == null)
                {
                    locations.Add(loc);
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
