using UnityEngine;
using UnityEngine.Serialization;

namespace DevonaProject {
    [CreateAssetMenu(menuName = "Devona Project/Combo Node", fileName = "ComboNode", order = 0)]
    public class ComboNode : ScriptableObject {
        [SerializeField] private string m_AnimationName;
        [SerializeField] private ComboNodeTransition[] m_Transitions;

        private int animationHash = -1;

        public void Execute(Animator animator, int layer) {
            if (animationHash == -1) {
                animationHash = Animator.StringToHash(m_AnimationName);
            }
            
            animator.Play(animationHash,layer);
        }

        public bool GetNodeFromTransition(float normalizedTime, ComboInput attackInput, out ComboNode comboNode) {
            foreach (var transition in m_Transitions) {
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
    }
}