using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class TacticsGameManager : MonoBehaviour
{
    [Header("Selected Unit"), NaughtyAttributes.ReadOnly]
    public TacticsCharacter m_SelectedUnit;

    [Header("Players")]
    public List<TacticsPlayer> m_Players = new List<TacticsPlayer>();

    [NaughtyAttributes.ReadOnly]
    public TacticsPlayer m_CurrentPlayerTurn = null;

    public delegate void UnitSelectedHandler(TacticsCharacter unit);
    public static event UnitSelectedHandler onUnitSelectedEvent;
    public static void OnUnitSelected(TacticsCharacter unit) { if (onUnitSelectedEvent != null) onUnitSelectedEvent(unit); }

    public delegate void UnitDeSelectedHandler();
    public static event UnitDeSelectedHandler onUnitDeSelectedEvent;
    public static void OnUnitDeSelected() { if (onUnitDeSelectedEvent != null) onUnitDeSelectedEvent(); }

    public delegate void GameStartHandler();
    public static event GameStartHandler onGameStartEvent;
    public static void OnGameStart() { if (onGameStartEvent != null) onGameStartEvent(); }

    public delegate void GameOverHandler(TacticsPlayer winner);
    public static event GameOverHandler onGameOverEvent;
    public static void OnGameOver(TacticsPlayer winner) { if (onGameOverEvent != null) onGameOverEvent(winner); }

    public delegate void NewTurnHandler(TacticsPlayer newPlayerTurn);
    public static event NewTurnHandler onNewTurnEvent;
    public static void OnNewTurn(TacticsPlayer newPlayerTurn) { if (onNewTurnEvent != null) onNewTurnEvent(newPlayerTurn); }

    // Singleton
    protected static TacticsGameManager _instance = null;
    public static TacticsGameManager Instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType<TacticsGameManager>(); }
            return _instance;
        }
        protected set { _instance = value; }
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
    }

    protected virtual void Start()
    {
        TriggerGameStart();
        TriggerNewTurn();
    }

    public void TriggerGameStart()
    {
        // Trigger the OnGameStart Event
        OnGameStart();
    }

    public void TriggerGameOver(TacticsPlayer winner)
    {
        // Trigger the OnGameOver event
        OnGameOver(winner);
    }

    public void TriggerNewTurn()
    {
        // Get the index
        var playerIndex = m_Players.IndexOf(m_CurrentPlayerTurn);
        playerIndex++;
        if (playerIndex >= m_Players.Count)
            playerIndex = 0;

        // Set the current player
        m_CurrentPlayerTurn = m_Players[playerIndex];

        // Reset the selected piece & units states
        DeselectUnit();
        ResetPlayersUnits();

        // Trigger the on new turn event
        OnNewTurn(m_CurrentPlayerTurn);
    }

    public void ResetPlayersUnits()
    {
        foreach (TacticsCharacter unit in m_CurrentPlayerTurn.m_Units)
        {
            unit.NewTurn();
        }
    }

    public TacticsCharacter TrySelectUnitAtTile(GridTile targetTile)
    {
        var unitAtTile = GridManager.Instance.GetGridObjectAtPosition(targetTile.m_GridPosition);
        if (unitAtTile != null)
        {
            var tacticsCharacterComponent = unitAtTile.GetComponent<TacticsCharacter>();
            if (tacticsCharacterComponent != null)
            {
                if (DoesUnitBelongToCurrentPlayer(tacticsCharacterComponent))
                {
                    m_SelectedUnit = tacticsCharacterComponent;
                    // Trigger the unit selected event
                    OnUnitSelected(m_SelectedUnit);
                    return m_SelectedUnit;
                }
            }
        }

        return null;
    }

    public void DeselectUnit()
    {
        // Clear the Unit's action ranges
        if (m_SelectedUnit != null)
        {
            m_SelectedUnit.ClearRanges();
            // Set the selected unit to null
            m_SelectedUnit = null;
            // Trigger the OnUnitDeSelect event
            OnUnitDeSelected();
        }
    }

    public TacticsCharacter SelectNextUnit()
    {
        // Get the index
        var unitIndex = m_CurrentPlayerTurn.m_Units.IndexOf(m_SelectedUnit);
        unitIndex++;
        // Looped through all the player's units
        if (unitIndex >= m_CurrentPlayerTurn.m_Units.Count)
        {
            TriggerNewTurn();
            return null;
        }

        m_SelectedUnit = m_CurrentPlayerTurn.m_Units[unitIndex];
        OnUnitSelected(m_SelectedUnit);
        return m_SelectedUnit;
    }

    public void SelectNextUnitFromButton()
    {
        SelectNextUnit();
    }

    public bool DoesUnitBelongToCurrentPlayer(TacticsCharacter unit)
    {
        return m_CurrentPlayerTurn.m_Units.Contains(unit);
    }

    public void TriggerSelectedUnitAttack()
    {
        if (m_SelectedUnit == null)
            return;

        if (m_SelectedUnit.CanAttack())
        {
            m_SelectedUnit.CalculateAttackRange(true);
        }
    }

    public void TriggerSelectedUnitMovement()
    {
        if (m_SelectedUnit == null)
            return;

        if (m_SelectedUnit.CanMove())
        {
            m_SelectedUnit.CalculateMovementRange(true);
        }
    }

    public void RemoveUnitFromGame(TacticsCharacter unit)
    {
        foreach (TacticsPlayer player in m_Players.ToList())
        {
            // Remove the unit from the player's units list
            if (player.m_Units.Contains(unit))
            {
                player.m_Units.Remove(unit);
            }
            // Check if the player has no more units
            if (!(player.m_Units.Count > 0))
            {
                // Remove the player from the players list
                m_Players.Remove(player);
                // If there is not more than 1 player do the GameOver
                if (!(m_Players.Count > 1))
                {
                    OnGameOver(m_Players[0]);
                }
            }
        }

    }

}
