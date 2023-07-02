using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerController", menuName = "NaughtyCharacter/PlayerController")]
public class PC : Ctrl
{
    public float ControlRotationSensitivity = 3.0f;

    private PlayerInput _playerInput;
    private PlayerCamera _playerCamera;

    public override void Init()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _playerCamera = FindObjectOfType<PlayerCamera>();
    }

    public override void OnIChFixedUpdate()
	{
		_playerCamera.SetPosition(ich.transform.position);
		_playerCamera.SetControlRotation(ich.GetControlRotation());
	}

    public override void OnIChUpdate()
    {
        _playerInput.UpdateInput();

        UpdateControlRotation();
		ich.SetMovementInput(GetMovementInput());
		ich.SetJumpInput(_playerInput.JumpInput);
	}
	private void UpdateControlRotation()
	{
		Vector2 camInput = _playerInput.CameraInput;
		Vector2 controlRotation = ich.GetControlRotation();

		// Adjust the pitch angle (X Rotation)
		float pitchAngle = controlRotation.x;
		pitchAngle -= camInput.y * ControlRotationSensitivity;

		// Adjust the yaw angle (Y Rotation)
		float yawAngle = controlRotation.y;
		yawAngle += camInput.x * ControlRotationSensitivity;

		controlRotation = new Vector2(pitchAngle, yawAngle);
		ich.SetControlRotation(controlRotation);
	}

	private Vector3 GetMovementInput()
	{
		// Calculate the move direction relative to the character's yaw rotation
		Quaternion yawRotation = Quaternion.Euler(0.0f, ich.GetControlRotation().y, 0.0f);
		Vector3 forward = yawRotation * Vector3.forward;
		Vector3 right = yawRotation * Vector3.right;
		Vector3 movementInput = (forward * _playerInput.MoveInput.y + right * _playerInput.MoveInput.x);

		if (movementInput.sqrMagnitude > 1f)
		{
			movementInput.Normalize();
		}

		return movementInput;
	}
}
