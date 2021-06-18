using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using System;

[Serializable]
public class GridObject : MonoBehaviour
{
    [Header("Grid Position")]
    [NaughtyAttributes.ReadOnly] public Vector2Int m_GridPosition = new Vector2Int(int.MaxValue, int.MaxValue);   // Default grid position on a non set GridObject
    [Header("Grid Tile")]
    [NaughtyAttributes.ReadOnly] public GridTile m_CurrentGridTile;  // Reference to the current GridTile this GridObject is at
    [NaughtyAttributes.ReadOnly] public GridTile m_PreviousGridTile; // Reference to the previous Gridtile we were at
    [Header("Initial Facing Relative Position")]
    public Orientations m_InitialOrientation = Orientations.North;

    [SerializeField, Header("Current Facing Relative Position"), NaughtyAttributes.ReadOnly]
    protected Vector2Int _facingDirection = Vector2Int.zero;          // Current facing direction

    public Vector2Int m_FacingDirection
    {
        get
        {
            if (_facingDirection == Vector2Int.zero)
            {
                _facingDirection = Vector2Int.up;
            }
            return _facingDirection;
        }
        set
        {
            m_PreviousFacingDirection = _facingDirection;
            _facingDirection = value;
        }
    }

    public Vector2Int m_PreviousFacingDirection
    {
        get
        {
            if (_previousFacingDirection == Vector2Int.zero)
            {
                _previousFacingDirection = m_FacingDirection;
            }
            return _previousFacingDirection;
        }
        set
        {
            _previousFacingDirection = value;
        }
    }

    public Vector2Int m_FacingGridPosition { get { return m_GridPosition + m_FacingDirection; } }
    public List<MovementBlocker> m_MovementBlockers { get; protected set; }

    protected Vector2Int _previousFacingDirection;  // Previous facing direction
    protected bool _initialized = false;            // Initialization flag
    protected GridMovement _gridMovement;

    protected virtual void OnDisable()
    {
        if (_initialized)
        {
            _initialized = false;
            RemoveFromTile();
            if (GridManager.Instance != null)
                GridManager.Instance.RemoveGridObject(this);
        }
    }

    protected virtual void Awake()
    {
        m_MovementBlockers = GetComponents<MovementBlocker>().ToList();
        _gridMovement = GetComponent<GridMovement>();
    }

    // You should always place GridObjects and GridTiles using their respective Brushes which initialize them automatically
    protected virtual void Start()
    {
        // Initial facing direction
        m_FacingDirection = GridManager.Instance.GetRelativeNeighborPositionFromOrientation(m_InitialOrientation.ToString(), (m_GridPosition.y & 1) == 1);
        if (_gridMovement != null)
        {
            _gridMovement.Rotate(m_FacingGridPosition);
        }

        // Initialize
        if (!_initialized && m_GridPosition != new Vector2Int(int.MaxValue, int.MaxValue))
        {
            Initialize(new Vector3Int(m_GridPosition.x, m_GridPosition.y, 0));
        }
    }

    public virtual void Initialize(Vector3Int? gridPosition = null)
    {
        _initialized = true;

        // Set the GridPosition
        if (gridPosition.HasValue)
        {
            m_GridPosition = gridPosition.Value.ToVector2IntXY();
        }
        else
        {
            // Set the grid position based on the world position
            m_GridPosition = GetGridPosFromWorldPosition();
        }

        // Check if there is a tile at the GridPosition
        if (GridManager.Instance.ExistsTileAtPosition(m_GridPosition))
        {
            GridTile gridTile = GridManager.Instance.GetGridTileAtPosition(m_GridPosition);
            SetCurrentGridTile(gridTile);   // Update the current tile on this GridObject
            AddToTile(gridTile);    // Add this Gridobject to the occupant list on the target Gridtile
        }
        else
        {
            Debug.Log("There is not a tile at this GridObject's GridPosition");
        }

        // Add this GridObject to the GridManager's GridObject List
        GridManager.Instance.AddGridObject(this);

        // Get the movement blockers for this GridObject
        m_MovementBlockers = GetComponents<MovementBlocker>().ToList();
    }

    // Method to get the GridPosition for the objects WorldPosition, this has been deprecated and will be removed in next update
    public virtual Vector2Int GetGridPosFromWorldPosition()
    {
        if (m_CurrentGridTile == null)
        {
            return base.transform.position.ToVector2IntXZ();
        }

        return (transform.position).ToVector2IntXZ();
    }

    // Sets the current GridTile
    public virtual void SetCurrentGridTile(GridTile targetTile)
    {
        m_PreviousGridTile = m_CurrentGridTile;
        m_CurrentGridTile = targetTile;
    }

    // Unsets the current GridTile
    public virtual void UnsetCurrentGridTile()
    {
        if (m_CurrentGridTile != null)
        {
            m_CurrentGridTile = null;
        }
    }

    // Add this Gridobject to the occupant list on the target Gridtile
    public virtual void AddToTile(GridTile gridTile)
    {
        // If the tile exists
        if (gridTile != null)
        {
            // And it is not already occupied
            if (!gridTile.IsTileOccupied())
            {
                m_CurrentGridTile.AddOccupyingGridObject(this);
            }
        }
    }

    // Removes this Gridobject of the occupant list on the current GridTile
    public virtual void RemoveFromTile()
    {
        if (m_CurrentGridTile != null)
        {
            m_CurrentGridTile.RemoveOccupyingGridObject(this);
        }
    }

    // This is going to be correctly implemented together with directional blocking on the first update
    public virtual bool BlocksMovementFor(GridObject gridObject)
    {
        for (int i = 0; i < m_MovementBlockers.Count; i++)
        {
            if (m_MovementBlockers[i].TryBlockMovementFor(gridObject))
            {
                return true;
            }
        }
        return false;
    }

    public virtual bool BlocksMovement()
    {
        for (int i = 0; i < m_MovementBlockers.Count; i++)
        {
            if (m_MovementBlockers[i] != null && m_MovementBlockers[i].BlocksMovement)
            {
                return true;
            }
        }
        return false;
    }
}
