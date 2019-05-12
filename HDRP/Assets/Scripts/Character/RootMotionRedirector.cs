using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionRedirector : MonoBehaviour
{
    public Transform targetTransform = null;
    public Animator animator = null;

    public bool affectPosition = true;
    public bool affectRotation = true;

    CharacterController controller;
    Rigidbody rigidbody;

    private void Start()
    {
        controller = targetTransform.GetComponent<CharacterController>();
        rigidbody = targetTransform.GetComponent<Rigidbody>();

    }

    private void OnAnimatorMove()
    {
        if (!animator || !targetTransform) return;

        if (affectPosition)
        {
            if (controller)
            {
                controller.Move(animator.deltaPosition);
            }
            else if (rigidbody)
            {
                rigidbody.MovePosition(rigidbody.position + animator.deltaPosition);
            }
            else
            {
                targetTransform.Translate(animator.deltaPosition);
            }
        }

        if (affectRotation)
        {
            targetTransform.rotation = animator.deltaRotation * targetTransform.rotation;
        }
    }
}
