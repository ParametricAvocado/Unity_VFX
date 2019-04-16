using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraPivot : MonoBehaviour
{
    [SerializeField]
    public float m_RotateAngle = 45f;

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Rotate(0, m_RotateAngle, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Rotate(0, -m_RotateAngle, 0);
        }
    }
}
