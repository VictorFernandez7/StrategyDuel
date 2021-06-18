using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using EPOOutline;
using TMPro;

using Game.UI;
using Game.Tiles;

namespace Game.Managers
{
    public class TileManager : Utils.SingletonBehaviour<TileManager>
    {
        #region Variables
        [TitleGroup("Board Generation")]
        [FoldoutGroup("Board Generation/Rules")] [SerializeField] private bool _mirror;
        [FoldoutGroup("Board Generation/Parameters")] [SerializeField] [MinMaxSlider(1, 10, true)] private Vector2 _randomForestAmount;
        [FoldoutGroup("Board Generation/Parameters")] [SerializeField] [MinMaxSlider(1, 10, true)] private Vector2 _randomHillAmount;
        [FoldoutGroup("Board Generation/Parameters")] [SerializeField] [MinMaxSlider(1, 10, true)] private Vector2 _randomMountainAmount;
        [FoldoutGroup("Board Generation/Parameters")] [SerializeField] [MinMaxSlider(1, 10, true)] private Vector2 _randomSwampAmount;
        [FoldoutGroup("Board Generation/Parameters")] [SerializeField] [MinMaxSlider(1, 10, true)] private Vector2 _randomIslandAmount;
        [FoldoutGroup("Board Generation/Parameters")] [SerializeField] [MinMaxSlider(1, 10, true)] private Vector2 _randomWaterAmount;
        [FoldoutGroup("Board Generation/Results")] [SerializeField] [DisableInPlayMode] private int _forestAmount;
        [FoldoutGroup("Board Generation/Results")] [SerializeField] [DisableInPlayMode] private int _hillAmount;
        [FoldoutGroup("Board Generation/Results")] [SerializeField] [DisableInPlayMode] private int _mountainAmount;
        [FoldoutGroup("Board Generation/Results")] [SerializeField] [DisableInPlayMode] private int _swampAmount;
        [FoldoutGroup("Board Generation/Results")] [SerializeField] [DisableInPlayMode] private int _islandAmount;
        [FoldoutGroup("Board Generation/Results")] [SerializeField] [DisableInPlayMode] private int _waterAmount;
        [FoldoutGroup("Board Generation/Currently Placing")] [SerializeField] [EnumToggleButtons] [HideLabel] [DisableInPlayMode] private TileType _currentlyPlacing;
        [FoldoutGroup("Board Generation/Timing")] [SerializeField] private float _timeBetweenTypes;
        [FoldoutGroup("Board Generation/Timing")] [SerializeField] private float _timeBetweenTiles;

        [TitleGroup("Parameters")]
        [FoldoutGroup("Parameters/Movement")] [SerializeField] private int _plainCost;
        [FoldoutGroup("Parameters/Movement")] [SerializeField] private int _forestCost;
        [FoldoutGroup("Parameters/Movement")] [SerializeField] private int _hillCost;
        [FoldoutGroup("Parameters/Movement")] [SerializeField] private int _mountainCost;
        [FoldoutGroup("Parameters/Movement")] [SerializeField] private int _swampCost;
        [FoldoutGroup("Parameters/Movement")] [SerializeField] private int _islandCost;
        [FoldoutGroup("Parameters/UI")] [SerializeField] private float _typeTooltipTimer;
        [FoldoutGroup("Parameters/UI")] [SerializeField] private Vector2 _typeTooltipOffset;
        [FoldoutGroup("Parameters/UI")] [SerializeField] private float _remakeAmount;
        [FoldoutGroup("Parameters/Display")] [SerializeField] private Color _moveColor;
        [FoldoutGroup("Parameters/Display")] [SerializeField] private Color _attackColor;
        [FoldoutGroup("Parameters/Display")] [SerializeField] private float _selectionLineMinWidth;
        [FoldoutGroup("Parameters/Display")] [SerializeField] private float _selectionLineMaxWidth;
        [FoldoutGroup("Parameters/Display")] [SerializeField] private float _selectionLineSpeed;

        [Title("Events")]
        [SerializeField] public UnityEvent onBoardGenerating;
        [SerializeField] public UnityEvent onBoardGenerated;

        [TitleGroup("References")]
        [FoldoutGroup("References/Tiles")] [SerializeField] public GameObject[] playerTiles;
        [FoldoutGroup("References/Tiles")] [SerializeField] public GameObject[] opponentTiles;
        [FoldoutGroup("References/Tiles")] [SerializeField] public LineRenderer tileSelectionLine;
        [FoldoutGroup("References/UI")] [SerializeField] public GameObject tooltip;
        [FoldoutGroup("References/UI")] [SerializeField] private TextMeshProUGUI _remakeTitle;

