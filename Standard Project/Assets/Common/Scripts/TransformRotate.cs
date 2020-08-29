using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotate : MonoBehaviour
{
	public Vector3 axis;
	public float rotation;
	
    void Update()
    {
		transform.Rotate(axis, rotation * Time.deltaTime);
    }
}
