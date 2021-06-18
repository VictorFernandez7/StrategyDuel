using System.Collections.Generic;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using EPOOutline;

using Game.Tiles;
using Game.Managers;
using Game.UI.Utility;

namespace Game.Characters
{
    public class CharacterMovement : MonoBehaviour
    {
        [TitleGroup("Parameters")]
        [FoldoutGroup("Parameters/Gameplay")] [SerializeField] private int _currentMovementPoints;

        [Title("References")]
        [SerializeField] private Button _moveButton;

        public int currentMovementPoints => _currentMovementPoints;
        public bool moveSelected => _moveSelected;

        private int _initialMovementPoints;
        private bool _isMoving;
        private bool _moveSelected;
        private GridObject _gridObject;
        private CharacterBasicInfo _characterInfo;
        private CharacterAttack _characterAttack;
        private CharacterPathfinder _characterPathfinder;
        private CharacterSelection _characterSelection;


        private void Awake()
        {
            _characterPathfinder = GetComponent<CharacterPathfinder>();
            _characterSelection = GetComponent<CharacterSelection>();
            _characterAttack = GetComponent<CharacterAttack>();
            _characterInfo = GetComponent<CharacterBasicInfo>();
            _gridObject = GetComponent<GridObject>();
        }

        private void Start()
        {
            GetMovementParameters();

            GameManager.instance.onNewTurn.AddListener(RefreshMovementPoints);
        }

        private void Update()
        {
            Movement();
        }

        /// <summary>
        /// Triggered every time the character enters or exits moving mode
        /// </summary>
        public void Moving()
        {
            if (_characterAttack.attackSelected) // If character is in attacking mode switch
                _characterAttack.Attacking();

            _moveSelected = !_moveSelected;

            if (_moveSelected)
            {
                TileManager.instance.ClearBoardDisplay(false);
                DisplayMovementRange(currentMovementPoints, _gridObject.m_CurrentGridTile);
                _gridObject.m_CurrentGridTile.GetComponentInChildren<Outlinable>().enabled = false;
                GameManager.instance.onEndTurn.AddListener(Moving);
                _moveButton.GetComponentInChildren<ParticleSystem>().Play();
            }

            else
            {
                GameManager.instance.onEndTurn.RemoveListener(Moving);
                TileManager.instance.ClearBoardDisplay(false);
                _moveButton.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }

        /// <summary>
        /// Moving to tile management
        /// </summary>
        private void Movement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GridManager.Instance.m_HoveredGridTile != null && GridManager.Instance.m_HoveredGridTile.m_IsTileWalkable && _characterSelection.selected && !EventSystem.current.IsPointerOverGameObject())
                {
                    _characterPathfinder.SetNewDestination(GridManager.Instance.m_HoveredGridTile, true);
                    TileManager.instance.ClearBoardDisplay(true);
                    _isMoving = true;

                    for (int i = 0; i < _characterPathfinder.Path.Count; i++)
                    {
                        _currentMovementPoints -= _characterPathfinder.Path[i].GetComponent<TileInfo>().costToMoveHere;
                    }
                }
            }

            if (_isMoving && _characterPathfinder.Path.Count == 1)
            {
                DisplayMovementRange(_currentMovementPoints, _characterPathfinder.m_TargetGridTile);
                _isMoving = false;
            }

            if (_characterSelection.selected)
                _moveButton.GetComponentInChildren<SegmentedPB>().SetSlider(_currentMovementPoints);
        }

        /// <summary>
        /// Displays on the board the movement range of this character
        /// </summary>
        /// <param name="currentMovementPoints"></param>
        /// <param name="currentTile"></param>
        public void DisplayMovementRange(int currentMovementPoints, GridTile currentTile)
        {
            for (int i = 0; i < currentTile.m_manualNeighbors.Count; i++)
            {
                if (currentTile.m_manualNeighbors[i].GetComponent<TileInfo>().costToMoveHere <= currentMovementPoints && currentTile.m_manualNeighbors[i].m_OccupyingGridObjects.Count != 1)
                {
                    currentTile.m_manualNeighbors[i].GetComponentInChildren<Outlinable>().enabled = true;
                    currentTile.m_manualNeighbors[i].GetComponentInChildren<Outlinable>().FrontParameters.FillPass.SetColor("_PublicColor", TileManager.instance.moveColor);
                    currentTile.m_manualNeighbors[i].m_IsTileWalkable = true;
                    DisplayMovementRange(currentMovementPoints - currentTile.m_manualNeighbors[i].GetComponent<TileInfo>().costToMoveHere, currentTile.m_manualNeighbors[i]);
                }
            }
        }

        /// <summary>
        /// Gain all the initial movement points at the start of the turn
        /// </summary>
        private void RefreshMovementPoints()
        {
            _currentMovementPoints = _initialMovementPoints;
        }

        /// <summary>
        /// Gets all the movement related parameters of this character from the army scriptable object
        /// </summary>
        private void GetMovementParameters()
        {
            for (int i = 0; i < GameManager.instance.army.characters.Count; i++)
            {
                if (GameManager.instance.army.characters[i].name == _characterInfo.character)
                {
                    _initialMovementPoints = GameManager.instance.army.characters[i].movementPoints;
                    RefreshMovementPoints();
                    break;
                }
            }
        }
    }
}