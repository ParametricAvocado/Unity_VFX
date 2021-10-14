using System.Runtime.InteropServices;
using UnityEngine;

namespace DevonaProject {
    public class CharacterCombat : MonoBehaviour {
        
        [Header("Data")]
        [SerializeField] private ComboTreeAsset m_ComboTree;
        
        private Character owner;
        
        public ComboNode CurrentNode { get; private set; }
        public bool IsExecutingCombo { get; private set; }

        public void Initialize(Character character) {
            owner = character;
            m_ComboTree?.Initialize();
        }
        
        private AnimatorStateInfo GetCurrentNodeStateInfo() {
            return owner.CurrentCombatLayerState.shortNameHash == CurrentNode.AnimationHash
                ? owner.CurrentCombatLayerState
                : owner.NextCombatLayerState;
        }

        private void ExecuteNode(ComboNode node) {
            if (node == null) return;
            
            IsExecutingCombo = true;
            CurrentNode = node;
            CurrentNode.Execute(owner.CharacterAnimator, owner.CombatLayerIndex);
        }

        public bool ExecuteCombo(ComboInput attackInput) {
            if (!IsExecutingCombo){
                switch (attackInput) {
                    case ComboInput.LightAttack:
                        ExecuteNode(owner.IsAirborne ? m_ComboTree.AirborneLightAttackRootNode : m_ComboTree.LightAttackRootNode);
                        return true;
                    case ComboInput.HeavyAttack:
                        ExecuteNode(m_ComboTree.HeavyAttackRootNode);
                        return true;
                }
                return false;
            }

            if (CurrentNode.GetNodeFromTransition(GetCurrentNodeStateInfo().normalizedTime, attackInput, out var node))
            {
                ExecuteNode(node);
                return true;
            }
            return false;
        }

        public ComboNodeDamageEvent GetDamageData() {
            return !IsExecutingCombo ? null : CurrentNode.GetDamageEvent(GetCurrentNodeStateInfo().normalizedTime);
        }
        
        public void ClearCombo() {
            if (!IsExecutingCombo) return;
            CurrentNode = null;
            IsExecutingCombo = false;
        }
    }
}