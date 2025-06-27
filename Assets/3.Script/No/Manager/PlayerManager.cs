using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        // 로비로 돌아올시 프리팹 초기화
        ResetCachedCharacterData();
        
        UpdateCharacterData();
        
        lobbyPanel.SetActive(true);
        lobbyRender.SetActive(true);
    }
    
    public CharacterDataSample InitializeCharacterData(CharacterDataSample character)
    {
        GameObject prefab = CharacterTable.GetPrefabByKey(character.characterCode);

        GameObject weaponPrefab = WeaponTable.GetPrefabByKey(character.weapon.weaponCode);
        
        CharacterData SyncCharacterData = prefab.GetComponent<CharacterData>();
        SyncCharacterData.CalculateStatFromLevel(character.level, character.weapon.level, weaponPrefab.GetComponent<WeaponData>());
        
        usingCharacterData.Add(SyncCharacterData);
        
        return character;
    }
    
    public void InitializeWeaponData(WeaponDataSample weapon)
    {
        GameObject prefab = WeaponTable.GetPrefabByKey(weapon.weaponCode);

        WeaponData SyncWeaponData = prefab.GetComponent<WeaponData>();
        SyncWeaponData.Level = weapon.level;
        usingWeaponData.Add(SyncWeaponData);
    }
    
    // 플레이어 정보 동기화 ( 초기값 셋팅 / 캐릭터 무기 강화 시 호출 )
    public void UpdateCharacterData()
    {
        Debug.Log("UpdateCharacterData");
        usingCharacterData.Clear();
        usingWeaponData.Clear();
        player = new PlayerDataSample();
        
        foreach (var character in FirebaseMainSession.Instance.FirebaseUser.playerData.HasCharacter)
        {
            var initializedSample = InitializeCharacterData(character);
            player.HasCharacter.Add(initializedSample);
        }

        foreach (var weapon in FirebaseMainSession.Instance.FirebaseUser.playerData.HasWeapon)
        {
            InitializeWeaponData(weapon);
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
    
    // 캐릭터 구매
    public async Task<bool> UpdateCharacterList(CharacterData character)
    {
        string userId = FirebaseMainSession.Instance.FirebaseUser.UserData.UserId;
        
        // 1. 유저 데이터 로드
        PlayerDataSample playerData = await FirestoreManager.Instance.ReadDataAsync<PlayerDataSample>(
            FirebaseCollections.Players,
            userId
        );
        
        if (playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return false;
        }
        
        // 2. 기본 무기 설정
        WeaponDataSample defaultWeapon = new WeaponDataSample
        {
            weaponCode = character.Stat.Weapon.ID,
            level = 1,
            currentCharacter = character.CharacterID
        };

        // 3. 신규 캐릭터 데이터 생성
        CharacterDataSample newCharacter = new CharacterDataSample
        {
            characterCode = character.CharacterID,
            level = 1,
            weapon = defaultWeapon,
            skills = new SkillDataSample[]
            {
                new SkillDataSample { skillCode = 0, level = 1 },
                new SkillDataSample { skillCode = 1, level = 1 }
            }
        };

        // 4. Firestore에 추가
        playerData.HasCharacter.Add(newCharacter);
        playerData.HasWeapon.Add(defaultWeapon);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "HasCharacter", playerData.HasCharacter },
            { "HasWeapon", playerData.HasWeapon }
        };

        await FirestoreManager.Instance.UpdateDataAsync(FirebaseCollections.Players, userId, updates);
        
        Debug.Log($"캐릭 구매 완료");
        return true;
    }
    
    
    // 무기 구매
    public async Task<bool> UpdateWeaponList(int weaponCode)
    {
        string userId = FirebaseMainSession.Instance.FirebaseUser.UserData.UserId;

        // 1. 유저 데이터 로드
        PlayerDataSample playerData = await FirestoreManager.Instance.ReadDataAsync<PlayerDataSample>(
            FirebaseCollections.Players,
            userId
        );

        if (playerData == null)
        {
            Debug.LogWarning("플레이어 데이터를 찾을 수 없습니다.");
            return false;
        }
        
        // 2. 무기 생성
        WeaponDataSample newWeapon = new WeaponDataSample
        {
            weaponCode = weaponCode,
            level = 1,
            currentCharacter = -1
        };

        // 3. Firestore에 추가
        playerData.HasWeapon.Add(newWeapon);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "HasWeapon", playerData.HasWeapon }
        };

        await FirestoreManager.Instance.UpdateDataAsync(FirebaseCollections.Players, userId, updates);

        Debug.Log($"무기 {weaponCode} 구매 완료");
        return true;
    }
}
