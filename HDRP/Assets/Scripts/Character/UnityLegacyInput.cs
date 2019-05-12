using UnityEngine;

public class UnityLegacyInput : CharacterInput
{
    [SerializeField]
    private string m_HorizontalAxis = "Horizontal";
    [SerializeField]
    private string m_VerticalAxis = "Vertical";

    [SerializeField]
    private string m_JumpButton = "Jump";
    [SerializeField]
    private string m_CrouchButton = string.Empty;

    [SerializeField]
    private string m_Action0Button = string.Empty;
    [SerializeField]
    private string m_Action1Button = string.Empty;
    [SerializeField]
    private string m_Action2Button = string.Empty;
    [SerializeField]
    private string m_Action3Button = string.Empty;

    private void Update()
    {
        if (!string.IsNullOrEmpty(m_HorizontalAxis))
            MoveX = Input.GetAxisRaw(m_HorizontalAxis);

        if (!string.IsNullOrEmpty(m_VerticalAxis))
            MoveY = Input.GetAxisRaw(m_VerticalAxis);

        if (!string.IsNullOrEmpty(m_JumpButton))
            Jump = Input.GetButton(m_JumpButton);

        if (!string.IsNullOrEmpty(m_CrouchButton))
            Crouch = Input.GetButton(m_CrouchButton);

        if (!string.IsNullOrEmpty(m_Action0Button))
            Action0 = Input.GetButton(m_Action0Button);

        if (!string.IsNullOrEmpty(m_Action1Button))
            Action1 = Input.GetButton(m_Action1Button);

        if (!string.IsNullOrEmpty(m_Action2Button))
            Action2 = Input.GetButton(m_Action2Button);

        if (!string.IsNullOrEmpty(m_Action3Button))
            Action3 = Input.GetButton(m_Action3Button);
    }
}
