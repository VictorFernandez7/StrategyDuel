using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

using Game.Data;
using Game.Characters.Utility;

namespace Game.Data.Pools
{
    public class CardCharacter : MonoBehaviour
    {
        [Title("Parameters")]
        [SerializeField] private MainData.Players _displayedPlayer;
        [SerializeField] private MainData.CharacterName _displayedCharacter;

        [Title("Camera")]
        [SerializeField] private Transform _dismountedPos;
        [SerializeField] private Transform _mountedPos;

        [Title("References")]
        [SerializeField] private List<CharacterModel> _characterModels = new List<CharacterModel>();
        [SerializeField] private Camera _characterCamera;

        public void UpdateDisplayedCharacter(MainData.Players desiredPlayer, MainData.CharacterName desiredCharacter, RenderTexture desiredRenderTexture)
        {
            _displayedPlayer = desiredPlayer;
            _displayedCharacter = desiredCharacter;

            for (int i = 0; i < _characterModels.Count; i++)
            {
                if (_characterModels[i].characterModel == desiredCharacter)
                {
                    _characterModels[i].SetPlayableCharacter(desiredPlayer);
                    _characterCamera.targetTexture = desiredRenderTexture;
                }

                else
                    _characterModels[i].ClearModels();
            }

            SetCameraPos(desiredCharacter);
        }

        private void SetCameraPos(MainData.CharacterName desiredCharacter)
        {
            if (desiredCharacter == MainData.CharacterName.HeavyCavalry || desiredCharacter == MainData.CharacterName.LightCavalry || desiredCharacter == MainData.CharacterName.MountedKing || desiredCharacter == MainData.CharacterName.MountedKnight || desiredCharacter == MainData.CharacterName.MountedMage || desiredCharacter == MainData.CharacterName.MountedPaladin || desiredCharacter == MainData.CharacterName.MountedPriest || desiredCharacter == MainData.CharacterName.MountedScout)
            {
                _characterCamera.transform.position = _mountedPos.position;
                _characterCamera.transform.localRotation = _mountedPos.localRotation;
            }

            else
            {
                _characterCamera.transform.position = _dismountedPos.position;
                _characterCamera.transform.localRotation = _dismountedPos.localRotation;
            }
        }
    }
}