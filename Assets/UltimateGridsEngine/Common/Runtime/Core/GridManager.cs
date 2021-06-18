using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;

[RequireComponent(typeof(Grid))]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public Grid m_Grid; // Reference to the current Grid being used
    public bool m_UsesHeight = false; // Wether or not we should use height for the pathfinding

    [HideInInspector] public Dictionary<Vector2Int, GridTile> m_GridTiles = new Dictionary<Vector2Int, GridTile>();// Grid tiles dictionary
    [HideInInspector] public List<GridObject> m_GridObjects = new List<GridObject>();// Grid objects list

    [Header("Neighboring")]
    public NeighboringTypes m_NeighboringType;
    [ShowIf("ShowCustomNeighbors")]
    public List<Vector2Int> m_CustomNeighbors = new List<Vector2Int>();  // List of the custom neighbors (if chosen so)
    public bool ShowCustomNeighbors() { return m_NeighboringType == NeighboringTypes.Custom; }

    [Header("Hovered Tile")]
    public GridTile m_HoveredGridTile = null; // Currently highlighted GridTile

    // Holders
    protected Transform _gridTilesHolder;
    public Transform GridTilesHolder
    {
        get
        {
            if (_gridTilesHolder == null) { GetHolders(); }
            return _gridTilesHolder;
        }
        protected set { _gridTilesHolder = value; }
    }
    protected Transform _gridObjectsHolder;
    public Transform GridObjectsHolder
    {
        get
        {
            if (_gridObjectsHolder == null) { GetHolders(); }
            return _gridObjectsHolder;
        }
        protected set { _gridObjectsHolder = value; }
    }
    // Singleton
    protected static GridManager _instance = null;
    public static GridManager Instance
    {
        get
        {
            if (_instance == null) { _instance = (GridManager)FindObjectOfType(typeof(GridManager)); }
            return _instance;
        }
        protected set { _instance = value; }
    }

    protected virtual void Reset()
    {
        m_Grid = GetComponent<Grid>();
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        // Update holders
        GetHolders();
    }

    public List<Vector2Int> GetNeighborPositions(bool oddRow = false)
    {
        var gridPositions = new List<Vector2Int>();
        switch (m_NeighboringType)
        {
            case NeighboringTypes.Rect4Directions:
                gridPositions = defaultRectangle4Directions;
                break;
            case NeighboringTypes.Rect8Directions:
                gridPositions = defaultRectangle8Directions;
                break;
            case NeighboringTypes.HexagonDefault:
                gridPositions = oddRow ? defaultOddHexagonalDirections : defaultEvenHexagonalDirections;
                break;
            case NeighboringTypes.Custom:
                gridPositions = m_CustomNeighbors;
                break;
        }

        return gridPositions;
    }

    public Vector3Int GetRotationAxisVector()
    {
        switch (m_Grid.cellSwizzle)
        {
            case Grid.CellSwizzle.XYZ:
            default:
                return Vector3.forward.ToVector3Int();
            case Grid.CellSwizzle.XZY:
                return Vector3Int.up;
            case Grid.CellSwizzle.YXZ:
                return Vector3.forward.ToVector3Int();
            case Grid.CellSwizzle.YZX:
                return Vector3Int.right;
            case Grid.CellSwizzle.ZXY:
                return Vector3Int.up;
            case Grid.CellSwizzle.ZYX:
                return Vector3Int.right;
        }
    }

    // Adds the tile to the tile dictionary if the position is not already occupied
    public void AddGridTile(Vector2Int gridPosition, GridTile gridTile)
    {
        if (!ExistsTileAtPosition(gridPosition))
        {
            m_GridTiles.Add(gridPosition, gridTile);
        }
    }

    // Removes the target tile from the tile dictionary
    public void RemoveGridTile(GridTile gridTile)
    {
        if (m_GridTiles.ContainsValue(gridTile))
        {
            m_GridTiles.Remove(gridTile.m_GridPosition);
        }
    }

    // Removes the tile at the target position
    public void RemoveGridTileAtPosition(Vector2Int gridPosition)
    {
        if (m_GridTiles.ContainsKey(gridPosition))
            m_GridTiles.Remove(gridPosition);
    }

    // Checks if a tile exists at the target position
    public bool ExistsTileAtPosition(Vector2Int gridPosition)
    {
        return Application.isPlaying ? m_GridTiles.ContainsKey(gridPosition) : GetGridTileAtPositionInEditor(gridPosition) != null;
    }

    // Returns the tile at the target position, if there is one
    public GridTile GetGridTileAtPosition(Vector2Int gridPosition)
    {
        if (!Application.isPlaying)
        {
            return GetGridTileAtPositionInEditor(gridPosition);
        }
        else
        {
            if (!ExistsTileAtPosition(gridPosition))
            {
                return null;
            }

            return m_GridTiles[gridPosition];
        }
    }

    // Sets the currently hovered tile
    public void SetHoveredTile(GridTile gridTile)
    {
        m_HoveredGridTile = gridTile;
    }

    // Unsets the currently hovered tile
    public void UnsetHoveredTile(GridTile gridTile)
    {
        if (m_HoveredGridTile == gridTile)
            m_HoveredGridTile = null;
    }

    // Adds a GridObject to the GridObject List
    public void AddGridObject(GridObject gridObject)
    {
        if (!m_GridObjects.Contains(gridObject))
        {
            m_GridObjects.Add(gridObject);
        }
    }

    // Removes a GridObject to the GridObject List
    public void RemoveGridObject(GridObject gridObject)
    {
        if (m_GridObjects.Contains(gridObject))
        {
            m_GridObjects.Remove(gridObject);
        }
    }

    public bool ExistsGridObjectAtPosition(Vector2Int gridPosition)
    {
        return (GetGridObjectAtPosition(gridPosition) != null);
    }

    public GridObject GetGridObjectAtPosition(Vector2Int gridPosition)
    {
        if (!Application.isPlaying)
        {
            return GetGridObjectAtPositionInEditor(gridPosition);
        }
        else
        {
            foreach (GridObject gObj in m_GridObjects)
            {
                if (gObj.m_GridPosition == gridPosition)
                {
                    return gObj;
                }
            }

            return null;
        }
    }

    // Default rectangle directions 
    public static List<Vector2Int> defaultRectangle4Directions = new List<Vector2Int>() {
        new Vector2Int(0, 1), // top
        new Vector2Int(1, 0), // right
        new Vector2Int(0, -1),// bottom
                new Vector2Int(-1, 0) // left
    };

    // Default rectangle 8 directions (diagonals) 
    public static List<Vector2Int> defaultRectangle8Directions = new List<Vector2Int>() {
        new Vector2Int(0, 1), // top
        new Vector2Int(1, 1), // top-right
        new Vector2Int(1, 0), // right
        new Vector2Int(1, -1), // bottom-right
        new Vector2Int(0, -1), // bottom
        new Vector2Int(-1, -1), // bottom-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(-1, 1) // top-left     
    };

    public static List<Vector2Int> defaultEvenHexagonalDirections = new List<Vector2Int>() {
        new Vector2Int(0, 1),// top-right
        new Vector2Int(1, 0),// right
        new Vector2Int(0, -1), // bottom-right
        new Vector2Int(-1, -1),// bottom-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(-1, 1) // top-left 
    };

    public static List<Vector2Int> defaultOddHexagonalDirections = new List<Vector2Int>() {
        new Vector2Int(1, 1), // top-right
        new Vector2Int(1, 0), // right
        new Vector2Int(1, -1), // bottom-right
        new Vector2Int(0, -1), // bottom-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(0, 1), // top-left 
    };

    public static List<string> Rectangle8DirOrientationsList = new List<string>() {
        "North",
        "NorthEast",
        "East",
        "SouthEast",
        "South",
        "SouthWest",
        "West",
        "NorthWest"
    };

    public static List<string> Rectangle4DirOrientationsList = new List<string>() {
        Rectangle8DirOrientationsList[0],
        Rectangle8DirOrientationsList[2],
        Rectangle8DirOrientationsList[4],
        Rectangle8DirOrientationsList[6],
    };

    public static List<string> HexagonDirOrientationsList = new List<string>() {
        Rectangle8DirOrientationsList[1],
        Rectangle8DirOrientationsList[2],
        Rectangle8DirOrientationsList[3],
        Rectangle8DirOrientationsList[5],
        Rectangle8DirOrientationsList[6],
        Rectangle8DirOrientationsList[7]
    };

    public Vector2Int GetRelativeNeighborPositionFromOrientation(string orientation, bool oddRow = false)
    {
        switch (m_NeighboringType)
        {
            case NeighboringTypes.Rect4Directions:
            case NeighboringTypes.Custom:
            default:
                if (!Rectangle4DirOrientationsList.Contains(orientation))
                {
                    var index = Rectangle8DirOrientationsList.IndexOf(orientation);
                    index++;
                    if (index >= Rectangle8DirOrientationsList.Count)
                        index = 0;
                    orientation = Rectangle8DirOrientationsList[index];
                }
                return GetNeighborPositions()[Rectangle4DirOrientationsList.IndexOf(orientation)];
            case NeighboringTypes.Rect8Directions:
                return GetNeighborPositions()[Rectangle8DirOrientationsList.IndexOf(orientation)];
            case NeighboringTypes.HexagonDefault:
                if (!HexagonDirOrientationsList.Contains(orientation))
                {
                    var index = Rectangle8DirOrientationsList.IndexOf(orientation);
                    index++;
                    orientation = Rectangle8DirOrientationsList[index];
                }
                return GetNeighborPositions(oddRow)[HexagonDirOrientationsList.IndexOf(orientation)];
        }
    }

    public Quaternion OrientationToRotation(Vector2Int initialPosition, Orientations orientation)
    {
        var targetPosition = initialPosition + GetRelativeNeighborPositionFromOrientation(orientation.ToString(), (initialPosition.y & 1) == 1);
        return PositionToRotation(initialPosition, targetPosition);
    }

    public Quaternion PositionToRotation(Vector2Int initialPosition, Vector2Int targetPosition)
    {
        var rotation = Quaternion.identity;
        var initialWorldPosition = GridManager.Instance.m_Grid.GetCellCenterWorld(initialPosition.ToVector3IntXY0());
        var targetWorldPosition = GridManager.Instance.m_Grid.GetCellCenterWorld(targetPosition.ToVector3IntXY0());
        var relativeVector = targetWorldPosition - initialWorldPosition;

        if (GridManager.Instance.m_Grid.cellSwizzle == GridLayout.CellSwizzle.XZY)
        {
            rotation = Quaternion.LookRotation(relativeVector);
        }
        else if (GridManager.Instance.m_Grid.cellSwizzle == GridLayout.CellSwizzle.XYZ)
        {
            var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * relativeVector;
            rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
        }

        return rotation;
    }



    // Returns the neighbor of the tile at the target position, if there is one
    public virtual GridTile NeighborAtPosition(GridTile gridTile, Vector2Int gridPosition)
    {
        var neighbors = WalkableNeighbors(gridTile);

        foreach (GridTile tile in neighbors)
        {
            if (tile.m_GridPosition == gridPosition)
            {
                return tile;
            }
        }

        return null;
    }

    // Returns a list with the neighbors of the tile
    public virtual List<GridTile> WalkableNeighbors(GridTile gridTile, bool ignoresHeight = false, bool unoccupiedTilesOnly = true, GridTile goalTile = null, List<Vector2Int> customDirections = null)
    {
        List<GridTile> results = new List<GridTile>();
        var directions = customDirections != null ? customDirections : GetNeighborPositions((gridTile.m_GridPosition.y & 1) == 1);

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newVector = dir + gridTile.m_GridPosition;
            if (ExistsTileAtPosition(newVector))
            {
                GridTile targetTile = GetGridTileAtPosition(newVector);
                if (targetTile != null)
                {
                    if (targetTile.m_IsTileWalkable || (goalTile != null && targetTile == goalTile))
                    {
                        if (unoccupiedTilesOnly && targetTile.IsTileOccupied() && (goalTile == null || (goalTile != null && targetTile != goalTile)))
                            continue;

                        if (m_UsesHeight && !ignoresHeight)
                        {
                            if (Mathf.Abs(Mathf.Abs(gridTile.m_TileHeight) - Mathf.Abs(targetTile.m_TileHeight)) <= 1)
                            {
                                results.Add(targetTile);
                            }
                        }
                        else
                        {
                            results.Add(targetTile);
                        }
                    }
                }
            }
        }

        // Add manual neighbors to the result
        foreach (GridTile tile in gridTile.m_manualNeighbors)
        {
            if (!results.Contains(tile))
            {
                results.Add(tile);
            }
        }

        results.Distinct();
        return results;
    }

    public virtual List<GridTile> Neighbors(GridTile gridTile, bool ignoresHeight = false, List<Vector2Int> customDirections = null)
    {
        List<GridTile> results = new List<GridTile>();
        var directions = customDirections != null ? customDirections : GetNeighborPositions((gridTile.m_GridPosition.y & 1) == 1); ;

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newVector = dir + gridTile.m_GridPosition;
            if (ExistsTileAtPosition(newVector))
            {
                GridTile targetTile = GetGridTileAtPosition(newVector);
                if (targetTile != null)
                {
                    results.Add(targetTile);
                }
            }
        }

        // Add manual neighbors to the result
        foreach (GridTile tile in gridTile.m_manualNeighbors)
        {
            results.Add(tile);
        }

        results.Distinct();
        return results;
    }

    public virtual GridTile InstantiateGridTile(GridTile gridTilePrefab, Vector2Int gridPosition, int heightInGrid = 0, Vector3? offsetPosition = null, Quaternion? rotation = null, Transform targetParent = null)
    {
        var cellWorldPosition = m_Grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0());
        var offset = offsetPosition.HasValue ? cellWorldPosition + offsetPosition.Value : cellWorldPosition;
        var rot = rotation.HasValue ? rotation.Value : Quaternion.identity;
        return InstantiateGridTile(gridTilePrefab, gridPosition, heightInGrid, rotation, offset, targetParent);
    }

    // Instantiates tiles to the grid
    public virtual GridTile InstantiateGridTile(GridTile gridTilePrefab, Vector2Int gridPosition, int heightInGrid = 0, Quaternion? rotation = null, Vector3? worldPosition = null, Transform targetParent = null)
    {
        if (gridTilePrefab == null)
            return null;

        // Check if there is another tile at the target position
        if (ExistsTileAtPosition(gridPosition))
        {
            //Debug.Log("Couldn't instantiate the tile at the target position, there is another tile there.");
            return null;
        }

        GridTile instantiatedGridTile = null;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            instantiatedGridTile = (GridTile)PrefabUtility.InstantiatePrefab(gridTilePrefab, gameObject.scene);
            if (instantiatedGridTile != null)
            {
                Undo.RegisterCreatedObjectUndo((Object)instantiatedGridTile.gameObject, "Paint Gridobject Prefab");
            }
        }
        else
        {
#endif
            instantiatedGridTile = Instantiate(gridTilePrefab) as GridTile;
#if UNITY_EDITOR
        }
