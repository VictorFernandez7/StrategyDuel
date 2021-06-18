using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using EPOOutline;

using Game.Managers;
using Game.UI.Utility;

namespace Game.Characters
{
    public class CharacterAttack : MonoBehaviour
    {
        [TitleGroup("Parameters")]
        [FoldoutGroup("Parameters/Gameplay")] [SerializeField] private int _attacks;
        [FoldoutGroup("Parameters/Gameplay")] [SerializeField] private int _attackPoints;
        [FoldoutGroup("Parameters/Gameplay")] [SerializeField] private int _attackRange;

        [Title("References")]
        [SerializeField] private Button _attackButton;

        public bool attackSelected => _attackSelected;

        private int _initialAttacks;
        private bool _attackSelected;
        private GridObject _gridObject;
        private CharacterBasicInfo _characterInfo;
        private CharacterSelection _characterSelection;
        private CharacterMovement _characterMovement;

        private void Awake()
        {
            _gridObject = GetComponent<GridObject>();
            _characterInfo = GetComponent<CharacterBasicInfo>();
            _characterSelection = GetComponent<CharacterSelection>();
            _characterMovement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            GetAttackParameters();

            GameManager.instance.onNewTurn.AddListener(RefreshAttacks);
        }

        private void Update()
        {
            if (_characterSelection.selected)
                _attackButton.GetComponentInChildren<SegmentedPB>().SetSlider(_attacks);
        }

        /// <summary>
        /// Triggered every time the character enters or exits attacking mode
        /// </summary>
        public void Attacking()
        {
            if (_characterMovement.moveSelected) // If character is in moving mode switch
                _characterMovement.Moving();

            _attackSelected = !_attackSelected;

            if (_attackSelected)
            {
                DisplayAttackRange(_attackRange, _gridObject.m_CurrentGridTile);
                GameManager.instance.onEndTurn.AddListener(Attacking);
                _attackButton.GetComponentInChildren<ParticleSystem>().Play();
            }

            else
            {
                TileManager.instance.ClearBoardDisplay(false);
                GameManager.instance.onEndTurn.RemoveListener(Attacking);
                _attackButton.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }

        /// <summary>
        /// Displays on the board the attack range of this character
        /// </summary>
        /// <param name="currentAttackRange"></param>
        /// <param name="currentTile"></param>
        private void DisplayAttackRange(int currentAttackRange, GridTile currentTile)
        {
            for (int i = 0; i < currentTile.m_manualNeighbors.Count; i++)
            {
                if (currentAttackRange >= 1)
                {
                    currentTile.m_manualNeighbors[i].GetComponentInChildren<Outlinable>().enabled = true;
                    currentTile.m_manualNeighbors[i].GetComponentInChildren<Outlinable>().FrontParameters.FillPass.SetColor("_PublicColor", TileManager.instance.attackColor);
                    DisplayAttackRange(currentAttackRange - 1, currentTile.m_manualNeighbors[i]);
                }
            }
        }

        /// <summary>
        /// Gain all the initial attacks at the start of the turn
        /// </summary>
        private void RefreshAttacks()
        {
            _attacks = _initialAttacks;
        }

        /// <summary>
        /// Gets all the attack related parameters of this character from the army scriptable object
        /// </summary>
        private void GetAttackParameters()
        {
            for (int i = 0; i < GameManager.instance.army.characters.Count; i++)
            {
                if (GameManager.instance.army.characters[i].name == _characterInfo.character)
                {
                    _attacks = GameManager.instance.army.characters[i].attacks;
                    _attackPoints = GameManager.instance.army.characters[i].attackPoints;
                    _attackRange = GameManager.instance.army.characters[i].attackRange;
                    RefreshAttacks();
                    break;
                }
            }
        }
    }
}