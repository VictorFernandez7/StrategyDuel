using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using EPOOutline;

using Game.Tiles;
using Game.Managers;

namespace Game.Characters
{
    public class CharacterSelection : MonoBehaviour
    {
        [Title("Selection info")]
        [SerializeField] private bool _selected;

        [Title("References")]
        [SerializeField] private Animator _canvasAnim;

        public bool selected => _selected;

        private Outlinable _outlinable;
        private CharacterBasicInfo _characterInfo;
        private CharacterMovement _characterMovement;
        private CharacterAttack _characterAttack;

        private void Awake()
        {
            _outlinable = GetComponentInChildren<Outlinable>();
            _characterInfo = GetComponent<CharacterBasicInfo>();
            _characterAttack = GetComponent<CharacterAttack>();
            _characterMovement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            GameManager.instance.onEndTurn.AddListener(DeselectCharacter);
        }

        /// <summary>
        /// Selection and hover management
        /// </summary>
        private void OnMouseOver()
        {
            if (GameManager.instance._playing)
            {
                if (GameManager.instance.currentTurn == Data.MainData.Players.Player) // During players turn we can select players characters
                {
                    if (Input.GetMouseButtonDown(0) && _characterInfo.player == Data.MainData.Players.Player)
                        SelectCharacter(!_selected);
                }

                else if (GameManager.instance.currentTurn == Data.MainData.Players.Opponent) // During opponents turn we can se players character info
                {
                    if (_characterInfo.player == Data.MainData.Players.Player && !GameManager.instance.characterInfoCard._showingPlayerCard)
                    {
                        GameManager.instance.characterInfoCard.ShowCharacterCard(_characterInfo.character, _characterInfo.player);
                        _canvasAnim.SetBool("Hover", true);
                    }
                }

                if (_characterInfo.player == Data.MainData.Players.Opponent && !GameManager.instance.characterInfoCard._showingOpponentCard) // During both turns we can se opponents character info
                    GameManager.instance.characterInfoCard.ShowCharacterCard(_characterInfo.character, _characterInfo.player);

                TileManager.instance.tileSelectionLine.enabled = true;
                TileManager.instance.tileSelectionLine.SetPositions(GetComponent<GridObject>().m_CurrentGridTile.GetComponent<TileInfo>().GetEdges());
                GetComponent<GridObject>().m_CurrentGridTile.GetComponent<TileType>().mouseOver = true;
            }
        }

        /// <summary>
        /// Hover end management
        /// </summary>
        private void OnMouseExit()
        {
            if (_characterInfo.player == Data.MainData.Players.Opponent)
                GameManager.instance.characterInfoCard.HideOpponentCard();

            if (_characterInfo.player == Data.MainData.Players.Player && GameManager.instance.currentTurn == Data.MainData.Players.Opponent)
            {
                GameManager.instance.characterInfoCard.HidePlayerCard();
                _canvasAnim.SetBool("Hover", false);
            }
        }

        /// <summary>
        /// Selects or deselects this character
        /// </summary>
        /// <param name="state"></param>
        public void SelectCharacter(bool state)
        {
            GameManager.instance.DeselectAllCharacters();

            _selected = state;
            _outlinable.enabled = state;
            _canvasAnim.SetBool("Selected", state);

            if (_selected)
                GameManager.instance.characterInfoCard.ShowCharacterCard(_characterInfo.character, _characterInfo.player);

            else
                GameManager.instance.characterInfoCard.HidePlayerCard();
        }

        /// <summary>
        /// Deselects this character. For events and managers
        /// </summary>
        public void DeselectCharacter()
        {
            _selected = false;
            _outlinable.enabled = false;
            _canvasAnim.SetBool("Selected", false);
            GameManager.instance.characterInfoCard.HidePlayerCard();

            if (_characterAttack.attackSelected)
                _characterAttack.Attacking();

            if (_characterMovement.moveSelected)
                _characterMovement.Moving();
        }
    }
}