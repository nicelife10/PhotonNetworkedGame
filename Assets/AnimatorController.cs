using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class AnimatorController : MonoBehaviour
{
    public Animator Anim;

    void ResetAllBools()
    {
        Anim.ResetTrigger("Jump");
        Anim.ResetTrigger("Jump2");
        Anim.SetBool("Idle", false);
        Anim.SetBool("crouch", false);
        Anim.SetBool("LeftWalk", false);
        Anim.SetBool("RightWalk", false);
    }

    public bool IsWalkPlaying => Anim.GetCurrentAnimatorStateInfo(0).IsName("LeftWalk") || Anim.GetCurrentAnimatorStateInfo(0).IsName("RightWalk");
    public bool IsJumping => Anim.GetCurrentAnimatorStateInfo(0).IsName("Jump2") || Anim.GetCurrentAnimatorStateInfo(0).IsName("Jump2");

    public void PlayIdle()
    {
        ResetAllBools();
        Anim.SetBool("Idle", true);
    }
    public void PlayCrouch()
    {
        ResetAllBools();
        Anim.SetBool("crouch", true);
    }

    #region Triggers
    //Fix IT
    public void PlayDash()
    {
        Anim.SetBool("crouch", true);
    }

    public void PlayJump()
    {
        Anim.SetTrigger("Jump");
    }
    public void PlayJump2()
    {
        Anim.SetTrigger("Jump2");
    }
    #endregion Triggers

    #region Leftie
    public void PlayLeftWalk()
    {
        ResetAllBools();
        Anim.SetBool("LeftWalk", true);
    }
    public void PlayLeftPunch()
    {
        Anim.SetTrigger("LeftPunch");
    }
    public void PlayLeftKick()
    {
        Anim.SetTrigger("LeftKick");
    }
    #endregion Leftie

    #region Rightie
    public void PlayRightWalk()
    {
        ResetAllBools();
        Anim.SetBool("RightWalk", true);
    }
    public void PlayRightPunch()
    {
        Anim.SetTrigger("RightPunch");
    }
    public void PlayRightKick()
    {
        Anim.SetTrigger("RightKick");
    }
    #endregion Rightie

}
