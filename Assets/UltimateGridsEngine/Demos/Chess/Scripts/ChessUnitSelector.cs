using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessUnitSelector : MonoBehaviour {

    protected ChessMoveSelector _moveSelector;

    protected void Awake() {
        _moveSelector = GetComponent<ChessMoveSelector>();
    }

    // Update is called once per frame
    protected void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (GridManager.Instance.m_HoveredGridTile != null) {
                var resultPiece = ChessGameManager.Instance.TrySelectPieceAtTile(GridManager.Instance.m_HoveredGridTile);
                if (resultPiece != null) {
                    ExitState(resultPiece);
                }
            }
        }
    }
    
    public void EnterState() {
        enabled = true;
    }

    protected void ExitState(GridObject movingPiece) {
        enabled = false;
        _moveSelector.EnterState(movingPiece);
    }
}
