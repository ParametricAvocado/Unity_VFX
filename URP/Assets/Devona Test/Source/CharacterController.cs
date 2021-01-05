using UnityEngine;

namespace DevonaProject {
    public enum ComboInput {
        LightAttack,
        HeavyAttack
    }

    public class CharacterController : MonoBehaviour {
        //Inspector
        [Header("Components")] 
        [SerializeField] private DamageTrigger m_DamageTrigger;
        
        [Header("Data")]
        [SerializeField] private ComboTree m_ComboTree;

        [Header("Input")]
        [SerializeField] private float m_InputPressDuration = 0.1f;

        [Header("Animation")]
        [SerializeField] AnimationCurve m_TurnRate = AnimationCurve.Linear(0,1000,1,800);
        [SerializeField] private float m_WalkThreshold = 0.5f;
        [SerializeField] private float m_WalkBlendRate = 2f;


        //Components
        public Animator CharacterAnimator { get; private set; }
        public Camera GameplayCamera { get; private set; }
        public CharacterTargeting Targeting { get; private set; }
        public CharacterCombatVFX CombatVFX { get; private set; }

        private DevonaActions actions;

        //Input
        private Vector2 moveVector;
        private float lightAttackInputTime = -1;
        private float heavyAttackInputTime = -1;

        //Locomotion
        private float moveVectorMagnitude;
        private Vector3 worldMoveDirection;
        private Vector3 worldMoveVector;
        private Vector3 worldLookDirection;
        private bool isMoving;
    
        //Visual
        private float animatorMoveSpeed;
        private float currentLookAngle;
        private float targetLookAngle;
    
        //Animator State
        private int combatLayerIndex;
        public AnimatorStateInfo CurrentCombatLayerState;
        public AnimatorStateInfo NextCombatLayerState;
        private bool isPerformingAttack;

        private readonly int hBoolIsMoving = Animator.StringToHash("IsMoving");
        private readonly int hFloatMoveSpeed = Animator.StringToHash("MoveSpeed");

        private readonly int hStateAttackNone = Animator.StringToHash("att_none");

        private void InitializeInput() {
            actions = new DevonaActions();
            actions.Character.Move.started += (ctx) => moveVector = ctx.ReadValue<Vector2>();
            actions.Character.Move.performed += (ctx) => moveVector = ctx.ReadValue<Vector2>();
            actions.Character.Move.canceled += (ctx) => moveVector = Vector2.zero;
            actions.Character.LightAttack.started += (ctx)=> lightAttackInputTime = Time.unscaledTime;
            actions.Character.HeavyAttack.started += (ctx)=> heavyAttackInputTime = Time.unscaledTime;
        }
    
        private void Awake() {
            InitializeInput();
            
            CharacterAnimator = GetComponent<Animator>();
            Targeting = GetComponent<CharacterTargeting>();
            CombatVFX = GetComponent<CharacterCombatVFX>();
            
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

            animatorMoveSpeed = Mathf.MoveTowards(animatorMoveSpeed, moveVectorMagnitude > m_WalkThreshold ? 1 : 0, m_WalkBlendRate * Time.deltaTime);
        
            if (!isPerformingAttack) {
                //Character Turn
                UpdateLookAngle(false);

                //Animator Update
                CharacterAnimator.SetBool(hBoolIsMoving, isMoving);
                CharacterAnimator.SetFloat(hFloatMoveSpeed, animatorMoveSpeed);

            
                //Ensure Combo Tree is clear 
                m_ComboTree.ClearCombo();
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
            else if (isPerformingAttack && m_ComboTree.CurrentNode.AllowsTurn) {
                UpdateLookAngle(true);
            }
            
            m_DamageTrigger.UpdateDamageData(m_ComboTree.GetDamageData());
            
            CharacterTurnUpdate();
        }

        private void OnExecuteCombo() {
            UpdateLookAngle(true);
            CharacterAnimator.SetBool(hBoolIsMoving, false);
            CombatVFX.SetDefaultCasterVFX(m_ComboTree.CurrentNode.AttackVFXCaster);
            CombatVFX.SetDefaultTargetVFX(m_ComboTree.CurrentNode.AttackVFXTarget);
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
    }
}