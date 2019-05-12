using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterClimb : CharacterComponent
{
    [Header("References")]
    public Transform climbAnchor;
    public Transform wallRayOrigin;

    [Header("Raycast")]
    public float wallRayLength;
    public float wallRayThickness;
    public float ledgeDepthBias;
    public float climbRayLength;
    public LayerMask climbLayerMask = 1;

    [Header("Timing")]
    public float climbSnapTime = 0.2f;
    public float hangCooldownTime = 0.2f;
    public float hangActionTime = 0.2f;
    public float climbUpActionTime = 0.2f;

    [Header("Timing")]
    public float climbUpForwardOffset = 0.2f;

    public bool IsHanging { get; private set; }
    public bool IsClimbingUp { get; private set; }
    public bool CanExecuteAction { get { return actionCooldown <= Time.time; } }

    private Vector3 wallHitPoint;
    private Vector3 wallNormal;

    private Vector3 groundHitPoint;
    private Vector3 climbPoint;

    private float lastReleaseTime;
    private float climbStartTime;
    private float actionCooldown;

    private void FixedUpdate()
    {
        if (IsClimbingUp)
        {
            if (climbStartTime + climbUpActionTime <= Time.time)
            {
                IsClimbingUp = false;
                SetHanging(false);
            }
        }
        else if (IsHanging)
        {
            if (!CanExecuteAction)
            {
                return;
            }

            if (OwnerCharacter.Input.Crouch)
            {
                lastReleaseTime = Time.time;
                SetHanging(false);
                return;
            }

            if (OwnerCharacter.Input.Jump)
            {
                OwnerCharacter.Locomotion.SetGrounded(true);

                IsClimbingUp = true;
                climbStartTime = Time.time;
                Vector3 climbUpOffset = Vector3.up * (OwnerCharacter.HalfCapsuleHeight + OwnerCharacter.SkinWidth) + -wallNormal * climbUpForwardOffset;

                transform.DOMove(climbPoint + climbUpOffset, climbUpActionTime).SetUpdate(UpdateType.Fixed);
                return;
            }
        }
        else
        {
            if (OwnerCharacter.Locomotion.IsGrounded) return;

            if (lastReleaseTime + hangCooldownTime > Time.time) return;

            Ray wallRay = new Ray(wallRayOrigin.position, wallRayOrigin.forward);
            RaycastHit hitInfo;
            if (Physics.SphereCast(wallRay, wallRayThickness, out hitInfo, wallRayLength, climbLayerMask))
            {
                wallNormal = Vector3.ProjectOnPlane(hitInfo.normal, Vector3.up).normalized;
                wallHitPoint = hitInfo.point;
                Debug.DrawLine(wallRay.origin, wallHitPoint, Color.blue, 2f);
                Debug.DrawRay(wallHitPoint, wallNormal, Color.cyan, 2f);


                Ray groundRay = new Ray(wallHitPoint + wallNormal * OwnerCharacter.Controller.radius, Vector3.down);

                if (Physics.SphereCast(groundRay, OwnerCharacter.Controller.radius, out hitInfo, OwnerCharacter.Controller.height / 2 + OwnerCharacter.Controller.skinWidth, climbLayerMask))
                {
                    Debug.DrawLine(transform.position, hitInfo.point, Color.red, 2f);
                    return;
                }

                Ray ledgeRay = new Ray(wallHitPoint - wallNormal * ledgeDepthBias + Vector3.up * climbRayLength, -Vector3.up);

                if (Physics.Raycast(ledgeRay, out hitInfo, climbRayLength, climbLayerMask))
                {
                    groundHitPoint = hitInfo.point;

                    climbPoint.x = wallHitPoint.x;
                    climbPoint.z = wallHitPoint.z;
                    climbPoint.y = groundHitPoint.y;

                    Debug.DrawLine(ledgeRay.origin, groundHitPoint, Color.red, 2f);
                    actionCooldown = Time.time + hangActionTime;

                    SetHanging(true);
                    BeginEdgeSnapping();
                }
            }
        }
    }

    private void BeginEdgeSnapping()
    {
        Quaternion facingRotation = Quaternion.LookRotation(-wallNormal, transform.up);
        Matrix4x4 facing = Matrix4x4.TRS(transform.position, facingRotation, transform.localScale);
        transform.DORotateQuaternion(facingRotation, climbSnapTime).SetUpdate(UpdateType.Fixed);
        Vector3 offset = climbPoint - facing.MultiplyPoint3x4(climbAnchor.localPosition);
        transform.DOMove(transform.position + offset, climbSnapTime, false).SetUpdate(UpdateType.Fixed);
    }

    private void SetHanging(bool hanging)
    {
        if (IsHanging == hanging) return;

        IsHanging = hanging;
        OwnerCharacter.Controller.enabled = !IsHanging;
        OwnerCharacter.Locomotion.enabled = !IsHanging;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(wallRayOrigin.position, wallRayOrigin.forward * wallRayLength);
    }
}
