using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : CharacterComponent
{
    private const float MIN_MOVE_SPEED = 0.01f;
    [Header("Ground Behaviour")]
    public float m_MinGroundedSpeed = 1.5f;
    public float m_MaxGroundedSpeed = 8.0f;

    public float m_GroundedAcceleration = 20.0f;
    public float m_GroundRayBias = 0.08f;
    public LayerMask m_GroundMask;

    [Header("Jump Behaviour")]
    public float m_AirAcceleration = 20.0f;
    public float m_GravityScale = 1.0f;
    public float m_JumpVelocity = 20.0f;
    public float m_JumpHoldTime = 0.3f;
    public float m_JumpCooldownTime = 0.1f;

    [Header("Input")]
    public float m_TurnRate;
    public float m_Deadzone = 0.1f;

    [SerializeField]
    [HideInInspector]
    private bool m_DebugInput;

    //Input
    private bool moveInput;
    private bool jumpInput;
    private float jumpHoldTimeLeft;
    private Vector2 rawMoveInput;
    private Vector2 moveInputDirection;
    private float moveInputMagnitude;

    //World Space Input
    private Vector3 wsRawInput;
    private Vector3 wsInputDirection;

    //Axes
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 characterUp;

    //Velocity
    private Vector3 planarVelocity = Vector3.zero;
    private Vector3 airVelocity = Vector3.zero;

    //Control
    private Vector3 targetPlanarVelocity;
    private Vector3 targetPlanarDelta;
    private Vector3 lookDirection;
    private Quaternion targetLookRotation;
    private float lastLandingTime;

    //Properties
    public bool IsGrounded { get; private set; }
    public Vector3 Acceleration { get; private set; }
    public float Speed { get; private set; }
    public float VerticalSpeed { get; private set; }
    public float Angle { get; private set; }
    public bool IsMoving { get; private set; }

    public bool CanJump => lastLandingTime + m_JumpCooldownTime <= Time.time;

    private void OnDisable()
    {
        airVelocity = Vector3.zero;
        planarVelocity = Vector3.zero;
        Acceleration = Vector3.zero;
        Speed = 0;
        VerticalSpeed = 0;
        IsMoving = false;
    }

    private void Start()
    {
        targetLookRotation = transform.rotation;
    }

    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
    }

    private void HandleMoveInput()
    {
        rawMoveInput.x = OwnerCharacter.Input.MoveX;
        rawMoveInput.y = OwnerCharacter.Input.MoveY;
        moveInput = rawMoveInput.magnitude >= m_Deadzone;

        if (moveInput)
        {
            moveInputMagnitude = Mathf.InverseLerp(m_Deadzone, 1.0f, rawMoveInput.magnitude);
            moveInputDirection = rawMoveInput.normalized;
        }
        else
        {
            rawMoveInput = Vector2.zero;
            moveInputMagnitude = 0.0f;
            moveInputDirection = Vector2.zero;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput != OwnerCharacter.Input.Jump)
        {
            jumpInput = OwnerCharacter.Input.Jump;
            if (!jumpInput && jumpHoldTimeLeft > 0)
            {
                jumpHoldTimeLeft = 0;
            }
        }
    }

    private void GroundDetection()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        if (Physics.SphereCast(groundRay, OwnerCharacter.Controller.radius, out hitInfo, (OwnerCharacter.Controller.height / 2) + m_GroundRayBias - OwnerCharacter.Controller.radius, m_GroundMask, QueryTriggerInteraction.Ignore))
        //if (Physics.Raycast(groundRay, out hitInfo, m_GroundRayDistance, m_GroundMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red);

            Land();
        }
        else
        {
            SetGrounded(false);
        }

    }

    public void SetGrounded(bool grounded)
    {
        if (IsGrounded == grounded) return;

        IsGrounded = grounded;

        if (IsGrounded)
        {
            airVelocity = Vector3.zero;
            lastLandingTime = Time.time;
        }
    }

    private void Land()
    {
        SetGrounded(true);
    }

    private void FixedUpdate()
    {
        ConvertRawInput();
        GroundDetection();

        if (IsGrounded)
        {
            if (jumpInput && CanJump)
            {
                BeginJump();
            }
        }
        else
        {
            if (!HoldJump())
            {
                Fall();
            }
        }

        if (moveInput)
        {
            targetPlanarVelocity = wsInputDirection * Mathf.Lerp(m_MinGroundedSpeed, m_MaxGroundedSpeed, moveInputMagnitude);
        }
        else
        {
            targetPlanarVelocity = Vector3.zero;
        }


        Acceleration = Vector3.MoveTowards(planarVelocity, targetPlanarVelocity, (IsGrounded ? m_GroundedAcceleration : m_AirAcceleration) * Time.fixedDeltaTime) - planarVelocity;
        planarVelocity += Acceleration;

        Speed = planarVelocity.magnitude;
        VerticalSpeed = airVelocity.y;
        IsMoving = Speed > MIN_MOVE_SPEED;

        if (IsMoving)
        {
            lookDirection = planarVelocity.normalized;
            targetLookRotation = Quaternion.LookRotation(lookDirection);
        }
        else
        {
            planarVelocity = Vector3.zero;
            Speed = 0.0f;
        }

        OwnerCharacter.Controller.Move((planarVelocity + airVelocity) * Time.fixedDeltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetLookRotation, m_TurnRate * Time.fixedDeltaTime);
    }

    private void Fall()
    {
        airVelocity += Physics.gravity * m_GravityScale * Time.fixedDeltaTime;
    }

    private bool HoldJump()
    {
        if (jumpHoldTimeLeft > 0)
        {
            airVelocity = transform.up * m_JumpVelocity;
            jumpHoldTimeLeft -= Time.deltaTime;
            return false;
        }
        else
        {
            return false;
        }
    }

    private void BeginJump()
    {
        jumpHoldTimeLeft = m_JumpHoldTime;
        airVelocity = transform.up * m_JumpVelocity;
        SetGrounded(false);
    }

    private void ConvertRawInput()
    {
        characterUp = transform.up;
        cameraForward = Camera.main.transform.forward;
        cameraRight = Camera.main.transform.right;
        Vector3.OrthoNormalize(ref characterUp, ref cameraForward, ref cameraRight);

        wsRawInput = Vector3.ClampMagnitude((rawMoveInput.x * cameraRight + rawMoveInput.y * cameraForward), 1);

        wsInputDirection = moveInputDirection.x * cameraRight + moveInputDirection.y * cameraForward;

        if (m_DebugInput)
        {
            Debug.DrawRay(transform.position, cameraForward, Color.blue);
            Debug.DrawRay(transform.position, cameraRight, Color.red);
            Debug.DrawRay(transform.position, wsRawInput, Color.magenta);
        }
    }
}