#endif

        if (instantiatedGridTile == null)
            return null;

        // Transform values
        var targetWorldPosition = worldPosition.HasValue ? worldPosition.Value : m_Grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0());
        var quatRotation = rotation.HasValue ? rotation.Value : Quaternion.identity;
        var parent = targetParent != null ? targetParent : GridTilesHolder;
        instantiatedGridTile.transform.parent = parent;
        instantiatedGridTile.transform.position = targetWorldPosition;
        instantiatedGridTile.transform.rotation = quatRotation;
        // Set the tile's settings
        instantiatedGridTile.m_GridPosition = gridPosition;
        instantiatedGridTile.m_TileHeight = heightInGrid;

        return instantiatedGridTile;
    }

    public virtual void EraseGridTileAtPosition(Vector2Int gridPosition)
    {
        var tileAtPosition = GetGridTileAtPosition(gridPosition);
        if (tileAtPosition != null)
        {
            EraseGridTile(tileAtPosition);
        }
    }

    public virtual void EraseGridTile(GridTile gridTile)
    {
        if (gridTile != null)
            DestroyImmediate(gridTile.gameObject);
    }


    public virtual void MoveTileToPosition(GridTile gridTile, Vector2Int gridPosition, Vector3 offset)
    {
        if (gridTile == null)
            return;

        // Check if there is another tile at the target position
        if (ExistsTileAtPosition(gridPosition))
        {
            //Debug.Log("Couldn't move the tile to the target position, there is another tile there.");
            return;
        }
        RemoveGridTile(gridTile);
        AddGridTile(gridPosition, gridTile);
        var targetWorldPosition = m_Grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0()) + offset;
        gridTile.transform.position = targetWorldPosition;
        gridTile.m_GridPosition = gridPosition;
    }

    // Instantiates objects to the grid
    public virtual GridObject InstantiateGridObject(GridObject gridObjectPrefab, Vector2Int gridPosition, Orientations? initialOrientation = null, Transform targetParent = null, bool? checkTileAtPosition = true)
    {
        if (gridObjectPrefab == null)
            return null;

        // Check if there is another object at the target position
        var gridObjectAtPosition = GetGridObjectAtPosition(gridPosition);
        if (gridObjectAtPosition != null)
        {
            Debug.Log("Couldn't instantiate the GridObject at the target position, there is another gridobject there.");
            return null;
        }

        // Check if there is another tile at the target position
        if (checkTileAtPosition.HasValue && checkTileAtPosition.Value && !ExistsTileAtPosition(gridPosition))
        {
            Debug.Log("Couldn't instantiate the GridObject at the target position, because there is no tile there.");
            return null;
        }

        // Check if the tile is walkable
        var tileAtPosition = GetGridTileAtPosition(gridPosition);
        if (checkTileAtPosition.HasValue && checkTileAtPosition.Value && !tileAtPosition.m_IsTileWalkable)
            return null;

        GridObject instantiatedGridObject = null;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            instantiatedGridObject = (GridObject)PrefabUtility.InstantiatePrefab(gridObjectPrefab, gameObject.scene);
            if (instantiatedGridObject != null)
            {
                Undo.RegisterCreatedObjectUndo((Object)instantiatedGridObject.gameObject, "Paint GridObject Prefab");
            }
        }
        else
        {
#endif
            instantiatedGridObject = Instantiate(gridObjectPrefab) as GridObject;
#if UNITY_EDITOR
        }
