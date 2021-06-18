using System;
using UnityEngine;

[Serializable]
public class Cooldown {
    public delegate void CooldownEndHandler();
    public delegate void CooldownStartHandler();

    [Range(0.05f, 10f)]
    public float DefaultDuration;
    public bool AutomaticTicking = true;

    public Cooldown.CooldownEndHandler onCooldownEnded;
    public Cooldown.CooldownStartHandler onCooldownStarted;

    public float Time {
        get;
        private set;
    }

    public bool InProgress {
        get;
        private set;
    }

    public Cooldown() {
        DefaultDuration = 0f;
    }

    public Cooldown(float defaultDuration) {
        DefaultDuration = defaultDuration;
    }

    public void Reset(float? duration = null) {
        SetTime((!duration.HasValue) ? DefaultDuration : duration.Value);
    }

    public void Update() {
        if (Time > 0f && AutomaticTicking) {
            SetTime(Time - UnityEngine.Time.deltaTime);
        }
    }

    public void Tick(float amount) {
        SetTime(Time - amount);
    }

    public void Cancel() {
        Time = 0f;
        InProgress = false;
    }

    private void StartCooldown() {
        InProgress = true;
        if (onCooldownStarted != null) {
            onCooldownStarted();
        }
    }

    private void SetTime(float time) {
        Time = time;
        if (InProgress && time <= 0f) {
            EndCooldown();
        } else if (!InProgress && time > 0f) {
            StartCooldown();
        }
    }

    private void EndCooldown() {
        InProgress = false;
        if (Time > 0f) {
            Time = 0f;
        }
        if (onCooldownEnded != null) {
            onCooldownEnded();
        }
    }
}
