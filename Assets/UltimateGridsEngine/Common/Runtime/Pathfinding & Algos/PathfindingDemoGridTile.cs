using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingDemoGridTile : GridTile
{  
    protected int _distance;
    protected Renderer _renderer;
    public int Distance {
		get {
			return _distance;
		}
		set {
			_distance = value;
			UpdateDistanceLabel();
		}
	}

    protected override void Start() {
        base.Start();
        DisableDistanceText();
        _renderer = GetComponent<Renderer>();
        if (_renderer == null) {
            _renderer = GetComponentInChildren<Renderer>();
        }
        if (!m_IsTileWalkable) {
            SetUnWalkable();
        }
    }

    protected virtual void Update() {
        // Tile to search from
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)) {
            // Check if this is the highlighted tile
            if (GridManager.Instance.m_HoveredGridTile != this)
            return;

            // Unset the tsf first
            if (PathfindingDemoVisualizer.instance.FromTile != null) {
                PathfindingDemoVisualizer.instance.FromTile.DisableFromTile();
            }

            EnableFromTile();
        } 
    }

    protected override void OnMouseEnter () {
		base.OnMouseEnter();
        EnableHighlight();
	}

	protected override void OnMouseExit () {
        base.OnMouseExit();
        DisableHighlight();
	}

    protected virtual void OnMouseDown() {

        if (Input.GetKey(KeyCode.LeftShift)) {
            // Unset the FromTile first
            if (PathfindingDemoVisualizer.instance.FromTile != null) {
                PathfindingDemoVisualizer.instance.FromTile.DisableFromTile();
            }

            EnableFromTile();
        } else if (m_IsTileWalkable) {
            SetUnWalkable();
        } else {
            SetWalkable();
        }
    }

    public virtual void DisableHighlight () {
        if (!m_IsTileWalkable)
            return;

        if (PathfindingDemoVisualizer.instance.FromTile == this) {
            return;
        }

        if (PathfindingDemoVisualizer.instance.ToTile == this)  {
            PathfindingDemoVisualizer.instance.ToTile = null;

            ChangeColor(PathfindingDemoVisualizer.instance.WalkableTileColor);
        }
	}
	
	public virtual void EnableHighlight () {
        // Check if the tile is walkable
        if (!m_IsTileWalkable)
            return;
            
        // Don't set the current target tile as highlighted if its the current search from tile
        if (PathfindingDemoVisualizer.instance.FromTile != null) {
            if ((PathfindingDemoVisualizer.instance.FromTile == GridManager.Instance.m_HoveredGridTile))
                return;
        }

        ChangeColor(PathfindingDemoVisualizer.instance.SearchToColor);

        PathfindingDemoVisualizer.instance.SetTargetTile(this); 
	}
    
    public virtual void EnableDistanceText () {
		Text text =  GetComponentInChildren<Text>();
		text.enabled = true;
	}
    public virtual void DisableDistanceText () {
		Text text = GetComponentInChildren<Text>();
		text.enabled = false;
	}
	
    public virtual void UpdateDistanceLabel () {
		Text label = GetComponentInChildren<Text>();
		label.text = Distance.ToString();
	}

    public virtual void EnableFromTile() {
        if (!m_IsTileWalkable)
            return;

        if (PathfindingDemoVisualizer.instance.ToTile == this) {
            DisableHighlight();
        }
        // Set the from tile
        PathfindingDemoVisualizer.instance.SetFromTile(this);
        // Distance text
        Distance = 0;
        UpdateDistanceLabel();
        EnableDistanceText();
        // Set the color
        ChangeColor(PathfindingDemoVisualizer.instance.SearchFromColor);

    }

    public virtual void DisableFromTile() {
        if (PathfindingDemoVisualizer.instance.FromTile == this) {
            PathfindingDemoVisualizer.instance.FromTile = null;
        }

        DisableDistanceText();
        ChangeColor(PathfindingDemoVisualizer.instance.WalkableTileColor);
    }

    public virtual void SetWalkable() {
        m_IsTileWalkable = true;
        
        DisableDistanceText();
        ChangeColor(PathfindingDemoVisualizer.instance.WalkableTileColor);
    }

    public virtual void SetUnWalkable() {
        m_IsTileWalkable = false;

        if (PathfindingDemoVisualizer.instance.FromTile == this) {
            PathfindingDemoVisualizer.instance.FromTile = null;
            PathfindingDemoVisualizer.instance.UnHighlightPath();
        }

        if (PathfindingDemoVisualizer.instance.ToTile == this) {
            PathfindingDemoVisualizer.instance.ToTile = null;
            PathfindingDemoVisualizer.instance.UnHighlightPath();
        }

        DisableDistanceText();
        ChangeColor(PathfindingDemoVisualizer.instance.NonWalkableTileColor);
    }

    public virtual void ChangeColor(Color color) {
        if (_renderer) {
            _renderer.material.color = color;
        }
    }
}
