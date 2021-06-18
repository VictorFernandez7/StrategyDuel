using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridObject))]
public class InteractiveObject : MonoBehaviour {   
    [Header ("Settings")]
    public GridObject m_GridObject;
    [Header ("Target Layer")]
    public LayerMask m_Layers;
    [Header ("Activate After Movement Ended")]
    public bool m_WaitTillMovementEnds; // This will be added on update 1.1


    protected void Reset() {
        m_Layers = LayerMask.NameToLayer("Everything");
        m_GridObject = GetComponent<GridObject>();
    }

    protected void Start() {
        if (m_GridObject != null && m_GridObject.m_CurrentGridTile != null) {
            m_GridObject.m_CurrentGridTile.OnGridObjectEnter += OnGridObjectEnteredTileCallBack;
            m_GridObject.m_CurrentGridTile.OnGridObjectExit += OnGridObjectExitedTileCallBack;
        }
    }

    protected void OnDisable() {
        if (m_GridObject != null && m_GridObject.m_CurrentGridTile != null) {
            m_GridObject.m_CurrentGridTile.OnGridObjectEnter -= OnGridObjectEnteredTileCallBack;
            m_GridObject.m_CurrentGridTile.OnGridObjectExit -= OnGridObjectExitedTileCallBack;
        }
    }

    // Callback to invoke the OnEnterTile events when a gridobject with the target layer enters the same gridtile as this
    protected virtual void OnGridObjectEnteredTileCallBack(GridObject gridObject, GridTile gridTile) {
        if (0 != (m_Layers.value & 1 << gridObject.gameObject.layer)) {
            OnEnterTileMethod(gridObject, gridTile);
        }
    }

    // Callback to invoke the OnEnterTile events when a gridobject with the target layer exits the same gridtile as this
    protected virtual void OnGridObjectExitedTileCallBack(GridObject gridObject, GridTile gridTile) {
        if (0 != (m_Layers.value & 1 << gridObject.gameObject.layer)) {
            OnExitTileMethod(gridObject, gridTile);
        }
    }

    protected virtual void OnEnterTileMethod(GridObject gridObject, GridTile gridTile) {

    }

    protected virtual void OnExitTileMethod(GridObject gridObject, GridTile gridTile) {

    }



}
