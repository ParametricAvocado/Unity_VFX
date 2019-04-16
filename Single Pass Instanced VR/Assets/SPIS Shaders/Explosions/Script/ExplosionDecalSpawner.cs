using UnityEngine;

public class ExplosionDecalSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ExplosionDecal;

    [SerializeField]
    private float m_DecalScale = 1.0f;

    [SerializeField]
    private float m_MaxDistance = 1.0f;

    [SerializeField]
    private float m_NormalBias = 0.001f;

    [SerializeField]
    private LayerMask m_RaycastLayermask = ~0;

    [SerializeField]
    private float m_RayOffset = 0.05f;
    [SerializeField]
    private Vector3 m_RayDirection = Vector3.down;

    [SerializeField]
    private bool m_SpawnOnAwake = true;


    private void Start()
    {
        if (m_SpawnOnAwake)
        {
            SpawnDecal();
        }
    }

    public void SpawnDecal()
    {
        Vector3 worldDirection = transform.TransformDirection(m_RayDirection);
        Ray decalRay = new Ray(transform.position - worldDirection * m_RayOffset, worldDirection);

        RaycastHit hitInfo;

        if (Physics.Raycast(decalRay, out hitInfo, m_MaxDistance, m_RaycastLayermask))
        {
            var decal = Instantiate(m_ExplosionDecal, hitInfo.point + hitInfo.normal * m_NormalBias, Quaternion.LookRotation(-hitInfo.normal));
            decal.transform.localScale = transform.TransformVector(Vector3.one * m_DecalScale);
            decal.transform.SetParent(hitInfo.collider.transform, true);
        }
    }
}
