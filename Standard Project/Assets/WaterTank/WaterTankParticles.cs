using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTankParticles : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Animator m_Animator;
    [Space]
    [SerializeField] private ParticleSystem m_StreamPS;
    [SerializeField] private ParticleSystem m_SplashPS;
    [SerializeField] private ParticleSystem m_BubblesPS;
    [SerializeField] private ParticleSystem m_SprayPS;
    [Space]
    [SerializeField] private Collider m_OverrideCollision;
    
    [Header("Settings")]
    [SerializeField] private AnimationCurve m_PressureResponseCurve = AnimationCurve.Linear(0,0,2,2);
    [SerializeField] [Range(0,2)] private float m_Pressure = 0;

    private readonly int hTrigSpray = Animator.StringToHash("Spray");
    private readonly int hFloatPressure = Animator.StringToHash("Pressure");

    private void Start() {
        if (!m_OverrideCollision) return;
        OverrideTriggerCollider(m_StreamPS.trigger, m_OverrideCollision);
        OverrideTriggerCollider(m_BubblesPS.trigger, m_OverrideCollision);
    }

    public void SetPressure(float pressure) {
        m_Pressure = pressure;
        m_Animator.SetFloat(hFloatPressure, m_PressureResponseCurve.Evaluate(m_Pressure));
    }

    public void TriggerSpray() {
        m_Animator.SetTrigger(hTrigSpray);
    }
    
    private static void OverrideTriggerCollider(ParticleSystem.TriggerModule triggerModule, Collider collider) {
        for (int i = triggerModule.colliderCount - 1; i >= 0; i--) {
            triggerModule.RemoveCollider(i);
            triggerModule.AddCollider(collider);
        }
    }
#if UNITY_EDITOR
    private void OnValidate() {
        if (Application.isPlaying) {
            SetPressure(m_Pressure);
        }
    }
#endif
}
