using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New GridTile Collection", menuName = "UGE/Collections/Create GridTile Collection")]
public class GridTileCollection : ScriptableObject
{
    protected static string lastGridTileCollectionEditPrefsKey = "LastUsedGTC";

    [SerializeField, Header("Tiles In Collection")] private List<GridTileBrush> _gridTileBrushes = new List<GridTileBrush>();
    public List<GridTileBrush> m_GridTileBrushes
    {
        get
        {
            if (_gridTileBrushes == null)
            {
                _gridTileBrushes = new List<GridTileBrush>();
            }
            return _gridTileBrushes;
        }
        set
        {
            if (value == null)
            {
                value = new List<GridTileBrush>();
            }
            _gridTileBrushes = value;
        }
    }

    public GridTileBrush m_SelectedGridTileBrush
    {
        get { return m_GridTileBrushes.Count > 0 && m_SelectedGridTileBrushIndex != -1 && m_GridTileBrushes.Count > m_SelectedGridTileBrushIndex ? m_GridTileBrushes[m_SelectedGridTileBrushIndex] : null; }
    }

    [NaughtyAttributes.ReadOnly]
    public int m_SelectedGridTileBrushIndex = 0;

    public void RemoveTile(GridTileBrush tileBrush)
    {
        if (m_GridTileBrushes.Contains(tileBrush))
        {
            m_GridTileBrushes.Remove(tileBrush);
        }
    }

    public void RemoveAllTiles()
    {
        m_GridTileBrushes.Clear();
    }

    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public string GetGUID()
    {
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
    }

    public int GetIndex()
    {
        string[] guids = GetAllGridTileCollectionGUIDs();
        for (int i = 0; i < guids.Length; i++)
        {
            if (guids[i] == GetGUID())
            {
                return i;
            }
        }
        return 0;
    }

    public void SetLastUsedGridTileCollection()
    {
        EditorPrefs.SetString(lastGridTileCollectionEditPrefsKey, GetGUID());
    }

    public static int GetLastUsedGridTileCollectionIndex()
    {
        //try to find the last used brush collection and return it
        if (EditorPrefs.HasKey(lastGridTileCollectionEditPrefsKey))
        {
            string guid = EditorPrefs.GetString(lastGridTileCollectionEditPrefsKey, "");
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GridTileCollection lastUsedCollection = AssetDatabase.LoadAssetAtPath<GridTileCollection>(path);
            return lastUsedCollection.GetIndex();
        }
        else
        {
            return 0;
        }
    }

    public static string[] GetAllGridTileCollectionGUIDs()
    {
        return AssetDatabase.FindAssets("t:GridTileCollection");
    }

    public static GridTileCollectionList GetBrushCollectionsInProject()
    {
        string[] guids = GetAllGridTileCollectionGUIDs();
        return new GridTileCollectionList(guids);
    }

    public struct GridTileCollectionList
    {
        public List<GridTileCollection> brushCollections;

        public GridTileCollectionList(string[] guids)
        {
            brushCollections = new List<GridTileCollection>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                brushCollections.Add(AssetDatabase.LoadAssetAtPath<GridTileCollection>(path));
            }
        }

        public string[] GetNameList()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < brushCollections.Count; i++)
            {
                if (brushCollections[i] != null)
                {
                    names.Add(brushCollections[i].name);
                }
                else
                {
                    brushCollections.Remove(brushCollections[i]);
                }
            }
            return names.ToArray();
        }
    }
}

