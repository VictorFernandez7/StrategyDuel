using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    [Header("Settings")]
    public Map m_MapSettings;
    public GridTile m_TilePrefab;
    public GridObject m_ObstaclePrefab;
    public GridObject m_CharacterPrefab;

    [Header("Randomize Map Settings")]
    public bool m_RandomizeMapSettings = true;
    public Vector2Int m_MinMapSize;
    public Vector2Int m_MaxMapSize;
    [Range(0, 1)]
    public float m_MinObstaclePercent;
    [Range(0, 1)]
    public float m_MaxObstaclePercent;
    public bool m_RandomizeSeed = true;

    protected List<Vector2Int> _allTileGridPositions;
    protected Queue<Vector2Int> _shuffledTileGridPositions;
    protected Queue<Vector2Int> _shuffledOpenTileGridPositions;
    protected Transform[,] _tileMap;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        // Randomize map settings
        if (m_RandomizeMapSettings)
        {
            RandomizeMapSettings();
        }

        _tileMap = new Transform[m_MapSettings.m_MapSize.x, m_MapSettings.m_MapSize.y];
        System.Random prng = new System.Random(m_MapSettings.m_Seed);

        // Generating Vector2Ints
        _allTileGridPositions = new List<Vector2Int>();
        for (int x = 0; x < m_MapSettings.m_MapSize.x; x++)
        {
            for (int y = 0; y < m_MapSettings.m_MapSize.y; y++)
            {
                _allTileGridPositions.Add(new Vector2Int(x, y));
            }
        }
        _shuffledTileGridPositions = new Queue<Vector2Int>(Helpers.ShuffleArray(_allTileGridPositions.ToArray(), m_MapSettings.m_Seed));

        // Create map holder object
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawning tiles
        for (int x = 0; x < m_MapSettings.m_MapSize.x; x++)
        {
            for (int y = 0; y < m_MapSettings.m_MapSize.y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                GridManager.Instance.InstantiateGridTile(m_TilePrefab, gridPosition, 0, Vector3.zero, Quaternion.identity, mapHolder);
            }
        }

        // Spawning obstacles
        bool[,] obstacleMap = new bool[(int)m_MapSettings.m_MapSize.x, (int)m_MapSettings.m_MapSize.y];
        int obstacleCount = (int)(m_MapSettings.m_MapSize.x * m_MapSettings.m_MapSize.y * m_MapSettings.m_ObstaclePercent);
        int currentObstacleCount = 0;
        List<Vector2Int> allOpenVector2Ints = new List<Vector2Int>(_allTileGridPositions);

        for (int i = 0; i < obstacleCount; i++)
        {
            Vector2Int randomVector2Int = GetRandomVector2Int();
            obstacleMap[randomVector2Int.x, randomVector2Int.y] = true;
            currentObstacleCount++;

            if (randomVector2Int != m_MapSettings.MapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                GridManager.Instance.InstantiateGridObject(m_ObstaclePrefab, randomVector2Int, null, mapHolder, false);
                allOpenVector2Ints.Remove(randomVector2Int);
            }
            else
            {
                obstacleMap[randomVector2Int.x, randomVector2Int.y] = false;
                currentObstacleCount--;
            }
        }

        _shuffledOpenTileGridPositions = new Queue<Vector2Int>(Helpers.ShuffleArray(allOpenVector2Ints.ToArray(), m_MapSettings.m_Seed));

        // Instantiate the player
        GridManager.Instance.InstantiateGridObject(m_CharacterPrefab, m_MapSettings.MapCentre, null, mapHolder, false);
    }

    public void RandomizeMapSettings()
    {
        // Calculate and set the map size
        //var lerpedVector3 = Vector3.Lerp(m_MinMapSize.ToVector3XY0(), m_MaxMapSize.ToVector3XY0(), Random.value);
        var randomXSize = Random.Range(m_MinMapSize.x, m_MaxMapSize.x + 1);
        var randomYSize = Random.Range(m_MinMapSize.y, m_MaxMapSize.y + 1);
        var randomMapSize = new Vector2Int(randomXSize, randomYSize);
        m_MapSettings.m_MapSize = randomMapSize;

        // Calculate and set the obstacle percentage
        var randomObstaclePercent = Random.Range(m_MinObstaclePercent, m_MaxObstaclePercent);
        m_MapSettings.m_ObstaclePercent = randomObstaclePercent;

        // Randomize the map's seed
        if (m_RandomizeSeed)
        {
            var randomSeed = Random.Range(0, 1000000);
            m_MapSettings.m_Seed = randomSeed;
        }
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(m_MapSettings.MapCentre);
        mapFlags[m_MapSettings.MapCentre.x, m_MapSettings.MapCentre.y] = true;

        int accessibleTileCount = 1;
        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Vector2Int(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(m_MapSettings.m_MapSize.x * m_MapSettings.m_MapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public Vector2Int GetRandomVector2Int()
    {
        Vector2Int randomVector2Int = _shuffledTileGridPositions.Dequeue();
        _shuffledTileGridPositions.Enqueue(randomVector2Int);
        return randomVector2Int;
    }

    public Transform GetRandomOpenTile()
    {
        Vector2Int randomVector2Int = _shuffledOpenTileGridPositions.Dequeue();
        _shuffledOpenTileGridPositions.Enqueue(randomVector2Int);
        return _tileMap[randomVector2Int.x, randomVector2Int.y];
    }

    [System.Serializable]
    public class Map
    {
        public Vector2Int m_MapSize;
        [Range(0, 1)]
        public float m_ObstaclePercent;
        public int m_Seed;

        public Vector2Int MapCentre
        {
            get
            {
                return new Vector2Int(m_MapSize.x / 2, m_MapSize.y / 2);
            }
        }
    }
}