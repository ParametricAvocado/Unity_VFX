using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace DevonaProject {
    public class DamageTrigger : MonoBehaviour {
        public class DamageTriggerEvent : UnityEvent<ComboNodeDamageEvent>{}

        private Collider collider;
        private ComboNodeDamageEvent currentDamageEvent;
        private CharacterController owner;

        private void Awake() {
            collider = GetComponent<Collider>();
            collider.enabled = false;
        }

        public void Initialize(CharacterController owner) {
            this.owner = owner;
        }
        
        public void UpdateDamageData(ComboNodeDamageEvent damageEvent) {
            currentDamageEvent = damageEvent;
            collider.enabled = currentDamageEvent;
        }

        private void OnTriggerEnter(Collider other) {
            var dummy = other.GetComponentInParent<TargetDummy>();
            var hitDirection = owner.transform.TransformDirection(currentDamageEvent.m_Direction);

            if (currentDamageEvent.m_IsKnockdown) { dummy.OnKnockdown(hitDirection); }
            else{ dummy.OnHit(hitDirection); }

            var hitPoint = transform.TransformPoint(Vector3.ProjectOnPlane(transform.InverseTransformPoint(dummy.transform.position), Vector3.up));
            
            owner.CombatVFX.SpawnTargetVFX(hitPoint, Quaternion.LookRotation(hitDirection,transform.up),Vector3.one);
        }
    }
}