using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace DevonaProject {
    public class CharacterCombatVFX : MonoBehaviour {
        [SerializeField] private GameObject m_VFXPrefab;
        [SerializeField] private Transform[] m_AttachmentPoints;

        private List<VisualEffect> pool = new List<VisualEffect>();

        private VisualEffect GetVisualEffect() {
            foreach (var instance in pool) {
                if (instance.aliveParticleCount == 0)
                    return instance;
            }

            var newInstance = Instantiate(m_VFXPrefab).GetComponent<VisualEffect>();
            pool.Add(newInstance);
            return newInstance;
        }
        
        public void SpawnVFX(string attachmentName) {
            var attachmentTransform = m_AttachmentPoints.FirstOrDefault(t=>t.name == attachmentName);

            if (!attachmentTransform) return;
            
            var vfx = GetVisualEffect();

            vfx.transform.SetPositionAndRotation(attachmentTransform.position,attachmentTransform.rotation);
            vfx.transform.localScale = attachmentTransform.localScale;
            
            vfx.Play();
        }
    }
}