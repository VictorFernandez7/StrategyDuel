using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Cinemachine;

// This enum defines the different possible states a unit may have, in this case units are able to move and action only once per turn..
public enum TacticsCharacterState { Idle, Wait }

// Allows you to click anywhere on screen, which will determine a new target and the character will pathfind its way to it
[RequireComponent(typeof(CharacterPathWalker))]
[RequireComponent(typeof(GridObject))]
[RequireComponent(typeof(Health))]
public class TacticsCharacter : MonoBehaviour
{
    [Header("State"), NaughtyAttributes.ReadOnly]
    public TacticsCharacterState m_CurrentState = TacticsCharacterState.Idle;

    [Header("Movement")]
    //public bool m_AlreadyMoved = false;
    public int m_MovesPerTurn = 1;
    public RangeParameters m_MovementRangeParameters;
    [HideInInspector]
    public List<GridTile> m_currentMovementRange = new List<GridTile>();

    [Header("Attack")]
    public int m_AttacksPerTurn = 1;
    public TacticsCharacterAttack m_CharacterAttack;

    protected int _movesMadeThisTurn = 0;
    protected int _attacksMadeThisTurn = 0;
    protected CharacterPathWalker _pathWalker;
    [HideInInspector]
    public GridObject _gridObject;
    [HideInInspector]
    public Health _health;
    [HideInInspector]
    public GridMovement _gridMovement;

    protected void OnEnable()
    {
        _health.onDeathEvent += OnUnitDeath;
    }

    protected void OnDisable()
    {
        _health.onDeathEvent -= OnUnitDeath;
    }

    protected void OnUnitDeath(Health health, GridObject attacker)
    {
        TacticsGameManager.Instance.RemoveUnitFromGame(this);
    }

    protected virtual void Awake()
    {
        _gridObject = GetComponent<GridObject>();
        _pathWalker = GetComponent<CharacterPathWalker>();
        _health = GetComponent<Health>();
        _gridMovement = GetComponent<GridMovement>();
        m_CharacterAttack = GetComponent<TacticsCharacterAttack>();
    }

    public virtual void CalculateMovementRange(bool andHighlight = false)
    {
        m_currentMovementRange = RangeAlgorithms.SearchByParameters(_gridObject.m_CurrentGridTile, m_MovementRangeParameters);
        if (andHighlight)
        {
            HighlightMovementRange();
        }
    }

    public virtual void HighlightMovementRange()
    {
        if (m_currentMovementRange != null && m_currentMovementRange.Count > 0)
        {
            HighlightManager.Instance.HighlighTiles(m_currentMovementRange);
        }
    }

    // Wether or not this unit is able to move
    public bool CanMove()
    {
        return ((m_MovesPerTurn - _movesMadeThisTurn) > 0) && m_CurrentState != TacticsCharacterState.Wait;
    }

    // Wether or not this unit is able to move to a target tile (based on wether or not it is inside the range)
    public bool CanMoveToTile(GridTile targetTile)
    {
        return (CanMove() && m_currentMovementRange.Contains(targetTile) && targetTile != _gridObject.m_CurrentGridTile);
    }

    public bool CanAttack()
    {
        return ((m_AttacksPerTurn - _attacksMadeThisTurn) > 0) && m_CurrentState != TacticsCharacterState.Wait;
    }

    public bool CanAttackTile(GridTile targetTile)
    {
        return (CanAttack() && m_CharacterAttack.CanAttackTile(targetTile));
    }

    public virtual void CalculateAttackRange(bool andHighlight = false)
    {
        m_CharacterAttack.CalculateAttackRange(andHighlight);
    }

    public virtual void CalculateAttackRangeWithVictimsOnly(bool andHighlight = false, bool unhighlightPrevious = false)
    {
        m_CharacterAttack.CalculateAttackRangeWithVictimsOnly(andHighlight);
    }

    public virtual void HighlightAttackRange()
    {
        m_CharacterAttack.HighlightAttackRange();
    }

    public void MoveToTile(GridTile targetTile, Action onMovementEndCallback)
    {
        // Set state of the unit
        SetWaitState();
        // Movement availability
        _movesMadeThisTurn++;
        // Actually move using the pathwalker
        _pathWalker.DeterminePath(targetTile, true, () => { SetIdleState(); onMovementEndCallback(); });
        ClearRanges();
    }

    public void AttackTile(GridTile targetTile, Action onAttackEndCallback)
    {
        var objectAtPos = GridManager.Instance.GetGridObjectAtPosition(targetTile.m_GridPosition);
        if (objectAtPos == null)
            return;

        var tactisCharacterComp = objectAtPos.GetComponent<TacticsCharacter>();
        if (tactisCharacterComp != null)
        {
            // Set state of the unit
            SetWaitState();
            // Attack availability
            _attacksMadeThisTurn++;
            // Actually trigger the attack
            m_CharacterAttack.AttackUnit(tactisCharacterComp, () => { SetIdleState(); onAttackEndCallback(); });
            ClearRanges();
        }
    }

    public void ClearRanges()
    {
        ClearMovementRange();
        ClearAttackRange();
    }

    public void ClearMovementRange()
    {
        m_currentMovementRange.Clear();
    }

    public void ClearAttackRange()
    {
        m_CharacterAttack.ClearAttackRange();
    }

    public void SetWaitState()
    {
        if (m_CurrentState != TacticsCharacterState.Wait)
        {
            m_CurrentState = TacticsCharacterState.Wait;
        }
    }

    public void SetIdleState()
    {
        if (m_CurrentState != TacticsCharacterState.Idle)
        {
            m_CurrentState = TacticsCharacterState.Idle;
        }
    }

    public void NewTurn()
    {
        _movesMadeThisTurn = 0;
        _attacksMadeThisTurn = 0;
    }
}