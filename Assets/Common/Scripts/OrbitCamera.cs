using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    Transform m_Target;
    [SerializeField]
    float m_Distance;
    Vector3 rotation;

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            rotation.x -= Input.GetAxis("Mouse Y")*2;
            rotation.y += Input.GetAxis("Mouse X")*2;

        }
    }

    void LateUpdate()
    {
        if (!m_Target) return;

        transform.rotation = Quaternion.Slerp(transform.rotation,  Quaternion.Euler(rotation),0.2f);
        transform.position = m_Target.position - transform.forward * m_Distance;

    }
}
