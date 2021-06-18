#pragma warning disable 67

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class GridTile : MonoBehaviour
{
    public delegate void GridObjectEnterHandler(GridObject gridObject, GridTile gridTile);
    public delegate void GridObjectExitHandler(GridObject gridObject, GridTile gridTile);
    public delegate void GridObjectEndMovementHandler(GridMovement gridMovement, GridTile startGridTile, GridTile endGridTile);
    public event GridObjectEnterHandler OnGridObjectEnter;
    public event GridObjectExitHandler OnGridObjectExit;
    public event GridObjectEndMovementHandler OnGridObjectEndMovement;

    [Header("GridTile Settings")]
    [NaughtyAttributes.ReadOnly] public Vector2Int m_GridPosition = new Vector2Int(int.MaxValue, int.MaxValue); 	// The position on the grid
    public int m_TileHeight = 0; 		// Height of this tile
    public Vector3 m_WorldPosition { get { return transform.position; } } // The Transforms scene position
    public bool m_IsTileWalkable = true; // Wether or not dynamic GridObjects will be able to move to the tile by default
    [Header("Agents Pivot Offset")]
    public Vector3 m_GridObjectsPivotOffset = new Vector3(0, 0.5f, 0);

    public Vector3 m_GridObjectsPivotPosition { get { return m_WorldPosition + m_GridObjectsPivotOffset; } }

    [Header("Pathfinding Settings")]
    public int m_costOfMovingToTile = 1; 		// The cost of moving to this tile (used in pathfinding)
    [HideInInspector] public float Priority; 	// Used for AStar pathfinding
    [Header("Manual Neighbors")]
    public List<GridTile> m_manualNeighbors = new List<GridTile>(); // This is to be able to assign a tile's neighbors manually when desired

    [Header("GridObjects in Tile")]
    public List<GridObject> m_OccupyingGridObjects = new List<GridObject>();

    protected bool _initialized = false;    // Initiliazed flag

    protected virtual void OnDisable()
    {
        if (_initialized)
        {
            _initialized = false;
            RemoveTile();
        }
    }

    //  Any GridTile or GridObject requires its GridPosition to be set so it can be initialized
    protected virtual void Start()
    {
        if (!_initialized && m_GridPosition != new Vector2Int(int.MaxValue, int.MaxValue))
        {
            Initialize(new Vector3Int(m_GridPosition.x, m_GridPosition.y, m_TileHeight));
        }
    }

    // Method used to initialize the tile
    public virtual void Initialize(Vector3Int posInfo)
    {
        _initialized = true;

        // set the positions
        m_GridPosition = posInfo.ToVector2IntXY();
        m_TileHeight = posInfo.z;

        // Check if there is already a tile at the target position, if there isn't proceed, if there is and it isn't this destroy the tile
        if (!GridManager.Instance.ExistsTileAtPosition(m_GridPosition))
        {
            GridManager.Instance.AddGridTile(m_GridPosition, this);
        }
        else if (GridManager.Instance.GetGridTileAtPosition(m_GridPosition) != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    // Remove this tile from the tile's dictionary on the GridManager
    protected virtual void RemoveTile()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.RemoveGridTile(this);
    }

    // Checks if a GridObject is able to move to the tile
    public virtual bool CanMoveToTile()
    {
        return (m_IsTileWalkable && !IsTileOccupied());
    }

    // Checks if any of the occupying GridObjects block movement
    public virtual bool IsTileOccupied()
    {
        if (m_OccupyingGridObjects != null && m_OccupyingGridObjects.Count > 0)
        {
            foreach (GridObject obj in m_OccupyingGridObjects)
            {
                if (obj == null)
                    continue;

                if (obj.BlocksMovement())
                {
                    return true;
                }
            }
        }

        return false;
    }

    // adds a GridObject to this tile
    public virtual void AddOccupyingGridObject(GridObject targetGridObject)
    {
        m_OccupyingGridObjects.Add(targetGridObject);

        // Invoke the OnGridObjectEnter event
        if (OnGridObjectEnter != null)
        {
            OnGridObjectEnter(targetGridObject, this);
        }
    }

    // Removes a GridObject from this tile
    public virtual void RemoveOccupyingGridObject(GridObject targetGridObject)
    {
        if (m_OccupyingGridObjects.Contains(targetGridObject))
        {
            m_OccupyingGridObjects.Remove(targetGridObject);
        }

        // Invoke the OnGridObjectExit event
        if (OnGridObjectExit != null)
        {
            OnGridObjectExit(targetGridObject, this);
        }
    }

    // Method which returns the cost of moving to this tile
    public virtual int Cost()
    {
        return m_costOfMovingToTile;
    }

    // Used to set the hovered tile
    protected virtual void OnMouseEnter()
    {
        GridManager.Instance.SetHoveredTile(this);
    }

    // Used to unset the hovered tile
    protected virtual void OnMouseExit()
    {
        GridManager.Instance.UnsetHoveredTile(this);
    }
}