using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeatherEffect : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    protected float m_Intensity = 1.0f;

    [SerializeField]
    protected float m_Radius = 20.0f;

    [SerializeField]
    protected Vector3 m_Offset = new Vector3(0, 20, 0);

    [SerializeField]
    protected Transform m_FollowTarget = null;

    [Header("Emission")]
    [SerializeField]
    protected float m_EmissionScale = 80.0f;

    [SerializeField]
    protected AnimationCurve m_EmissionCurve;

    [SerializeField]
    protected float m_TurbulenceScale;

    [SerializeField]
    protected AnimationCurve m_TurbulenceCurve;

    [Header("Fog")]
    [SerializeField]
    protected bool m_AffectsFog = false;

    [SerializeField]
    protected Color m_FogColor = Color.white;

    [SerializeField]
    protected float m_FogDensityScale = 0.05f;

    [SerializeField]
    protected AnimationCurve m_FogDensityCurve;

    protected ParticleSystem m_ParticleSystem;
    protected ParticleSystem.EmissionModule emission;
    protected ParticleSystem.NoiseModule noise;
    protected ParticleSystem.ShapeModule shape;

    protected bool initialized;
    protected bool isDirty;
    public float Intensity
    {
        get
        {
            return m_Intensity;
        }

        set
        {
            m_Intensity= value;
            isDirty = true;
        }
    }

    protected virtual void Initialize()
    {
        if (initialized) return;

        m_ParticleSystem = GetComponent<ParticleSystem>();

        if (!m_ParticleSystem) return;

        noise = m_ParticleSystem.noise;
        emission = m_ParticleSystem.emission;
        shape = m_ParticleSystem.shape;
        initialized = true;
        isDirty = true;
    }

    protected virtual void UpdateParticleSystemSettings()
    {
        Initialize();

        if (!m_ParticleSystem) return;

        emission.rateOverTime = m_EmissionCurve.Evaluate(Intensity) * m_EmissionScale;
        noise.strength = m_TurbulenceCurve.Evaluate(Intensity) * m_TurbulenceScale;

        shape.radius = m_Radius;
        shape.position = m_Offset;

        if (m_AffectsFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = m_FogColor;
            RenderSettings.fogDensity = m_FogDensityCurve.Evaluate(Intensity) * m_FogDensityScale;
        }
    }

    private void FixedUpdate()
    {
        if (m_FollowTarget)
        {
            transform.position = m_FollowTarget.position;
        }
        if (isDirty)
        {
            UpdateParticleSystemSettings();
            isDirty = false;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            initialized = false;
            UpdateParticleSystemSettings();
        }
    }
}
