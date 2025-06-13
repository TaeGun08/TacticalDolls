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

    private IDamageAble currentTarget;
    
    private void Start()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i;
            skillButtons[i].onClick.AddListener(() => OnSkillButtonClicked(index));
        }
    }

    public void Open(IDamageAble targetData)
    {
        panel.SetActive(true);
        currentTarget = targetData;

        OpenSkills(currentTarget);
    }

    private void OpenSkills(IDamageAble unit)
    {
        for (int i = 0; i < skillNameTexts.Length; i++)
        {
            if (i < unit.Stat.Skills.Count && unit.Stat.Skills[i] != null)
            {
                skillNameTexts[i].text = unit.Stat.Skills[i].Name;
                skillButtons[i].interactable = true;

                int capturedIndex = i;
                skillButtons[i].onClick.RemoveAllListeners();
                skillButtons[i].onClick.AddListener(() => OnSkillButtonClicked(capturedIndex));
            }
        }
    }
    
    public void Close()
    {
        panel.SetActive(false);
        currentTarget = null;
    }

    private void OnSkillButtonClicked(int index)
    {
        if (index >= currentTarget.Stat.Skills.Count) return;

        SkillSO skill = currentTarget.Stat.Skills[index];
        if (skill == null) return;

        SkillRangeSystem.Instance.ClearUsableTiles();
        SkillRangeSystem.Instance.ClearDamageAbles();
        SkillRangeSystem.Instance.ShowSkillRange(currentTarget, index);

        TestCombat(index);
    }

    public void TestCombat(int skillIndex)
    {
        Debug.Log($"skillIndex: {skillIndex}");
        CombatSystem.Instance.ExecuteSkill(currentTarget, skillIndex);
    }
}
