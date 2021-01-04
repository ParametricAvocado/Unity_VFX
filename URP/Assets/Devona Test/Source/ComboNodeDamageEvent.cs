using UnityEngine;

namespace DevonaProject {
    [System.Serializable]
    public class ComboNodeDamageEvent {
        public Vector2 m_TimeRange;
        public Vector3 m_Direction;
        public bool m_IsKnockdown;
        public Vector2 m_DamageRange;

        public static implicit operator bool(ComboNodeDamageEvent damageEvent) {
            return damageEvent != null;
        }
    }
}