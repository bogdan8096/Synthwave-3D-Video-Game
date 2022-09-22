using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public static AnimationHandler Instance { set; get; }

    public Animator animator;


    private int a_DyingHash;
    private int a_JumpingHash;
    private int a_SlidingHash;
    private int a_AnimationSpeedHash;
    private int a_RunningSpeedHash;

    private void Awake()
    {
        Instance = this;

        animator = GetComponent<Animator>();

        a_DyingHash             = Animator.StringToHash("isDying");
        a_JumpingHash           = Animator.StringToHash("isJumping");
        a_SlidingHash           = Animator.StringToHash("isSliding");
        a_AnimationSpeedHash    = Animator.StringToHash("animationSpeed");
        a_RunningSpeedHash      = Animator.StringToHash("runningSpeed");
    }
  

    public void SetJumpAnimation()
    {
        animator.SetBool(a_JumpingHash, true);
    }
    public void ResetJumpAnimation()
    {
        animator.SetBool(a_JumpingHash, false);
    }

    public void SetSlideAnimation()
    {
        animator.SetBool(a_SlidingHash, true);
    }
    public void ResetSlideAnimation()
    {
        animator.SetBool(a_SlidingHash, false);
    }


    public void TriggerDeathAnimation()
    {
        animator.SetTrigger(a_DyingHash);
    }

    public void SetAnimationSpeed(float animationSpeed)
    {
        animator.SetFloat(a_AnimationSpeedHash, animationSpeed);
    }

    public void SetAnimatorState(bool animatorEnabled)
    {
        animator.enabled = animatorEnabled;
    }

    public void RebindAnimator()
    {
        animator.Rebind();
    }

    public void SetPauseAnimationSpeed(bool gamePaused)
    {
        if (gamePaused)
        {
            animator.SetFloat(a_RunningSpeedHash, 0f);
            animator.SetFloat(a_AnimationSpeedHash, 0f);
        }
        else 
        {
            animator.SetFloat(a_RunningSpeedHash, 1f);
            animator.SetFloat(a_AnimationSpeedHash, 1f); 
        }
    }
}
