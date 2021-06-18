using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GridObject))]
public class PathVisualizer3D : MonoBehaviour {   

    public bool m_Activated = false;

    public GameObject m_NodePrefab;
    public Vector3 m_NodeWorldPositionOffSet;

    protected GridObject _gridObject;
    protected List<GridTile> _currentPath;
    protected Transform _visualPathHolder;
    protected string _holderName = "Visual Path Holder";

    private void Awake() {
        _gridObject = GetComponent<GridObject>();
    }

    private void Update() {

        // Enable or Disable on keypress
        if (Input.GetKeyDown(KeyCode.C)) {
            m_Activated = !m_Activated;

            if (!m_Activated) {
                ClearVisualPath();
            }
        }

        // Update only every 6 frames
        if (Time.frameCount % 6 != 0) 
            return;
        // If it isn't activated just return
        if (!m_Activated) {
            return;
        }

        var targetTile = GridManager.Instance.m_HoveredGridTile;
        var path = targetTile == null ? null : AStar.Search(_gridObject.m_CurrentGridTile, targetTile);

        if (!Helpers.CompareLists(_currentPath, path)) {
            _currentPath = path;
            ClearVisualPath();
            
            if (_currentPath != null && _currentPath.Count > 0 && _currentPath.Contains(targetTile)) {
                PopulateVisualPath();
            }
        }
    }

    private void PopulateVisualPath() {
        if (_visualPathHolder == null) {
            _visualPathHolder = new GameObject(_holderName).transform;
        }

        var lastDirection = _currentPath[0].m_GridPosition - _gridObject.m_GridPosition;
        for (int i = 0; i < _currentPath.Count; i++) {
            var newNode = Instantiate(m_NodePrefab, _currentPath[i].m_WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
            newNode.parent = _visualPathHolder;

            if (i + 1 < _currentPath.Count) {
                lastDirection = _currentPath[i+1].m_GridPosition - _currentPath[i].m_GridPosition;
            }
            // Set the rotation of the node so it faces the next position on the path
            newNode.rotation = Quaternion.LookRotation(lastDirection.ToVector3X0Z());

        }

        foreach(GridTile tile in _currentPath) {
            //var newNode = Instantiate(m_NodePrefab, tile.m_WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
            //newNode.parent = _visualPathHolder;
        }
    }

    private void ClearVisualPath() {
        if (_visualPathHolder != null) {
            DestroyImmediate(_visualPathHolder.gameObject);
        }
    }

}
