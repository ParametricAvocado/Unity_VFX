using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevonaProject {
    public class DamageTrigger : MonoBehaviour {
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
            dummy.OnHit(owner.transform.TransformDirection(currentDamageEvent.m_Direction));
        }
    }
}