using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridMovement))]
[RequireComponent(typeof(GridObject))]
public class KeyboardDrivenController : MonoBehaviour {

    protected Vector2Int _currentInput = Vector2Int.zero;
    protected Vector2Int _queuedInput = Vector2Int.zero;
    [SerializeField]
    protected GridObject _gridObject;
    [SerializeField]
    protected GridMovement _gridMovement;

    [Header("Movement Settings")]
    public bool m_AnimateMovement = false;
    public bool m_RotateTowardsDirection = false;

    [Header ("Input Settings")]
    public bool m_SwapAxis = false;
    public bool m_InvertXAxis = false;
    public bool m_InvertYAxis = false;

    protected virtual void Reset() {
        Setup();
    }

    protected virtual void Setup() {
        _gridObject = GetComponent<GridObject>();
        _gridMovement = GetComponent<GridMovement>();
    }

    protected virtual void OnEnable() {
        _gridMovement.OnMovementEnd += MovementEnded;
    }

    protected virtual void OnDisable() {
        _gridMovement.OnMovementEnd -= MovementEnded;
    }

    protected virtual void Update() {
        _currentInput = GetKeyboardInput();
        ExecuteInput();
    }

    // Gets the current keyboard input
    public virtual Vector2Int GetKeyboardInput() {
        var invertX = m_InvertXAxis ? -1 : 1;
        var invertY = m_InvertYAxis ? -1 : 1;
        Vector2 input = m_SwapAxis ? new Vector2(Input.GetAxisRaw("Vertical") * invertX, Input.GetAxisRaw("Horizontal") * invertY) : new Vector2(Input.GetAxisRaw("Horizontal") * invertX, Input.GetAxisRaw("Vertical") * invertY);
        bool flag = Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("Left") || Input.GetButtonDown("Right");

        var direction = Vector2Int.zero;
        if (flag) {
            direction = input.ToVector2Int();
        }
        return direction;
    }

    protected virtual void ExecuteInput(Vector2Int? direction = null) {
        Vector2Int actionDirection;
        if (!direction.HasValue) {
            actionDirection = _currentInput;
        } else {
            actionDirection = direction.Value;
        }

        // If the direction is null/zero return
        if (actionDirection == Vector2Int.zero)
            return;

        // Try to move to the target position
        var targetPosition = _gridObject.m_GridPosition + actionDirection;
        MovementResult movementResult = _gridMovement.TryMoveToNeighborInPosition(targetPosition, m_AnimateMovement, m_RotateTowardsDirection);

        // Queue the desired input if the movement is currently in cooldown
        if (movementResult == MovementResult.Cooldown) {
            _queuedInput = _currentInput;
            return;
        }

        // If movement was succesful or failed for some other reason we remove the current input
        _currentInput = Vector2Int.zero;
    }


    protected virtual void ExecuteQueuedInput() {
        // If there is not queued input direction
        if (_queuedInput == Vector2Int.zero) {
            return;
        }

        Vector2Int specificAction = _queuedInput;
        Clear_queuedInput();
        ExecuteInput(specificAction);
    }

    // Clear the queue
    protected virtual void Clear_queuedInput() {
        _queuedInput = Vector2Int.zero;
    }

    // Callback for the movement ended on GridMovement, used to execute queued input
    protected virtual void MovementEnded(GridMovement movement, GridTile fromGridPos, GridTile toGridPos) {
        ExecuteQueuedInput();
    }

    // This method will be used in future updates for mobile devices tapping implementation 
    protected virtual Vector2Int GetDefaultMoveDirection() {
        return Vector2Int.up;
    }
}
