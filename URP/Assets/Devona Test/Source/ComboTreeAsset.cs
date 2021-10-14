using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.IMGUI.Controls;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

namespace DevonaProject {
    [CreateAssetMenu(menuName = "Devona Project/Combo Tree", fileName = "ComboTree", order = 0)]
    public class ComboTreeAsset : ScriptableObject {
        [SerializeField] ComboNode m_LightAttackRootNode;
        [SerializeField] ComboNode m_HeavyAttackRootNode;
        [SerializeField] ComboNode m_AirborneLightAttackRootNode;

        private HashSet<ComboNode> comboNodes = new HashSet<ComboNode>();

        public ComboNode LightAttackRootNode => m_LightAttackRootNode;
        public ComboNode HeavyAttackRootNode => m_HeavyAttackRootNode;
        public ComboNode AirborneLightAttackRootNode => m_AirborneLightAttackRootNode;

        private void InitializeComboNodesRecursively(ComboNode comboNode) {
            if (comboNodes.Contains(comboNode)) return;
            
            comboNode.Initialize();

            foreach (var childNode in comboNode.Transitions.Select(t => t.targetNode)) {
                InitializeComboNodesRecursively(childNode);
            }
        } 
        
        public void Initialize() {
            InitializeComboNodesRecursively(m_LightAttackRootNode);
            InitializeComboNodesRecursively(m_AirborneLightAttackRootNode);
            InitializeComboNodesRecursively(m_HeavyAttackRootNode);
        }

    }
}