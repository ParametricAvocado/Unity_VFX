using UnityEngine;

namespace DevonaProject {
    public class PlayerCharacter : Character {
        
        [Header("Input")]
        [SerializeField] private float m_InputPressDuration = 0.1f;
        
        private DevonaActions actions;
        
        protected Vector2 moveVector;
        protected float lightAttackInputTime = float.MinValue;
        protected float heavyAttackInputTime = float.MinValue;
        protected float jumpInputTime = float.MinValue;
        
        protected override void InitializeInput() {
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

        protected override void Start() {
            base.Start();
            actions.Enable();
        }

        protected override void OnDestroy() {
            actions.Disable();
            base.OnDestroy();
        }

        protected override void UpdateMoveVector() {
            var characterUp = transform.up;
            var projCamForward=  Vector3.ProjectOnPlane(GameplayCamera.transform.forward, characterUp).normalized;
            var projCamRight=  Vector3.ProjectOnPlane(GameplayCamera.transform.right, characterUp).normalized;
            worldMoveVector = projCamForward * moveVector.y + projCamRight * moveVector.x;
            
            base.UpdateMoveVector();
        }

        public override bool LightAttackInput => Time.unscaledTime - lightAttackInputTime < m_InputPressDuration;
        public override bool HeavyAttackInput => Time.unscaledTime - heavyAttackInputTime < m_InputPressDuration;
        public override bool JumpInput => Time.unscaledTime - jumpInputTime < m_InputPressDuration;
    }
}