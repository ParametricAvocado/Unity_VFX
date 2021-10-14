using Cinemachine;
using UnityEngine;

namespace DevonaProject {
    public class CameraRotation : MonoBehaviour {
        private CinemachineFreeLook vc;
        private DevonaActions actions;
        private Vector2 cameraDelta;

        private void Awake() {
            actions = new DevonaActions();
            actions.Character.Look.started += (ctx) => cameraDelta = ctx.ReadValue<Vector2>();
            actions.Character.Look.performed += (ctx) => cameraDelta = ctx.ReadValue<Vector2>();
            actions.Character.Look.canceled += (ctx) => cameraDelta = Vector2.zero;
            actions.Enable();
            vc = GetComponent<CinemachineFreeLook>();
        }

        private void OnEnable() {
            if(actions != null)
                actions.Enable();
        }

        private void OnDisable() {
            actions.Disable();
        }

        private void Update() {
            vc.m_XAxis.m_InputAxisValue = cameraDelta.x;
            vc.m_YAxis.m_InputAxisValue = cameraDelta.y;

        }
    }
}