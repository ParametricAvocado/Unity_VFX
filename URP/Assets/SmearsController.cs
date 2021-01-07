using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public class SmearsController : MonoBehaviour
{
    [SerializeField] AnimationCurve m_AngularResponse = AnimationCurve.Linear(0,0,1,1);
    
    private Vector3 prevUp;
    
    private float angle;
    private Vector3 rotationAxis;
    
    private Renderer renderer;
    private MaterialPropertyBlock propertyBlock;
    private int angularSmearID = Shader.PropertyToID("_AngularSmear");
    private int angularSmearAxisID = Shader.PropertyToID("_AngularSmearAxis");
    private int smearMatrixID = Shader.PropertyToID("_SmearMatrix");
    private Matrix4x4 prevTransform;

    private void Awake() {
        renderer = GetComponent<Renderer>();
    }

    private void Start() {
        prevUp = transform.up;
        prevTransform = transform.localToWorldMatrix;
        
        propertyBlock = new MaterialPropertyBlock();
        
        renderer.SetPropertyBlock(propertyBlock);
    }

    private void FixedUpdate() {
        //angle = Vector3.SignedAngle(transform.up, prevUp, transform.forward) * Mathf.Deg2Rad;

        
        var fromTo = Quaternion.FromToRotation(transform.InverseTransformDirection(prevUp), Vector3.up);
        
        fromTo.ToAngleAxis(out angle, out rotationAxis);
        
        prevUp = transform.up;
        
        propertyBlock.SetFloat(angularSmearID, m_AngularResponse.Evaluate(angle*Mathf.Deg2Rad));

        propertyBlock.SetVector(angularSmearAxisID, rotationAxis);

        propertyBlock.SetMatrix(smearMatrixID, Application.isPlaying ? prevTransform : transform.localToWorldMatrix);

        renderer.SetPropertyBlock(propertyBlock);
        
        prevTransform = transform.localToWorldMatrix;
    }
}
