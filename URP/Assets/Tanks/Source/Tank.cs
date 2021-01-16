using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Tank : MonoBehaviour {
    [Header("Combat")]
    [SerializeField] private int m_MaxHealth = 10;
    [SerializeField] private GameObject m_ProjectilePrefab;
    [SerializeField] private Transform m_ProjectileOrigin;
    [Header("Movement")]
    [SerializeField] private float m_TurnSpeed = 300f;
    [SerializeField] private float m_MaxForwardSpeed = 5f;
    [SerializeField] private float m_MaxBackwardSpeed = 5f;
    [SerializeField] private float m_Acceleration = 3f;
    [SerializeField] private float m_Deceleration = 2f;
    [SerializeField] private float m_Braking = 5f;

    [Header("Cosmetics")] [SerializeField] private ParticleSystem m_FireParticleSystem;

    private float motorForce;
    private Vector2 moveVector;
    private bool fireInput;
    private Rigidbody rigidbody;
    private Vector3 localVelocity;
    private int health;
    private bool isDead;

    public void OnFire(InputValue value) {
        var projectileInstance = Instantiate(m_ProjectilePrefab);
        projectileInstance.transform.SetPositionAndRotation(m_ProjectileOrigin.position,m_ProjectileOrigin.rotation);

        var projectileComponent = projectileInstance.GetComponent<Projectile>();
        projectileComponent.SetOwner(this);
        
        m_FireParticleSystem.Play();
    }
    
    public void OnMove(InputValue value) {
        moveVector = value.Get<Vector2>();
    }

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        health = m_MaxHealth;
    }

    private void FixedUpdate() {
        if (isDead) return;
        
        localVelocity = transform.InverseTransformVector(rigidbody.velocity);

        if (moveVector.y == 0) {
            motorForce = Mathf.MoveTowards(motorForce, 0, m_Deceleration * Time.deltaTime);
        }
        else if(moveVector.y > 0) {
            float rate = localVelocity.y > 0 ? m_Acceleration : m_Braking;
            motorForce = Mathf.MoveTowards(motorForce, m_MaxForwardSpeed, rate * Time.deltaTime);
        }
        else {
            float rate = localVelocity.y < 0 ? m_Acceleration : m_Braking;
            motorForce = Mathf.MoveTowards(motorForce, -m_MaxBackwardSpeed, rate * Time.deltaTime);
        }

        rigidbody.AddRelativeForce(Vector3.forward * motorForce);
        rigidbody.AddRelativeTorque(Vector3.up * (m_TurnSpeed * moveVector.x));
    }

    private void OnGUI() {
        GUILayout.Label($"Local Velocity - {localVelocity}");
    }

    public void Damage(int amount) {
        if (isDead) return;
        
        health -= amount;

        if (health <= 0)
            isDead = true;
    }
}
