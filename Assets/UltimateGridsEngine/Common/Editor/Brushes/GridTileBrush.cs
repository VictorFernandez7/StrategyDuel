using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GridTileBrush
{
    [Header("Tile Prefab")]
    public GridTile m_GridTile;

    [Header("Height in Grid")]
    public int m_Height = 0;

    [Header("Position Offset")]
    public Vector3 m_Offset = Vector3.zero;

    [Header("Rotation")]
    public Vector3 m_Rotation = Vector3.zero;

    public GridTileBrush(GridTileBrush gridTileBrush)
    {
        this.m_GridTile = gridTileBrush.m_GridTile;
        this.m_Height = gridTileBrush.m_Height;
        this.m_Offset = gridTileBrush.m_Offset;
        this.m_Rotation = gridTileBrush.m_Rotation;
    }

    public GridTileBrush()
    {

    }

    public void ResetParameters()
    {
        m_Height = 0;
        m_Offset = Vector3.zero;
        m_Rotation = Vector3.zero;
    }
}