        // TILE LIST STORAGE
        private List<int> _temporalPlayerTiles;
        private List<int> _temporalOpponentTiles;
        private List<int> _randomizedPlayerTiles;
        private List<int> _randomizedOpponentTiles;

        // PRIVATE VARIABLES
        private int _tileTypes = 7;
        private bool _placingTiles;
        #endregion

        #region Properties
        public float typeTooltipTimer => _typeTooltipTimer;
        public int plainCost => _plainCost;
        public int forestCost => _forestCost;
        public int hillCost => _hillCost;
        public int mountainCost => _mountainCost;
        public int swampCost => _swampCost;
        public int islandCost => _islandCost;
        public Color moveColor => _moveColor;
        public Color attackColor => _attackColor;
        public float selectionLineMinWidth => _selectionLineMinWidth;
        public float selectionLineMaxWidth => _selectionLineMaxWidth;
        public float selectionLineSpeed => _selectionLineSpeed;
        #endregion

        public enum TileType
        {
            Plain,
            Forest,
            Hill,
            Mountain,
            Swamp,
            Island,
            Water
        }

        private void Start()
        {
            // Board generation
            onBoardGenerating.Invoke();
            GenerateRandomTileAmounts();
            CalculateRandomIDs();
            StartCoroutine(TilePlacing());

            // OTHER
            UpdateTileCostOnUI();
        }

        private void Update()
        {
            if (_placingTiles && Input.GetKeyDown(KeyCode.Space))
            {
                _timeBetweenTypes = 0.01f;
                _timeBetweenTiles = 0.01f;
            }
        }

        /// <summary>
        /// Clears and remakes the current board. Triggered via UI
        /// </summary>
        public void RemakeBoard()
        {
            _remakeAmount--;

            if (_remakeAmount >= 0)
            {
                _remakeTitle.text = _remakeAmount + (_remakeAmount == 1 ? " remake left" : " remakes left");

                ClearBoardTiles();
                onBoardGenerating.Invoke();
                GenerateRandomTileAmounts();
                CalculateRandomIDs();
                StartCoroutine(TilePlacing());
            }

            // TODO display no remakes left notification
        }

        /// <summary>
        /// Generates a random amount of each tile type given vector2 parameters
        /// </summary>
        public void GenerateRandomTileAmounts()
        {
            _forestAmount = (int)Random.Range(_randomForestAmount.x, _randomForestAmount.y + 1);
            _hillAmount = (int)Random.Range(_randomHillAmount.x, _randomHillAmount.y + 1);
            _mountainAmount = (int)Random.Range(_randomMountainAmount.x, _randomMountainAmount.y + 1);
            _swampAmount = (int)Random.Range(_randomSwampAmount.x, _randomSwampAmount.y + 1);
            _islandAmount = (int)Random.Range(_randomIslandAmount.x, _randomIslandAmount.y + 1);
            _waterAmount = (int)Random.Range(_randomWaterAmount.x, _randomWaterAmount.y + 1);
        }

        /// <summary>
        /// Creates two new lists with random order
        /// </summary>
        private void CalculateRandomIDs()
        {
            _temporalPlayerTiles = new List<int>();
            _temporalOpponentTiles = new List<int>();
            _randomizedPlayerTiles = new List<int>();
            _randomizedOpponentTiles = new List<int>();

            for (int i = 0; i < playerTiles.Length; i++)
            {
                _temporalPlayerTiles.Add(i);
            }

            for (int i = 0; i < playerTiles.Length; i++)
            {
                int ranNum = _temporalPlayerTiles[Random.Range(0, _temporalPlayerTiles.Count)];
                _randomizedPlayerTiles.Add(ranNum);
                _temporalPlayerTiles.Remove(ranNum);
            }

            for (int i = 0; i < opponentTiles.Length; i++)
            {
                _temporalOpponentTiles.Add(i);
            }

            for (int i = 0; i < opponentTiles.Length; i++)
            {
                int ranNum = _temporalOpponentTiles[Random.Range(0, _temporalOpponentTiles.Count)];
                _randomizedOpponentTiles.Add(ranNum);
                _temporalOpponentTiles.Remove(ranNum);
            }
        }

