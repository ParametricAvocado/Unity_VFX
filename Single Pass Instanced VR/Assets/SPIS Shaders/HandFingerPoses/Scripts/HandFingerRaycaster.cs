using UnityEngine;

public class HandFingerRaycaster : MonoBehaviour
{
    //[SerializeField]
    //private OVRInput.Controller m_controller;

    [SerializeField]
    private Animator m_animator = null;

    [SerializeField]
    private LayerMask m_surfaceLayer;

    [SerializeField]
    private float m_radius = 0.015f;

    [SerializeField]
    private float m_weightBlendRate = 20f;

    [SerializeField]
    private float m_rayExtraDistance = 0.02f;

    [SerializeField]
    private Vector3 m_TipOffset = Vector3.zero;

    [SerializeField]
    private Vector3 m_RootOffset = Vector3.zero;

    [Header("Finger Root Transforms")]
    [SerializeField]
    private Transform m_indexRoot;

    [SerializeField]
    private Transform m_middleRoot;

    [SerializeField]
    private Transform m_ringRoot;

    [SerializeField]
    private Transform m_pinkyRoot;

    [Header("Finger Tip Transforms")]
    [SerializeField]
    private Transform m_indexTip;

    [SerializeField]
    private Transform m_middleTip;

    [SerializeField]
    private Transform m_ringTip;

    [SerializeField]
    private Transform m_pinkyTip;

    [HideInInspector]
    [SerializeField]
    private bool m_ApplyChanges;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_indexRayOrigin;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_middleRayOrigin;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_ringRayOrigin;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_pinkyRayOrigin;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_indexRayDirection;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_middleRayDirection;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_ringRayDirection;

    [HideInInspector]
    [SerializeField]
    private Vector3 m_pinkyRayDirection;

    [HideInInspector]
    [SerializeField]
    private float m_indexRayLength;

    [HideInInspector]
    [SerializeField]
    private float m_middleRayLength;

    [HideInInspector]
    [SerializeField]
    private float m_ringRayLength;

    [HideInInspector]
    [SerializeField]
    private float m_pinkyRayLength;

    public const string ANIM_INDEX_NAME = "Index";
    public const string ANIM_MIDDLE_NAME = "Middle";
    public const string ANIM_RING_NAME = "Ring";
    public const string ANIM_PINKY_NAME = "Pinky";

    private int m_animLayerIndex = -1;
    private int m_animLayerMiddle = -1;
    private int m_animLayerRing = -1;
    private int m_animLayerPinky = -1;

    private int m_animParamIndex = -1;
    private int m_animParamMiddle = -1;
    private int m_animParamRing = -1;
    private int m_animParamPinky = -1;

    private float globalBlend = 0;

    private void Start()
    {
        m_animLayerIndex = m_animator.GetLayerIndex(ANIM_INDEX_NAME);
        m_animLayerMiddle = m_animator.GetLayerIndex(ANIM_MIDDLE_NAME);
        m_animLayerRing = m_animator.GetLayerIndex(ANIM_RING_NAME);
        m_animLayerPinky = m_animator.GetLayerIndex(ANIM_PINKY_NAME);

        m_animParamIndex = Animator.StringToHash(ANIM_INDEX_NAME);
        m_animParamMiddle = Animator.StringToHash(ANIM_MIDDLE_NAME);
        m_animParamRing = Animator.StringToHash(ANIM_RING_NAME);
        m_animParamPinky = Animator.StringToHash(ANIM_PINKY_NAME);

    }

    private void OnEnable()
    {
        globalBlend = 0;
    }

    private void OnDisable()
    {
        globalBlend = 0;
        m_animator.SetLayerWeight(m_animLayerIndex, 0);
        m_animator.SetLayerWeight(m_animLayerMiddle, 0);
        m_animator.SetLayerWeight(m_animLayerRing, 0);
        m_animator.SetLayerWeight(m_animLayerPinky, 0);
    }

    private void LateUpdate()
    {
        //float flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        float flex = 0;
        //globalBlend = Mathf.MoveTowards(globalBlend, flex, Time.deltaTime * m_weightBlendRate);
        globalBlend = 1;
        UpdateFingerStatus(m_indexRayOrigin, m_indexRayDirection, m_indexRayLength, m_animLayerIndex, m_animParamIndex);
        UpdateFingerStatus(m_middleRayOrigin, m_middleRayDirection, m_middleRayLength, m_animLayerMiddle, m_animParamMiddle);
        UpdateFingerStatus(m_ringRayOrigin, m_ringRayDirection, m_ringRayLength, m_animLayerRing, m_animParamRing);
        UpdateFingerStatus(m_pinkyRayOrigin, m_pinkyRayDirection, m_pinkyRayLength, m_animLayerPinky, m_animParamPinky);
    }

