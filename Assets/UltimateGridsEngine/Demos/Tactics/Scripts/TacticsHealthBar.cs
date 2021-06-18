using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticsHealthBar : MonoBehaviour
{
    [Header("Slider")]
    public Slider m_SliderBar;

    [Header("Health Reference")]
    public Health m_Health;

    [Header("Transform to Follow")]
    public Transform m_TransformToFollow;

    [Header("Position Offset")]
    public Vector3 m_PositionOffset;

    [Header("Update Every Frame")]
    public bool m_UpdateEveryFrame = false;

    protected float _intervalTimeLeft;
    private Vector3 _lastFollowPosition;

    private void Reset()
    {
        if (m_Health == null)
        {
            m_Health = GetComponentInParent<Health>();
        }

        if (m_TransformToFollow == null)
        {
            m_TransformToFollow = transform.parent;
        }
    }

    private void Awake()
    {
        if (m_Health == null || m_TransformToFollow == null)
            Reset();

        UpdateBarPosition();
    }

    private void Update()
    {
        if (m_TransformToFollow.position != _lastFollowPosition || m_UpdateEveryFrame)
        {
            UpdateBarPosition();
        }
    }

    private void UpdateBarPosition()
    {
        Vector2 followScreenPosition = Camera.main.WorldToScreenPoint(m_TransformToFollow.position + m_PositionOffset);
        var targetPosition = followScreenPosition;
        m_SliderBar.transform.position = targetPosition;
        _lastFollowPosition = m_TransformToFollow.position;
    }

    private void OnEnable()
    {
        m_Health.onHealthChangedEvent += OnHealthChanged;
    }
    private void OnDisable()
    {
        m_Health.onHealthChangedEvent -= OnHealthChanged;
    }

    private void OnHealthChanged(Health health, GridObject attacker, int healthChangedAmount)
    {
        // Calculate the slider value
        var value = Mathf.Clamp01((float)health.m_CurrentHealth / (float)health.m_MaxHealth);
        // Set the value on the slider
        m_SliderBar.value = value;
    }
}
