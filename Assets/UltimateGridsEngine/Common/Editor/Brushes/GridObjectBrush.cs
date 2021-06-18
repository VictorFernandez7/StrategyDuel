using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class GridObjectBrush
{
    [Header("Tile Prefab")]
    public GridObject m_GridObject;

    [Header("Initial Orientation")]
    public Orientations m_InitialOrientation = Orientations.North;

    public GridObjectBrush(GridObjectBrush gridTileBrush)
    {
        this.m_GridObject = gridTileBrush.m_GridObject;
        this.m_InitialOrientation = gridTileBrush.m_InitialOrientation;
    }

    public GridObjectBrush() { }

    public void ResetParameters()
    {
        m_InitialOrientation = Orientations.North;
    }
}
