using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerStates
    {
        Idle,
        crouch,
        Dash,
        Jump,
        Jump2,
        LeftKick,
        LeftPunch,
        LeftWalk,
        RightKick,
        RightPunch,
        RightWalk
    }
    public PlayerStates CurrentState;
    //public SkeletonAnimation SkeletonAnimation;
    //public AnimationReferenceAsset idle, rightWalk;
    //public string currentState;

    public float speed;

    [Header("InputHelpers")]
    public float movement;
    public float vertical;
    public float JumpPressedTime = -2;

    public bool LeftKick;
    public bool LeftPunch;
    public bool RightKick;
    public bool RightPunch;
    public float NormalAttackKeyPressTimeStamp;
    public float NormalAttackWaitTime;

    public bool PunchCombo;
    public bool KickCombo;
    public bool PunchKickCombo;
    public bool SpecialMove;


    public bool canJump()
    {
        return (Time.time - JumpPressedTime) < 0.15f;
    }
    public bool jumpCoolDownPassed()
    {
        return (Time.time - JumpPressedTime) > 0.45f;
    }

    public Vector2 force;
    public bool jump;
    public bool sit;

    [Space(20)]
    [Header("Important Stuff")]
    public Rigidbody2D rigidbody;
    public string currentAnimation;
    public AnimatorController AnimatorController;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        SetCharacterState(PlayerStates.Idle);
        rigidbody = GetComponent<Rigidbody2D>();
        InvokeRepeating(nameof(Testing), 1f, 1f);
    }

    void Testing()
    {
        int[] marks = new int[] { 99, 98, 92, 97, 95 };
        foreach (int x in marks)
        {
            if (x == 98)
                break;
            Debug.Log(x);
        }
    }

    private void Update()
    {
        GetInput();
        GetAttackInputs();
        if (AnimatorController.IsJumping)
        {
            vertical = 0;
        }

    }
    private void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleVerticalMovement(); //Sit and Stand
    }

    void GetInput()
    {
        //Handle Horizontal Movement
        movement = Input.GetAxis("Horizontal");

        //HandleUpDownMovement
        if(!AnimatorController.IsJumping)
            vertical = Input.GetAxis("Vertical");

        if(vertical > 0 && jumpCoolDownPassed())
        {            
            jump = true;
            JumpPressedTime = Time.time;
            sit = false;
        }
        else if(vertical < -0.5)
        {
            jump = false;
            sit = true;
        }
        else
        {
            jump = false;
            sit = false;
        }


        if(Mathf.Abs(movement) > 0f)
        {
            vertical = 0;
        }
    }
    void GetAttackInputs()
    {
        if (Input.GetKeyDown(KeyCode.L)) //LeftKick
        {
            LeftKick = true;
            NormalAttackKeyPressTimeStamp = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.K)) //RightKick
        {
            RightKick = true;
            NormalAttackKeyPressTimeStamp = Time.time;
        }

        //Check for combo
        if(Time.time - NormalAttackKeyPressTimeStamp < NormalAttackWaitTime)
        {

        }
    }

    public void HandleHorizontalMovement()
    {
        float moveforce = movement * speed;
        force = new Vector2(moveforce, 0);

        if(AnimatorController.IsWalkPlaying && sit == false) //not walking or trying to sit
            rigidbody.AddForce(force);

        if (movement > 0)
        {
            SetCharacterState(PlayerStates.RightWalk);
        }
        else if (movement < 0)
        {
            SetCharacterState(PlayerStates.LeftWalk);
        }
        else
        {
            SetCharacterState(PlayerStates.Idle);
        }
    }


    public void HandleVerticalMovement()
    {
        if(sit)
        {
//            if(Mathf.Abs(movement) < 0.5)
            {
                SetCharacterState(PlayerStates.crouch);
            }
        }
        else if(canJump())
        {
            if(!AnimatorController.IsJumping)
                SetCharacterState(PlayerStates.Jump);
        }
    }


    //checks character state and sets the animation accordingly
    public void SetCharacterState(PlayerStates state)
    {
        if (CurrentState == state)
            return;

        CurrentState = state;
        switch (CurrentState)
        {
            case PlayerStates.Idle:
                //anim.SetBool("LeftWalk", false);
                //anim.SetBool("RightWalk", false);
                AnimatorController.PlayIdle();
                break;
            case PlayerStates.crouch:
                AnimatorController.PlayCrouch();
                break;
            case PlayerStates.Dash:
                break;
            case PlayerStates.Jump:
                AnimatorController.PlayJump();
                break;
            case PlayerStates.Jump2:
                break;
            case PlayerStates.LeftKick:
                break;
            case PlayerStates.LeftPunch:
                break;
            case PlayerStates.LeftWalk:
                //anim.SetBool("LeftWalk", true);
                //anim.SetBool("RightWalk", false);
                AnimatorController.PlayLeftWalk();
                break;
            case PlayerStates.RightKick:
                break;
            case PlayerStates.RightPunch:
                break;
            case PlayerStates.RightWalk:
                //anim.SetBool("LeftWalk", false);
                //anim.SetBool("RightWalk", true);
                AnimatorController.PlayRightWalk();
                break;
        }
    }
}
