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

        if (targetData is CharacterData character)
        {
            OpenCharacterSkills(character);
        }
        else if (targetData is EnemyData enemy)
        {
            OpenEnemySkills(enemy);
        }
    }

    private void OpenCharacterSkills(CharacterData characterData)
    {
        for (int i = 0; i < skillNameTexts.Length; i++)
        {
            if (i < characterData.Skills.Count && characterData.Skills[i] != null)
            {
                skillNameTexts[i].text = characterData.Skills[i].Name;
                skillButtons[i].interactable = true;

                int capturedIndex = i;
                skillButtons[i].onClick.RemoveAllListeners();
                skillButtons[i].onClick.AddListener(() => OnSkillButtonClicked(capturedIndex));
            }
        }
    }

    private void OpenEnemySkills(EnemyData enemyData)
    {
        for (int i = 0; i < skillNameTexts.Length; i++)
        {
            if (i < enemyData.Skills.Count && enemyData.Skills[i] != null)
            {
                skillNameTexts[i].text = enemyData.Skills[i].Name;
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
        if (currentTarget is CharacterData character)
        {
            if (index >= character.Skills.Count) return;

            SkillSO skill = character.Skills[index];
            if (skill == null) return;

            SkillRangeSystem.Instance.ClearUsableTiles();
            SkillRangeSystem.Instance.ClearDamageAbles();
            SkillRangeSystem.Instance.ShowSkillRange(character, index);

            TestCombat(index);
        }
        else if (currentTarget is EnemyData enemy)
        {
            if (index >= enemy.Skills.Count) return;

            SkillSO skill = enemy.Skills[index];
            if (skill == null) return;

            SkillRangeSystem.Instance.ClearUsableTiles();
            SkillRangeSystem.Instance.ClearDamageAbles();
            SkillRangeSystem.Instance.ShowSkillRange(enemy, index);

            TestCombat(index);
        }
        else
        {
            Debug.LogWarning("스킬을 사용할 수 있는 대상이 아닙니다.");
        }
    }


    public void TestCombat(int skillIndex)
    {
        Debug.Log($"skillIndex: {skillIndex}");
        CombatSystem.Instance.ExecuteSkill(currentTarget, skillIndex);
    }
}
