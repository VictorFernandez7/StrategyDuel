using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using Game.UI;
using Game.Data;
using Utils.UI;
using Game.Characters;

namespace Game.Managers
{
    public class GameManager : Utils.SingletonBehaviour<GameManager>
    {
        [Title("Current Round")]
        [SerializeField] [HideLabel] private int _currentRound;

        [Title("Current Turn")]
        [SerializeField] [HideLabel] [EnumToggleButtons] private MainData.Players _currentTurn;

        [TitleGroup("Parameters")]
        [FoldoutGroup("Parameters/Turns")] [SerializeField] private float _turnTime;

        [TitleGroup("References")]
        [FoldoutGroup("References/UI")] [SerializeField] private CharacterInfoCard _characterInfoCard;
        [FoldoutGroup("References/UI")] [SerializeField] private Animator _turnAnim;
        [FoldoutGroup("References/UI")] [SerializeField] private Animator _roundAnim;
        [FoldoutGroup("References/UI")] [SerializeField] private TextMeshProUGUI _roundTitle;
        [FoldoutGroup("References/UI")] [SerializeField] private Button _nextTurnButton;
        [FoldoutGroup("References/UI")] [SerializeField] private Slider _timeSlider;
        [FoldoutGroup("References/Data")] [SerializeField] private ArmyScriptableObject _army;
        [FoldoutGroup("References/Characters")] [SerializeField] private List<GameObject> _playerCharacters = new List<GameObject>();

        // PUBLIC PROPERTIES
        public CharacterInfoCard characterInfoCard => _characterInfoCard;
        public MainData.Players currentTurn => _currentTurn;
        public ArmyScriptableObject army => _army;
        public List<GameObject> playerCharacters
        {
            get => _playerCharacters;
            set { _playerCharacters = value; }
        }

        // PUBLIC VARIABLES
        [HideInInspector] public UnityEvent onNewTurn;
        [HideInInspector] public UnityEvent onEndTurn;
        [HideInInspector] public bool _playing;

        // PRIVATE PROPERTIES
        private float _currentTurnTime
        {
            get => _turnTime;
            set
            {
                _turnTime = value;
                _timeSlider.value = value;
            }
        }

        // PRIVATE VARIABLES
        private float _savedTurnTime;

        private void Start()
        {
            _savedTurnTime = _turnTime;

            SetInitialUI();
        }

        private void Update()
        {
            TurnTime();
        }

        /// <summary>
        /// Managment of the time during the game phase
        /// </summary>
        private void TurnTime()
        {
            if (_playing)
            {
                _currentTurnTime -= Time.deltaTime;

                if (_currentTurnTime <= 0)
                    NewTurn();
            }
        }

        /// <summary>
        /// Initial UI value set up
        /// </summary>
        private void SetInitialUI()
        {
            // Next turn button
            _nextTurnButton.interactable = false;
            _nextTurnButton.GetComponentInChildren<ButtonInteractableIcon>().SwitchIconStatus();

            // Time slider
            _timeSlider.maxValue = _turnTime;
        }

        /// <summary>
        /// Triggers a new turn. Called via UI and turn timer
        /// </summary>
        public void NewTurn()
        {
            StartCoroutine(NewTurnProcess());
        }

        /// <summary>
        /// Triggers a process of events htat end at the beginning of a new turn
        /// </summary>
        /// <returns></returns>
        private IEnumerator NewTurnProcess()
        {
            _playing = false;
            _nextTurnButton.interactable = false;
            _nextTurnButton.GetComponentInChildren<ButtonInteractableIcon>().SwitchIconStatus();

            _currentTurnTime = _savedTurnTime;

            if (_currentTurn == MainData.Players.Player)
            {
                _currentTurn = MainData.Players.Opponent;
                _turnAnim.SetTrigger("OpponentTurn");
                onEndTurn.Invoke();
            }

            else if (_currentTurn == MainData.Players.Opponent)
            {
                _currentTurn = MainData.Players.Player;
                _turnAnim.SetTrigger("PlayerTurn");
                onNewTurn.Invoke();
            }

            TileManager.instance.ClearBoardDisplay(false);

            yield return new WaitForSeconds(2f); // Anim duration

            _playing = true;
            _nextTurnButton.interactable = true;
            _nextTurnButton.GetComponentInChildren<ButtonInteractableIcon>().SwitchIconStatus();
        }

        /// <summary>
        /// Trigger a new round. Called via UI and via king death (PENDING)
        /// </summary>
        public void NewRound()
        {
            StartCoroutine(NewRoundProcess());
        }

        /// <summary>
        /// Triggers a process of events htat end at the beginning of a new round
        /// </summary>
        /// <returns></returns>
        private IEnumerator NewRoundProcess()
        {
            _playing = false;

            _currentRound++;
            _roundTitle.text = _currentRound.ToString();
            _roundAnim.SetTrigger("NewRound");

            // TODO if round != 1 { Detect player who lost for him to start first next round

            yield return new WaitForSeconds(3f); // Anim duration

            NewTurn();
        }

        /// <summary>
        /// Deselects all player characters
        /// </summary>
        public void DeselectAllCharacters()
        {
            for (int i = 0; i < playerCharacters.Count; i++)
            {
                playerCharacters[i].GetComponent<CharacterSelection>().DeselectCharacter();
            }
        }
    }
}