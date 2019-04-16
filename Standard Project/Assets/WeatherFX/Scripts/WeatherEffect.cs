using UnityEngine;

public class WeatherEffect : MonoBehaviour
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

    [Header("Simulation")]
    [SerializeField]
    protected float m_SpeedScale = 1.0f;
    [SerializeField]
    protected AnimationCurve m_SpeedCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));


    [Header("Emission")]
    [SerializeField]
    protected float m_EmissionScale = 80.0f;

    [SerializeField]
    protected AnimationCurve m_EmissionCurve = new AnimationCurve(new Keyframe(0, 0.0f), new Keyframe(1, 1.0f));

    [Header("Noise")]
    [SerializeField]
    protected float m_TurbulenceScale;

    [SerializeField]
    protected AnimationCurve m_TurbulenceCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

    [Header("Fog")]
    [SerializeField]
    protected bool m_AffectsFog = false;

    [SerializeField]
    protected Color m_FogColor = Color.white;

    [SerializeField]
    protected float m_FogDensityScale = 0.05f;

    [SerializeField]
    protected AnimationCurve m_FogDensityCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

    [Header("Sky")]
    [SerializeField]
    protected bool m_AffectsSky = false;

    [SerializeField]
    protected Color m_SkyTint = Color.gray;

    [SerializeField]
    protected AnimationCurve m_AtmosphereThickness = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(0.0f, 1.0f));

    [SerializeField]
    protected AnimationCurve m_ExposureCurve = new AnimationCurve(new Keyframe(0.0f, 1.3f), new Keyframe(0.0f, 1.3f));

    protected ParticleSystem m_ParticleSystem;
    protected ParticleSystem.EmissionModule emission;
    protected ParticleSystem.MainModule main;
    protected ParticleSystem.NoiseModule noise;
    protected ParticleSystem.ShapeModule shape;

    protected bool initialized;
    protected bool isDirty;

    protected Material skyMaterial;
    protected Color defaultSkyTintColor;
    protected int skyTintPropID = Shader.PropertyToID("_SkyTint");
    protected int atmosThickPropID = Shader.PropertyToID("_AtmosphereThickness");
    protected int exposurePropID = Shader.PropertyToID("_Exposure");
    [SerializeField]
    [HideInInspector]
    protected WeatherEffect parent;

    [SerializeField]
    [HideInInspector]
    protected WeatherEffect[] children;

    [SerializeField]
    [HideInInspector]
    protected bool isRoot;

    public float Intensity
    {
        get
        {
            return m_Intensity;
        }

        set
        {
            if (m_Intensity == value)
            {
                return;
            }

            m_Intensity = value;
            isDirty = true;
        }
    }

    protected virtual void Initialize()
    {
        if (initialized)
        {
            return;
        }

        m_ParticleSystem = GetComponent<ParticleSystem>();

        if (!m_ParticleSystem)
        {
            return;
        }

        main = m_ParticleSystem.main;
        noise = m_ParticleSystem.noise;
        emission = m_ParticleSystem.emission;
        shape = m_ParticleSystem.shape;
        initialized = true;
        isDirty = true;
    }

    protected virtual void UpdateParticleSystemSettings()
    {
        Initialize();

        if (!m_ParticleSystem)
        {
            return;
        }
        main.simulationSpeed = m_SpeedCurve.Evaluate(Intensity) * m_SpeedScale;
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

        if (m_AffectsSky)
        {
            if (skyMaterial != RenderSettings.skybox)
            {
                skyMaterial = RenderSettings.skybox;
                defaultSkyTintColor = skyMaterial.GetColor(skyTintPropID);
            }
            skyMaterial.SetColor(skyTintPropID, Color.Lerp(defaultSkyTintColor, m_SkyTint, Intensity));
            skyMaterial.SetFloat(atmosThickPropID, m_AtmosphereThickness.Evaluate(Intensity));
            skyMaterial.SetFloat(exposurePropID, m_ExposureCurve.Evaluate(Intensity));
        }
    }

    private void OnEnable()
    {
        isDirty = true;
    }

    private void OnDisable()
    {
        Intensity = 0;
        UpdateParticleSystemSettings();
    }
    private void FixedUpdate()
    {
        if (m_FollowTarget)
        {
            transform.position = m_FollowTarget.position;
        }

        if (parent)
        {
            Intensity = parent.Intensity;
        }


        if (isDirty)
        {
            UpdateParticleSystemSettings();

            isDirty = false;
        }
    }

    private void OnValidate()
    {
        isDirty = true;

        bool root = GetComponentInParent<WeatherEffect>();

        if (isRoot != root)
        {
            isRoot = root;
            children = GetComponentsInChildren<WeatherEffect>();
            if (isRoot)
            {
                parent = null;
                foreach (var effect in children)
                {
                    if (effect != this)
                    {
                        effect.SetParent(this);
                    }
                }
            }
        }

        if (!Application.isPlaying)
        {
            initialized = false;

            if(isRoot)
            {
                foreach(var effect in children)
                {
                    effect.m_Intensity = Intensity;
                    effect.UpdateParticleSystemSettings();
                }
            }

            UpdateParticleSystemSettings();
        }
    }

    private void SetParent(WeatherEffect weatherEffect)
    {
        isRoot = false;
        parent = weatherEffect;
    }
}
