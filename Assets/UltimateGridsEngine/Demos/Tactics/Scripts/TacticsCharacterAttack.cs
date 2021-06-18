using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[RequireComponent(typeof(TacticsCharacter))]
public class TacticsCharacterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int m_AttackDamage = 7;
    public int m_ProjectilesPerAttack = 3;
    public RangeParameters m_AttackRangeParameters;
    [HideInInspector]
    public List<GridTile> m_currentAttackRange = new List<GridTile>();

    [Header("Attack Visual")]
    public LineRenderer m_LineRendererPrefab;
    public Transform m_GunBarrel;

    protected TacticsCharacter _tacticsCharacter;

    protected void Awake()
    {
        _tacticsCharacter = GetComponent<TacticsCharacter>();
    }

    public void CalculateAttackRange(bool andHighlight = false)
    {
        ClearAttackRange();
        m_currentAttackRange = RangeAlgorithms.SearchByParameters(_tacticsCharacter._gridObject.m_CurrentGridTile, m_AttackRangeParameters);
        if (andHighlight)
        {
            HighlightAttackRange();
        }
    }

    public void CalculateAttackRangeWithVictimsOnly(bool andHighlight, bool unhighlightPrevious = false)
    {
        ClearAttackRange();
        var range = RangeAlgorithms.SearchByParameters(_tacticsCharacter._gridObject.m_CurrentGridTile, m_AttackRangeParameters);

        foreach (GridTile tile in range)
        {
            var victimAtPosition = GetAttackableUnitAtTile(tile);
            if (victimAtPosition != null)
            {
                if (!m_currentAttackRange.Contains(tile))
                    m_currentAttackRange.Add(tile);
            }
        }

        if (andHighlight)
        {
            HighlightAttackRange(unhighlightPrevious);
        }
    }

    public virtual void HighlightAttackRange(bool unhighlightPrevious = false)
    {
        if (m_currentAttackRange != null && m_currentAttackRange.Count > 0)
        {
            HighlightManager.Instance.HighlighTiles(m_currentAttackRange, 1, unhighlightPrevious);
        }
    }

    public bool CanAttackTile(GridTile targetTile)
    {
        return (m_currentAttackRange.Contains(targetTile) && IsAttackableUnitAtTile(targetTile));
    }

    public bool IsAttackableUnitAtTile(GridTile targetTile)
    {
        var objectAtPos = GridManager.Instance.GetGridObjectAtPosition(targetTile.m_GridPosition);
        if (objectAtPos == null)
            return false;
        var tactisCharacterComp = objectAtPos.GetComponent<TacticsCharacter>();
        return tactisCharacterComp != null ? CanAttackUnit(tactisCharacterComp) : false;
    }

    public TacticsCharacter GetAttackableUnitAtTile(GridTile targetTile)
    {
        var objectAtPos = GridManager.Instance.GetGridObjectAtPosition(targetTile.m_GridPosition);
        if (objectAtPos == null)
            return null;
        var tactisCharacterComp = objectAtPos.GetComponent<TacticsCharacter>();
        return tactisCharacterComp != null && CanAttackUnit(tactisCharacterComp) ? tactisCharacterComp : null;
    }

    public bool CanAttackUnit(TacticsCharacter targetUnit)
    {
        return targetUnit._health.CanBeAttackedBy(_tacticsCharacter._gridObject) && !TacticsGameManager.Instance.DoesUnitBelongToCurrentPlayer(targetUnit);
    }

    public void AttackUnit(TacticsCharacter targetUnit, Action onAttackEndCallback)
    {
        _tacticsCharacter._gridMovement.RotateTo(targetUnit._gridObject.m_GridPosition);
        ShootUnit(targetUnit, onAttackEndCallback);
    }

    private void ShootUnit(TacticsCharacter targetUnit, Action onShootComplete)
    {
        StartCoroutine(ShootUnitRoutine(targetUnit, onShootComplete));
    }

    private IEnumerator ShootUnitRoutine(TacticsCharacter targetUnit, Action onShootRoutineComplete)
    {
        // Time to wait for the initial rotation
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < m_ProjectilesPerAttack; i++)
        {
            if (targetUnit == null || targetUnit._health == null)
                break;

            LineRenderer lineRenderer = null;
            // Attack visual / Line Renderer
            if (m_LineRendererPrefab != null)
            {
                var startPosition = m_GunBarrel != null ? m_GunBarrel.position : transform.position;
                var targetPosition = targetUnit.transform.position + (Vector3.up * 0.5f);
                var relativePosition = targetPosition - startPosition;
                lineRenderer = Instantiate(m_LineRendererPrefab, startPosition, Quaternion.identity);
                Vector3[] pos = new Vector3[2] { startPosition, targetPosition - (relativePosition.normalized * 0.2f) };
                lineRenderer.SetPositions(pos);
            }
            // Apply damage and create the popup text
            DamageHealth(targetUnit._health);
            PopupTextManager.Instance.CreatePopupText(targetUnit.transform.position, m_AttackDamage.ToString(), Vector3.down * 0.15f + UnityEngine.Random.insideUnitSphere * 0.25f);
            // Wait 1 frame
            yield return null;
            // Camera Shake
            var cis = GetComponent<CinemachineImpulseSource>();
            if (cis != null)
                cis.GenerateImpulse(Vector3.one);
            // Wait time before deactivating the line renderere
            yield return new WaitForSeconds(0.04f);

            // Destroy the line renderere
            if (lineRenderer != null)
            {
                lineRenderer.gameObject.SetActive(false);
                Destroy(lineRenderer.gameObject, 0.1f);
            }

            if (i + 1 < m_ProjectilesPerAttack)
            {
                yield return new WaitForSeconds(0.18f);
            }
        }

        onShootRoutineComplete();
    }

    public void DamageHealth(Health targetHealth)
    {
        targetHealth.DamageHealth(_tacticsCharacter._gridObject, m_AttackDamage);
    }

    public void ClearAttackRange()
    {
        m_currentAttackRange.Clear();
    }
}
