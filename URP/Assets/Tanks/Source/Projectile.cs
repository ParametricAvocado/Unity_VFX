using System;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float m_ProjectileVelocity = 10;
    [SerializeField] private GameObject m_ExplosionPrefab;
    
    private Tank owner;

    private Rigidbody rigidbody;
    private CinemachineImpulseSource impulseSource;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start() {
        rigidbody.velocity = transform.forward * m_ProjectileVelocity;
        
        impulseSource.GenerateImpulse();
    }

    private void OnTriggerEnter(Collider other) {
        var tank = other.GetComponent<Tank>();
        if (tank) {
            if (tank == owner) return;

            tank.Damage(1);
        }
        impulseSource.GenerateImpulse();
        Instantiate(m_ExplosionPrefab, transform.position,transform.rotation);
        
        Destroy(gameObject);
    }
    
    public void SetOwner(Tank tank) {
        owner = tank;
    }
}