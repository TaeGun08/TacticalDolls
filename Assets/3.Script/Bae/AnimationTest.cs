using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (animator.GetBool("isCrouching"))
            {
                animator.SetBool("isCrouching", false);
            }
            else
            {
                animator.SetBool("isCrouching", true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
            PlayAnimationByTrigger("Shoot");
        else if (Input.GetKeyDown(KeyCode.E))
            PlayAnimationByTrigger("Hit");
        else if (Input.GetKeyDown(KeyCode.R))
            PlayAnimationByTrigger("Die");
    }

    private void PlayAnimationByTrigger(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
    
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    // private void OnAnimatorIK(int layerIndex)
    // {
    //     if (rightHandTarget != null)
    //     {
    //         animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
    //         animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    //         animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
    //         animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
    //     }
    //
    //     if (leftHandTarget != null)
    //     {
    //         animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
    //         animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
    //         animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
    //         animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
    //     }
    // }
}
