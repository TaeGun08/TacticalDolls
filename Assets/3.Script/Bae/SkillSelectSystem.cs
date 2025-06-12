using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectSystem : MonoBehaviour
{
    public GameObject panel;
    public Button[] skillButtons = new Button[4];
    public TMP_Text[] skillNameTexts = new TMP_Text[4];

    private CharacterData currentCharacter;
    
    private void Start()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i;
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClicked(index));
        }
    }
    
    public void Open(CharacterData characterData)
    {
        currentCharacter = characterData;
        
        for (int i = 0; i < skillNameTexts.Length; i++)
        {
            if (i < characterData.Skills.Count && characterData.Skills[i] != null)
            {
                skillNameTexts[i].text = characterData.Skills[i].Name;
                skillButtons[i].interactable = true;
            }
            else
            {
                skillNameTexts[i].text = "Empty";
                skillButtons[i].interactable = false;
            }
        }
        
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
    
    private void OnSkillButtonClicked(int index)
    {
        if (currentCharacter == null || index >= currentCharacter.Skills.Count)
            return;

        SkillSO skill = currentCharacter.Skills[index];
        if (skill == null)
            return;

        // 범위 표시 요청
        SkillRangeSystem.Instance.ShowSkillRange(currentCharacter, index);
    }
}
