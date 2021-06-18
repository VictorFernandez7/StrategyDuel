using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(TilePaletteGridObjectBrush))]
    public class RemakeGridObjectBrushEditor : GridBrushEditorBase
    {
        // Target brush
        public TilePaletteGridObjectBrush m_GridObjectBrush { get { return target as TilePaletteGridObjectBrush; } }
        // Editor Preview Collection/Selection
        private Vector2 _scrollViewScrollPosition = new Vector2();
        public GridObjectCollection m_Collection;
        public int m_SelectedGridObjectCollectionIndex = 0;
        // OnScene Preview
        private int m_LastPreviewRefreshHash;
        private GridLayout m_LastGrid;
        private GameObject m_LastBrushTarget;
        private BoundsInt? m_LastBounds;
        private GridBrushBase.Tool? m_LastTool;

        // Editor cached colors
        public static Color red = Utilities.ColorFromRGB(239, 80, 80);
        public static Color green = Utilities.ColorFromRGB(93, 173, 57);
        public static Color PrimarySelectedColor = Utilities.ColorFromRGB(10, 153, 220);
        public static RemakeGridObjectBrushEditor Instance { get; private set; }

        void OnEnable()
        {
            Instance = this;
            Undo.undoRedoPerformed += ClearLastPreview;
            m_SelectedGridObjectCollectionIndex = GridObjectCollection.GetLastUsedGridObjectCollectionIndex();
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= ClearLastPreview;
        }

        private void ClearLastPreview()
        {
            ClearPreviewAll();
            m_LastPreviewRefreshHash = 0;
        }

        public override void OnPaintInspectorGUI()
        {
            if (m_GridObjectBrush.pickedObject)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Currently Picked Objects (Pick Tool)", EditorStyles.boldLabel);
                if (GUILayout.Button("Unpick Objects"))
                {
                    m_GridObjectBrush.ResetPick();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(7.5f);

                int rowLength = 1;
                int maxRowLength = m_GridObjectBrush.size.x;
                var previewSize = Mathf.Min(((Screen.width - 35) / maxRowLength), 100);

                _scrollViewScrollPosition = EditorGUILayout.BeginScrollView(_scrollViewScrollPosition, false, false);

                if (maxRowLength < 1)
                {
                    maxRowLength = 1;
                }

                foreach (TilePaletteGridObjectBrush.BrushCell tileBrush in m_GridObjectBrush.cells)
                {
                    //check if row is longer than max row length
                    if (rowLength > maxRowLength)
                    {
                        rowLength = 1;
                        EditorGUILayout.EndHorizontal();
                    }
                    //begin row if rowLength == 1
                    if (rowLength == 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    GUIContent btnContent = tileBrush != null && tileBrush.gridObject != null ?
                    new GUIContent(AssetPreview.GetAssetPreview(tileBrush.gridObject.gameObject), tileBrush.gridObject.gameObject.name) :
                    new GUIContent("", "There is no tile at this position.");
                    if (GUILayout.Button(btnContent, GUILayout.Width(previewSize), GUILayout.Height(previewSize)))
                    {

                    }
                    rowLength++;
                }

                //check if row is longer than max row length
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
            else
            { // If there is no tile picked show the collections GUI
                #region GridTile Collection
                SerializedObject serializedObject_brushObject = null;
                int prevSelectedGridTileCollectionIndex = m_SelectedGridObjectCollectionIndex;
                if (m_Collection != null)
                {
                    serializedObject_brushObject = new SerializedObject(m_Collection);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Active GridTile Collection:");
                GridObjectCollection.GridObjectCollectionList gridObjectCollectionList = GridObjectCollection.GetGridObjectCollectionsInProject();
                m_SelectedGridObjectCollectionIndex = EditorGUILayout.Popup(m_SelectedGridObjectCollectionIndex, gridObjectCollectionList.GetNameList());
                if (prevSelectedGridTileCollectionIndex != m_SelectedGridObjectCollectionIndex || m_Collection == null) //select only when brush collection changed or is null
                {
                    m_Collection = gridObjectCollectionList.brushCollections[m_SelectedGridObjectCollectionIndex];
                    m_Collection.SetLastUsedGridObjectCollection();
                    m_GridObjectBrush.ClearCellFromEditor();
                    var tileBrush = m_Collection.m_SelectedGridObjectBrush;
                    if (tileBrush != null)
                    {
                        m_GridObjectBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_GridObject, tileBrush.m_InitialOrientation);
                    }
                }

                if (GUILayout.Button("+"))
                {
                    Debug.Log("Create a new collection");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(7.5f);
                int rowLength = 1;
                int maxRowLength = Mathf.FloorToInt((Screen.width - 15) / 100);
                int columns = Mathf.CeilToInt((m_Collection.m_GridObjectBrushes.Count / maxRowLength)) * 3;
                _scrollViewScrollPosition = EditorGUILayout.BeginScrollView(_scrollViewScrollPosition, false, false);

                if (maxRowLength < 1)
                {
                    maxRowLength = 1;
                }

                foreach (GridObjectBrush tileBrush in m_Collection.m_GridObjectBrushes)
                {
                    //check if brushObject is null, if so skip this brush
                    if (tileBrush == null || tileBrush.m_GridObject == null)
                    {
                        continue;
                    }

                    //check if row is longer than max row length
                    if (rowLength > maxRowLength)
                    {
                        rowLength = 1;
                        EditorGUILayout.EndHorizontal();
                    }
                    //begin row if rowLength == 1
                    if (rowLength == 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    //change color
                    Color guiColor = GUI.backgroundColor;
                    if (m_Collection.m_SelectedGridObjectBrush != null && m_Collection.m_SelectedGridObjectBrush.m_GridObject != null && m_Collection.m_SelectedGridObjectBrush.m_GridObject == tileBrush.m_GridObject)
                    {
                        GUI.backgroundColor = PrimarySelectedColor;
                        if (m_GridObjectBrush.editorCell != null && m_GridObjectBrush.editorCell.gridObject != tileBrush.m_GridObject)
                        {
                            m_GridObjectBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_GridObject, tileBrush.m_InitialOrientation);
                        }
                    }

                    //Create the brush entry in the scroll view and check if the user clicked on the created button (change the currently selected/edited brush accordingly and add it to the current brushes if possible)
                    GUIContent btnContent = new GUIContent(AssetPreview.GetAssetPreview(tileBrush.m_GridObject.gameObject), tileBrush.m_GridObject.gameObject.name);
                    if (GUILayout.Button(btnContent, GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        //select the currently edited brush and deselect all selected brushes
                        if (m_Collection.m_SelectedGridObjectBrush != tileBrush)
                        {

                            m_Collection.m_SelectedGridObjectBrushIndex = m_Collection.m_GridObjectBrushes.IndexOf(tileBrush);
                            m_GridObjectBrush.SetCellFromEditor(Vector3Int.zero, tileBrush.m_GridObject, tileBrush.m_InitialOrientation);
                        }
                        else
                        {
                            //m_Collection.m_SelectedGridTileBrush = null;
                            m_Collection.m_SelectedGridObjectBrushIndex = -1;
                            m_GridObjectBrush.ClearCellFromEditor();
                        }
                    }
                    GUI.backgroundColor = guiColor;
                    rowLength++;
                }

                //check if row is longer than max row length
                if (rowLength > maxRowLength)
                {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                if (rowLength == 1)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                //add button
                if (GUILayout.Button(new GUIContent("+", "Add a GridObject to the collection."), GUILayout.Width(100), GUILayout.Height(100)))
                {
                    AddGridObjectBrushPopup.Initialize(m_Collection.m_GridObjectBrushes);
                }
                Color guiBGColor = GUI.backgroundColor;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = green;
                if (GUILayout.Button(new GUIContent("Add GridObject", "Add a GridObject to the collection.")))
                {
                    AddGridObjectBrushPopup.Initialize(m_Collection.m_GridObjectBrushes);
                }
                EditorGUI.BeginDisabledGroup(m_Collection.m_SelectedGridObjectBrush == null || m_Collection.m_SelectedGridObjectBrush.m_GridObject == null);
                GUI.backgroundColor = red;
                //remove selected brushes button
                if (GUILayout.Button(new GUIContent("Remove Selected GridObject", "Removes the selected gridobject from the collection.")))
                {
                    if (m_Collection.m_SelectedGridObjectBrush != null)
                    {
                        m_Collection.RemoveObject(m_Collection.m_SelectedGridObjectBrush);
                        m_Collection.m_SelectedGridObjectBrushIndex = -1;
                        m_Collection.Save();
                    }
                }
                EditorGUI.EndDisabledGroup();
                //remove all brushes button
                EditorGUI.BeginDisabledGroup(m_Collection.m_GridObjectBrushes.Count == 0);
                if (GUILayout.Button(new GUIContent("Remove All GridObjects", "Removes all tiles from the collection.")) && RemoveAllBrushes_Dialog(m_Collection.m_GridObjectBrushes.Count))
                {
                    m_Collection.RemoveAllObjects();
                    m_Collection.Save();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = guiBGColor;

                if (m_Collection.m_GridObjectBrushes != null && m_Collection.m_GridObjectBrushes.Count > 0 && m_Collection.m_SelectedGridObjectBrush != null && m_Collection.m_SelectedGridObjectBrush.m_GridObject != null)
                {
                    EditorGUILayout.Space(10f);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField("GridObject Settings:" + "  (" + m_Collection.m_SelectedGridObjectBrush.m_GridObject.gameObject.name + ")", EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Reset Settings", "Restores the settings for the current GridObject."), GUILayout.MaxWidth(120)))
                    {
                        m_Collection.m_SelectedGridObjectBrush.ResetParameters();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    m_Collection.m_SelectedGridObjectBrush.m_InitialOrientation = (Orientations)EditorGUILayout.EnumPopup(new GUIContent("Initial Orientation", "Changes the initial orientation of the gridobject."), m_Collection.m_SelectedGridObjectBrush.m_InitialOrientation);
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        // Update the cell's settings
                        m_GridObjectBrush.SetCellFromEditor(Vector3Int.zero, m_Collection.m_SelectedGridObjectBrush.m_GridObject, m_Collection.m_SelectedGridObjectBrush.m_InitialOrientation);
                    }
                }
                EditorGUILayout.Space(10f);
                /*
                if (GUI.changed && m_Collection != null)
                {
                    m_Collection.Save();
                }
                */
                #endregion
            }
        }

        public void RotateSelectedTile(GridBrushBase.RotationDirection rotateDirection)
        {

            if (m_Collection != null && m_Collection.m_GridObjectBrushes != null && m_Collection.m_GridObjectBrushes.Count > 0 && m_Collection.m_SelectedGridObjectBrush != null && m_Collection.m_SelectedGridObjectBrush.m_GridObject != null)
            {
                /*
                m_Collection.m_SelectedGridObjectBrush.m_Rotation = (Quaternion.Euler(m_Collection.m_SelectedGridObjectBrush.m_Rotation) * orientation).eulerAngles;
                EditorUtility.SetDirty(this);
                */
            }

        }


        public override bool canChangeZPosition
        {
            get { return false; }
            set { }
        }

        bool RemoveAllBrushes_Dialog(int brushCount)
        {
            return EditorUtility.DisplayDialog(
                "Remove all GridTiles?",
                "Are you sure you want to remove all GridTiles (" + brushCount + ") from this collection?",
                "Remove all",
                "Cancel");
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            BoundsInt gizmoRect = position;
            bool refreshPreviews = false;
            if (Event.current.type == EventType.Layout)
            {
                int newPreviewRefreshHash = GetHash(gridLayout, brushTarget, position, tool, m_GridObjectBrush);
                refreshPreviews = newPreviewRefreshHash != m_LastPreviewRefreshHash;
                if (refreshPreviews)
                    m_LastPreviewRefreshHash = newPreviewRefreshHash;
            }
            // Move preview - To be fully implemented on the next version
            /*
            if (tool == GridBrushBase.Tool.Move)
            {
                if (refreshPreviews && executing)
                {
                    ClearPreview();
                    PaintPreview(gridLayout, brushTarget, position.min);
                }
            }
            // Paint preview
            else*/
            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
            {
                if (refreshPreviews)
                {
                    ClearPreviewAll();
                    if (tool != GridBrushBase.Tool.Erase)
                    {
                        PaintPreview(gridLayout, brushTarget, position.min);
                    }
                }
                gizmoRect = new BoundsInt(position.min - m_GridObjectBrush.pivot, m_GridObjectBrush.size);
            }
            // BoxFill Preview
            else if (tool == GridBrushBase.Tool.Box)
            {
                if (refreshPreviews)
                {
                    ClearPreviewAll();
                    BoxFillPreview(gridLayout, brushTarget, position);
                }
            }

            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);

            // Paint the hovered grid position onto the scene
            var labelText = "Grid Position: " + position.position;
            if (position.size.x > 1 || position.size.y > 1)
            {
                labelText += " Size: " + position.size;
            }
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            Handles.Label(gridLayout.CellToWorld(position.position), labelText, style);
        }

        public virtual void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - m_GridObjectBrush.pivot;
            Vector3Int max = min + m_GridObjectBrush.size;
            BoundsInt bounds = new BoundsInt(min, max - min);

            var pvmanager = PreviewManager.Instance;
            if (pvmanager == null)
                return;

            if (brushTarget != null && gridLayout != null)
            {
                foreach (Vector3Int location in bounds.allPositionsWithin)
                {
                    Vector3Int brushPosition = location - min;
                    TilePaletteGridObjectBrush.BrushCell cell = m_GridObjectBrush.cells[m_GridObjectBrush.GetCellIndex(brushPosition)];
                    if (cell.gridObject != null)
                    {
                        SetPreviewCell(gridLayout, location, cell);
                    }
                }
            }

            m_LastGrid = gridLayout;
            m_LastBounds = bounds;
            m_LastBrushTarget = brushTarget;
            m_LastTool = GridBrushBase.Tool.Paint;
        }

        public virtual void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget != null)
            {
                var pvmanager = PreviewManager.Instance;
                if (pvmanager == null)
                    return;

                foreach (Vector3Int location in position.allPositionsWithin)
                {
                    Vector3Int local = location - position.min;
                    TilePaletteGridObjectBrush.BrushCell cell = m_GridObjectBrush.cells[m_GridObjectBrush.GetCellIndexWrapAround(local.x, local.y, local.z)];
                    if (cell.gridObject != null)
                    {
                        SetPreviewCell(gridLayout, location, cell);
                    }
                }
            }

            m_LastGrid = gridLayout;
            m_LastBounds = position;
            m_LastBrushTarget = brushTarget;
            m_LastTool = GridBrushBase.Tool.Box;
        }

        public virtual void ClearPreview()
        {
            if (m_LastGrid == null || m_LastBounds == null || m_LastBrushTarget == null || m_LastTool == null)
                return;

            var pvmanager = PreviewManager.Instance;
            if (pvmanager != null)
            {
                switch (m_LastTool)
                {
                    /*
                    case GridBrushBase.Tool.FloodFill:
                        {
                            map.ClearAllEditorPreviewTiles();
                            break;
                        }
                    */
                    case GridBrushBase.Tool.Box:
                        {
                            Vector3Int min = m_LastBounds.Value.position;
                            Vector3Int max = min + m_LastBounds.Value.size;
                            BoundsInt bounds = new BoundsInt(min, max - min);
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                    case GridBrushBase.Tool.Paint:
                        {
                            BoundsInt bounds = m_LastBounds.Value;
                            foreach (Vector3Int location in bounds.allPositionsWithin)
                            {
                                ClearPreviewCell(location);
                            }
                            break;
                        }
                }
            }

            m_LastBrushTarget = null;
            m_LastGrid = null;
            m_LastBounds = null;
            m_LastTool = null;
        }

        private static void SetPreviewCell(GridLayout grid, Vector3Int position, TilePaletteGridObjectBrush.BrushCell cell)
        {
            if (cell.gridObject == null || grid == null)
                return;

            var pvmanager = PreviewManager.Instance;
            if (pvmanager == null)
                return;

            PreviewManager.Instance.InstantiatePreviewGridObjectAtPosition(cell.gridObject, position.ToVector2IntXY(), Vector3.zero, cell.orientation);
        }

        private static void ClearPreviewCell(Vector3Int location)
        {
            var pvmanager = PreviewManager.Instance;
            if (pvmanager == null)
                return;

            PreviewManager.Instance.ClearPreviewObjectAtPosition(location.ToVector2IntXY());
        }

        private static void ClearPreviewAll()
        {
            var pvmanager = PreviewManager.Instance;
            if (pvmanager == null)
                return;

            PreviewManager.Instance.ClearAllPreviewTiles();
        }

        public override void OnMouseLeave()
        {
            ClearPreviewAll();
        }

        public override void OnToolDeactivated(GridBrushBase.Tool tool)
        {
            ClearPreviewAll();
        }

        private static int GetHash(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, TilePaletteGridObjectBrush brush)
        {
            int hash = 0;
            unchecked
            {
                hash = hash * 33 + (gridLayout != null ? gridLayout.GetHashCode() : 0);
                hash = hash * 33 + (brushTarget != null ? brushTarget.GetHashCode() : 0);
                hash = hash * 33 + position.GetHashCode();
                hash = hash * 33 + tool.GetHashCode();
                hash = hash * 33 + (brush != null ? brush.GetHashCode() : 0);
            }

            return hash;
        }
    }
}