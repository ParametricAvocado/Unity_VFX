using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePointerDragCamera : MonoBehaviour {
    [SerializeField] private float m_SensitivityX = 3f;
    [SerializeField] private float m_SensitivityY = 2f;
    [SerializeField][Range(0.01f,1f)] private float m_Smoothness = 1;


    private Vector3 currentRotation;
    private Vector3 smoothRotation;
    private Vector3 mousePosition;
    private Vector3 mouseDelta;

    private void Start() {
        smoothRotation = currentRotation = transform.localEulerAngles;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            mousePosition = Input.mousePosition;
            mouseDelta = Vector2.zero;
        }else if (Input.GetMouseButton(0)) {
            mouseDelta = mousePosition - Input.mousePosition;
            mousePosition = Input.mousePosition;

            const float SENSITIVITY_MUL = 20;
            currentRotation.x = Mathf.Clamp(currentRotation.x - mouseDelta.y / Screen.width * SENSITIVITY_MUL * m_SensitivityY, -90, 90);
            currentRotation.y += mouseDelta.x / Screen.width * SENSITIVITY_MUL * m_SensitivityX;

        }

        smoothRotation = Vector3.Lerp(smoothRotation, currentRotation, Time.deltaTime * 10 / m_Smoothness);
        
        transform.localEulerAngles = smoothRotation;
    }
}
