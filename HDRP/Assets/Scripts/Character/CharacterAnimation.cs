using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : CharacterComponent
{
    private const float ACCEL_DEADZONE = 0.01f;

    [Range(0, 20)]
    public float m_LeanSmoothing = 0.1f;

    //Locomotion
    private static int hMove = Animator.StringToHash("IsMoving");
    private static int hGrounded = Animator.StringToHash("IsGrounded");
    private static int hSpeed = Animator.StringToHash("Speed");
    private static int hVertSpeed = Animator.StringToHash("VerticalSpeed");
    private static int hAngle = Animator.StringToHash("Angle");

    private static int hLean = Animator.StringToHash("Lean");
    private static int hLeanAngle = Animator.StringToHash("LeanAngle");
    private Vector3 smoothAcceleration;

    //Climbing
    private static int hHanging = Animator.StringToHash("IsHanging");
    private static int hIsClimbingUp = Animator.StringToHash("IsClimbingUp");


    void FixedUpdate()
    {
        if (!OwnerCharacter.Animator) return;

        UpdateLocomotion(OwnerCharacter.Animator, OwnerCharacter.Locomotion);
        UpdateClimb(OwnerCharacter.Animator, OwnerCharacter.Climb);
    }


    private void UpdateLocomotion(Animator animator, CharacterLocomotion locomotion)
    {
        if (!locomotion) return;

        animator.SetBool(hMove, locomotion.IsMoving);
        animator.SetFloat(hSpeed, locomotion.Speed);
        animator.SetFloat(hAngle, locomotion.Angle);

        animator.SetBool(hGrounded, locomotion.IsGrounded);
        if (!locomotion.IsGrounded)
        {
            animator.SetFloat(hVertSpeed, locomotion.VerticalSpeed);
        }

        smoothAcceleration = Vector3.MoveTowards(smoothAcceleration, locomotion.Acceleration, m_LeanSmoothing * Time.fixedDeltaTime);
        if (smoothAcceleration.magnitude > ACCEL_DEADZONE)
        {
            animator.SetFloat(hLean, smoothAcceleration.magnitude);
            animator.SetFloat(hLeanAngle, Vector3.SignedAngle(transform.forward, smoothAcceleration, transform.up));
        }
        else
        {
            animator.SetFloat(hLean, 0);
            animator.SetFloat(hLeanAngle, 0);
        }
    }

    private void UpdateClimb(Animator animator, CharacterClimb climb)
    {
        if (!climb) return;

        animator.SetBool(hHanging, climb.IsHanging);
        animator.SetBool(hIsClimbingUp, climb.IsClimbingUp);
    }
}
