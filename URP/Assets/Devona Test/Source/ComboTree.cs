using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.IMGUI.Controls;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
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
        
        private HashSet<ComboNode> comboNodes = new HashSet<ComboNode>();
        private CharacterController owner;

        private void InitializeComboNodesRecursively(ComboNode comboNode) {
            if (comboNodes.Contains(comboNode)) return;
            
            comboNode.Initialize();

            foreach (var childNode in comboNode.Transitions.Select(t => t.targetNode)) {
                InitializeComboNodesRecursively(childNode);
            }
        } 
        
        public void Initialize(CharacterController owner) {
            this.owner = owner;
            InitializeComboNodesRecursively(lightAttackRootNode);
            InitializeComboNodesRecursively(heavyAttackRootNode);
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
            CurrentNode.Execute(animator, combatLayerIndex);
        }

        public bool ExecuteCombo(ComboInput attackInput) {
            if (!IsExecutingCombo){
                switch (attackInput) {
                    case ComboInput.LightAttack:
                        ExecuteNode(lightAttackRootNode);
                        return true;
                    case ComboInput.HeavyAttack:
                        ExecuteNode(heavyAttackRootNode);
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

        public void SetAnimator(Animator animator, int combatLayerIndex) {
            this.animator = animator;
            this.combatLayerIndex = combatLayerIndex;
        }
    }
}