using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevonaProject {
    public class TargetDummy : MonoBehaviour {
        private Animator animator;

        private static int hStateCombatHit = Animator.StringToHash("combat_hit");
        private static int hFloatAngle = Animator.StringToHash("hit_angle");

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        public void OnHit(Vector3 direction) {
            float angle = Vector3.SignedAngle(transform.forward, direction, transform.up)/180f;
            animator.Play(hStateCombatHit, 0,0f);
            
            animator.SetFloat(hFloatAngle, angle);
        }
    }
}