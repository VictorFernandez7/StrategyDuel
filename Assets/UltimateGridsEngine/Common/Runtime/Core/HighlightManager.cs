using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{

    public List<GridTile> m_CurrentlyHighlightedTiles = new List<GridTile>();

    [Header("Visual Node Settings")]
    public GameObject[] m_NodePrefabs;
    public Vector3 m_NodeWorldPositionOffSet;

    protected Transform _highlightNodePrefabsHolder;
    protected string _holderName = "Highlight Holder";

    // Singleton
    protected static HighlightManager _instance = null;
    public static HighlightManager Instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType<HighlightManager>(); }
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

    public void HighlighTiles(List<GridTile> tilesToHighlight, int prefabIndex = 0, bool UnhighlightPreviousTiles = true)
    {
        if (UnhighlightPreviousTiles)
            UnHighlightTiles();

        if (tilesToHighlight != null && tilesToHighlight.Count > 0)
        {
            if (_highlightNodePrefabsHolder == null)
            {
                _highlightNodePrefabsHolder = new GameObject(_holderName).transform;
            }

            for (int i = 0; i < tilesToHighlight.Count; i++)
            {
                var newNode = Instantiate(m_NodePrefabs[prefabIndex], tilesToHighlight[i].m_WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
                newNode.parent = _highlightNodePrefabsHolder;
            }
        }


    }

    public void UnHighlightTiles()
    {
        if (_highlightNodePrefabsHolder != null)
        {
            m_CurrentlyHighlightedTiles.Clear();
            DestroyImmediate(_highlightNodePrefabsHolder.gameObject);
        }
    }
}
