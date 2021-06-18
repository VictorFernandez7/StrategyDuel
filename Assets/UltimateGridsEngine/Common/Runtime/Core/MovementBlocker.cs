using System;
using UnityEngine;

[RequireComponent(typeof(GridObject))]
[Serializable]
public class MovementBlocker : MonoBehaviour {
    [Tooltip("Stops entities from moving into this grid object's position.")]
    public bool BlocksMovement = true;

    public GridObject OwnerGridObject { get; protected set; }

    protected virtual void Awake() {
        OwnerGridObject = GetComponent<GridObject>();
    }

    public virtual bool TryBlockMovementFor(GridObject targetGridObject) {
        if (!BlocksMovement) {
            return false;
        }
        if (!targetGridObject) {
            return true;
        }
        return true;
    }
}
