using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using System;

[RequireComponent(typeof(GridMovement))]
public class CharacterPathWalker : MonoBehaviour
{

    [Header("Settings")]
    public bool m_FollowPath = false;
    public bool m_DeactivateOnReachDestination = true;

    [Header("Movement Settings")]
    public bool m_AnimateMovement = true;
    public bool m_RotateTowardsDirection = false;
    public bool m_ChecksMovementCooldown = true;

    /// The current path
    [Header("Path Info"), NaughtyAttributes.ReadOnly]
    public List<GridTile> m_Path = new List<GridTile>();
    /// The index of the next point target point in the path
    [NaughtyAttributes.ReadOnly]
    public int m_NextPathIndex = -1;

    protected GridObject _gridObject;
    protected GridMovement _gridMovement;
    protected Action _onReachedDestination;

    /// <summary>
    /// On Awake we grab our components
    /// </summary>
    protected virtual void Awake()
    {
        _gridMovement = GetComponent<GridMovement>();
        _gridObject = GetComponent<GridObject>();
    }

    /// <summary>
    /// Sets a new Path
    /// </summary>
    /// <param name="path"></param>
    public virtual void SetPath(List<GridTile> path, bool andMove = false, Action onReachedDestination = null)
    {
        m_Path = path;
        m_NextPathIndex = 0;
        if (andMove && !m_FollowPath)
            m_FollowPath = true;

        if (onReachedDestination != null)
            _onReachedDestination = onReachedDestination;
    }

    /// <summary>
    /// Sets a new Path to follow
    /// </summary>
    /// <param name="destinationGridTile"></param>
    public virtual void StartMoving()
    {
        if (!m_FollowPath)
            m_FollowPath = true;
    }

    /// <summary>
    /// Sets a new Path to follow
    /// </summary>
    /// <param name="destinationGridTile"></param>
    public virtual void StopMoving()
    {
        if (m_FollowPath)
            m_FollowPath = false;
    }

    public void SetOnReachedDestinationAction(Action onReachedDestination)
    {
        _onReachedDestination = onReachedDestination;
    }

    public void ResetOnReachedDestinationAction()
    {
        _onReachedDestination = null;
    }

    /// <summary>
    /// On Update, we check if we should move and determine next point in path based on the current grid position
    /// </summary>
    protected virtual void Update()
    {
        if (!m_FollowPath)
        {
            return;
        }

        if (_gridMovement.m_IsMoving)
        {
            return;
        }

        DetermineNextPathIndex();
        MoveGridObject();
    }

    /// <summary>
    /// Moves the controller towards the next point
    /// </summary>
    protected virtual void MoveGridObject()
    {
        if ((m_NextPathIndex < 0) || m_Path.Count <= 0)
        {
            return;
        }
        else
        {
            _gridMovement.TryMoveTo(m_Path[m_NextPathIndex], m_AnimateMovement, m_RotateTowardsDirection, m_ChecksMovementCooldown);
        }
    }

    /// <summary>
    /// Determines the next tile in the path based on our current gridtile
    /// </summary>
    protected virtual void DetermineNextPathIndex()
    {
        if (m_Path.Count <= 0)
        {
            return;
        }
        if (m_NextPathIndex < 0)
        {
            return;
        }

        if (m_Path[m_NextPathIndex] == _gridObject.m_CurrentGridTile)
        {
            if (m_NextPathIndex + 1 < m_Path.Count)
            {
                m_NextPathIndex++;
            }
            else
            {
                m_NextPathIndex = -1;
                if (m_DeactivateOnReachDestination)
                    StopMoving();

                if (_onReachedDestination != null)
                {
                    _onReachedDestination();
                }
            }
        }
    }

    /// <summary>
    /// Determines the path to the target GridTile. NextPathIndex will be -1 if a path couldn't be found
    /// </summary>
    /// <param name="targetGridTile"></param>
    /// <returns></returns>
    public virtual void DeterminePath(GridTile targetGridTile, bool andMove = true, Action onReachedDestination = null)
    {
        m_NextPathIndex = -1;

        var newPath = AStar.Search(_gridObject.m_CurrentGridTile, targetGridTile);
        if (newPath != null && newPath.Count > 0 && newPath.Contains(targetGridTile))
        {
            SetPath(newPath, andMove, onReachedDestination);
        }
    }

    /// <summary>
    /// Determines the path to the target GridTile. NextPathIndex will be -1 if a path couldn't be found
    /// </summary>
    /// <param name="targetGridPosition"></param>
    /// <returns></returns>
    public virtual void DeterminePath(Vector2Int targetGridPosition, bool andMove = true, Action onReachedDestination = null)
    {
        m_NextPathIndex = -1;

        var newPath = AStar.Search(_gridObject.m_CurrentGridTile, targetGridPosition);

        if (newPath != null && newPath.Count > 0)
        {
            var pathContainsTileAtPosition = newPath.Any(tile => tile.m_GridPosition == targetGridPosition);
            if (pathContainsTileAtPosition)
            {
                SetPath(newPath, andMove, onReachedDestination);
            }
        }
    }
}