using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemFaceCamera : MonoBehaviour
{

    [SerializeField]
    Camera m_FacingCamera;

    ParticleSystem ps;
    
    void Awake()
    {
        if(m_FacingCamera == null)
        {
            m_FacingCamera = Camera.main;
        }

        if (!m_FacingCamera) return;

        FaceCamera();
    }

    void FaceCamera()
    {
        var deltaPos = m_FacingCamera.transform.position - transform.position;

        deltaPos.y = 0;

        transform.rotation = Quaternion.LookRotation(deltaPos);
    }

    private void LateUpdate()
    {
        FaceCamera();
    }
}
