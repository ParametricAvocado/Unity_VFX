using System.Collections;
using UnityEngine;

public class ExplosionDecal : MonoBehaviour
{
    [SerializeField]
    private float m_FadeInTime = 0.2f;

    [SerializeField]
    private float m_LifeTime = 5.0f;

    [SerializeField]
    private float m_FadeOutTime = 4.0f;

    private Renderer renderer;
    private MaterialPropertyBlock mpb;
    private static readonly int colorID = Shader.PropertyToID("_Color");

    private Color colorPropertyValue;

    private IEnumerator FadeAlpha(float from, float to, float time)
    {
        float t = 0;
        while (t < time)
        {
            t = Mathf.MoveTowards(t, time, Time.deltaTime);


            if (renderer.HasPropertyBlock())
            {
                renderer.GetPropertyBlock(mpb);
            }

            colorPropertyValue.a = Mathf.Lerp(from, to, t / time);

            mpb.SetColor(colorID, colorPropertyValue);
            renderer.SetPropertyBlock(mpb);
            yield return new WaitForEndOfFrame();
        }
    }


    private IEnumerator Start()
    {
        mpb = new MaterialPropertyBlock();
        renderer = GetComponent<Renderer>();

        colorPropertyValue = renderer.sharedMaterial.GetColor(colorID);

        yield return StartCoroutine(FadeAlpha(0, colorPropertyValue.a, m_FadeInTime));

        yield return new WaitForSeconds(m_LifeTime);

        yield return StartCoroutine(FadeAlpha(colorPropertyValue.a, 0, m_FadeOutTime));

        Destroy(gameObject);
    }
}
