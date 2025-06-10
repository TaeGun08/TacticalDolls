using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkillGUI : MonoBehaviour
{
    private string selectedSkill = "None";
    [SerializeField] private InGameUIManager inGameUIManager;
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 150), "Choose a Skill");

        if (GUI.Button(new Rect(30, 40, 160, 30), "Skill 1"))
        {
            inGameUIManager.OnClickedConfirmSelectSkillButtonWrapper1();
        }

        if (GUI.Button(new Rect(30, 80, 160, 30), "Skill 2"))
        {
            inGameUIManager.OnClickedConfirmSelectSkillButtonWrapper2();
        }

        if (GUI.Button(new Rect(30, 120, 160, 30), "Skill 3"))
        {
            inGameUIManager.OnClickedConfirmSelectSkillButtonWrapper3();
        }

        GUI.Label(new Rect(10, 170, 200, 30), $"Selected: {selectedSkill}");
    }

    void UseSkill(int skillNumber)
    {
        selectedSkill = $"Skill {skillNumber}";
        Debug.Log($"Skill {skillNumber} used!");
    }
}
