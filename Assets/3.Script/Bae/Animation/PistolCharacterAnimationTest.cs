using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolCharacterAnimationTest : MonoBehaviour
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
            PlayAnimationByTrigger("Skill_1");
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PlayAnimationByTrigger("Skill_2");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            PlayAnimationByTrigger("Skill_3");
        }
        else if (Input.GetKeyDown(KeyCode.T))
            PlayAnimationByTrigger("Attacked");
        else if (Input.GetKeyDown(KeyCode.Y))
            PlayAnimationByTrigger("Die");
    }
    
    private void PlayAnimationByTrigger(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
}
