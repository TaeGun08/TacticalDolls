using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleCharacterAnimationTest : MonoBehaviour
{
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject pistol;
    
    private Animator animator;
    private bool isSwitchingWeapon = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckInput();

        if (isSwitchingWeapon)
        {
            CheckAnimationEnd();
        }
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
            rifle.SetActive(false);
            PlayAnimationByTrigger("Skill_2");
            isSwitchingWeapon = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            PlayAnimationByTrigger("Skill_3");
            isSwitchingWeapon = true;
        }
        else if (Input.GetKeyDown(KeyCode.T))
            PlayAnimationByTrigger("Attacked");
        else if (Input.GetKeyDown(KeyCode.Y))
            PlayAnimationByTrigger("Die");
    }

    private void CheckAnimationEnd()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        
        if (info.IsName("Skill_2") && info.normalizedTime > 0.99f)
        {
            rifle.SetActive(true);

            isSwitchingWeapon = false;
        }
        
        if (info.IsName("Skill_3") && info.normalizedTime > 0.1f)
        {
            rifle.SetActive(false);
            pistol.SetActive(true);
        }
        if (info.IsName("Skill_3") && info.normalizedTime > 0.8f)
        {
            rifle.SetActive(true);
            pistol.SetActive(false);

            isSwitchingWeapon = false;
        }
    }
    
    private void PlayAnimationByTrigger(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
}
