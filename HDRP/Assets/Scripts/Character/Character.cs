using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_CharacterController;
    [SerializeField]
    private CharacterLocomotion m_CharacterLocomotion;
    [SerializeField]
    private CharacterClimb m_CharacterClimb;
    [SerializeField]
    private CharacterInput m_CharacterInput;

	[SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private CharacterAnimation m_CharacterAnimation;

    public CharacterController Controller => m_CharacterController;
    public CharacterLocomotion Locomotion => m_CharacterLocomotion;
    public CharacterClimb Climb => m_CharacterClimb;
    public CharacterInput Input => m_CharacterInput;

    public Animator Animator => m_Animator;
    public CharacterAnimation Animation => m_CharacterAnimation;

    public float HalfCapsuleHeight => Controller.height / 2;
    public float SkinWidth => Controller.skinWidth;



    private void Awake()
    {
        if (!Input)
        {
            m_CharacterInput = gameObject.AddComponent<DummyInput>();
        }
    }
}
