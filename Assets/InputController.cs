using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class InputController : MonoBehaviour
{
	[Header("Keyboard")]
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	public KeyCode upKey = KeyCode.W;
	public KeyCode downKey = KeyCode.S;
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode attackKey = KeyCode.J;
	public KeyCode slideKey = KeyCode.K;

	void Start()
	{
		GetComponent<FightingPlayerController>().HandleInput += HandleInput;
	}

	void HandleInput(FightingPlayerController controller)
	{
		bool JUMP_isPressed = false;
		bool JUMP_wasPressed = false;
		bool SLIDE_wasPressed = false;
		bool ATTACK_wasPressed = false;
		Vector2 moveStick = Vector2.zero;

//		if (useKeyboard)
		{
			moveStick.x = ((Input.GetKey(leftKey) ? -1 : 0) + (Input.GetKey(rightKey) ? 1 : 0));
			moveStick.y = ((Input.GetKey(downKey) ? -1 : 0) + (Input.GetKey(upKey) ? 1 : 0));
			JUMP_wasPressed = Input.GetKeyDown(jumpKey);
			JUMP_isPressed = Input.GetKey(jumpKey);
			SLIDE_wasPressed = Input.GetKeyDown(slideKey);
			ATTACK_wasPressed = Input.GetKeyDown(attackKey);
		}

		controller.Input(moveStick, JUMP_isPressed, JUMP_wasPressed, SLIDE_wasPressed, ATTACK_wasPressed);
	}
}
