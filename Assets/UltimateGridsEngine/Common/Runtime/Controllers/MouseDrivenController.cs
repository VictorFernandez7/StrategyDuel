using UnityEngine; 
using System.Collections; 

// Allows you to click anywhere on screen, which will determine a new target and the character will pathfind its way to it
[RequireComponent(typeof(CharacterPathfinder))]
public class MouseDrivenController:MonoBehaviour {

    protected CharacterPathfinder _characterPathfinder; 

    // On awake we get the GridPathfinder component
    protected virtual void Awake() {
        _characterPathfinder = GetComponent < CharacterPathfinder > (); 
    }

    // On Update we look for a mouse click
    protected virtual void Update() {
        DetectMouse(); 
    }

    // If the mouse is clicked, we make the currently hovered tile the pathfinding target
    protected virtual void DetectMouse() {
        if (Input.GetMouseButtonDown(0)) {
            if (GridManager.Instance.m_HoveredGridTile != null) {
               _characterPathfinder.SetNewDestination(GridManager.Instance.m_HoveredGridTile); 
            }
        }
    }
}
