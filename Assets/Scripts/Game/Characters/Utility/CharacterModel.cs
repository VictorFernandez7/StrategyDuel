using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

using Game.Data;

namespace Game.Characters.Utility
{
    public class CharacterModel : MonoBehaviour
    {
        [Title("Parameters")]
        [SerializeField] private MainData.CharacterName _characterModel;

        [Title("References")]
        [SerializeField] private GameObject _playerModel;
        [SerializeField] private GameObject _opponentModel;
        [SerializeField] private GameObject _canPlaceModel;
        [SerializeField] private GameObject _cantPlaceModel;

        public MainData.CharacterName characterModel => _characterModel;

        public enum ModelVariations
        {
            CanPlaceModel,
            CantPlaceModel,
            NoModel
        }

        /// <summary>
        /// Sets the correct playable model for a specific player
        /// </summary>
        public void SetPlayableCharacter(MainData.Players desiredPlayer)
        {
            if (_canPlaceModel != null && _cantPlaceModel != null)
            {
                _canPlaceModel.SetActive(false);
                _cantPlaceModel.SetActive(false);
            }

            switch (desiredPlayer)
            {
                case MainData.Players.Player:
                    _playerModel.SetActive(true);
                    _opponentModel.SetActive(false);
                    break;
                case MainData.Players.Opponent:
                    _playerModel.SetActive(false);
                    _opponentModel.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// Sets the correct variation of the placeholder model
        /// </summary>
        public void SetPlaceholderCharacter(ModelVariations desiredVariation)
        {
            switch (desiredVariation)
            {
                case ModelVariations.CanPlaceModel:
                    _canPlaceModel.SetActive(true);
                    _cantPlaceModel.SetActive(false);
                    break;
                case ModelVariations.CantPlaceModel:
                    _canPlaceModel.SetActive(false);
                    _cantPlaceModel.SetActive(true);
                    break;
                case ModelVariations.NoModel:
                    _canPlaceModel.SetActive(false);
                    _cantPlaceModel.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// Clears all the model variations
        /// </summary>
        public void ClearModels()
        {
            _playerModel.SetActive(false);
            _opponentModel.SetActive(false);
        }
    }
}