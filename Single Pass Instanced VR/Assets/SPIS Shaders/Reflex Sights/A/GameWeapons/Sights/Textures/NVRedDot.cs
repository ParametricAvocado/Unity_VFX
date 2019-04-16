using UnityEngine;

public class NVRedDot : MonoBehaviour
{
    public Renderer SightRend;
    public bool reddot;
    public bool SFHolo;
    public bool OSHolo;
    public bool OSReddot;
    public Color OsOrange;

    Material SightMat = null;

    float m_prevTransparency = 1f;
    Color m_prevGlassColour = Color.clear;
    Color m_prevReflectColor = Color.clear;

    void Start()
    {
        EnsureMaterial();
    }

    void OnDisable()
    {
        //NightVision.TurnedOff -= TurnSightRed;
        //NightVision.TurnedOn -= TurnSightWhite;
    }

    void OnEnable()
    {
        //NightVision.TurnedOff += TurnSightRed;
        //NightVision.TurnedOn += TurnSightWhite;

        //if (NightVision.IsMyNightVisionOn)
            //TurnSightWhite();
        //else
            //TurnSightRed();
    }

    void TurnSightRed()
    {
        EnsureMaterial();

        if (OSReddot)
            SightMat.SetColor("_reticleColour", OsOrange);
        else
            SightMat.SetColor("_reticleColour", Color.red);

        OptimizeForNVG(false);
    }

    void TurnSightWhite()
    {
        EnsureMaterial();

        SightMat.SetColor("_reticleColour", Color.white);

        OptimizeForNVG(true);
    }

    void EnsureMaterial()
    {
        if (SightMat == null)
        {
            SightRend = GetComponent<Renderer>();

            if (reddot || OSReddot)
                SightMat = SightRend.material;

            if (SFHolo || OSHolo)
                SightMat = SightRend.materials[1];

            m_prevTransparency = -1f;
            m_prevGlassColour = Color.clear;
            m_prevReflectColor = Color.clear;

            if (SightMat.HasProperty("_glassTrans"))
                m_prevTransparency = SightMat.GetFloat("_glassTrans");

            if (SightMat.HasProperty("_glassColour"))
                m_prevGlassColour = SightMat.GetColor("_glassColour");

            if (SightMat.HasProperty("_reflectColor"))
                m_prevReflectColor = SightMat.GetColor("_reflectColor");
        }
    }

    void OptimizeForNVG(bool optimize)
    {
        // Increase transparency of the glass so we can make the background darker
        if (m_prevTransparency != -1f)
            SightMat.SetFloat("_glassTrans", optimize ? 0.4f : m_prevTransparency);

        // Make background darker to increase contrast
        if (m_prevGlassColour != Color.clear)
            SightMat.SetColor("_glassColour", optimize ? Color.black : m_prevGlassColour);

        // Disable reflections
        if (m_prevReflectColor != Color.clear)
            SightMat.SetColor("_reflectColor", optimize ? Color.black : m_prevReflectColor);
    }
}