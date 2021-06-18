using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TacticsPlayerControllerState { SelectingUnit, SelectedUnit, WaitingForUnitCallback, GameOver }

public class TacticsPlayerController : MonoBehaviour
{
    [NaughtyAttributes.ReadOnly, Header("Controller State")]
    public TacticsPlayerControllerState m_CurrentState = TacticsPlayerControllerState.SelectingUnit;

    private void OnEnable()
    {
        TacticsGameManager.onNewTurnEvent += OnNewTurn;
        TacticsGameManager.onUnitSelectedEvent += OnSelectedUnit;
        TacticsGameManager.onUnitDeSelectedEvent += OnDeSelectedUnit;
        TacticsGameManager.onGameOverEvent += OnGameOver;
    }
    private void OnDisable()
    {
        TacticsGameManager.onNewTurnEvent -= OnNewTurn;
        TacticsGameManager.onUnitSelectedEvent -= OnSelectedUnit;
        TacticsGameManager.onUnitDeSelectedEvent -= OnDeSelectedUnit;
        TacticsGameManager.onGameOverEvent -= OnGameOver;
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // store the hovered tile for usage in all states
            var hoveredTile = GridManager.Instance.m_HoveredGridTile;
            // Selecting Unit State
            if (m_CurrentState == TacticsPlayerControllerState.SelectingUnit && hoveredTile != null)
            {
                var selectedUnitAtTile = TacticsGameManager.Instance.TrySelectUnitAtTile(hoveredTile);
                if (selectedUnitAtTile != null)
                {
                    EnterSelectedUnitState(selectedUnitAtTile);
                }
            }
            else if (m_CurrentState == TacticsPlayerControllerState.SelectedUnit && hoveredTile != null) // Unit selected state
            {
                var selectedUnit = TacticsGameManager.Instance.m_SelectedUnit;
                if (selectedUnit.CanAttackTile(hoveredTile))
                {
                    // Switch states
                    EnterWaitingForUnitCallbackState();
                    // Attack the target tile
                    selectedUnit.AttackTile(hoveredTile, () => { EnterSelectedUnitState(selectedUnit); });
                }
                else if (selectedUnit.CanMoveToTile(hoveredTile))
                {
                    // Switch states
                    EnterWaitingForUnitCallbackState();
                    // Move to the target tile
                    selectedUnit.MoveToTile(hoveredTile, () => { EnterSelectedUnitState(selectedUnit); });
                }
            }
        }
    }

    public void SetControllerState(TacticsPlayerControllerState state)
    {
        if (m_CurrentState == TacticsPlayerControllerState.GameOver)
            return;

        var previousState = m_CurrentState;
        m_CurrentState = state;

        /*
        if (state == TacticsPlayerControllerState.SelectedUnit)
        {
            // Unit selected event
        }
        else if (state == TacticsPlayerControllerState.SelectingUnit)
        {
            // waiting for callback event
        }
        else if (state == TacticsPlayerControllerState.WaitingForUnitCallback)
        {
            // waiting for callback event
        }
        else if (state == TacticsPlayerControllerState.GameOver)
        {
            // waiting for callback event
        }
        */
    }

    public void EnterSelectingUnitState()
    {
        SetControllerState(TacticsPlayerControllerState.SelectingUnit);
    }

    public void EnterSelectedUnitState(TacticsCharacter unit)
    {
        // Unit selected event
        TacticsGameManager.OnUnitSelected(unit);
    }

    public void EnterWaitingForUnitCallbackState()
    {
        // Unit deselected event
        TacticsGameManager.OnUnitDeSelected();
        // Wait for callback state
        SetControllerState(TacticsPlayerControllerState.WaitingForUnitCallback);
    }

    private void OnNewTurn(TacticsPlayer player)
    {
        var nextUnit = TacticsGameManager.Instance.SelectNextUnit();
    }

    private void OnDeSelectedUnit()
    {
        HighlightManager.Instance.UnHighlightTiles();
    }

    private void OnSelectedUnit(TacticsCharacter unit)
    {
        // Selectedunit state
        SetControllerState(TacticsPlayerControllerState.SelectedUnit);
        if (unit.CanMove())
        {
            unit.CalculateMovementRange(true);
            if (unit.CanAttack())
            {
                unit.CalculateAttackRangeWithVictimsOnly(true);
            }
        }
        else if (unit.CanAttack())
        {
            unit.CalculateAttackRange(true);
        }
        else
        {
            TacticsGameManager.Instance.SelectNextUnit();
        }
    }

    private void OnGameOver(TacticsPlayer winner)
    {
        SetControllerState(TacticsPlayerControllerState.GameOver);
    }
}