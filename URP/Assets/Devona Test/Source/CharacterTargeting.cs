using UnityEngine;

namespace DevonaProject {
    public class CharacterTargeting : MonoBehaviour {
        
        public Vector3 InputDirection { get; set; }
        public Vector3 PreferredDirection { get; set; }
        public Transform ClosestTarget { get; set; }

        public virtual void UpdateTargeting() {
            PreferredDirection = InputDirection;
        }
    }
}