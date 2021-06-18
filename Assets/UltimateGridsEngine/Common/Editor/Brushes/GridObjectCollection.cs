using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New GridObject Collection", menuName = "UGE/Collections/Create GridObject Collection")]
public class GridObjectCollection : ScriptableObject
{
    protected static string lastGridObjectCollectionEditPrefsKey = "LastUsedGOC";

    [SerializeField, Header("GridObjects In Collection")] private List<GridObjectBrush> _gridObjectBrushes = new List<GridObjectBrush>();
    public List<GridObjectBrush> m_GridObjectBrushes
    {
        get
        {
            if (_gridObjectBrushes == null)
            {
                _gridObjectBrushes = new List<GridObjectBrush>();
            }
            return _gridObjectBrushes;
        }
        set
        {
            if (value == null)
            {
                value = new List<GridObjectBrush>();
            }
            _gridObjectBrushes = value;
        }
    }

    public GridObjectBrush m_SelectedGridObjectBrush
    {
        get { return m_GridObjectBrushes.Count > 0 && m_SelectedGridObjectBrushIndex != -1 && m_GridObjectBrushes.Count > m_SelectedGridObjectBrushIndex ? m_GridObjectBrushes[m_SelectedGridObjectBrushIndex] : null; }
    }

    [NaughtyAttributes.ReadOnly]
    public int m_SelectedGridObjectBrushIndex = 0;

    public void RemoveObject(GridObjectBrush objectBrush)
    {
        if (m_GridObjectBrushes.Contains(objectBrush))
        {
            m_GridObjectBrushes.Remove(objectBrush);
        }
    }

    public void RemoveAllObjects()
    {
        m_GridObjectBrushes.Clear();
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
        string[] guids = GetAllGridObjectCollectionGUIDs();
        for (int i = 0; i < guids.Length; i++)
        {
            if (guids[i] == GetGUID())
            {
                return i;
            }
        }
        return 0;
    }

    public void SetLastUsedGridObjectCollection()
    {
        EditorPrefs.SetString(lastGridObjectCollectionEditPrefsKey, GetGUID());
    }

    public static int GetLastUsedGridObjectCollectionIndex()
    {
        //try to find the last used brush collection and return it
        if (EditorPrefs.HasKey(lastGridObjectCollectionEditPrefsKey))
        {
            string guid = EditorPrefs.GetString(lastGridObjectCollectionEditPrefsKey, "");
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GridObjectCollection lastUsedCollection = AssetDatabase.LoadAssetAtPath<GridObjectCollection>(path);
            return lastUsedCollection.GetIndex();
        }
        else
        {
            return 0;
        }
    }

    public static string[] GetAllGridObjectCollectionGUIDs()
    {
        return AssetDatabase.FindAssets("t:GridObjectCollection");
    }

    public static GridObjectCollectionList GetGridObjectCollectionsInProject()
    {
        string[] guids = GetAllGridObjectCollectionGUIDs();
        return new GridObjectCollectionList(guids);
    }

    public struct GridObjectCollectionList
    {
        public List<GridObjectCollection> brushCollections;

        public GridObjectCollectionList(string[] guids)
        {
            brushCollections = new List<GridObjectCollection>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                brushCollections.Add(AssetDatabase.LoadAssetAtPath<GridObjectCollection>(path));
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

