using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DevonaProject {
    public class CharacterTargeting : MonoBehaviour {
        [SerializeField] private float m_TargetingRadius = 5f;
        [SerializeField] private LayerMask m_LayerMask = 1;
        [SerializeField] private float m_TargetingAngle = 30f;
        [SerializeField] private int m_FrameSkip = 3;
        
        private Collider[] overlapResults = new Collider[10];

        public Vector3 InputDirection { get; set; }
        public Vector3 PreferredDirection { get; set; }

        public Transform closest { get; set; }

        private int frameCount = 0;
        
        private void FixedUpdate() {
            if (frameCount > 0) {
                frameCount--;
                return;
            }

            frameCount = m_FrameSkip;
            
            int overlaps = Physics.OverlapSphereNonAlloc(transform.position, m_TargetingRadius, overlapResults, m_LayerMask, QueryTriggerInteraction.Ignore);

            float minDist = float.MaxValue;

            closest = null;
            PreferredDirection = InputDirection;
            
            for (int i = 0; i < overlaps; i++) {
                if(overlapResults[i].transform == transform)continue;

                Vector2 FromXZ(Vector3 vector3) {
                    return new Vector2(vector3.x, vector3.z);
                }

                Vector3 ToXZ(Vector2 vector2) {
                    return new Vector3(vector2.x, 0, vector2.y);
                }
                
                
                var delta = FromXZ(overlapResults[i].transform.position - transform.position);
                float sqDist = (delta).sqrMagnitude;

                if (!(sqDist < minDist)) continue;
                
                var dist = Mathf.Sqrt(sqDist);
                var dir2d = delta / dist;
                var dot = Vector3.Dot(FromXZ(InputDirection), dir2d);

                if (dot < 0 || Mathf.Acos(dot) > m_TargetingAngle * Mathf.Deg2Rad) continue;

                closest = overlapResults[i].transform;
                minDist = dist;
                PreferredDirection = ToXZ(dir2d);
            }
        }

        private void OnDrawGizmosSelected() {
            //if (!closest) return;

            Gizmos.matrix = Matrix4x4.Translate(closest ? closest.position : transform.position) *
                            Matrix4x4.Scale(Vector3.one * 0.3f) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(Time.unscaledTime * 360f,new Vector3(5,4, 1)));
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}