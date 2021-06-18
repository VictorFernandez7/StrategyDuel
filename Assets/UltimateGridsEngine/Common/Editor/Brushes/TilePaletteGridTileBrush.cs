using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using System.Linq;

namespace UnityEditor.Tilemaps
{
    [CustomGridBrush(true, false, false, "UGE: GridTile Brush")]
    public class TilePaletteGridTileBrush : GridBrushBase
    {
        private BrushCell[] m_Cells;
        private Vector3Int m_Size;
        private Vector3Int m_Pivot;
        private Vector3Int m_MoveStartPos;
        private BrushCell m_EditorCell;
        private bool m_PickedTile = false;

        public BrushCell[] cells { get { return m_Cells; } }
        public Vector3Int pivot { get { return m_Pivot; } }
        public Vector3Int size { get { return m_Size; } }
        public BrushCell editorCell { get { return m_EditorCell; } set { m_EditorCell = value; } }
        public bool pickedTile { get { return (!CellsAreNull() && m_PickedTile); } set { m_PickedTile = value; } }

        public bool CellsAreNull()
        {
            foreach (BrushCell cell in m_Cells)
            {
                if (cell.gridTile != null)
                    return false;
            }
            return true;
        }
        public TilePaletteGridTileBrush()
        {
            Init(Vector3Int.one, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size)
        {
            Init(size, Vector3Int.zero);
            SizeUpdated();
        }

        public void Init(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), Vector3Int.zero);
            m_MoveStartPos = position.min;

            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                PickCellSceneReference(pos, brushPosition, gridLayout, brushTarget.transform);
            }
        }

        public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            // Settings for the loop
            var relativePos = position.min - m_MoveStartPos;
            var startX = relativePos.x > 0 ? position.size.x - 1 : 0;
            var endX = relativePos.x > 0 ? 0 : position.size.x - 1;
            var actionX = relativePos.x > 0 ? -1 : 1;
            var startY = relativePos.y > 0 ? position.size.y - 1 : 0;
            var endY = relativePos.y > 0 ? 0 : position.size.y - 1;
            var actionY = relativePos.y > 0 ? -1 : 1;
            var x = startX;
            var y = startY;

            //Debug.Log("startX: " + startX.ToString() + " endX: " + endX.ToString() + " actionX: " + actionX.ToString());
            //Debug.Log("startY: " + startY.ToString() + " endY: " + endY.ToString() + " actionY: " + actionY.ToString());

            // Loop through positions within the bounds based on the direction
            while (true)
            {
                while (true)
                {
                    var local = new Vector3Int(x, y, 0);
                    var location = local + position.position;
                    BrushCell cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                    if (cell != null && cell.gridTile != null)
                    {
                        GridManager.Instance.MoveTileToPosition(cell.gridTile, location.ToVector2IntXY(), cell.offset);
                    }

                    if (x == endX)
                        break;

                    x += actionX;
                }
                if (y == endY)
                {
                    break;
                }

                y += actionY;
                x = startX;
            }
            ResetPick();
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            if (!pickedTile)
            {
                ResetPick();
            }

            Vector3Int min = position - m_Pivot;
            BoundsInt bounds = new BoundsInt(min, m_Size);
            BoxFill(gridLayout, brushTarget, bounds);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {

            if (brushTarget == null || gridLayout == null)
                return;
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = m_Cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
                if (cell != null)
                {
                    PaintCell(gridLayout, location, cell);
                }
            }
        }

        private void PaintCell(GridLayout grid, Vector3Int position, BrushCell cell)
        {
            if (cell.gridTile != null)
            {
                GridManager.Instance.InstantiateGridTile(cell.gridTile, position.ToVector2IntXY(), cell.height, cell.offset, cell.orientation);
            }
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Reset();
            UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), new Vector3Int(pickStart.x, pickStart.y, 0));

            bool pickedATile = false;
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
                var tileAtPos = PickCellPrefab(pos, brushPosition, gridLayout, brushTarget.transform);
                if (tileAtPos)
                    pickedATile = true;
            }
            // Wether or not we picked a tile
            if (pickedATile)
            {
                pickedTile = true;
            }
            else
            {
                ResetPick();
            }
        }

        private bool PickCellPrefab(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var gridComp = grid as Grid;
                Vector3 cellCenter = gridComp.GetCellCenterWorld(position);
                GridTile tileAtPos = GridManager.Instance.GetGridTileAtPosition(position.ToVector2IntXY());

                if (tileAtPos != null)
                {
                    Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(tileAtPos);

                    if (prefab)
                    {
                        SetGridTile(brushPosition, (GridTile)prefab);
                        SetHeight(brushPosition, tileAtPos.m_TileHeight);
                        SetOffset(brushPosition, tileAtPos.transform.position - cellCenter);
                        SetOrientation(brushPosition, tileAtPos.transform.localRotation);
                        return true;
                    }
                }
            }

            return false;
        }

        private void PickCellSceneReference(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
        {
            if (parent != null)
            {
                var gridComp = grid as Grid;
                Vector3 cellCenter = gridComp.GetCellCenterWorld(position);
                GridTile tileAtPos = GridManager.Instance.GetGridTileAtPosition(position.ToVector2IntXY());

                if (tileAtPos != null)
                {
                    SetGridTile(brushPosition, tileAtPos);
                    SetHeight(brushPosition, tileAtPos.m_TileHeight);
                    SetOffset(brushPosition, tileAtPos.transform.position - cellCenter);
                    SetOrientation(brushPosition, tileAtPos.transform.localRotation);
                }
            }
        }

        public void SetCellFromEditor(Vector3Int brushPosition, GridTile gridTile, int height, Vector3 offSet, Quaternion orientation)
        {
            Reset();
            SetGridTile(brushPosition, gridTile);
            SetHeight(brushPosition, height);
            SetOffset(brushPosition, offSet);
            SetOrientation(brushPosition, orientation);

            m_EditorCell = new BrushCell();
            m_EditorCell.gridTile = gridTile;
            m_EditorCell.height = height;
            m_EditorCell.offset = offSet;
            m_EditorCell.orientation = orientation;
        }

        public void ClearCellFromEditor()
        {
            ResetPick();
            m_EditorCell = null;
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            ResetPick();

            if (brushTarget == null || gridLayout == null)
                return;

            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Vector3Int min = position - m_Pivot;
            BoundsInt bounds = new BoundsInt(min, m_Size);
            BoxErase(gridLayout, brushTarget, bounds);
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            if (brushTarget == null || gridLayout == null)
                return;

            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                EraseCell(gridLayout, location, brushTarget.transform);
            }

            ResetPick();
        }

        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            GridManager.Instance.EraseGridTileAtPosition(position.ToVector2IntXY());
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Debug.LogWarning("FloodFill not supported");
        }

        public override void Rotate(RotationDirection direction, Grid.CellLayout layout)
        {
            switch (layout)
            {
                case GridLayout.CellLayout.Hexagon:
                    {
                        var rotationDirection = direction == RotationDirection.Clockwise ? 60 : -60;
                        var rotationAxis = GridManager.Instance.GetRotationAxisVector();
                        Quaternion orientation = Quaternion.Euler(rotationAxis * rotationDirection);
                        foreach (BrushCell cell in m_Cells)
                        {
                            cell.orientation = cell.orientation * orientation;
                        }
                    }
                    break;
                case Grid.CellLayout.Isometric:
                case Grid.CellLayout.IsometricZAsY:
                case GridLayout.CellLayout.Rectangle:
                    {
                        var rotationDirection = direction == RotationDirection.Clockwise ? 90 : -90;
                        var rotationAxis = GridManager.Instance.GetRotationAxisVector();
                        Quaternion orientation = Quaternion.Euler(rotationAxis * rotationDirection);
                        foreach (BrushCell cell in m_Cells)
                        {
                            cell.orientation = cell.orientation * orientation;
                        }
#if UNITY_EDITOR
                        // Rotate the selected tile in the collection (if any)
                        TilePaletteGridTileBrushEditor.Instance.RotateSelectedTile(orientation);
#endif
                    }
                    break;
            }
        }

        public override void Flip(FlipAxis flip, Grid.CellLayout layout)
        {
            Debug.Log("Flip not supported");
        }

        public void Reset()
        {
            UpdateSizeAndPivot(Vector3Int.one, Vector3Int.zero);
            m_MoveStartPos = Vector3Int.zero;
        }

        public void ResetPick()
        {
            Reset();
            pickedTile = false;

            if (m_EditorCell != null && m_EditorCell.gridTile != null)
            {
                m_Cells[0] = m_EditorCell;
            }
        }

        public void UpdateSizeAndPivot(Vector3Int size, Vector3Int pivot)
        {
            m_Size = size;
            m_Pivot = pivot;
            SizeUpdated();
        }

        private void SizeUpdated()
        {
            m_Cells = new BrushCell[m_Size.x * m_Size.y * m_Size.z];
            BoundsInt bounds = new BoundsInt(Vector3Int.zero, m_Size);
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                m_Cells[GetCellIndex(pos)] = new BrushCell();
            }
        }

        public void SetGridTile(Vector3Int position, GridTile gt)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].gridTile = gt;
        }

        public void SetHeight(Vector3Int position, int height)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].height = height;
        }

        public void SetOffset(Vector3Int position, Vector3 offset)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].offset = offset;
        }

        public void SetOrientation(Vector3Int position, Quaternion orientation)
        {
            if (ValidateCellPosition(position))
                m_Cells[GetCellIndex(position)].orientation = orientation;
        }

        public int GetCellIndex(Vector3Int brushPosition)
        {
            return GetCellIndex(brushPosition.x, brushPosition.y, brushPosition.z);
        }

        public int GetCellIndex(int x, int y, int z)
        {
            return x + m_Size.x * y + m_Size.x * m_Size.y * z;
        }
        public int GetCellIndex(int x, int y, int z, int sizex, int sizey, int sizez)
        {
            return x + sizex * y + sizex * sizey * z;
        }
        public int GetCellIndexWrapAround(int x, int y, int z)
        {
            return (x % m_Size.x) + m_Size.x * (y % m_Size.y) + m_Size.x * m_Size.y * (z % m_Size.z);
        }

        private bool ValidateCellPosition(Vector3Int position)
        {
            var valid =
                position.x >= 0 && position.x < m_Size.x &&
                position.y >= 0 && position.y < m_Size.y &&
                position.z >= 0 && position.z < m_Size.z;
            if (!valid)
                throw new ArgumentException(string.Format("Position {0} is an invalid cell position. Valid range is between [{1}, {2}).", position, Vector3Int.zero, m_Size));
            return valid;
        }

        [Serializable]
        public class BrushCell
        {
            public GridTile gridTile { get { return m_GridTile; } set { m_GridTile = value; } }
            public int height { get { return m_Height; } set { m_Height = value; } }
            public Vector2 offset { get { return m_Offset; } set { m_Offset = value; } }
            public Quaternion orientation { get { return m_Orientation; } set { m_Orientation = value; } }

            [SerializeField]
            GridTile m_GridTile;
            [SerializeField]
            int m_Height = 0;
            [SerializeField]
            Vector3 m_Offset = Vector3.zero;
            [SerializeField]
            Quaternion m_Orientation = Quaternion.identity;

            public override int GetHashCode()
            {
                int hash = 0;
                unchecked
                {
                    hash = gridTile != null ? gridTile.GetInstanceID() : 0;
                    hash = hash * 33 + m_Orientation.GetHashCode();
                }
                return hash;
            }
        }
    }
}