using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using Utils.UI;
using Game.Data;
using Game.Characters;
using Utils.UI.Tooltips;
using Utils.Pools.Controllers;

namespace Game.Managers
{
    public class BuyManager : Utils.SingletonBehaviour<BuyManager>
    {
        [TitleGroup("Parameters")]
        [FoldoutGroup("Parameters/Gold")] [SerializeField] private int _gold;
        [FoldoutGroup("Parameters/Time")] [SerializeField] private float _buyingTime;

        [Title("Events")]
        [SerializeField] public UnityEvent onBuyBegin;
        [SerializeField] public UnityEvent onBuyEnd;

        [TitleGroup("References")]
        [FoldoutGroup("References/UI")] [SerializeField] private Slider _timeSlider;
        [FoldoutGroup("References/UI")] [SerializeField] private TextMeshProUGUI _goldText;
        [FoldoutGroup("References/UI")] [SerializeField] private Button _declareWarButton;
        [FoldoutGroup("References/Grid")] [SerializeField] private GameObject _gridObjects;
        [FoldoutGroup("References/Pools")] [SerializeField] private PlayableCharacterPoolController _playableCharacterPoolController;

        private bool _canPlace;
        private bool _isBuying;
        private bool _isPlacing;
        private GridTile _targetTile;
        private GameObject _targetCharacter;
        private MainData.CharacterName _desiredCharacter;

        #region Properties
        public bool canPlace
        {
            get => _canPlace;
            set { _canPlace = value; }
        }

        private int _currentGold
        {
            get => _gold;
            set
            {
                _gold = value;
                _goldText.text = value.ToString();
            }
        }

        private float _currentBuyingTime
        {
            get => _buyingTime;
            set
            {
                _buyingTime = value;
                _timeSlider.value = value;
            }
        }
        #endregion

        private void Start()
        {
            onBuyBegin.AddListener(SetUpBuyingUI);
        }

        private void Update()
        {
            TimeSliderProgress();
            CharacterPlacement();
        }

        /// <summary>
        /// Start of the buying phase. Called via UI
        /// </summary>
        public void StartBuying()
        {
            _isBuying = true;
            onBuyBegin.Invoke();
        }

        /// <summary>
        /// End of the buying phase. Called via UI or timer
        /// </summary>
        public void EndBuyPhase()
        {
            _isBuying = false;
            onBuyEnd.Invoke();
        }

        /// <summary>
        /// Managment of the time during the buy phase
        /// </summary>
        private void TimeSliderProgress()
        {
            if (_isBuying)
            {
                _currentBuyingTime -= Time.deltaTime;

                if (_currentBuyingTime <= 0)
                    EndBuyPhase();
            }
        }

        /// <summary>
        /// Called every frame for the player to place bought characters
        /// </summary>
        private void CharacterPlacement()
        {
            if (_isPlacing && GridManager.Instance.m_HoveredGridTile != null)
            {
                if (GridManager.Instance.m_HoveredGridTile.m_IsTileWalkable)
                {
                    _targetCharacter.transform.position = GridManager.Instance.m_HoveredGridTile.transform.position + (Vector3.up * 0.5f);

                    if (GridManager.Instance.m_HoveredGridTile.m_costOfMovingToTile >= 2)
                    {
                        _canPlace = false;
                        _targetCharacter.GetComponent<Characters.CharacterBasicInfo>().SetPlaceholderCharacter(_desiredCharacter, Characters.Utility.CharacterModel.ModelVariations.CantPlaceModel);
                    }

                    else
                    {
                        _canPlace = true;
                        _targetCharacter.GetComponent<Characters.CharacterBasicInfo>().SetPlaceholderCharacter(_desiredCharacter, Characters.Utility.CharacterModel.ModelVariations.CanPlaceModel);
                        _targetTile = GridManager.Instance.m_HoveredGridTile;
                    }
                }

                else
                {
                    _canPlace = false;
                    _targetCharacter.GetComponent<Characters.CharacterBasicInfo>().SetPlaceholderCharacter(_desiredCharacter, Characters.Utility.CharacterModel.ModelVariations.NoModel);
                }
            }
        }

        /// <summary>
        /// Initial UI value set up
        /// </summary>
        private void SetUpBuyingUI()
        {
            // Time Slider
            _timeSlider.maxValue = _buyingTime;

            // Gold
            _currentGold = _gold;
        }

        /// <summary>
        /// Gets the character reference form the army data
        /// </summary>
        /// <param name="desiredCharacter"></param>
        public void GetCharacter(MainData.CharacterName desiredCharacter)
        {
            _isPlacing = true;

            for (int i = 0; i < GameManager.instance.army.characters.Count; i++)
            {
                if (GameManager.instance.army.characters[i].name == desiredCharacter)
                {
                    _desiredCharacter = desiredCharacter;
                    _targetCharacter = _playableCharacterPoolController.RetrievePlayableCharacter().gameObject;
                    _targetCharacter.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Deletes the character reference player is placing
        /// </summary>
        public void DeleteCharacter()
        {
            _isPlacing = false;

            if (_targetCharacter != null)
            {
                _playableCharacterPoolController.DispatchPlayableCharacterInstance(_targetCharacter.GetComponent<CharacterBasicInfo>());
                _targetCharacter = null;
            }
        }

        /// <summary>
        /// Destroys the temporary prefab and instantiates another one inside the grid system
        /// </summary>
        /// <param name="desiredCharacter"></param>
        public void PlaceCharacter(MainData.CharacterName desiredCharacter)
        {
            if (_canPlace)
            {
                if (desiredCharacter == MainData.CharacterName.King)
                    UnlockNextRound();

                DeleteCharacter();

                for (int i = 0; i < GameManager.instance.army.characters.Count; i++)
                {
                    if (GameManager.instance.army.characters[i].name == desiredCharacter)
                    {
                        GridObject newGridObject = GridManager.Instance.InstantiateGridObject(GameManager.instance.army.playableCharacter.GetComponent<GridObject>(), _targetTile.m_GridPosition, Orientations.North, _gridObjects.transform, false);
                        newGridObject.GetComponent<Characters.CharacterBasicInfo>().SetPlayableCharacter(MainData.Players.Player, desiredCharacter);
                        GameManager.instance.playerCharacters.Add(newGridObject.gameObject);
                    }
                }
            }

            else
                DeleteCharacter();
        }

        /// <summary>
        /// Allows the player to skip to the next gameplay phase
        /// </summary>
        private void UnlockNextRound()
        {
            _declareWarButton.interactable = true;
            _declareWarButton.GetComponentInChildren<ButtonInteractableIcon>().SwitchIconStatus();
            _declareWarButton.GetComponent<TooltipTrigger>().canDisplayTooltip = false;
        }
    }
}