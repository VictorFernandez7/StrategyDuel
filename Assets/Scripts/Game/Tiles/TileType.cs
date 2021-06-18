using System.Collections.Generic;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;

using Game.Managers;

namespace Game.Tiles
{
    public class TileType : MonoBehaviour
    {
        [Title("TileType")]
        [SerializeField] [EnumToggleButtons] [HideLabel] [DisableInPlayMode] private TileManager.TileType _tileType;

        [TitleGroup("Parameters")]
        [FoldoutGroup("Parameters/Visuals")] [SerializeField] [MinMaxSlider(0, 0.5f, true)] private Vector2 _randomHeight;

        [TitleGroup("References")]
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _plain;
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _forest;
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _hill;
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _mountain;
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _swamp;
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _island;
        [FoldoutGroup("References/Variations")] [SerializeField] private GameObject _water;
        [FoldoutGroup("References/Other")] [SerializeField] private GameObject _visuals;

        public TileManager.TileType tileType => _tileType;
        public bool mouseOver
        {
            get => _mouseOver;
            set { _mouseOver = value;}
        }

        [HideInInspector] public UnityEvent onTileSpawn;

        private bool _mouseOver;
        private bool _tooltipTimer;
        private float _randomRotation;
        private float _currentLineWidth;
        private float _targetLineWidth;
        private Vector3 _startingPos;
        private Animator _anim;
        private TileInfo _tileInfo;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _tileInfo = GetComponent<TileInfo>();
        }

        private void Start()
        {
            _startingPos = transform.position;
            _currentLineWidth = TileManager.instance.selectionLineMinWidth;
            _targetLineWidth = TileManager.instance.selectionLineMaxWidth;
        }

        private void Update()
        {
            if (_mouseOver)
                LineRendererAnimation();
        }

        private void OnMouseEnter()
        {
            if (!_tooltipTimer && GameManager.instance._playing && !EventSystem.current.IsPointerOverGameObject())
            {
                _mouseOver = true;
                StartCoroutine("ShowTooltip");
                TileManager.instance.tileSelectionLine.enabled = true;
                TileManager.instance.tileSelectionLine.SetPositions(_tileInfo.GetEdges());
            }
        }

        private void OnMouseExit()
        {
            StopCoroutine("ShowTooltip");
            TileManager.instance.ShowTooltip(_tileType, false);
            _tooltipTimer = false;
            TileManager.instance.tileSelectionLine.enabled = false;
            _mouseOver = false;
        }

        /// <summary>
        /// Resets the tile and set up a new type
        /// </summary>
        /// <param name="desiredType"></param>
        public void GenerateType(TileManager.TileType desiredType)
        {
            // Reset tile in case a borad remake happened
            _plain.SetActive(false);
            _forest.SetActive(false);
            _hill.SetActive(false);
            _mountain.SetActive(false);
            _swamp.SetActive(false);
            _island.SetActive(false);
            _water.SetActive(false);

            switch (desiredType)
            {
                case TileManager.TileType.Plain:
                    _plain.SetActive(true);
                    break;
                case TileManager.TileType.Forest:
                    _forest.SetActive(true);
                    break;
                case TileManager.TileType.Hill:
                    _hill.SetActive(true);
                    break;
                case TileManager.TileType.Mountain:
                    _mountain.SetActive(true);
                    break;
                case TileManager.TileType.Swamp:
                    _swamp.SetActive(true);
                    break;
                case TileManager.TileType.Island:
                    _island.SetActive(true);
                    break;
                case TileManager.TileType.Water:
                    _water.SetActive(true);
                    break;
            }

            _tileType = desiredType;
            RandomHeight();
            RandomRotation();
            _anim.SetTrigger("Spawn");

            onTileSpawn.Invoke();
        }

        /// <summary>
        /// Changes the rotation of the visual of the tile
        /// </summary>
        private void RandomRotation()
        {
            _randomRotation = Random.Range(0f, 120f);

            if (_randomRotation <= 20)
                _visuals.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

            else if (_randomRotation <= 40)
                _visuals.transform.localRotation = Quaternion.Euler(new Vector3(0f, 60f, 0f));

            else if (_randomRotation <= 60)
                _visuals.transform.localRotation = Quaternion.Euler(new Vector3(0f, 120f * 2, 0f));

            else if (_randomRotation <= 80)
                _visuals.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f * 3, 0f));

            else if (_randomRotation <= 100)
                _visuals.transform.localRotation = Quaternion.Euler(new Vector3(0f, 240f * 4, 0f));

            else if (_randomRotation <= 120)
                _visuals.transform.localRotation = Quaternion.Euler(new Vector3(0f, 300f * 5, 0f));
        }

        /// <summary>
        /// Changes the height of the tile
        /// </summary>
        private void RandomHeight()
        {
            transform.position = _startingPos; // Reset height in case there was a board remake

            if (_tileType != TileManager.TileType.Water && _tileType != TileManager.TileType.Island)
                transform.position = new Vector3(transform.position.x, Random.Range(_randomHeight.x, _randomHeight.y), transform.position.z);
        }

        /// <summary>
        /// Triggered via OnMouseEnter. Timer for the tooltip to be displayed
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowTooltip()
        {
            _tooltipTimer = true;
            yield return new WaitForSeconds(TileManager.instance.typeTooltipTimer);
            TileManager.instance.ShowTooltip(_tileType, true);
        }

        /// <summary>
        /// Line renderer width animation
        /// </summary>
        private void LineRendererAnimation()
        {
            _currentLineWidth = Mathf.Lerp(_currentLineWidth, _targetLineWidth, TileManager.instance.selectionLineSpeed * Time.deltaTime);
            TileManager.instance.tileSelectionLine.startWidth = _currentLineWidth;
            TileManager.instance.tileSelectionLine.endWidth = _currentLineWidth;

            if (_currentLineWidth >= TileManager.instance.selectionLineMaxWidth - 0.005f)
                _targetLineWidth = TileManager.instance.selectionLineMinWidth;

            if (_currentLineWidth <= TileManager.instance.selectionLineMinWidth + 0.005f)
                _targetLineWidth = TileManager.instance.selectionLineMaxWidth;
        }
    }
}