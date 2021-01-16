using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthRing : MonoBehaviour {
    [SerializeField] private Image[] m_BarParts;
    [SerializeField] private Color m_BaseColor;
    [SerializeField] private Color m_DamageColor;
    
    [SerializeField, Range(0f,1f)] private float m_FillRatio = 1;
    [SerializeField, Range(0f,1f)] private float m_MaxFillAmount = 0.25f;
    [SerializeField] private Quaternion m_LookAtCorrection = Quaternion.Euler(-90, 0, 0);
    
    public float FillRatio {
        get => m_FillRatio;
        set {
            m_FillRatio = value;
        }
    }

    private float smoothFillRatio;
    private float scaledFillRatio;

    private Camera mainCamera;

    public void DamageFlash(float duration) {
        foreach (var barPart in m_BarParts) {
            barPart.canvasRenderer.SetColor(m_DamageColor);
            barPart.CrossFadeColor(m_BaseColor, duration, false, true);
        }
    }
    
    private void Start() {
        mainCamera = Camera.main;
        smoothFillRatio = m_FillRatio;
        
        foreach (var barPart in m_BarParts) {
            barPart.canvasRenderer.SetColor(m_BaseColor);
        }
    }

    private void FixedUpdate() {
        smoothFillRatio = Mathf.Lerp(smoothFillRatio, m_FillRatio, 1 / 20f);
        scaledFillRatio = smoothFillRatio * m_MaxFillAmount;
    }

    private void LateUpdate() {
        foreach (var barPart in m_BarParts) {
            if (barPart) {
                barPart.fillAmount = scaledFillRatio;
            }
        }

        var cameraVector = Vector3.ProjectOnPlane(mainCamera.transform.position - transform.position, Vector3.up);
        var lookAtCamera = Quaternion.LookRotation(cameraVector) * m_LookAtCorrection;

        transform.rotation = lookAtCamera;
    }

    private void OnValidate() {
        var fillAmount = m_FillRatio * m_MaxFillAmount;
        const float FILL_THRESHOLD = 0.01f;
        
        foreach (var barPart in m_BarParts) {
            if (!barPart) continue;
            
            if (Mathf.Abs( barPart.fillAmount - fillAmount) > FILL_THRESHOLD) {
                barPart.fillAmount = fillAmount;
            }

            if (barPart.canvasRenderer.GetColor() != m_BaseColor) {
                barPart.canvasRenderer.SetColor(m_BaseColor);
            }
        }
    }
}
