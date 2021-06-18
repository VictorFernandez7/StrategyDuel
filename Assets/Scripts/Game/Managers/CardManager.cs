using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

using Game.Data;
using Game.Data.Pools;
using Utils.Pools.Controllers;

namespace Game.Managers
{
    public class CardManager : Utils.SingletonBehaviour<CardManager>
    {
        [TitleGroup("Player Data")]
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private List<MainData.CharacterName> _currentPlayerCardCharacters = new List<MainData.CharacterName>();
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private List<RuntimeRenderTexture> _playerRenderTextures = new List<RuntimeRenderTexture>();
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private List<CardCharacter> _playerCardCharacters = new List<CardCharacter>();

        [TitleGroup("Opponent Data")]
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private List<MainData.CharacterName> _currentOpponentCardCharacters = new List<MainData.CharacterName>();
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private List<RuntimeRenderTexture> _opponentRenderTextures = new List<RuntimeRenderTexture>();
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private List<CardCharacter> _opponentCardCharacters = new List<CardCharacter>();

        [Title("Pools")]
        [SerializeField] private CardCharacterPoolController _cardCharacterPoolController;
        [SerializeField] private RenderTexturePoolController _renderTexturePoolController;

        /// <summary>
        /// Creates a new card character along with a unique render texture
        /// </summary>
        /// <param name="desiredCharacter"></param>
        /// <param name="desiredPlayer"></param>
        public void CreateNewCharacter(MainData.CharacterName desiredCharacter, MainData.Players desiredPlayer)
        {
            if (desiredPlayer == MainData.Players.Player)
            {
                // MAIN DATA
                _currentPlayerCardCharacters.Add(desiredCharacter);

                // RENDER TEXTURE
                RuntimeRenderTexture newRuntimeRenderTexture = _renderTexturePoolController.RetrieveRenderTextureInstance();
                newRuntimeRenderTexture.gameObject.SetActive(true);
                _playerRenderTextures.Add(newRuntimeRenderTexture);

                // CARD CHARACTER
                CardCharacter newCardCharacter = _cardCharacterPoolController.RetrieveCardCharacterInstance();
                newCardCharacter.UpdateDisplayedCharacter(desiredPlayer, desiredCharacter, newRuntimeRenderTexture.renderTexture);
                _playerCardCharacters.Add(newCardCharacter);
                newCardCharacter.transform.position = new Vector3(0f, 0f, _playerCardCharacters.Count * 50);
                newCardCharacter.gameObject.SetActive(true);
            }

            else if (desiredPlayer == MainData.Players.Opponent)
            {
                // MAIN DATA
                _currentOpponentCardCharacters.Add(desiredCharacter);

                // RENDER TEXTURE
                RuntimeRenderTexture newRuntimeRenderTexture = _renderTexturePoolController.RetrieveRenderTextureInstance();
                newRuntimeRenderTexture.gameObject.SetActive(true);
                _opponentRenderTextures.Add(newRuntimeRenderTexture);

                // CARD CHARACTER
                CardCharacter newCardCharacter = _cardCharacterPoolController.RetrieveCardCharacterInstance();
                newCardCharacter.UpdateDisplayedCharacter(desiredPlayer, desiredCharacter, newRuntimeRenderTexture.renderTexture);
                _opponentCardCharacters.Add(newCardCharacter);
                newCardCharacter.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Destroy an existing card character
        /// </summary>
        /// <param name="desiredCharacter"></param>
        /// <param name="desiredPlayer"></param>
        public void RemoveExistingCharacter(MainData.CharacterName desiredCharacter, MainData.Players desiredPlayer)
        {
            if (desiredPlayer == MainData.Players.Player)
            {
                for (int i = 0; i < _currentPlayerCardCharacters.Count; i++)
                {
                    if (_currentPlayerCardCharacters[i] == desiredCharacter)
                    {
                        _cardCharacterPoolController.DispatchCardCharacterInstance(_playerCardCharacters[i]);
                        _renderTexturePoolController.DispatchRenderTextureInstance(_playerRenderTextures[i]);

                        _currentPlayerCardCharacters.RemoveAt(i);
                        _playerCardCharacters.RemoveAt(i);
                        _playerRenderTextures.RemoveAt(i);
                    }
                }
            }

            else if (desiredPlayer == MainData.Players.Opponent)
            {
                for (int i = 0; i < _currentOpponentCardCharacters.Count; i++)
                {
                    if (_currentOpponentCardCharacters[i] == desiredCharacter)
                    {
                        _cardCharacterPoolController.DispatchCardCharacterInstance(_opponentCardCharacters[i]);
                        _renderTexturePoolController.DispatchRenderTextureInstance(_opponentRenderTextures[i]);

                        _currentOpponentCardCharacters.RemoveAt(i);
                        _opponentCardCharacters.RemoveAt(i);
                        _opponentRenderTextures.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the render texture of an existing card character
        /// </summary>
        /// <param name="desiredCharacter"></param>
        /// <param name="desiredPlayer"></param>
        /// <returns></returns>
        public RenderTexture GetRenderTexture(MainData.CharacterName desiredCharacter, MainData.Players desiredPlayer)
        {
            if (desiredPlayer == MainData.Players.Player)
            {
                for (int i = 0; i < _currentPlayerCardCharacters.Count; i++)
                {
                    if (_currentPlayerCardCharacters[i] == desiredCharacter)
                        return _playerRenderTextures[i].renderTexture;
                }
            }

            else if (desiredPlayer == MainData.Players.Opponent)
            {
                for (int i = 0; i < _currentOpponentCardCharacters.Count; i++)
                {
                    if (_currentOpponentCardCharacters[i] == desiredCharacter)
                        return _opponentRenderTextures[i].renderTexture;
                }
            }

            return null;
        }
    }
}