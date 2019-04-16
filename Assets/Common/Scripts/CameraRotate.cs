using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform target;
    public float rotationRate;
    Vector3 delta;


    Quaternion animatedRotation = Quaternion.identity;
    private void OnEnable()
    {
        delta = transform.position - target.position;

    }


    private void Update()
    {
        animatedRotation = Quaternion.Euler(0, rotationRate * Time.deltaTime, 0) * animatedRotation;

        transform.position = target.position + animatedRotation * delta;
        transform.LookAt(target);
    }
}
