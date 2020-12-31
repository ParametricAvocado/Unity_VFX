using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CopyTransform : MonoBehaviour {
    [SerializeField] private bool m_IsActive;
    [SerializeField] private Transform m_TargetTransform;
    [SerializeField] private bool m_IsLocal;
    [SerializeField] private bool m_CopyPosition;
    [SerializeField] private bool m_CopyRotation;
    [SerializeField] private bool m_CopyScale;

    void LateUpdate() {
        if (!m_IsActive || !m_TargetTransform) return;

        if (m_CopyPosition) {
            if (m_IsLocal) {
                transform.localPosition = m_TargetTransform.localPosition;
            }
            else {
                transform.position = m_TargetTransform.position;
            }
        }

        if (m_CopyRotation) {
            if (m_IsLocal) {
                transform.localRotation = m_TargetTransform.localRotation;
            }
            else {
                transform.rotation = m_TargetTransform.rotation;
            }
        }

        if (m_CopyScale) {
            transform.localScale = m_TargetTransform.localScale;
        }
    }
}
