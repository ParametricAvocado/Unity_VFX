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
        
        [Header("Data")]
        [SerializeField] private ComboTree m_ComboTree;

        [Header("Input")]
        [SerializeField] private float m_InputPressDuration = 0.1f;

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
        public CharacterCombatVFX CombatVFX { get; private set; }

        private DevonaActions actions;

        //Input
        private Vector2 moveVector;
        private float lightAttackInputTime = float.MinValue;
        private float heavyAttackInputTime = float.MinValue;
        private float jumpInputTime = float.MinValue;
        private bool jumpInputDown;

        //Locomotion
        private float moveVectorMagnitude;
        private Vector3 worldMoveDirection;
        private Vector3 worldMoveVector;
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
        private int jumpLayerIndex;
        private int combatLayerIndex;
        public AnimatorStateInfo CurrentCombatLayerState;
        public AnimatorStateInfo NextCombatLayerState;
        private bool isPerformingAttack;

        private readonly int hBoolIsMoving = Animator.StringToHash("IsMoving");
        private readonly int hFloatMoveSpeed = Animator.StringToHash("MoveSpeed");

        private readonly int hStateAttackNone = Animator.StringToHash("att_none");
        private readonly int hStateJumpStart = Animator.StringToHash("jump_start");
        private readonly int hStateJumpEnd = Animator.StringToHash("jump_end");
        
        
        private void InitializeInput() {
            actions = new DevonaActions();
            actions.Character.Move.started += (ctx) => moveVector = ctx.ReadValue<Vector2>();
            actions.Character.Move.performed += (ctx) => moveVector = ctx.ReadValue<Vector2>();
            actions.Character.Move.canceled += (ctx) => moveVector = Vector2.zero;
            actions.Character.LightAttack.started += (ctx)=> lightAttackInputTime = Time.unscaledTime;
            actions.Character.HeavyAttack.started += (ctx)=> heavyAttackInputTime = Time.unscaledTime;
            actions.Character.Jump.started += (ctx)=> {
                jumpInputTime = Time.unscaledTime;
                jumpInputDown = true;
            };
            actions.Character.Jump.canceled += (ctx)=> jumpInputDown = false;
        }
    
        private void Awake() {
            InitializeInput();
            
            CharacterAnimator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
            Targeting = GetComponent<CharacterTargeting>();
            CombatVFX = GetComponent<CharacterCombatVFX>();

            jumpLayerIndex =  CharacterAnimator.GetLayerIndex("Jump");
            combatLayerIndex =  CharacterAnimator.GetLayerIndex("Combat");

            m_ComboTree.SetAnimator(CharacterAnimator, combatLayerIndex);
            m_ComboTree.Initialize(this);
            
            m_DamageTrigger.Initialize(this);
        }

        private void Start() {
            GameplayCamera = Camera.main;
            actions.Enable();
            currentLookAngle = transform.rotation.eulerAngles.y;
            worldLookDirection = worldMoveDirection = transform.forward;
        }

        private void OnDestroy() {
            actions.Disable();
        }

        private void FixedUpdate() {
            CurrentCombatLayerState = CharacterAnimator.GetCurrentAnimatorStateInfo(combatLayerIndex);
            NextCombatLayerState = CharacterAnimator.GetNextAnimatorStateInfo(combatLayerIndex);
            
            isPerformingAttack = CurrentCombatLayerState.shortNameHash != hStateAttackNone 
                                 || NextCombatLayerState.shortNameHash != hStateAttackNone && NextCombatLayerState.shortNameHash != 0;
        
            //Input Update
            var characterUp = transform.up;
            var projCamForward=  Vector3.ProjectOnPlane(GameplayCamera.transform.forward, characterUp).normalized;
            var projCamRight=  Vector3.ProjectOnPlane(GameplayCamera.transform.right, characterUp).normalized;
            worldMoveVector = projCamForward * moveVector.y + projCamRight * moveVector.x;

            isMoving = moveVector != Vector2.zero;

            if (isMoving) {
                moveVectorMagnitude = moveVector.magnitude;
                worldMoveDirection = worldMoveVector.normalized;
                Targeting.InputDirection = worldMoveDirection;
            }
            else {
                Targeting.InputDirection = transform.forward;
            }
            
            Targeting.UpdateTargeting();
            
            animatorMoveSpeed = Mathf.MoveTowards(animatorMoveSpeed, moveVectorMagnitude > m_WalkThreshold ? 1 : 0, m_WalkBlendRate * Time.deltaTime);
            
            //Character Turn
            if (!IsAirborne && (!isPerformingAttack || m_ComboTree.CurrentNode.AllowsTurn)) {
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
                    CharacterAnimator.CrossFadeInFixedTime(hStateJumpEnd,0.05f, jumpLayerIndex, 0f);
                    isJumping = false;
                    IsAirborne = false;
                }
            }

            else {
                if (!isPerformingAttack) {
                    if (JumpInput && !isJumping) {
                        CharacterAnimator.CrossFadeInFixedTime(hStateJumpStart, 0.1f, jumpLayerIndex,0f);


                        jumpHold = jumpInputDown;
                        isJumping = true;
                        jumpTimer = 0;
                    }
                
                    //Animator Update
                    CharacterAnimator.SetBool(hBoolIsMoving, isMoving);
                    CharacterAnimator.SetFloat(hFloatMoveSpeed, animatorMoveSpeed);

                    //Ensure Combo Tree is clear 
                    m_ComboTree.ClearCombo();
                }
            }
        
            if (LightAttackInput) {
                if (m_ComboTree.ExecuteCombo(ComboInput.LightAttack)) {
                    OnExecuteCombo();
                }
            }
            else if (HeavyAttackInput) {
                if (m_ComboTree.ExecuteCombo(ComboInput.HeavyAttack)) {
                    OnExecuteCombo();
                }
            }
            
            m_DamageTrigger.UpdateDamageData(m_ComboTree.GetDamageData());
            
            CharacterTurnUpdate();
        }

        private void OnExecuteCombo() {
            UpdateLookAngle(true);
            CharacterAnimator.SetBool(hBoolIsMoving, false);
            CombatVFX.SetDefaultCasterVFX(m_ComboTree.CurrentNode.AttackVFXCaster);
            CombatVFX.SetDefaultTargetVFX(m_ComboTree.CurrentNode.AttackVFXTarget);
            airVelocity.x = 0;
            airVelocity.z = 0;
        }

        private void CharacterTurnUpdate() {
            var turnRate = m_TurnRate.Evaluate(animatorMoveSpeed);
            if (isPerformingAttack) {
                turnRate *= m_ComboTree.CurrentNode.TurnSpeedMultiplier;
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

        public bool LightAttackInput => Time.unscaledTime - lightAttackInputTime < m_InputPressDuration;
        public bool HeavyAttackInput => Time.unscaledTime - heavyAttackInputTime < m_InputPressDuration;
        public bool JumpInput => Time.unscaledTime - jumpInputTime < m_InputPressDuration;
    }
}