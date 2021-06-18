using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Tilemaps;

public class AddGridObjectBrushPopup : EditorWindow
{
    public List<GridObjectBrush> m_Brushes;

    public static AddGridObjectBrushPopup Instance;

    private static string _windowName = "Add GridObject Brush";
    private GridObjectBrush _gridObjectBrushToAdd = new GridObjectBrush();

    public static void Initialize(List<GridObjectBrush> m_Brushes)
    {
        if (Instance != null)
        {
            return;
        }

        Instance = (AddGridObjectBrushPopup)EditorWindow.GetWindowWithRect(typeof(AddGridObjectBrushPopup), new Rect(0, 0, 400, 180));
        GUIContent titleContent = new GUIContent(_windowName);
        Instance.titleContent = titleContent;
        Instance.m_Brushes = m_Brushes;
        Instance.ShowUtility();
        Instance.Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _gridObjectBrushToAdd.m_GridObject = (GridObject)EditorGUILayout.ObjectField("GridObject Prefab", _gridObjectBrushToAdd.m_GridObject, typeof(GridObject), false);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = RemakeGridObjectBrushEditor.red;
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }

        GUI.backgroundColor = RemakeGridObjectBrushEditor.green;
        if (GUILayout.Button("Add"))
        {
            if (_gridObjectBrushToAdd != null && _gridObjectBrushToAdd.m_GridObject != null && PrefabUtility.IsPartOfAnyPrefab(_gridObjectBrushToAdd.m_GridObject.gameObject))
            {
                m_Brushes.Add(new GridObjectBrush(_gridObjectBrushToAdd));

                if (RemakeGridObjectBrushEditor.Instance != null)
                {
                    EditorUtility.SetDirty(RemakeGridObjectBrushEditor.Instance);
                    RemakeGridObjectBrushEditor.Instance.m_Collection.Save();
                }
            }
            Instance.Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}