    private Collider[] fingerOverlapped = new Collider[2];
    private void UpdateFingerStatus(Vector3 rayOrigin, Vector3 rayDirection, float distance, int layerID, int paramID)
    {
        RaycastHit hitInfo;

        bool hit = Physics.SphereCast(transform.TransformPoint(rayOrigin), m_radius, transform.TransformDirection(rayDirection), out hitInfo, distance, m_surfaceLayer, QueryTriggerInteraction.Ignore);

        int overlaps = Physics.OverlapSphereNonAlloc(transform.TransformPoint(rayOrigin), m_radius, fingerOverlapped, m_surfaceLayer, QueryTriggerInteraction.Ignore);

        Debug.Log(overlaps);

        m_animator.SetLayerWeight(layerID, Mathf.MoveTowards(m_animator.GetLayerWeight(layerID), ((hit || overlaps > 0) ? 1 : 0) * globalBlend, Time.deltaTime * m_weightBlendRate));

        float fingerDistance = overlaps > 0 ? 0 : (hitInfo.distance / (distance - m_rayExtraDistance));

        if (hit || overlaps > 0)
        {

            Debug.DrawLine(transform.TransformPoint(rayOrigin), hitInfo.point, Color.red);
        }

        m_animator.SetFloat(paramID, Mathf.MoveTowards(m_animator.GetFloat(paramID), fingerDistance, Time.deltaTime * m_weightBlendRate));
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 indexPos = m_indexRayOrigin;
        Vector3 middlePos = m_middleRayOrigin;
        Vector3 ringPos = m_ringRayOrigin;
        Vector3 pinkyPos = m_pinkyRayOrigin;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(indexPos, m_radius);
        Gizmos.DrawWireSphere(middlePos, m_radius);
        Gizmos.DrawWireSphere(ringPos, m_radius);
        Gizmos.DrawWireSphere(pinkyPos, m_radius);

        Gizmos.DrawLine(indexPos, indexPos + m_indexRayDirection * m_indexRayLength);
        Gizmos.DrawLine(middlePos, middlePos + m_middleRayDirection * m_middleRayLength);
        Gizmos.DrawLine(ringPos, ringPos + m_ringRayDirection * m_ringRayLength);
        Gizmos.DrawLine(pinkyPos, pinkyPos + m_pinkyRayDirection * m_pinkyRayLength);
    }

    private void OnValidate()
    {
        if (Application.isPlaying || !m_ApplyChanges)
        {
            return;
        }

        m_ApplyChanges = false;

        m_indexRayOrigin = transform.InverseTransformPoint(m_indexTip.position) + m_TipOffset;
        m_middleRayOrigin = transform.InverseTransformPoint(m_middleTip.position) + m_TipOffset;
        m_ringRayOrigin = transform.InverseTransformPoint(m_ringTip.position) + m_TipOffset;
        m_pinkyRayOrigin = transform.InverseTransformPoint(m_pinkyTip.position) + m_TipOffset;

        Vector3 indexTipToRoot = (transform.InverseTransformPoint(m_indexRoot.position) + m_RootOffset - m_indexRayOrigin);
        Vector3 middleTipToRoot = (transform.InverseTransformPoint(m_middleRoot.position) + m_RootOffset - m_middleRayOrigin);
        Vector3 ringTipToRoot = (transform.InverseTransformPoint(m_ringRoot.position) + m_RootOffset - m_ringRayOrigin);
        Vector3 pinkyTipToRoot = (transform.InverseTransformPoint(m_pinkyRoot.position) + m_RootOffset - m_pinkyRayOrigin);

        m_indexRayDirection = indexTipToRoot.normalized;
        m_middleRayDirection = middleTipToRoot.normalized;
        m_ringRayDirection = ringTipToRoot.normalized;
        m_pinkyRayDirection = pinkyTipToRoot.normalized;

        m_indexRayLength = indexTipToRoot.magnitude + m_rayExtraDistance;
        m_middleRayLength = middleTipToRoot.magnitude + m_rayExtraDistance;
        m_ringRayLength = ringTipToRoot.magnitude + m_rayExtraDistance;
        m_pinkyRayLength = pinkyTipToRoot.magnitude + m_rayExtraDistance;
    }
}
