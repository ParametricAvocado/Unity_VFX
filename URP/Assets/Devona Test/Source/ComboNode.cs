using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Serialization;

namespace DevonaProject {
    [CreateAssetMenu(menuName = "Devona Project/Combo Node", fileName = "ComboNode", order = 0)]
    public class ComboNode : ScriptableObject {
        [SerializeField] private float m_TransitionDuration = 0.1f;
        [SerializeField] private string m_AnimationName;
        [SerializeField] private ComboNodeTransition[] m_Transitions;
        [SerializeField] private ComboNodeDamageEvent[] m_DamageEvents;

        private int animationHash = -1;

        public ComboNodeTransition[] Transitions {
            get => m_Transitions;
        }

        public int AnimationHash => animationHash;

        public void Initialize() {
            animationHash = Animator.StringToHash(m_AnimationName);
        }
        
        public void Execute(Animator animator, int layer) {
            animator.CrossFade(AnimationHash, m_TransitionDuration, layer);
        }

        public bool GetNodeFromTransition(float normalizedTime, ComboInput attackInput, out ComboNode comboNode) {
            foreach (var transition in Transitions) {
                if (transition.input != attackInput || normalizedTime < transition.transitionBegin ||
                    normalizedTime > transition.transitionEnd) {
                    continue;
                }

                comboNode = transition.targetNode;
                return true;
            }

            comboNode = null;
            return false;
        }

        public ComboNodeDamageEvent GetDamageEvent(float time) {
            return m_DamageEvents.FirstOrDefault(damageEvent => time >= damageEvent.m_TimeRange.x && time <= damageEvent.m_TimeRange.y);
        }
    }
}