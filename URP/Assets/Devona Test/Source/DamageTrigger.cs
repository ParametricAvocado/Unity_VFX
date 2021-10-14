using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace DevonaProject {
    public class DamageTrigger : MonoBehaviour {
        private Collider collider;
        private ComboNodeDamageEvent currentDamageEvent;
        private Character owner;

        private void Awake() {
            collider = GetComponent<Collider>();
            collider.enabled = false;
        }

        public void Initialize(Character owner) {
            this.owner = owner;
        }
        
        public void UpdateDamageData(ComboNodeDamageEvent damageEvent) {
            currentDamageEvent = damageEvent;
            collider.enabled = currentDamageEvent;
        }

        private void OnTriggerEnter(Collider other) {
            var character = other.GetComponentInParent<Character>();
            
            if (character == owner) return;
            
            var hitDirection = owner.transform.TransformDirection(currentDamageEvent.m_Direction);

            if (currentDamageEvent.m_IsKnockdown) { character.OnKnockdown(hitDirection); }
            else{ character.OnHit(hitDirection); }

            var hitPoint = transform.TransformPoint(Vector3.ProjectOnPlane(transform.InverseTransformPoint(character.transform.position), Vector3.up));
            
            owner.CombatVFX.SpawnTargetVFX(hitPoint, Quaternion.LookRotation(hitDirection,transform.up),Vector3.one);
        }
    }
}