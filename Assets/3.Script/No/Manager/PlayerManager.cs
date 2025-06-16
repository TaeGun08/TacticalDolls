using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public PlayerDataSample player;
    public List<int> usingCharacter;
    public List<CharacterData> usingCharacterData;

    public GameObject SelectedCharacterPanel;
    public Button StartBtn;

    public CharacterSpawnController CharacterSpawnController;
    
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
        // 초기 플레이어가 사용가능한 캐릭터 데이터 조회
        var sampleCharacters = new List<CharacterDataSample>
        {
            new CharacterDataSample
            {
                characterCode = 0,
                level = 1,
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

        // 사용가능한 캐릭터 데이터 캐싱
        foreach (var sample in sampleCharacters)
        {
            var initializedSample = InitializeCharacterSampleData(sample);
            player.HasCharacter.Add(initializedSample);
        }
    }
    
    public CharacterDataSample InitializeCharacterSampleData(CharacterDataSample sample)
    {
        GameObject prefab = GameManager.Instance.CharacterTable.GetPrefabByIndex(sample.characterCode);

        CharacterData character = prefab.GetComponent<CharacterData>();
        character.CalculateStatFromLevel(sample.level);
        
        usingCharacterData.Add(character);
        
        return sample;
    }

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
    
    public void ResetCachedCharacterData()
    {
        for (int i = 0; i < usingCharacterData.Count; i++)
        {
            CharacterData cachedData = usingCharacterData[i];
            int charID = cachedData.CharacterID;

            // 원본 프리팹 가져오기
            GameObject prefab = GameManager.Instance.CharacterTable.GetPrefabByIndex(charID);
            CharacterData prefabData = prefab.GetComponent<CharacterData>();

            if (prefabData == null)
            {
                Debug.LogWarning($"Prefab for CharacterID {charID} not found or has no CharacterData.");
                continue;
            }

            cachedData.ResetStatFrom();
        }
    
        Debug.Log("캐싱된 캐릭터 데이터 모두 초기화 완료.");
    }
}
