using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractOnGridObject : InteractiveObject {   
    [Header ("Events")]
    public UnityEvent m_OnEnterTile, m_OnExitTile;

    protected override void OnEnterTileMethod(GridObject gridObject, GridTile gridTile) {
        m_OnEnterTile.Invoke();
    }

    protected override void OnExitTileMethod(GridObject gridObject, GridTile gridTile) {
        m_OnExitTile.Invoke();
    }
}
