using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    // 임시 캐릭터 프리랩 리스트
    public PrefabsTable CharacterTable;
    public PrefabsTable WeaponTable;

    public PlayerDataSample player;
    
    // 플레이어가 사용 가능한 캐릭터 
    public List<int> usingCharacter;
    public List<CharacterData> usingCharacterData;
    public List<CharacterData> characterIcons;
    // 플레이어가 사용 가능한 무기
    public List<WeaponData> usingWeaponData;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        player = new PlayerDataSample();
    }

    private void Start()
    {
        // 로비로 돌아올시 프리팹 초기화
        ResetCachedCharacterData();
        
        // 플레이어 데이터 조회 테스트 디버그
        Debug.Log(FirebaseMainSession.Instance.FirebaseUser.UserData.Email);
        Debug.Log(FirebaseMainSession.Instance.FirebaseUser.UserData);
        
        UpdateCharacterData();
    }
    
    public CharacterDataSample InitializeCharacterData(CharacterDataSample character)
    {
        GameObject prefab = CharacterTable.GetPrefabByIndex(character.characterCode);

        CharacterData SyncCharacterData = prefab.GetComponent<CharacterData>();
        SyncCharacterData.CalculateStatFromLevel(character.level, character.weapon.level);
        
        usingCharacterData.Add(SyncCharacterData);
        
        return character;
    }
    
    public void InitializeCharacterData(WeaponDataSample weapon)
    {
        GameObject prefab = WeaponTable.GetPrefabByIndex(weapon.weaponCode);

        WeaponData SyncWeaponData = prefab.GetComponent<WeaponData>();
        SyncWeaponData.Level = weapon.level;
        usingWeaponData.Add(SyncWeaponData);
    }
    
    // 플레이어 정보 동기화 ( 초기값 셋팅 / 캐릭터 무기 강화 시 호출 )
    public void UpdateCharacterData()
    {
        usingCharacterData.Clear();
        usingWeaponData.Clear();
        
        foreach (var character in FirebaseMainSession.Instance.FirebaseUser.playerData.HasCharacter)
        {
            var initializedSample = InitializeCharacterData(character);
            player.HasCharacter.Add(initializedSample);
        }

        foreach (var weapon in FirebaseMainSession.Instance.FirebaseUser.playerData.HasWeapon)
        {
            InitializeCharacterData(weapon);
        }
    }
    
    // 프리팹 초기화
    public void ResetCachedCharacterData()
    {
        for (int i = 0; i < usingCharacterData.Count; i++)
        {
            CharacterData cachedData = usingCharacterData[i];
            int charID = cachedData.CharacterID;

            // 원본 프리팹 가져오기
            GameObject prefab = CharacterTable.GetPrefabByIndex(charID);
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
