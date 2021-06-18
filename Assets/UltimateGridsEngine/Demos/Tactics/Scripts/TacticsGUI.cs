using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticsGUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button m_AttackButton, m_EndTurnButton;
    [Header("Current PlayerName")]
    public Text m_CurrentPlayerNameText;
    [Header("GameOver Screen")]
    public GameObject m_GameOverScreenHolder;
    public Text m_GameOverWinnerText;

    private void OnEnable()
    {
        TacticsGameManager.onUnitSelectedEvent += OnUnitSelected;
        TacticsGameManager.onUnitDeSelectedEvent += OnUnitDeSelected;
        TacticsGameManager.onGameStartEvent += OnGameStart;
        TacticsGameManager.onGameOverEvent += OnGameOver;
        TacticsGameManager.onNewTurnEvent += OnNewTurn;
    }
    private void OnDisable()
    {
        TacticsGameManager.onUnitSelectedEvent -= OnUnitSelected;
        TacticsGameManager.onUnitDeSelectedEvent -= OnUnitDeSelected;
        TacticsGameManager.onGameStartEvent -= OnGameStart;
        TacticsGameManager.onGameOverEvent -= OnGameOver;
        TacticsGameManager.onNewTurnEvent -= OnNewTurn;
    }

    private void OnUnitSelected(TacticsCharacter unit)
    {
        // Attack Button
        /*
        if (unit.CanAttack() && !m_AttackButton.IsActive())
        {
            m_AttackButton.gameObject.SetActive(true);
        }
        else if (!unit.CanAttack() && m_AttackButton.IsActive() && m_AttackButton.interactable)
        {
            m_AttackButton.interactable = false;
        }
        */
    }

    private void OnUnitDeSelected()
    {
        if (m_AttackButton.IsActive())
        {
            m_AttackButton.gameObject.SetActive(false);
        }
    }

    private void OnGameStart()
    {
        m_AttackButton.gameObject.SetActive(false);
        m_EndTurnButton.gameObject.SetActive(false);
    }

    private void OnNewTurn(TacticsPlayer player)
    {
        m_EndTurnButton.gameObject.SetActive(true);
        m_CurrentPlayerNameText.text = player.m_PlayerName;
    }

    private void OnGameOver(TacticsPlayer winner)
    {
        m_GameOverScreenHolder.SetActive(true);
        var text = winner.m_PlayerName + " wins!";
        m_GameOverWinnerText.text = text.ToUpper();
    }
}
