using System;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public enum RangeSearchType { RectangleByGridPosition, RectangleByMovement, HexagonByGridPosition, HexagonByMovement }

[Serializable]
public class RangeParameters
{
    [Header("Type")]
    public RangeSearchType m_RangeSearchType = RangeSearchType.RectangleByGridPosition;
    [ShowIf("SearchTypeIsRectByGridPosition")]
    public bool m_SquareRange = false;
    public bool SearchTypeIsRectByGridPosition() { return m_RangeSearchType == RangeSearchType.RectangleByGridPosition; }

    [Header("Reach")]
    public int m_MaxReach = 3;
    public int m_MinimunReach = 0;

    [Header("Tile Settings")]
    public bool m_WalkableTilesOnly = true;
    public bool m_UnOccupiedTilesOnly = true;
    public bool m_IgnoreTilesHeight = false;
    public bool m_IncludeStartingTile = false;
}