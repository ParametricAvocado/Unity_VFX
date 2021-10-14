using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

[ExecuteAlways]
public class AnimationMotionPaths : MonoBehaviour {
    [SerializeField] Color m_Color = Color.green;
    [SerializeField] private int m_OffsetStart = -5;
    [SerializeField] private int m_OffsetEnd = 5;
    [SerializeField] private float m_NodeSize = 0.1f;

    [SerializeField] private List<GameObject> m_AlwaysDisplayed;
    
    private GameObject proxy;
    private GameObject selection;
    private bool selectionIsChild;
    private Transform selectionProxy;
    
    private readonly List<string> tempChildrenPath = new List<string>();

    private readonly List<Transform> alwaysDisplayedProxies = new List<Transform>();
    private readonly List<Vector3> trails = new List<Vector3>();

    private void OnDisable() {
        if (proxy) {
            DestroyImmediate(proxy);
        }
    }

    private bool IsValidChild(GameObject go) {
        return go.GetComponentInParent<AnimationMotionPaths>() == this;
    }

    private Transform FindTransformInProxy(Transform t) {
        var current = t;

        tempChildrenPath.Clear();
        while (current != transform) {
            tempChildrenPath.Add(current.name);
            current = current.parent;
        }
        
        var result = proxy.transform;
        for (int i = tempChildrenPath.Count - 1; i >= 0; i--) {
            result = result.Find(tempChildrenPath[i]);
        }

        return result;
    }

    private void UpdateSelection() {
        if (!proxy) {
            proxy = Instantiate(gameObject);
            proxy.name = "AnimationMotionProxy";
            proxy.hideFlags = HideFlags.HideAndDontSave;

            foreach (var r in proxy.GetComponentsInChildren<Renderer>()) {
                r.enabled = false;
            }
            
        }
        
        alwaysDisplayedProxies.Clear();
        if (m_AlwaysDisplayed != null) {
            foreach (var go in m_AlwaysDisplayed) {
                if (!go || !IsValidChild(go)) return;

                alwaysDisplayedProxies.Add(FindTransformInProxy(go.transform));
            }
        }

        if (Selection.activeGameObject == selection) return;
        
        selection = Selection.activeGameObject;

        if (!selection) return; 

        selectionIsChild =  IsValidChild(selection);

        if (!selectionIsChild) return;

        selectionProxy = FindTransformInProxy(selection.transform);
    }


    private void OnDrawGizmos() {
        UpdateSelection();
        
        var animWindows = Resources.FindObjectsOfTypeAll(typeof(AnimationWindow));

        if (animWindows.Length == 0) return;

        var animWindow = animWindows[0] as AnimationWindow;
        
        if (animWindow == null) return;

        if (!animWindow.previewing) return;

        if (!proxy) return;
        
        var slice = 1 / animWindow.animationClip.frameRate;
        
        Vector3 prevPos = Vector3.zero;

        Gizmos.color = m_Color;
        
        for (int i = m_OffsetStart; i < m_OffsetEnd; i++) {
            animWindow.animationClip.SampleAnimation(proxy, (animWindow.time + slice * i) % animWindow.animationClip.length);
            if (!animWindow.animationClip.hasRootCurves) {
                proxy.transform.position = transform.position;
            }
            
            for (int proxyIndex = 0; proxyIndex < alwaysDisplayedProxies.Count; proxyIndex++) {

                if (i > m_OffsetStart) {
                    Gizmos.DrawLine(trails[proxyIndex], alwaysDisplayedProxies[proxyIndex].position);
                }
                
                Gizmos.DrawWireSphere(alwaysDisplayedProxies[proxyIndex].position, m_NodeSize);

                if (trails.Count > proxyIndex) {
                    trails[proxyIndex] = (alwaysDisplayedProxies[proxyIndex].position);
                }
                else {
                    trails.Add(alwaysDisplayedProxies[proxyIndex].position);
                }
            }
            
            if (!selection || !selectionIsChild) continue;
            
            if (i > m_OffsetStart) {
                Gizmos.DrawLine(prevPos, selectionProxy.position);
            }
            
            prevPos = selectionProxy.position;
            Gizmos.DrawWireSphere(selectionProxy.position, m_NodeSize);
            
        }
    }
}
