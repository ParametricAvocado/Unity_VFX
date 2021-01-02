using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DevonaProject {
    public enum ComboInput {
        LightAttack,
        HeavyAttack
    }

    public class CharacterController : MonoBehaviour {
        //Inspector
        [Header("Data")]
        [SerializeField] private ComboTree m_ComboTree;

        [Header("Input")]
        [SerializeField] private float m_InputPressDuration = 0.1f;

        [Header("Animation")]
        [SerializeField] AnimationCurve m_TurnRate = AnimationCurve.Linear(0,1000,1,800);
        [SerializeField] private float m_WalkThreshold = 0.5f;
        [SerializeField] private float m_WalkBlendRate = 2f;


        //Components
        private Animator animator;
        private Camera gameplayCamera;

        private DevonaActions actions;

        //Input
        private Vector2 moveVector;
        private float lightAttackInputTime = -1;
        private float heavyAttackInputTime = -1;

        //Locomotion
        private float moveVectorMagnitude;
        private Vector3 worldMoveDirection;
        private Vector3 worldMoveVector;
        private bool isMoving;
    
        //Visual
        private float animatorMoveSpeed;
        private float lookAngle;
    
        //Animator State
        private int combatLayerIndex;
        private AnimatorStateInfo currentCombatLayerState;
        private bool isPerformingAttack;

        private readonly int hBoolIsMoving = Animator.StringToHash("IsMoving");
        private readonly int hFloatMoveSpeed = Animator.StringToHash("MoveSpeed");

        private readonly int hStateAttackNone = Animator.StringToHash("att_none");

        private void InitializeInput() {
            actions = new DevonaActions();
            actions.Character.Move.performed += (ctx) => moveVector = ctx.ReadValue<Vector2>();
            actions.Character.LightAttack.started += (ctx)=> lightAttackInputTime = Time.unscaledTime;
            actions.Character.HeavyAttack.started += (ctx)=> heavyAttackInputTime = Time.unscaledTime;
        }
    
        private void Awake() {
            InitializeInput();
            animator = GetComponent<Animator>();
            combatLayerIndex =  animator.GetLayerIndex("Combat");
            m_ComboTree.SetAnimator(animator, combatLayerIndex);
        }

        private void Start() {
            gameplayCamera = Camera.main;
            actions.Enable();
            lookAngle = transform.rotation.eulerAngles.y;
        }

        private void OnDestroy() {
            actions.Disable();
        }

        private void FixedUpdate() {
            currentCombatLayerState = animator.GetCurrentAnimatorStateInfo(combatLayerIndex);
            isPerformingAttack = currentCombatLayerState.shortNameHash != hStateAttackNone;
        
            //Input Update
            var characterUp = transform.up;
            var projCamForward=  Vector3.ProjectOnPlane(gameplayCamera.transform.forward, characterUp).normalized;
            var projCamRight=  Vector3.ProjectOnPlane(gameplayCamera.transform.right, characterUp).normalized;
            worldMoveVector = projCamForward * moveVector.y + projCamRight * moveVector.x;

            isMoving = moveVector != Vector2.zero;

            if (isMoving) {
                moveVectorMagnitude = moveVector.magnitude;
                worldMoveDirection = worldMoveVector.normalized;
            }

            animatorMoveSpeed = Mathf.MoveTowards(animatorMoveSpeed, moveVectorMagnitude > m_WalkThreshold ? 1 : 0, m_WalkBlendRate * Time.deltaTime);
        
            if (!isPerformingAttack) {
                //Character Turn
                lookAngle = Mathf.MoveTowardsAngle(lookAngle, 
                    Mathf.Atan2(worldMoveDirection.x, worldMoveDirection.z) * Mathf.Rad2Deg, 
                    m_TurnRate.Evaluate(animatorMoveSpeed) * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, lookAngle, 0);

                //Animator Update
                animator.SetBool(hBoolIsMoving, isMoving);
                animator.SetFloat(hFloatMoveSpeed, animatorMoveSpeed);

            
                //Ensure Combo Tree is clear 
                m_ComboTree.ClearCombo();
            }
        
            if (LightAttackInput) {
                m_ComboTree.ExecuteCombo(ComboInput.LightAttack);
                animator.SetBool(hBoolIsMoving, false);
            }
            else if (HeavyAttackInput) {
                m_ComboTree.ExecuteCombo(ComboInput.HeavyAttack);
                animator.SetBool(hBoolIsMoving, false);
            }
        }

        public bool LightAttackInput => Time.unscaledTime - lightAttackInputTime < m_InputPressDuration;
        public bool HeavyAttackInput => Time.unscaledTime - heavyAttackInputTime < m_InputPressDuration;
    }
}