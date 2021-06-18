using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackTrigger : MonoBehaviour
{

    [HideInInspector] public Attack m_Attack;

    [Header("Fixed Attack Direction")]
    public bool m_OverrideAttackDirection;
    public Vector2Int m_CustomAttackDirection = Vector2Int.up;
    public Vector2Int m_AttackDirection { get; private set; }

    protected GridObject _gridObject;

    protected virtual void Start()
    {
        if (m_Attack)
        {
            _gridObject = m_Attack.m_GridObject;
        }
    }

    public virtual bool TrySetAttackDirection(Vector2Int direction)
    {
        if (m_OverrideAttackDirection)
        {
            m_AttackDirection = m_CustomAttackDirection;
            return false;
        }

        m_AttackDirection = direction;
        return true;
    }

    public abstract List<Health> GetVictims();

    public abstract Health GetVictimAtPosition(Vector2Int position);

    public abstract List<Vector2Int> GetTargetPositions();

    public abstract List<GridTile> GetTargetTiles();

    public abstract bool HasVictimAtPosition(Vector2Int position);
}
