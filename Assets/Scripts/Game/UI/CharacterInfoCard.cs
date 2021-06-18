using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

using Game.Data;
using Game.Managers;

namespace Game.UI
{
    public class CharacterInfoCard : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private Card _playerCard;
        [SerializeField] private Card _opponentCard;

        [HideInInspector] public bool _showingPlayerCard;
        [HideInInspector] public bool _showingOpponentCard;

        private void Start()
        {
            HidePlayerCard();
            HideOpponentCard();
        }

        /// <summary>
        /// Displays a new card in the UI
        /// </summary>
        /// <param name="targetCharacter"></param>
        /// <param name="targetPlayer"></param>
        public void ShowCharacterCard(MainData.CharacterName targetCharacter, MainData.Players targetPlayer)
        {
            if (targetPlayer == MainData.Players.Player)
            {
                _playerCard.character = targetCharacter;
                _playerCard.gameObject.SetActive(true);
                _showingPlayerCard = true;
            }

            if (targetPlayer == MainData.Players.Opponent)
            {
                _opponentCard.character = targetCharacter;
                _opponentCard.gameObject.SetActive(true);
                _showingOpponentCard = true;
            }
        }

        /// <summary>
        /// Hides the current player card
        /// </summary>
        public void HidePlayerCard()
        {
            _showingPlayerCard = false;
            _playerCard.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides the current opponent card
        /// </summary>
        public void HideOpponentCard()
        {
            _showingOpponentCard = false;
            _opponentCard.gameObject.SetActive(false);
        }
    }
}