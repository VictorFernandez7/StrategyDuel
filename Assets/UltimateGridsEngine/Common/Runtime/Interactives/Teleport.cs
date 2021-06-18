using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : InteractiveObject
{
    [Header("Teleport Settings")]
    public bool m_Activable = true;
    public Teleport m_TargetTeleport;
    [Header("Direction to face when teleporting from this")]
    public Vector2Int m_DirectionToFaceOnTeleport;

    protected List<GridObject> _ignoreList = new List<GridObject>();

    protected override void OnEnterTileMethod(GridObject gridObject, GridTile gridTile)
    {
        if (!m_Activable)
            return;

        // Check if it is on the ignore list
        if (_ignoreList.Contains(gridObject))
        {
            return;
        }

        // Check if the target gridObject is teleportable
        if (IsTeleportable(gridObject))
        {
            Activate(gridObject);
        }
    }

    protected override void OnExitTileMethod(GridObject gridObject, GridTile gridTile)
    {
        RemoveFromIgnoreList(gridObject);
    }

    protected virtual void Activate(GridObject gridObject)
    {
        AddToIgnoreList(gridObject);
        m_TargetTeleport.AddToIgnoreList(gridObject);
        var destinationTile = m_TargetTeleport.m_GridObject.m_CurrentGridTile;
        var gridMovement = gridObject.GetComponent<GridMovement>();

        StartCoroutine(ActivateRoutine(destinationTile, gridMovement));
    }

    protected virtual IEnumerator ActivateRoutine(GridTile destinationTile, GridMovement gridMovement)
    {
        yield return null;

        // Stop the current movement
        gridMovement.TryStop();

        // Wait for 1 frame till the movement actually stops
        //yield return null;

        gridMovement.TryMoveTo(destinationTile, false, false, false);
        gridMovement.RotateTo(m_TargetTeleport.m_GridObject.m_GridPosition + m_DirectionToFaceOnTeleport, false);
    }

    protected virtual bool IsTeleportable(GridObject gridObject)
    {
        // Check if the gridObject has a movement component attached to it
        var gridMovement = gridObject.GetComponent<GridMovement>();
        if (!gridMovement)
            return false;
        // Check if there is a target tile
        var destinationTile = m_TargetTeleport.m_GridObject.m_CurrentGridTile;
        if (!destinationTile)
            return false;
        // Check if it is occupied
        if (!destinationTile.CanMoveToTile())
            return false;

        return true;
    }

    // Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
    public virtual void AddToIgnoreList(GridObject gridObjectToIgnore)
    {
        _ignoreList.Add(gridObjectToIgnore);
    }

    // Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
    public virtual void RemoveFromIgnoreList(GridObject gridObjectToIgnore)
    {
        if (_ignoreList.Contains(gridObjectToIgnore))
            _ignoreList.Remove(gridObjectToIgnore);
    }
}
