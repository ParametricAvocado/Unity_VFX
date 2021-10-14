using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Tank : MonoBehaviour {
    [Header("Visuals")] 
    [SerializeField] private GameObject m_Graphics;
    [SerializeField] private GameObject m_ExplosionPrefab;
    [SerializeField] private LineRenderer m_LaserSight;
    [SerializeField] private HealthRing m_HealthRing;

    [Header("Combat")]
    [SerializeField] private int m_MaxHealth = 10;
    [SerializeField] private GameObject m_ProjectilePrefab;
    [SerializeField] private Transform m_ProjectileOrigin;
    [SerializeField] private float m_ShotForce = 500f;

    [Header("Movement")]
    [SerializeField] private float m_TurnSpeed = 300f;
    [SerializeField] private float m_MaxForwardSpeed = 5f;
    [SerializeField] private float m_MaxBackwardSpeed = 5f;

    [Header("Cosmetics")] [SerializeField] private ParticleSystem m_FireParticleSystem;

    private Rigidbody rigidbody;
    private Collider collider;
    private PlayerInput playerInput;
    
    private float motorForce;
    private Vector2 moveVector;
    private bool fireInput;
    private Vector3 localVelocity;
    private int health;
    private bool isDead;
    private RaycastHit laserRaycastHit;

    public void OnFire(InputValue value) {
        if (isDead) return;
        
        m_LaserSight.enabled = value.isPressed;

        if (value.isPressed) return;
        
        var projectileInstance = Instantiate(m_ProjectilePrefab);
        projectileInstance.transform.SetPositionAndRotation(m_ProjectileOrigin.position,
            m_ProjectileOrigin.rotation);

        var projectileComponent = projectileInstance.GetComponent<Projectile>();
        projectileComponent.SetOwner(this);

        m_FireParticleSystem.Play();
        
        rigidbody.AddRelativeForce(Vector3.back * m_ShotForce);
    }

    public void OnMove(InputValue value) {
        if (isDead) return;

        moveVector = value.Get<Vector2>();
    }

    private void Awake() {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start() {
        health = m_MaxHealth;
        m_LaserSight.enabled = false;
    }

    private void FixedUpdate() {
        if (isDead) return;
        
        localVelocity = transform.InverseTransformVector(rigidbody.velocity);

        if (moveVector.y == 0) {
            motorForce = 0;
        }
        else if(moveVector.y > 0) {
            motorForce = m_MaxForwardSpeed * moveVector.y;
        }
        else {
            motorForce = m_MaxBackwardSpeed * moveVector.y;
        }

        rigidbody.AddRelativeForce(Vector3.forward * motorForce);
        rigidbody.AddRelativeTorque(Vector3.up * (m_TurnSpeed * moveVector.x));
        var hit = Physics.Raycast(m_ProjectileOrigin.position, m_ProjectileOrigin.forward, out laserRaycastHit, float.MaxValue, 1,
            QueryTriggerInteraction.Ignore);

        if (hit) {
            m_LaserSight.SetPosition(1, m_LaserSight.transform.InverseTransformPoint(laserRaycastHit.point));
        }
    }

    private void OnGUI() {
        GUILayout.Label($"Local Velocity - {localVelocity}");
    }

    public void Damage(int amount, Vector3 force, Vector3 position) {
        if (isDead) return;
        
        health -= amount;
        m_HealthRing.FillRatio = (float)health / m_MaxHealth;
        m_HealthRing.DamageFlash(0.3f);
        
        rigidbody.AddForceAtPosition(force, position);

        if (health > 0) return;
        
        isDead = true;
        
        Instantiate(m_ExplosionPrefab, transform.position, transform.rotation);
        m_Graphics.SetActive(false);
        m_LaserSight.enabled = false;
        
        rigidbody.isKinematic = true;
        collider.enabled = false;
    }
}
