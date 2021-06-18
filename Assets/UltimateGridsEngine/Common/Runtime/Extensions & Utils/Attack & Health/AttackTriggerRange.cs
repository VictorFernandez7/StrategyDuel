using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerRange : AttackTrigger
{
    [Header("Range Parameters")]
    public RangeParameters m_RangeParameters;


    public override List<Health> GetVictims()
    {
        List<Health> victims = new List<Health>();

        var targetPositions = GetTargetPositions();
        foreach (Vector2Int targetPos in targetPositions)
        {
            var gridObjectAtPos = GridManager.Instance.GetGridObjectAtPosition(targetPos);
            if (gridObjectAtPos != null)
            {
                var healthComp = gridObjectAtPos.GetComponent<Health>();
                if (healthComp != null && !victims.Contains(healthComp))
                {
                    victims.Add(healthComp);
                }
            }
        }

        return victims;
    }

    public override Health GetVictimAtPosition(Vector2Int position)
    {
        var targetPositions = GetTargetPositions();
        foreach (Vector2Int targetPos in targetPositions)
        {
            if (targetPos != position)
            {
                continue;
            }

            var gridObjectAtPos = GridManager.Instance.GetGridObjectAtPosition(targetPos);
            if (gridObjectAtPos != null)
            {
                var healthComp = gridObjectAtPos.GetComponent<Health>();
                if (healthComp != null)
                {
                    return healthComp;
                }
            }
        }

        return null;
    }

    public override List<GridTile> GetTargetTiles()
    {
        var targetTiles = RangeAlgorithms.SearchByParameters(_gridObject.m_CurrentGridTile, m_RangeParameters);

        return targetTiles;
    }

    public override List<Vector2Int> GetTargetPositions()
    {
        var positions = new List<Vector2Int>();

        var targetTiles = GetTargetTiles();

        foreach (GridTile tile in targetTiles)
        {
            if (!positions.Contains(tile.m_GridPosition))
            {
                positions.Add(tile.m_GridPosition);
            }
        }

        return positions;
    }

    public override bool HasVictimAtPosition(Vector2Int position)
    {
        return GetVictimAtPosition(position) != null;
    }
}
