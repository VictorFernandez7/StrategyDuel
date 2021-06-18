using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessGameManager : MonoBehaviour
{
    [Header("Selected Unit")]
    public GridObject m_SelectedPiece = null;   // Currently selected piece

    [Header("Players")]
    public ChessPlayer m_WhitePlayer = new ChessPlayer("white", true);
    public ChessPlayer m_BlackPlayer = new ChessPlayer("black", false);

    [HideInInspector]
    public ChessPlayer _currentPlayer;
    [HideInInspector]
    public ChessPlayer _otherPlayer;

    [Header("Game End UI")]
    public GameObject m_GameOverCanvas;
    public Text m_MessageText;

    [Header("Turn UI")]
    public Text m_TurnText;

    public static ChessGameManager Instance = null;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected void Start()
    {
        _currentPlayer = m_WhitePlayer;
        _otherPlayer = m_BlackPlayer;
    }

    public GridObject TrySelectPieceAtTile(GridTile targetTile)
    {
        var piece = GridManager.Instance.GetGridObjectAtPosition(targetTile.m_GridPosition);

        if (DoesPieceBelongToCurrentPlayer(piece))
        {
            m_SelectedPiece = piece;
            return piece;
        }

        return null;
    }

    public void DeselectPiece()
    {
        m_SelectedPiece = null;
    }

    public bool DoesPieceBelongToCurrentPlayer(GridObject piece)
    {
        return _currentPlayer.m_Pieces.Contains(piece);
    }

    public List<Vector2Int> MovesForPiece(GridObject piece)
    {
        var piececomp = piece as ChessPiece;
        var locations = piececomp.MoveLocations(piece.m_GridPosition);

        // filter out locations with friendly piece
        locations.RemoveAll(tile => FriendlyPieceAt(tile));

        return locations;
    }

    public bool FriendlyPieceAt(Vector2Int gridPosition)
    {
        GridObject piece = GridManager.Instance.GetGridObjectAtPosition(gridPosition);

        if (piece == null)
        {
            return false;
        }

        if (_otherPlayer.m_Pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }

    public void NextPlayer()
    {
        var tempPlayer = _currentPlayer;
        _currentPlayer = _otherPlayer;
        _otherPlayer = tempPlayer;

        // Update the turn UI
        m_TurnText.text = _currentPlayer.name;
    }

    public void CapturePieceAt(Vector2Int gridPosition)
    {
        GridObject pieceToCapture = GridManager.Instance.GetGridObjectAtPosition(gridPosition);

        // End the game if the captures piece is the king
        if ((pieceToCapture as ChessPiece).type == PieceType.King)
        {
            //Debug.Log(_currentPlayer.name + " wins!");

            m_GameOverCanvas.SetActive(true);
            var text = _currentPlayer.name + " wins!";
            m_MessageText.text = text.ToUpper();

            Destroy(GetComponent<ChessUnitSelector>());
            Destroy(GetComponent<ChessMoveSelector>());
        }

        var selectedPiece = m_SelectedPiece as ChessPiece;
        if (selectedPiece != null)
        {
            selectedPiece.m_Attack.TryAttack(gridPosition, false);
        }
    }
}
