using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMoveSelector : MonoBehaviour
{

    public GameObject m_MoveLocationPrefab;
    public GameObject m_AttackLocationPrefab;

    protected GridObject _movingPiece;
    protected ChessUnitSelector _unitSelector;
    protected List<Vector2Int> _moveLocations;
    protected List<GameObject> _locationHighlights;

    // Start is called before the first frame update
    protected void Start()
    {
        _unitSelector = GetComponent<ChessUnitSelector>();
        enabled = false;
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GridManager.Instance.m_HoveredGridTile != null)
            {
                if (ChessGameManager.Instance.FriendlyPieceAt(GridManager.Instance.m_HoveredGridTile.m_GridPosition))
                {
                    SwapPiece(ChessGameManager.Instance.TrySelectPieceAtTile(GridManager.Instance.m_HoveredGridTile));
                    return;
                }

                // Return is the target position is invalid
                if (!_moveLocations.Contains(GridManager.Instance.m_HoveredGridTile.m_GridPosition))
                {
                    DeselectMovementPiece();
                    return;
                }
                // Normal movement, just check if there is no GridObject at the target position
                if (GridManager.Instance.GetGridObjectAtPosition(GridManager.Instance.m_HoveredGridTile.m_GridPosition) == null)
                {
                    _movingPiece.GetComponent<GridMovement>().TryMoveTo(GridManager.Instance.m_HoveredGridTile, false, false, false);
                }
                else
                {
                    // Capture the piece
                    ChessGameManager.Instance.CapturePieceAt(GridManager.Instance.m_HoveredGridTile.m_GridPosition);

                    _movingPiece.GetComponent<GridMovement>().TryMoveTo(GridManager.Instance.m_HoveredGridTile, false, false, false);
                }

                ExitState();
            }
            else
            {
                DeselectMovementPiece();
            }
        }
    }

    public void EnterState(GridObject piece)
    {
        _movingPiece = piece;
        enabled = true;

        _moveLocations = ChessGameManager.Instance.MovesForPiece(_movingPiece);
        _locationHighlights = new List<GameObject>();

        foreach (Vector2Int position in _moveLocations)
        {
            //Highlight move and attack/capture positions
            GameObject highlight;
            var targetWorldPosition = GridManager.Instance.GetGridTileAtPosition(position).m_WorldPosition;
            if (GridManager.Instance.GetGridObjectAtPosition(position))
            {
                highlight = Instantiate(m_AttackLocationPrefab, targetWorldPosition, Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(m_MoveLocationPrefab, targetWorldPosition, Quaternion.identity, gameObject.transform);
            }
            _locationHighlights.Add(highlight);
        }
    }

    protected void ExitState()
    {
        DeselectMovementPiece();
        ChessGameManager.Instance.NextPlayer();
    }

    public void SwapPiece(GridObject piece)
    {
        CLearHighlights();
        EnterState(piece);
    }

    public void DeselectMovementPiece()
    {
        enabled = false;
        ChessGameManager.Instance.DeselectPiece();
        _movingPiece = null;
        _unitSelector.EnterState();
        CLearHighlights();
    }

    protected void CLearHighlights()
    {
        foreach (GameObject highlight in _locationHighlights)
        {
            Destroy(highlight);
        }
    }
}