        /// <summary>
        /// Tile placing management
        /// </summary>
        /// <returns></returns>
        private IEnumerator TilePlacing()
        {
            _placingTiles = true;
            ResetTimingValues();

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < _tileTypes; i++)
            {
                switch (i)
                {
                    case 0:
                        StartCoroutine(PlaceTile(TileType.Forest, new Vector2(0, _forestAmount), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                    case 1:
                        StartCoroutine(PlaceTile(TileType.Hill, new Vector2(_forestAmount, _forestAmount + _hillAmount), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                    case 2:
                        StartCoroutine(PlaceTile(TileType.Mountain, new Vector2(_forestAmount + _hillAmount, _forestAmount + _hillAmount + _mountainAmount), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                    case 3:
                        StartCoroutine(PlaceTile(TileType.Swamp, new Vector2(_forestAmount + _hillAmount + _mountainAmount, _forestAmount + _hillAmount + _mountainAmount + _swampAmount), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                    case 4:
                        StartCoroutine(PlaceTile(TileType.Island, new Vector2(_forestAmount + _hillAmount + _mountainAmount + _swampAmount, _forestAmount + _hillAmount + _mountainAmount + _swampAmount + _islandAmount), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                    case 5:
                        StartCoroutine(PlaceTile(TileType.Water, new Vector2(_forestAmount + _hillAmount + _mountainAmount + _swampAmount + _islandAmount, _forestAmount + _hillAmount + _mountainAmount + _swampAmount + _islandAmount + _waterAmount), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                    case 6:
                        StartCoroutine(PlaceTile(TileType.Plain, new Vector2(_forestAmount + _hillAmount + _mountainAmount + _swampAmount + _islandAmount + _waterAmount, _randomizedPlayerTiles.Count), _timeBetweenTiles));
                        yield return new WaitForSeconds(_timeBetweenTypes);
                        break;
                }
            }

            yield return new WaitForSeconds(0.5f);

            _placingTiles = false;
            onBoardGenerated.Invoke();
        }

        /// <summary>
        /// Places a tile given various parameters
        /// </summary>
        /// <param name="desiredType"></param>
        /// <param name="range"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        private IEnumerator PlaceTile(TileType desiredType, Vector2 range, float waitTime)
        {
            _currentlyPlacing = desiredType;

            for (int i = (int)range.x; i < range.y; i++)
            {
                playerTiles[_randomizedPlayerTiles[i]].GetComponent<Tiles.TileType>().GenerateType(desiredType);

                if (_mirror)
                    opponentTiles[_randomizedPlayerTiles[i]].GetComponent<Tiles.TileType>().GenerateType(desiredType);

                else
                    opponentTiles[_randomizedOpponentTiles[i]].GetComponent<Tiles.TileType>().GenerateType(desiredType);

                yield return new WaitForSeconds(waitTime);
            }
        }

        /// <summary>
        /// Clears visuals displays on the board
        /// </summary>
        /// <param name="onlyVisuals"></param>
        public void ClearBoardDisplay(bool onlyVisuals)
        {
            for (int i = 0; i < playerTiles.Length; i++)
            {
                playerTiles[i].GetComponentInChildren<Outlinable>().enabled = false;

                if (!onlyVisuals)
                    playerTiles[i].GetComponent<GridTile>().m_IsTileWalkable = false;
            }

            for (int i = 0; i < opponentTiles.Length; i++)
            {
                opponentTiles[i].GetComponentInChildren<Outlinable>().enabled = false;

                if (!onlyVisuals)
                    opponentTiles[i].GetComponent<GridTile>().m_IsTileWalkable = false;
            }
        }

        /// <summary>
        /// Sets movement costs on UI tooltip
        /// </summary>
        private void UpdateTileCostOnUI()
        {
            tooltip.GetComponent<TileTypeTooltip>().UpdateCosts(_plainCost, _forestCost, _hillCost, _mountainCost, _swampCost, _islandCost);
        }

        /// <summary>
        /// Displays a tooltip of the desired tile type
        /// </summary>
        /// <param name="desiredType"></param>
        /// <param name="state"></param>
        public void ShowTooltip(TileType desiredType, bool state)
        {
            Vector3 desiredOffset = new Vector3(_typeTooltipOffset.x, _typeTooltipOffset.y, 0f);

            if (Input.mousePosition.x > Screen.width / 2)
                desiredOffset = new Vector3(-desiredOffset.x, desiredOffset.y, desiredOffset.z);

            if (Input.mousePosition.y > Screen.height / 2)
                desiredOffset = new Vector3(desiredOffset.x, -desiredOffset.y, desiredOffset.z);

            tooltip.transform.position = Input.mousePosition + desiredOffset;
            tooltip.SetActive(state);
            tooltip.GetComponent<TileTypeTooltip>().UpdateTooltip(desiredType);
        }

        /// <summary>
        /// Resets the animator of all the tiles on the board
        /// </summary>
        private void ClearBoardTiles()
        {
            for (int i = 0; i < playerTiles.Length; i++)
            {
                playerTiles[i].GetComponent<Animator>().SetTrigger("Respawn");
            }

            for (int i = 0; i < opponentTiles.Length; i++)
            {
                opponentTiles[i].GetComponent<Animator>().SetTrigger("Respawn");
            }
        }

        private void ResetTimingValues()
        {
            _timeBetweenTypes = 0.5f;
            _timeBetweenTiles = 0.015f;
        }
    }
}