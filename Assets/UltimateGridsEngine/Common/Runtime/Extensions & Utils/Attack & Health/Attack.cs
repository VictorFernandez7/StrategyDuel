using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public delegate void AttackStartedHandler(Attack attack);
    public delegate void AttackFinishedHandler(Attack attack);

    public event AttackStartedHandler OnAttackStarted;
    public event AttackFinishedHandler OnAttackFinished;

    [HideInInspector]
    public HashSet<AttackTrigger> m_AttackTriggers = new HashSet<AttackTrigger>();

    [Header("Settings")]
    public int m_Damage = 1;
    public bool m_IgnoresInvicibility;
    public bool m_InProgress { get; private set; }
    public bool m_CanAttack => !m_InProgress && !m_Cooldown.InProgress;

    [Header("CoolDown")]
    [SerializeField] protected Cooldown _cooldown;
    public Cooldown m_Cooldown { get { return _cooldown; } }
    public bool m_CausesMovementCooldown = true;

    [Header("Delay")]
    public bool m_DelayBeforeDamaging = false;
    public float m_DelayTime = 0f;

    [Header("Animation")]
    public Animator m_Animator;
    public string m_AttackAnimationTriggerName = "Attack";

    [Header("References")]
    public GridObject m_GridObject;
    public GridMovement m_GridMovement;
    public Health m_OwnerHealth;

    protected virtual void Reset()
    {
        Awake();
    }

    protected virtual void Awake()
    {
        if (m_GridObject == null)
        {
            m_GridObject = GetComponent<GridObject>();
            if (m_GridObject == null)
            {
                m_GridObject = GetComponentInParent<GridObject>();
            }
        }
        if (m_GridMovement == null)
        {
            m_GridMovement = GetComponent<GridMovement>();
            if (m_GridMovement == null)
            {
                m_GridMovement = GetComponentInParent<GridMovement>();
            }
        }
        if (m_OwnerHealth == null)
        {
            m_OwnerHealth = GetComponent<Health>();
            if (m_OwnerHealth == null)
            {
                m_OwnerHealth = GetComponentInParent<Health>();
            }
        }

        // Finds and adds the attack triggers for this attack
        AttackTrigger[] componentsInChildren = GetComponentsInChildren<AttackTrigger>();
        foreach (AttackTrigger item in componentsInChildren)
        {
            if (!m_AttackTriggers.Contains(item))
            {
                m_AttackTriggers.Add(item);
                item.m_Attack = this;
            }
        }
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        _cooldown.Update();
    }

    public virtual AttackResult TryAttack(Vector2Int? targetPosition = null, bool triggersCooldown = true)
    {

        // The failed attack result will be used later when the attack system is expanded
        AttackResult attackResult = AttackResult.Failed;

        // Check if the attack is already in progress and if it is on cooldown
        if (!m_CanAttack)
        {
            return AttackResult.Cooldown;
        }

        // Triggers the movement cooldown
        if (m_CausesMovementCooldown && m_GridObject && m_GridMovement)
        {
            m_GridMovement.Cooldown.Reset(null);
        }

        // Trigger the cooldown
        if (triggersCooldown)
        {
            ResetCooldown();
        }

        DoAttack(targetPosition.HasValue ? targetPosition.Value : targetPosition);
        attackResult = AttackResult.Success;
        return attackResult;
    }

    public virtual void DoAttack(Vector2Int? targetPosition = null)
    {
        // Start the attack
        AttackStarted();

        // Get the victim(s) list from trigger(s)
        var victims = targetPosition.HasValue ? GetVictimsFromTriggerAtPosition(targetPosition.Value) : GetVictimsFromTriggers();
        // Damage the health(s)
        foreach (Health item in victims)
        {
            if (m_DelayBeforeDamaging)
            {
                StartCoroutine(DamageHealth(item, m_DelayTime));
            }
            else
            {
                DamageHealth(item);
            }
        }

        // End the attack
        AttackFinished();
    }

    public virtual List<Health> GetVictimsFromTriggerAtPosition(Vector2Int position)
    {
        var healths = new List<Health>();

        if (m_AttackTriggers.Count > 0)
        {
            foreach (AttackTrigger attackTrigger in m_AttackTriggers)
            {
                if (m_GridObject)
                {
                    attackTrigger.TrySetAttackDirection(m_GridObject.m_FacingDirection);
                }

                if (attackTrigger.HasVictimAtPosition(position))
                {
                    foreach (Health item in attackTrigger.GetVictims())
                    {
                        if (item.CanBeAttackedBy(this))
                        {
                            healths.Add(item);
                        }
                    }
                }
            }
        }

        return healths;
    }

    public virtual List<Health> GetVictimsFromTriggers()
    {
        var healths = new List<Health>();

        if (m_AttackTriggers.Count > 0)
        {
            foreach (AttackTrigger attackTrigger in m_AttackTriggers)
            {
                if (m_GridObject)
                {
                    attackTrigger.TrySetAttackDirection(m_GridObject.m_FacingDirection);
                }
                foreach (Health item in attackTrigger.GetVictims())
                {
                    if (item.CanBeAttackedBy(this))
                    {
                        healths.Add(item);
                    }
                }
            }
        }

        return healths;
    }

    public virtual List<Vector2Int> GetAllTargetPositions()
    {
        var positions = new List<Vector2Int>();

        if (m_AttackTriggers.Count > 0)
        {
            foreach (AttackTrigger attackTrigger in m_AttackTriggers)
            {
                if (m_GridObject)
                {
                    attackTrigger.TrySetAttackDirection(m_GridObject.m_FacingDirection);
                }
                var tempPositions = attackTrigger.GetTargetPositions();
                if (tempPositions.Count > 0)
                {
                    positions.AddRange(tempPositions);
                }
            }
        }

        return positions;
    }

    public virtual List<GridTile> GetAllTargetTiles()
    {
        var positions = GetAllTargetPositions();
        var tiles = new List<GridTile>();

        foreach (Vector2Int pos in positions)
        {
            var tileAtPosition = GridManager.Instance.GetGridTileAtPosition(pos);
            if (tileAtPosition && !tiles.Contains(tileAtPosition))
            {
                tiles.Add(tileAtPosition);
            }
        }

        return tiles;
    }

    public virtual void DamageHealth(Health health)
    {
        health.DamageHealth(this, m_Damage, m_IgnoresInvicibility);
    }

    public virtual IEnumerator DamageHealth(Health health, float delay)
    {
        yield return new WaitForSeconds(delay);
        DamageHealth(health);
    }

    protected virtual void AttackStarted()
    {
        if (this.OnAttackStarted != null)
        {
            this.OnAttackStarted(this);
        }
        m_InProgress = true;

        if (m_Animator != null)
        {
            m_Animator.SetTriggerIfExists(m_AttackAnimationTriggerName);
        }
    }

    protected virtual void AttackFinished()
    {
        if (this.OnAttackFinished != null)
        {
            this.OnAttackFinished(this);
        }
        m_InProgress = false;
    }

    public void ResetCooldown(float? duration = default(float?))
    {
        m_Cooldown.Reset(duration);
    }
}
