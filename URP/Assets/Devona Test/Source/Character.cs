using UnityEditor.Rendering;
using UnityEngine;

namespace DevonaProject {
    public enum ComboInput {
        LightAttack,
        HeavyAttack
    }


    public class Character : MonoBehaviour {
        //Inspector
        [Header("Components")] 
        [SerializeField] private DamageTrigger m_DamageTrigger;

        [Header("Locomotion")]
        [SerializeField] private float m_JumpDelay = 0.15f;
        [SerializeField] private float m_JumpVSpeed = 10;
        [SerializeField] private float m_JumpHSpeed = 5f;
        [SerializeField] private float m_JumpHoldTime = 0.3f;
        [SerializeField] private float m_JumpHoldSpeed = 10f;
        [SerializeField] private float m_JumpGravity = 30;
        [SerializeField] private float m_JumpMaxFallSpeed = 10;
        [SerializeField] private float m_JumpCombatFallSpeed = 1;

        [Header("Animation")] 
        [SerializeField] AnimationCurve m_TurnRate = AnimationCurve.Linear(0,1000,1,800);
        [SerializeField] private float m_WalkThreshold = 0.5f;
        [SerializeField] private float m_WalkBlendRate = 2f;

        //Components
        public Animator CharacterAnimator { get; private set; }
        public Camera GameplayCamera { get; private set; }
        public CharacterController Controller { get; private set; }
        public CharacterTargeting Targeting { get; private set; }
        public CharacterCombat Combat { get; private set; }
        public CharacterCombatVFX CombatVFX { get; private set; }

        //Input
        protected bool jumpInputDown;

        //Locomotion
        private float moveVectorMagnitude;
        private Vector3 worldMoveDirection;
        protected Vector3 worldMoveVector;
        private Vector3 worldLookDirection;
        private bool isMoving;

        public bool IsAirborne { get; private set; }
        private bool isJumping;
        private float jumpTimer;
        private bool jumpHold;
        private Vector3 airVelocity;
    
        //Visual
        private float animatorMoveSpeed;
        private float currentLookAngle;
        private float targetLookAngle;
    
        //Animator State
        public int JumpLayerIndex { get; set; }
        public int CombatLayerIndex { get; set; }
        public AnimatorStateInfo CurrentCombatLayerState;
        public AnimatorStateInfo NextCombatLayerState;
        private bool isPerformingAttack;

        private static int hBoolIsMoving = Animator.StringToHash("IsMoving");
        private static int hFloatMoveSpeed = Animator.StringToHash("MoveSpeed");
        private static int hFloatAngle = Animator.StringToHash("HitAngle");

        private static int hStateAttackNone = Animator.StringToHash("att_none");
        private static int hStateJumpStart = Animator.StringToHash("jump_start");
        private static int hStateJumpEnd = Animator.StringToHash("jump_end");
        
        private static int hStateCombatHit = Animator.StringToHash("combat_hit");
        private static int hStateCombatKnockdown = Animator.StringToHash("combat_knockdown_start");
        
        protected virtual void InitializeInput(){}
    
        private void Awake() {
            InitializeInput();
            
            CharacterAnimator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
            Targeting = GetComponent<CharacterTargeting>() ?? gameObject.AddComponent<CharacterTargeting>();
            Combat = GetComponent<CharacterCombat>() ?? gameObject.AddComponent<CharacterCombat>();
            CombatVFX = GetComponent<CharacterCombatVFX>();

            JumpLayerIndex = CharacterAnimator.GetLayerIndex("Jump");
            CombatLayerIndex = CharacterAnimator.GetLayerIndex("Combat");

            Combat.Initialize(this);
            
            m_DamageTrigger.Initialize(this);
        }

        protected virtual void Start() {
            GameplayCamera = Camera.main;
            currentLookAngle = transform.rotation.eulerAngles.y;
            worldLookDirection = worldMoveDirection = transform.forward;
        }

        protected virtual void OnDestroy() { }

        protected virtual void UpdateMoveVector() {

            isMoving = worldMoveVector != Vector3.zero;

            if (isMoving) {
                moveVectorMagnitude = worldMoveVector.magnitude;
                worldMoveDirection = worldMoveVector.normalized;
                Targeting.InputDirection = worldMoveDirection;
            }
            else {
                Targeting.InputDirection = transform.forward;
            }
        }
        
