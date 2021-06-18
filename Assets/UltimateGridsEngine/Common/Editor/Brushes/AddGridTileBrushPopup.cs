using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Tilemaps;

public class AddGridTileBrushPopup : EditorWindow
{
    public List<GridTileBrush> m_Brushes;

    private GridTileBrush _gridTileBrushToAdd = new GridTileBrush();

    public static AddGridTileBrushPopup Instance;
    private static string _windowName = "Add GridTile Brush";

    public static void Initialize(List<GridTileBrush> m_Brushes)
    {
        if (Instance != null)
        {
            return;
        }

        Instance = (AddGridTileBrushPopup)EditorWindow.GetWindowWithRect(typeof(AddGridTileBrushPopup), new Rect(0, 0, 400, 180));//ScriptableObject.CreateInstance<AddGridTileBrushPopup>();
        GUIContent titleContent = new GUIContent(_windowName);
        Instance.titleContent = titleContent;
        Instance.m_Brushes = m_Brushes;
        Instance.ShowUtility();
        Instance.Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _gridTileBrushToAdd.m_GridTile = (GridTile)EditorGUILayout.ObjectField("GridTile Prefab", _gridTileBrushToAdd.m_GridTile, typeof(GridTile), false);
        _gridTileBrushToAdd.m_Height = EditorGUILayout.IntField("Tile Height", _gridTileBrushToAdd.m_Height);
        _gridTileBrushToAdd.m_Offset = EditorGUILayout.Vector3Field("Position Offset", _gridTileBrushToAdd.m_Offset);
        _gridTileBrushToAdd.m_Rotation = EditorGUILayout.Vector3Field("Rotation Offset", _gridTileBrushToAdd.m_Rotation);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = TilePaletteGridTileBrushEditor.red;
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }

        GUI.backgroundColor = TilePaletteGridTileBrushEditor.green;
        if (GUILayout.Button("Add"))
        {
            if (_gridTileBrushToAdd != null && _gridTileBrushToAdd.m_GridTile != null && PrefabUtility.IsPartOfAnyPrefab(_gridTileBrushToAdd.m_GridTile.gameObject))
            {
                m_Brushes.Add(new GridTileBrush(_gridTileBrushToAdd));

                if (TilePaletteGridTileBrushEditor.Instance != null)
                {
                    EditorUtility.SetDirty(TilePaletteGridTileBrushEditor.Instance);
                    TilePaletteGridTileBrushEditor.Instance.m_Collection.Save();
                }
            }
            this.Close();
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