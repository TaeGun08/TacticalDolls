using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public PlayerDataSample player;
    public List<int> usingCharacter;

    public GameObject SelectedCharacterPanel;
    public Button StartBtn;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = new PlayerDataSample();

        // test data 생성
        TestPlayerHasCharacter();

        StartBtn.onClick.AddListener(StartGame);
    }

    public void TestPlayerHasCharacter()
    {
        var sampleCharacters = new List<CharacterDataSample>
        {
            new CharacterDataSample
            {
                characterCode = 0,
                level = 1,
                //weapon = new WeaponDataSample { weaponCode = 0, level = 1 },
                skills = new SkillDataSample[]
                {
                    new SkillDataSample { skillCode = 1, level = 1 },
                    new SkillDataSample { skillCode = 2, level = 3 }
                }
            },
            new CharacterDataSample
            {
                characterCode = 1,
                level = 2,
                skills = new SkillDataSample[]
                {
                    new SkillDataSample { skillCode = 0, level = 1 },
                    new SkillDataSample { skillCode = 2, level = 3 }
                }
            },
            new CharacterDataSample
            {
                characterCode = 2,
                level = 3,
                skills = new SkillDataSample[]
                {
                    new SkillDataSample { skillCode = 0, level = 1 },
                    new SkillDataSample { skillCode = 2, level = 3 }
                }
            }
        };

        foreach (var sample in sampleCharacters)
        {
            var initializedSample = InitializeCharacterSampleData(sample);
            player.HasCharacter.Add(initializedSample);
        }
    }
    
    public CharacterDataSample InitializeCharacterSampleData(CharacterDataSample sample)
    {
        CharacterData prefabData = GameManager.Instance.CharacterTable
            .GetPrefabByIndex(sample.characterCode)
            .GetComponent<CharacterData>();

        // 레벨 기준으로 스탯 생성
        StatData stat = prefabData.CalculateStatFromLevel(prefabData.Stat.Level);

        // 스킬 설정
        // stat.Skills = new List<SkillSO>();
        // foreach (var skillData in sample.skills)
        // {
        //     SkillSO skill = GameManager.Instance.SkillTable.GetSkillByCode(skillData.skillCode);
        //     if (skill != null)
        //     {
        //         stat.Skills.Add(skill);
        //     }
        // }

        // **중요: stat을 실제로 prefabData에 반영**
        typeof(CharacterData)
            .GetField("stat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(prefabData, stat);

        return sample;
    }
    
    // 임시 데이터 생성
    // public CharacterDataSample InitializeCharacterSampleData(CharacterDataSample sample)
    // {
    //     CharacterData prefabData = GameManager.Instance.CharacterTable
    //         .GetPrefabByIndex(sample.characterCode)
    //         .GetComponent<CharacterData>();
    //
    //     StatData stat = prefabData.CalculateStatFromLevel(sample.level);
    //     //stat.Skills = new List<SkillSO>();
    //     
    //     foreach (var skill in stat.Skills)
    //     {
    //         stat.Skills.Add(skill);
    //     }
    //     
    //     return sample;
    // }

    private void Update()
    {
        StartBtn.gameObject.SetActive(usingCharacter.Count > 0);
    }

    public void StartGame()
    {
        TileManager.Instance.combatScript.SetActive(true);
        MoveRangeSystem.Instance.ResetAllHighlights();
        SelectedCharacterPanel.SetActive(false);
    }
}
