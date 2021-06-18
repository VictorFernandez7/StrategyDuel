using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

using Game.Data;
using Game.Characters.Utility;

namespace Game.Characters
{
    public class CharacterBasicInfo : MonoBehaviour
    {
        [Title("Character")]
        [SerializeField] [HideLabel] private MainData.CharacterName _character;
        [SerializeField] [EnumToggleButtons] [HideLabel] private MainData.Players _player;

        [Title("References")]
        [SerializeField] private List<CharacterModel> _characterModels = new List<CharacterModel>();

        public MainData.CharacterName character => _character;
        public MainData.Players player => _player;
        public List<CharacterModel> characterModels => _characterModels;

        /// <summary>
        /// Sets the correct playable model for a specific player and character type
        /// </summary>
        public void SetPlayableCharacter(MainData.Players desiredPlayer, MainData.CharacterName desiredCharacter)
        {
            _character = desiredCharacter;
            _player = desiredPlayer;

            for (int i = 0; i < _characterModels.Count; i++)
            {
                if (_characterModels[i].characterModel == desiredCharacter)
                    _characterModels[i].SetPlayableCharacter(desiredPlayer);
            }
        }

        /// <summary>
        /// Sets the correct placeholder model for a specific character type
        /// </summary>
        public void SetPlaceholderCharacter(MainData.CharacterName desiredCharacter, CharacterModel.ModelVariations desiredVariation)
        {
            for (int i = 0; i < _characterModels.Count; i++)
            {
                if (_characterModels[i].characterModel == desiredCharacter)
                    _characterModels[i].SetPlaceholderCharacter(desiredVariation);
            }
        }
    }
}