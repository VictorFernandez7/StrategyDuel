using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : KeyboardDrivenController
{

    [Header("Attack")]
    public Attack m_Attack;

    protected override void Setup()
    {
        base.Setup();
        m_Attack = GetComponentInChildren<Attack>();
    }

    protected override void ExecuteInput(Vector2Int? direction = null)
    {
        Vector2Int actionDirection;
        if (!direction.HasValue)
        {
            actionDirection = _currentInput;
        }
        else
        {
            actionDirection = direction.Value;
        }

        // If the direction is null/zero return
        if (actionDirection == Vector2Int.zero)
            return;


        // Try to attack the target position
        var flagAttacked = false;
        var targetPosition = _gridObject.m_GridPosition + actionDirection;
        var gridObjectAtPosition = GridManager.Instance.GetGridObjectAtPosition(targetPosition);
        if (gridObjectAtPosition != null && m_Attack != null)
        {
            var healthComponents = m_Attack.GetVictimsFromTriggerAtPosition(targetPosition);
            if (healthComponents != null && healthComponents.Count > 0)
            {
                var attkResult = m_Attack.TryAttack(targetPosition);
                if (attkResult == AttackResult.Success)
                {
                    flagAttacked = true;
                    return;
                }
            }
        }

        if (!flagAttacked)
        {
            MovementResult movementResult = _gridMovement.TryMoveToNeighborInPosition(targetPosition, m_AnimateMovement, m_RotateTowardsDirection);
            // Queue the desired input if the movement is currently in cooldown
            if (movementResult == MovementResult.Cooldown)
            {
                _queuedInput = _currentInput;
                return;
            }

            // Try to rotate towards the target
            if (movementResult == MovementResult.Failed)
            {
                var rotated = _gridMovement.TryRotateTo(actionDirection);
                // Queue the desired input if the movement is currently in cooldown
                if (!rotated)
                {
                    _queuedInput = _currentInput;
                    return;
                }
            }
        }

        // If movement was succesful or failed for some other reason we remove the current input
        _currentInput = Vector2Int.zero;
    }
}
