using UnityEngine;

namespace DevonaProject {
    [CreateAssetMenu(menuName = "Devona Project/Combo Tree", fileName = "ComboTree", order = 0)]
    public class ComboTree : ScriptableObject {
        [SerializeField] ComboNode lightAttackRootNode;
        [SerializeField] ComboNode heavyAttackRootNode;
        
        private Animator animator;
        private int combatLayerIndex;

        public ComboNode CurrentNode { get; private set; }
        public bool IsExecutingCombo { get; private set; }
        private float AnimatorNormalizedTime => animator.GetCurrentAnimatorStateInfo(combatLayerIndex).normalizedTime;

        private void ExecuteNode(ComboNode node) {
            if (node == null) return;
            
            IsExecutingCombo = true;
            CurrentNode = node;
            CurrentNode.Execute(animator, combatLayerIndex);
        }

        public void ClearCombo() {
            if (!IsExecutingCombo) return;
            CurrentNode = null;
            IsExecutingCombo = false;
        }

        public void ExecuteCombo(ComboInput attackInput) {
            if (!IsExecutingCombo){
                switch (attackInput) {
                    case ComboInput.LightAttack:
                        ExecuteNode(lightAttackRootNode);
                        break;
                    case ComboInput.HeavyAttack:
                        ExecuteNode(heavyAttackRootNode);
                        break;
                }
            }
            else {
                if (CurrentNode.GetNodeFromTransition(AnimatorNormalizedTime, attackInput, out var node))
                {
                    ExecuteNode(node);
                }
            }
        }

        public void SetAnimator(Animator animator, int combatLayerIndex) {
            this.animator = animator;
            this.combatLayerIndex = combatLayerIndex;
        }
    }
}