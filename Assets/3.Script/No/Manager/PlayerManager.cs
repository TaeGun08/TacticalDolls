using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerManager: MonoBehaviour
{
    public static PlayerManager Instance;
    
    //public List<CharacterData> CharacterPrefab;
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
        CharacterDataSample sampleCharacter = new CharacterDataSample
        {
            characterCode = 0,
            level = 1,
                
            weapon = new WeaponDataSample
            {
                weaponCode = 0,
                level = 1,
            },
                
            skills = new SkillDataSample[2]
            {
                new SkillDataSample
                {
                    skillCode = 1,
                    level = 1
                },
                new SkillDataSample
                {
                    skillCode = 2,
                    level = 3
                }
            }
        };
        
        CharacterDataSample sampleCharacter2 = new CharacterDataSample
        {
            characterCode = 1,
            level = 2,
                
            weapon = new WeaponDataSample
            {
                weaponCode = 0,
                level = 1,
            },
                
            skills = new SkillDataSample[2]
            {
                new SkillDataSample
                {
                    skillCode = 0,
                    level = 1
                },
                new SkillDataSample
                {
                    skillCode = 2,
                    level = 3
                }
            }
        };
        
        CharacterDataSample sampleCharacter3 = new CharacterDataSample
        {
            characterCode = 2,
            level = 3,
                
            weapon = new WeaponDataSample
            {
                weaponCode = 0,
                level = 1,
            },
                
            skills = new SkillDataSample[2]
            {
                new SkillDataSample
                {
                    skillCode = 0,
                    level = 1
                },
                new SkillDataSample
                {
                    skillCode = 2,
                    level = 3
                }
            }
        };
        

        player.HasCharacter.Add(CreateCharacterDataFromSample(sampleCharacter));
        player.HasCharacter.Add(CreateCharacterDataFromSample(sampleCharacter2));
        player.HasCharacter.Add(CreateCharacterDataFromSample(sampleCharacter3));
    }
    
    public CharacterDataSample CreateCharacterDataFromSample(CharacterDataSample sample)
    {
        CharacterData baseData = GameManager.Instance.PrefabsTable.GetPrefabByIndex(sample.characterCode).GetComponent<CharacterData>();
        
        //CharacterData baseData = CharacterPrefab[sample.characterCode];
        
        // 스탯 초기화
        baseData.Stat = baseData.CalculateStatFromLevel(sample.level);

        return sample;
    }


    private void Update()
    {
        if (usingCharacter.Count > 0)
        {
            StartBtn.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        TileManager.Instance.combatScript.SetActive(true);
        MoveRangeSystem.Instance.ResetAllHighlights();
        SelectedCharacterPanel.SetActive(false);
    }
}