        private void FixedUpdate() {
            UpdateCombatLayerState();

            //Input Update
            UpdateMoveVector();
            
            Targeting.UpdateTargeting();
            
            animatorMoveSpeed = Mathf.MoveTowards(animatorMoveSpeed, moveVectorMagnitude > m_WalkThreshold ? 1 : 0, m_WalkBlendRate * Time.deltaTime);
            
            //Character Turn
            if (!IsAirborne && (!isPerformingAttack || Combat.CurrentNode.AllowsTurn)) {
                UpdateLookAngle(true);
            }
            
            if (isJumping) {
                jumpTimer += Time.deltaTime;
                
                if (!jumpInputDown || jumpTimer > m_JumpHoldTime) jumpHold = false;
                
                
                if (!IsAirborne && jumpTimer >= m_JumpDelay) {
                    IsAirborne = true;
                    airVelocity.x = worldMoveVector.x * m_JumpHSpeed;
                    airVelocity.z = worldMoveVector.z * m_JumpHSpeed;
                    
                    airVelocity.y = m_JumpVSpeed;
                }
            }

            if (IsAirborne) {
                Controller.Move(airVelocity * Time.deltaTime);
                float maxFallSpeed = isPerformingAttack ? m_JumpCombatFallSpeed : m_JumpMaxFallSpeed;
                airVelocity.y = Mathf.Max(airVelocity.y - m_JumpGravity * Time.deltaTime, jumpHold ? m_JumpHoldSpeed : -maxFallSpeed);

                if (airVelocity.y < 0 && Controller.isGrounded) {
                    IsAirborne = false;
                    if(JumpLayerIndex != -1)
                    CharacterAnimator.CrossFadeInFixedTime(hStateJumpEnd,0.05f, JumpLayerIndex, 0f);
                    isJumping = false;
                    IsAirborne = false;
                }
            }

            else {
                if (!isPerformingAttack) {
                    if (JumpInput && !isJumping) {
                        if(JumpLayerIndex != -1)
                        CharacterAnimator.CrossFadeInFixedTime(hStateJumpStart, 0.1f, JumpLayerIndex,0f);

                        jumpHold = jumpInputDown;
                        isJumping = true;
                        jumpTimer = 0;
                    }
                
                    //Animator Update
                    CharacterAnimator.SetBool(hBoolIsMoving, isMoving);
                    CharacterAnimator.SetFloat(hFloatMoveSpeed, animatorMoveSpeed);

                    //Ensure Combo Tree is clear 
                    Combat.ClearCombo();
                }
            }
        
            if (LightAttackInput) {
                if (Combat.ExecuteCombo(ComboInput.LightAttack)) {
                    OnExecuteCombo();
                }
            }
            else if (HeavyAttackInput) {
                if (Combat.ExecuteCombo(ComboInput.HeavyAttack)) {
                    OnExecuteCombo();
                }
            }
            
            m_DamageTrigger.UpdateDamageData(Combat.GetDamageData());
            
            CharacterTurnUpdate();
        }

        private void UpdateCombatLayerState() {
            if (CombatLayerIndex != -1) {
                CurrentCombatLayerState = CharacterAnimator.GetCurrentAnimatorStateInfo(CombatLayerIndex);
                NextCombatLayerState = CharacterAnimator.GetNextAnimatorStateInfo(CombatLayerIndex);

                isPerformingAttack = CurrentCombatLayerState.shortNameHash != hStateAttackNone
                                     || NextCombatLayerState.shortNameHash != hStateAttackNone &&
                                     NextCombatLayerState.shortNameHash != 0;
            }
            else {
                isPerformingAttack = false;
            }
        }

        private void OnExecuteCombo() {
            UpdateLookAngle(true);
            CharacterAnimator.SetBool(hBoolIsMoving, false);
            CombatVFX.SetDefaultCasterVFX(Combat.CurrentNode.AttackVFXCaster);
            CombatVFX.SetDefaultTargetVFX(Combat.CurrentNode.AttackVFXTarget);
            airVelocity.x = 0;
            airVelocity.z = 0;
        }

        private void CharacterTurnUpdate() {
            var turnRate = m_TurnRate.Evaluate(animatorMoveSpeed);
            if (isPerformingAttack) {
                turnRate *= Combat.CurrentNode.TurnSpeedMultiplier;
            }
            
            currentLookAngle = Mathf.MoveTowardsAngle(currentLookAngle,
                targetLookAngle,
                turnRate * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, currentLookAngle, 0);
        }

        private void UpdateLookAngle(bool useTargeting) {
            if (useTargeting) {
                worldLookDirection = Targeting.PreferredDirection;
            }else
            if (isMoving) {
                worldLookDirection = worldMoveDirection;
            }
            
            targetLookAngle = Mathf.Atan2(worldLookDirection.x, worldLookDirection.z) * Mathf.Rad2Deg;
        }

        public void OnHit(Vector3 direction) {
            float angle = Vector3.SignedAngle(transform.forward, direction, transform.up)/90f;
            CharacterAnimator.CrossFadeInFixedTime(hStateCombatHit, 0.1f, CombatLayerIndex, 0f);
            
            CharacterAnimator.SetFloat(hFloatAngle, angle);

        }

        public void OnKnockdown(Vector3 hitDirection) {
            transform.rotation = Quaternion.LookRotation(-hitDirection);
            CharacterAnimator.CrossFadeInFixedTime(hStateCombatKnockdown, 0.1f, CombatLayerIndex, 0f);
        }
        
        public virtual bool LightAttackInput => false;
        public virtual bool HeavyAttackInput => false;
        public virtual bool JumpInput => false;
    }
}