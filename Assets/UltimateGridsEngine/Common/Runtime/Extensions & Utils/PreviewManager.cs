using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class PreviewManager : MonoBehaviour
{
#if UNITY_EDITOR
    [NaughtyAttributes.ReadOnly, SerializeField]
    protected List<PreviewObject> m_PreviewObjects = new List<PreviewObject>();
    // Holders
    protected Transform _previewObjectsHolder;
    public Transform m_PreviewObjectsHolder
    {
        get
        {
            if (_previewObjectsHolder == null) { GetHolders(); }
            return _previewObjectsHolder;
        }
        protected set { _previewObjectsHolder = value; }
    }
    // Singleton
    protected static PreviewManager _instance = null;
    public static PreviewManager Instance
    {
        get
        {
            if (_instance == null) { _instance = (PreviewManager)FindObjectOfType(typeof(PreviewManager)); }
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

        // Update holders
        GetHolders();
    }

    protected virtual void GetHolders()
    {
        if (_previewObjectsHolder == null)
        {
            _previewObjectsHolder = transform.Find("PreviewGameObjects");
            if (_previewObjectsHolder == null)
            {
                _previewObjectsHolder = new GameObject("PreviewGameObjects").transform;
                _previewObjectsHolder.SetParent(transform);
                _previewObjectsHolder.localPosition = Vector3.zero;
            }
        }
    }

    public void InstantiatePreviewTileAtPosition(GridTile tilePrefab, Vector2Int position, Vector3 offsetPosition, Quaternion rotation)
    {
        // Parameters
        var gridCellCenter = GridManager.Instance.m_Grid.GetCellCenterWorld(position.ToVector3IntXY0());
        var worldPosition = gridCellCenter + offsetPosition;
        GridTile instantiatedTile = null;

        if (PrefabUtility.IsPartOfPrefabAsset(tilePrefab.gameObject))
        {
            instantiatedTile = (GridTile)PrefabUtility.InstantiatePrefab(tilePrefab);
        }
        else
        {
            instantiatedTile = (GridTile)Instantiate(tilePrefab, worldPosition, rotation, m_PreviewObjectsHolder) as GridTile;
        }
        instantiatedTile.transform.parent = m_PreviewObjectsHolder;
        instantiatedTile.transform.position = worldPosition;
        instantiatedTile.transform.rotation = rotation;
        // Preview tile 
        var previewTile = new PreviewObject(instantiatedTile.gameObject, position);
        m_PreviewObjects.Add(previewTile);
        // Make the renderers attached to it transparent
        MakeVisualizerTransparent(instantiatedTile.transform);
        // Destroy the grid component
        //DestroyImmediate(instantiatedTile);
        var components = instantiatedTile.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour comp in components)
        {
            comp.enabled = false;
        }
    }

    public void InstantiatePreviewGridObjectAtPosition(GridObject objectPrefab, Vector2Int position, Vector3 offsetPosition, Orientations orientation)
    {
        // Check if there is a tile there
        var tileAtPosition = GridManager.Instance.GetGridTileAtPosition(position);
        if (!tileAtPosition)
            return;


        // Parameters
        var gridCellCenter = GridManager.Instance.m_Grid.GetCellCenterWorld(position.ToVector3IntXY0());
        var worldPosition = gridCellCenter + offsetPosition;
        GridObject instantiatedObject = null;
        if (PrefabUtility.IsPartOfPrefabAsset(objectPrefab.gameObject))
        {
            instantiatedObject = (GridObject)PrefabUtility.InstantiatePrefab(objectPrefab);
        }
        else
        {
            instantiatedObject = (GridObject)Instantiate(objectPrefab, worldPosition, Quaternion.identity, m_PreviewObjectsHolder) as GridObject;
        }
        var cellPosition = tileAtPosition.m_GridObjectsPivotPosition;
        instantiatedObject.transform.parent = m_PreviewObjectsHolder;
        instantiatedObject.transform.position = cellPosition;
        instantiatedObject.transform.rotation = GridManager.Instance.OrientationToRotation(position, orientation);
        // Preview tile 
        var previewObject = new PreviewObject(instantiatedObject.gameObject, position);
        m_PreviewObjects.Add(previewObject);
        // Make the renderers attached to it transparent
        MakeVisualizerTransparent(instantiatedObject.transform);

        var components = instantiatedObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour comp in components)
        {
            comp.enabled = false;
        }
    }

    public void ClearPreviewObjectAtPosition(Vector2Int position)
    {
        var previewTileAtPosition = m_PreviewObjects.OrderBy(t => t.m_GridPosition == position).FirstOrDefault();
        if (previewTileAtPosition.m_GameObject != null && previewTileAtPosition.m_GridPosition == position)
        {
            m_PreviewObjects.Remove(previewTileAtPosition);
            DestroyImmediate(previewTileAtPosition.m_GameObject);
        }
        else if (previewTileAtPosition.m_GameObject == null)
        {
            m_PreviewObjects.Remove(previewTileAtPosition);
        }
    }

    [ContextMenu("Debug: Remove all preview objects")]
    public void ClearAllPreviewTiles()
    {
        foreach (PreviewObject previewObject in m_PreviewObjects.ToList())
        {
            m_PreviewObjects.Remove(previewObject);
            if (previewObject.m_GameObject != null)
            {
                DestroyImmediate(previewObject.m_GameObject);
            }
        }
    }

    private static Transform MakeVisualizerTransparent(Transform transform)
    {
        // Attempt to get reference to GameObject Renderer
        Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();

        // If a Renderer was found
        if (meshRenderer != null)
        {
            // Define temporary Material used to create transparent copy of GameObject Material
            Material tempMat = new Material(Shader.Find("Standard"));

            Material[] tempMats = new Material[meshRenderer.sharedMaterials.Length];

            // Loop through each material in GameObject
            for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                // Get material from GameObject
                tempMat = new Material(meshRenderer.sharedMaterials[i]);

                // Change Shader to "Standard"
                tempMat.shader = Shader.Find("Standard");

                // Make Material transparent
                tempMat.SetFloat("_Mode", 2);
                tempMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                tempMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                tempMat.SetInt("_ZWrite", 0);
                tempMat.DisableKeyword("_ALPHATEST_ON");
                tempMat.EnableKeyword("_ALPHABLEND_ON");
                tempMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                tempMat.renderQueue = 3000;

                Color32 tempColor = tempMat.color;
                tempColor.a = (byte)((int)tempColor.a * 0.5);
                tempMat.color = tempColor;

                // Replace GameObject Material with transparent one
                tempMats[i] = tempMat;
            }

            meshRenderer.sharedMaterials = tempMats;
        }

        // Recursively run this method for each child transform
        foreach (Transform child in transform)
        {
            MakeVisualizerTransparent(child);
        }

        return transform;
    }
#endif
    [System.Serializable]
    public struct PreviewObject
    {
        public GameObject m_GameObject;
        public Vector2Int m_GridPosition;

        public PreviewObject(GameObject gameObject, Vector2Int gridPosition)
        {
            m_GameObject = gameObject;
            m_GridPosition = gridPosition;
        }
    }
}