#endif

        // Transform Values
        var cellPosition = tileAtPosition != null ? tileAtPosition.m_GridObjectsPivotPosition : m_Grid.GetCellCenterWorld(gridPosition.ToVector3IntXY0());
        var targetWorldPosition = cellPosition;
        var orientation = initialOrientation.HasValue ? initialOrientation.Value : instantiatedGridObject.m_InitialOrientation;
        var parent = targetParent != null ? targetParent : GridObjectsHolder;
        instantiatedGridObject.transform.parent = parent;
        instantiatedGridObject.transform.position = targetWorldPosition;
        // Set the GridObjects settings
        instantiatedGridObject.m_GridPosition = gridPosition;
        instantiatedGridObject.m_CurrentGridTile = tileAtPosition;
        instantiatedGridObject.m_InitialOrientation = orientation;

        return instantiatedGridObject;
    }

    public virtual void EraseGridObjectAtPosition(Vector2Int gridPosition)
    {
        var objectAtPosition = GetGridObjectAtPosition(gridPosition);
        if (objectAtPosition != null)
        {
            EraseGridObject(objectAtPosition);
        }
    }

    public virtual void EraseGridObject(GridObject gridObject)
    {
        if (gridObject != null)
            DestroyImmediate(gridObject.gameObject);
    }

    protected virtual void GetHolders()
    {
        if (_gridObjectsHolder == null)
        {
            _gridObjectsHolder = transform.Find("GridObjects");
            if (_gridObjectsHolder == null)
            {
                _gridObjectsHolder = new GameObject("GridObjects").transform;
                _gridObjectsHolder.SetParent(transform);
                _gridObjectsHolder.localPosition = Vector3.zero;
            }
        }

        if (_gridTilesHolder == null)
        {
            _gridTilesHolder = transform.Find("GridTiles");
            if (_gridTilesHolder == null)
            {
                _gridTilesHolder = new GameObject("GridTiles").transform;
                _gridTilesHolder.SetParent(transform);
                _gridTilesHolder.localPosition = Vector3.zero;
            }
        }
    }

    private GridTile GetGridTileAtPositionInEditor(Vector2Int gridPosition)
    {
        int childCount = GridTilesHolder.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = GridTilesHolder.GetChild(i);
            var gridTileComp = child.GetComponent<GridTile>();
            if (gridTileComp)
            {
                if (gridPosition == gridTileComp.m_GridPosition)
                {
                    return gridTileComp;
                }
            }
        }

        return null;
    }

    private GridObject GetGridObjectAtPositionInEditor(Vector2Int gridPosition)
    {
        int childCount = GridObjectsHolder.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = GridObjectsHolder.GetChild(i);
            var gridObjectComp = child.GetComponent<GridObject>();
            if (gridObjectComp)
            {
                if (gridPosition == gridObjectComp.m_GridPosition)
                {
                    return gridObjectComp;
                }
            }
        }

        return null;
    }
}