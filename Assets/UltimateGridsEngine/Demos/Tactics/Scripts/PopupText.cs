using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    [Header("Text Mesh")]
    public TextMeshPro m_TextMesh;

    [Header("Destroy Time")]
    public float m_LifeTime = .6f;

    [Header("Flash Text")]
    public bool m_FlashText = true;
    public Color m_FlashColor = Color.white;
    public float m_FlashStartDelay = 0f;
    public float m_FlashWaitTime = .1f;
    public float m_FlashDurationTime = .1f;

    private float _flashWaitTimer = 0f;
    private float _flashDurationTimer = 0f;
    private Color _originalTextColor;

    private void Awake()
    {
        // Try to find the text mesh pro component in case it hasn't being assigned
        if (m_TextMesh == null)
        {
            m_TextMesh = GetComponent<TextMeshPro>();
            if (m_TextMesh = null)
            {
                m_TextMesh = GetComponentInChildren<TextMeshPro>();
                if (m_TextMesh == null)
                {
                    Debug.Log("There is no TextMeshPro attached to this PopupText!");
                }
            }
        }

        // Store the original color
        _originalTextColor = m_TextMesh.color;

        // Life/Destroy timer
        Destroy(gameObject, m_LifeTime);
    }

    private void Start()
    {
        // Invoke the flash text color
        Invoke("FlashTextColor", m_FlashStartDelay);
    }

    private void Update()
    {
        FlashTextUpdate();
    }

    private void FlashTextUpdate()
    {
        // Flash duration time
        if (_flashDurationTimer > 0f)
        {
            // Wait time (if any)
            if (_flashWaitTimer > 0f)
            {
                _flashWaitTimer -= Time.deltaTime;
                return;
            }

            _flashDurationTimer -= Time.deltaTime;
            UpdateFlashTextColor();
            if (_flashDurationTimer <= 0f)
            {
                SetOriginalTextcolor();
            }
        }
    }

    public void SetText(string text)
    {
        m_TextMesh.SetText(text);
    }

    public void Setcolor(Color color)
    {
        m_TextMesh.color = color;
    }

    public void FlashTextColor()
    {
        _flashWaitTimer = m_FlashWaitTime;
        _flashDurationTimer = m_FlashDurationTime;
        UpdateFlashTextColor();
    }

    public void UpdateFlashTextColor()
    {
        var progress = _flashDurationTimer / m_FlashDurationTime;
        m_TextMesh.color = Color.Lerp(_originalTextColor, m_FlashColor, progress);
    }

    public void SetOriginalTextcolor()
    {
        m_TextMesh.color = _originalTextColor;
    }
}
