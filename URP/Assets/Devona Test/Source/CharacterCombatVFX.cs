using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace DevonaProject {
    public class CharacterCombatVFX : MonoBehaviour {
        [SerializeField] private GameObject[] m_VFXPrefab;
        [SerializeField] private Transform[] m_AttachmentPoints;


        private readonly Dictionary<string, Transform> attachmentPoints = new Dictionary<string, Transform>();
        private readonly Dictionary<string, GameObject> vfxPrefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, List<VisualEffect>> vfxPools = new Dictionary<string, List<VisualEffect>>();
        private GameObject defaultCasterVFX;
        private GameObject defaultTargetVFX;

        public void SetDefaultCasterVFX(GameObject vfxPrefab) {
            defaultCasterVFX = vfxPrefab;
        }

        public void SetDefaultTargetVFX(GameObject vfxPrefab) {
            defaultTargetVFX = vfxPrefab;
        }
        
        private void Start() {
            foreach (var prefab in m_VFXPrefab) {
                vfxPrefabs.Add(prefab.name, prefab);
            }

            foreach (var point in m_AttachmentPoints) {
                attachmentPoints.Add(point.name, point);
            }
        }

        private VisualEffect GetVisualEffect(string vfxName) {
            if (vfxPools.ContainsKey(vfxName)) {
                foreach (var instance in vfxPools[vfxName]) {
                    if (instance.aliveParticleCount == 0)
                        return instance;
                }    
            }

            if (!vfxPrefabs.ContainsKey(vfxName)) return null;
            
            var vfxInstance = Instantiate(vfxPrefabs[vfxName]).GetComponent<VisualEffect>();
            
            if (!vfxPools.ContainsKey(vfxName)) {
                vfxPools[vfxName] = new List<VisualEffect>();
            }
            
            vfxPools[vfxName].Add(vfxInstance);            

            return vfxInstance;
        }

        public void SpawnCasterVFX(string attachmentName) {
            SpawnVFX(defaultCasterVFX, attachmentName);
        }

        public void SpawnTargetVFX(Vector3 position, Quaternion rotation, Vector3 scale) {
            SpawnVFX(defaultTargetVFX, position, rotation, scale);
        }
        
        public void SpawnVFX(string vfxName, string attachmentName) {
            var attachmentTransform = attachmentPoints[attachmentName];

            SpawnVFX(vfxName, attachmentTransform);
        }

        private void SpawnVFX(string vfxName, Transform attachmentTransform) {
            if (!attachmentTransform) return;

            SpawnVFX(vfxName, attachmentTransform.position, attachmentTransform.rotation, attachmentTransform.localScale);
        }

        public void SpawnVFX(string vfxName, Vector3 position, Quaternion rotation, Vector3 scale) {
            var vfx = GetVisualEffect(vfxName);

            SpawnVFX(vfx, position, rotation, scale);
        }
        
        public void SpawnVFX(GameObject vfxPrefab, string attachmentName) {
            var attachmentTransform = attachmentPoints[attachmentName];

            SpawnVFX(vfxPrefab, attachmentTransform);
        }

        public void SpawnVFX(GameObject vfxPrefab, Transform attachmentTransform) {
            if(!attachmentTransform) return;
            SpawnVFX(vfxPrefab, attachmentTransform.position, attachmentTransform.rotation, attachmentTransform.localScale);
        }

        public void SpawnVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation, Vector3 scale) {
            if (!vfxPrefab) return;
            
            if (!vfxPrefabs.ContainsKey(vfxPrefab.name)) {
                vfxPrefabs.Add(vfxPrefab.name, vfxPrefab);
            }
            
            SpawnVFX(vfxPrefab.name, position, rotation, scale);
        }

        public static void SpawnVFX(VisualEffect vfx, Vector3 position, Quaternion rotation, Vector3 scale) {
            if (!vfx) return;

            vfx.transform.SetPositionAndRotation(position, rotation);
            vfx.transform.localScale = scale;

            vfx.Play();
        }
    }
}