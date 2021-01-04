using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

namespace DevonaProject {
    public class TargetDummy : MonoBehaviour {
        [SerializeField] private float m_FollowDistance = 4f;
        [SerializeField] private Transform m_FollowTarget;
        
        private Animator animator;

        private static int hStateCombatHit = Animator.StringToHash("combat_hit");
        private static int hStateCombatKnockdown = Animator.StringToHash("combat_knockdown_start");
        private static int hStateFollowRun = Animator.StringToHash("run");
        private static int hStateIdle = Animator.StringToHash("idle");
        private static int hFloatAngle = Animator.StringToHash("hit_angle");

        private bool isFollowing = false;
        
        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Update() {
            var animatorState = animator.GetCurrentAnimatorStateInfo(0);

            var deltaToTarget = m_FollowTarget.position -transform.position;
            float distanceToTarget = deltaToTarget.magnitude;
            
            if (isFollowing) {
                transform.rotation = Quaternion.LookRotation(m_FollowTarget.position - transform.position);

                if (!(distanceToTarget < m_FollowDistance)) return;
                
                animator.CrossFadeInFixedTime(hStateIdle,0.2f);
                isFollowing = false;
            }
            else if (animatorState.shortNameHash == hStateIdle) {
                if (!(distanceToTarget > m_FollowDistance)) return;
                
                animator.CrossFadeInFixedTime(hStateFollowRun,0.5f);
                isFollowing = true;
            }

        }

        public void OnHit(Vector3 direction) {
            float angle = Vector3.SignedAngle(transform.forward, direction, transform.up)/90f;
            animator.CrossFadeInFixedTime(hStateCombatHit, 0.1f, 0, 0f);
            
            animator.SetFloat(hFloatAngle, angle);
            isFollowing = false;

        }

        public void OnKnockdown(Vector3 hitDirection) {
            transform.rotation = Quaternion.LookRotation(-hitDirection);
            animator.CrossFadeInFixedTime(hStateCombatKnockdown, 0.1f, 0, 0f);
            isFollowing = false;
        }
    }
}