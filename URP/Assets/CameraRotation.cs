using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DevonaProject {
    public class CameraRotation : MonoBehaviour {
        private CinemachineVirtualCamera vc;
        private CinemachineOrbitalTransposer transposer;
        private DevonaActions actions;
        private Vector2 cameraDelta;

        private void Awake() {
            actions = new DevonaActions();
            actions.Character.Look.started += (ctx) => cameraDelta = ctx.ReadValue<Vector2>();
            actions.Character.Look.performed += (ctx) => cameraDelta = ctx.ReadValue<Vector2>();
            actions.Character.Look.canceled += (ctx) => cameraDelta = Vector2.zero;
            actions.Enable();
            vc = GetComponent<CinemachineVirtualCamera>();
            transposer = vc.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }

        private void OnEnable() {
            if(actions != null)
                actions.Enable();
        }

        private void OnDisable() {
            actions.Disable();
        }

        private void Update() {
            transposer.m_XAxis.m_InputAxisValue = cameraDelta.x;
        }
    }
}