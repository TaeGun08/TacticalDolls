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
    
    // 로비 Panel
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject lobbyRender;
    
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
        // 플레이어 데이터 조회 테스트 디버그
        Debug.Log(FirebaseMainSession.Instance.FirebaseUser.UserData.Email);
        Debug.Log(FirebaseMainSession.Instance.FirebaseUser.UserData);

        foreach (var character in FirebaseMainSession.Instance.FirebaseUser.playerData.HasCharacter)
        {
            //Debug.Log($"캐릭터 코드: {character.characterCode}, 레벨: {character.level}");
            
            var initializedSample = InitializeCharacterSampleData(character);
            player.HasCharacter.Add(initializedSample);
        }

        foreach (var weapon in FirebaseMainSession.Instance.FirebaseUser.playerData.HasWeapon)
        {
            //Debug.Log($"무기 코드: {weapon.weaponCode}, 레벨: {weapon.level}");
            
            InitializeCharacterSampleData(weapon);
        }
        
        lobbyPanel.SetActive(true);
        lobbyRender.SetActive(true);
    }
    
    public CharacterDataSample InitializeCharacterSampleData(CharacterDataSample character)
    {
        GameObject prefab = CharacterTable.GetPrefabByIndex(character.characterCode);

        CharacterData SyncCharacterData = prefab.GetComponent<CharacterData>();
        SyncCharacterData.CalculateStatFromLevel(character.level);
        
        usingCharacterData.Add(SyncCharacterData);
        
        return character;
    }
    
    public void InitializeCharacterSampleData(WeaponDataSample weapon)
    {
        GameObject prefab = WeaponTable.GetPrefabByIndex(weapon.weaponCode);

        WeaponData SyncWeaponData = prefab.GetComponent<WeaponData>();
        SyncWeaponData.Level = weapon.level;
        usingWeaponData.Add(SyncWeaponData);
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
