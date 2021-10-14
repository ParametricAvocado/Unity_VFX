using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessUI : MonoBehaviour {
    [SerializeField] private SceneBrightnessCalculator m_SceneBrightnessCalculator;
    [SerializeField][Range(0,1)] private float m_BrightnessThreshold;
    [SerializeField] private Color m_LightColor = Color.white;
    [SerializeField] private Color m_DarkColor = Color.black;
    [SerializeField] private Slider m_BrightnessSlider;
    
    private Graphic[] childGraphics;

    private bool isDarkMode;
    private void Start() {
        childGraphics = GetComponentsInChildren<Graphic>();
    }

    private void Update() {
        m_BrightnessSlider.value = m_SceneBrightnessCalculator.AverageBrightness;
        
        bool isBrightnessOverThreshold = m_SceneBrightnessCalculator.AverageBrightness > m_BrightnessThreshold;

        if (isBrightnessOverThreshold == isDarkMode) return;
        
        isDarkMode = isBrightnessOverThreshold;

        var color = isDarkMode ? m_DarkColor : m_LightColor;
            
        foreach (var graphic in childGraphics) {
            graphic.CrossFadeColor(color, 0.3f, true, false);
        }
    }
}
