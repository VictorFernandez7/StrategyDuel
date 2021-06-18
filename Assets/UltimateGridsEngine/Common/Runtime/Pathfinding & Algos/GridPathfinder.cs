using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridPathfinder : MonoBehaviour {
    
    [Header("Target GridObject")]
    /// the target the character should pathfind to
    public GridObject m_TargetGridObject;
    [Header("Target GridTile")]
    /// The goal Tile
    public GridTile m_DestinationTile;

    [Header("Movement Settings")]
    public bool m_AnimateMovement = false;
    public bool m_RotateTowardsDirection = false;

    [Header("Debug")]
    /// Whether or not we should draw debug spheres to show the current gridtiles path of the character (on the inspector)
    public bool m_DebugDrawPath;
    /// The index of the next point target point in the path
    public int m_NextPathpointIndex;

    /// The current path
    public List<GridTile> Path = new List<GridTile>();

    protected GridObject _gridObject;
    protected GridMovement _gridMovement;

    /// <summary>
    /// On Awake we grab our components
    /// </summary>
    protected virtual void Awake() {
        _gridMovement = GetComponent<GridMovement>();
        _gridObject = GetComponent<GridObject>();
    }

    /// <summary>
    /// Sets a new destination the character will pathfind to
    /// </summary>
    /// <param name="destinationGridTile"></param>
    public virtual void SetNewDestination(GridTile destinationGridTile) {
        m_DestinationTile = destinationGridTile;
        DeterminePath(_gridObject.m_CurrentGridTile, destinationGridTile);
    }

    /// <summary>
    /// On Update, we draw the path if needed, determine the next waypoint, and move to it if needed
    /// </summary>
    protected virtual void Update() {
        if (m_DestinationTile == null) {
            return;
        }
        DetermineNextPathpoint();
        MoveGridObject();
    }

    /// <summary>
    /// Moves the controller towards the next point
    /// </summary>
    protected virtual void MoveGridObject() {
        if ((m_DestinationTile == null) || (m_NextPathpointIndex < 0) || Path.Count <= 0) {
            return;
        } else {
            _gridMovement.TryMoveTo(Path[m_NextPathpointIndex], m_AnimateMovement, m_RotateTowardsDirection);
        }
    }

    /// <summary>
    /// Determines the path to the target GridTile. NextPathPointIndex will be -1 if a path couldn't be found
    /// </summary>
    /// <param name="startingGridTile"></param>
    /// <param name="targetGridTile"></param>
    /// <returns></returns>
    protected virtual void DeterminePath(GridTile startingGridTile, GridTile targetGridTile) {
        m_NextPathpointIndex = -1;

        Path = AStar.Search(startingGridTile, targetGridTile);
        if (Path != null && Path.Count > 0 && Path.Contains(targetGridTile)) {

            m_NextPathpointIndex = 0;
        }
    }

    /// <summary>
    /// Determines the next path point 
    /// </summary>
    protected virtual void DetermineNextPathpoint() {
        if (Path == null) {
            return;
        }

        if (Path.Count <= 0) {
            return;
        }
        if (m_NextPathpointIndex < 0) {
            return;
        }

        if (_gridObject.m_GridPosition.GridDistance(Path[m_NextPathpointIndex].m_GridPosition) <= 0) {
            if (m_NextPathpointIndex + 1 < Path.Count) {
                m_NextPathpointIndex++;
            } else {
                m_NextPathpointIndex = -1;
            }
        } else {
            // Try to recalculate the path since the current one is blocked
            DeterminePath(_gridObject.m_CurrentGridTile, m_DestinationTile);
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Draws wire spheres on top of each tile in the current path and a line to visualize the path
    /// </summary>
    protected virtual void OnDrawGizmosSelected() {
        if (m_DebugDrawPath) {
            if (Path != null && Path.Count > 0) {
                Handles.color = Color.grey;
                for (int i = 0; i < Path.Count; i++) {
                    var height = Vector3.up * .5f;

                    if (i == Path.Count - 1)
                        Handles.color = Color.green;

                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                    Handles.DrawWireCube(Path[i].m_WorldPosition + height, Vector3.one * 0.3f);
                    Handles.CubeHandleCap(0, Path[i].m_WorldPosition + height, Quaternion.identity, 0.3f, EventType.Repaint);

                    if (i > 0)
                        Debug.DrawLine(Path[i - 1].m_WorldPosition + height, Path[i].m_WorldPosition + height, Color.yellow, 0.0f, true);
                }
            }
        }
    }
#endif
}