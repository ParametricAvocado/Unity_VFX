using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputListener : CharacterInput
{
	CharacterControls inputActions;

	private void OnEnable() {
		inputActions = new CharacterControls();

		//inputActions.DefaultMap.Move.performed += OnMove;
		//inputActions.DefaultMap.Jump.performed += OnJump;
		//inputActions.DefaultMap.Sprint.performed += OnSprint;
		//inputActions.DefaultMap.Crouch.performed += OnCrouch;
	}

	private void OnJump(InputAction.CallbackContext context) => Jump = context.ReadValueAsButton();

	private void OnSprint(InputAction.CallbackContext context) => Sprint = context.ReadValueAsButton();

	private void OnCrouch(InputAction.CallbackContext context) => Crouch = context.ReadValueAsButton();


	private void OnMove(InputAction.CallbackContext context) {
		Vector2 moveVec = context.ReadValue<Vector2>();
		MoveX = moveVec.x;
		MoveY = moveVec.y;
	}
}
