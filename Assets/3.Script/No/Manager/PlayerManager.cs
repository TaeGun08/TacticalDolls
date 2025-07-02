using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
        InitializePlayerManager();
    }
    
    public void InitializePlayerManager()
    {
        ResetCachedCharacterData();
        UpdateCharacterData();
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
        
        if (FirebaseMainSession.Instance.FirebaseUser.player.Gold < character.price)
        {
            return false;
        }
        
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
    public async Task<bool> UpdateWeaponList(WeaponData weaponData)
    {
        string userId = FirebaseMainSession.Instance.FirebaseUser.UserData.UserId;

        if (FirebaseMainSession.Instance.FirebaseUser.player.Gold < weaponData.Price)
        {
            return false;
        }
        
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
            weaponCode = weaponData.ID,
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

        Debug.Log($"무기 {weaponData.ID} 구매 완료");
        return true;
    }
    
    // 재화 감소
    public async Task DecreaseGold(int itemPrice)
    {
        var player = FirebaseMainSession.Instance.FirebaseUser.player;

        if (player.Gold >= itemPrice)
        {
            int newGold = player.Gold - itemPrice;

            await FirebaseMainSession.Instance.UpdateGoldAsync(newGold);
            Debug.Log("아이템 구매 성공");
        }
        else
        {
            Debug.Log("골드 부족");
        }
    }
    
    // 재화 증가
    public async Task IncreaseGold(int itemPrice)
    {
        var player = FirebaseMainSession.Instance.FirebaseUser.player;

        if (player.Gold >= itemPrice)
        {
            int newGold = player.Gold + itemPrice;

            await FirebaseMainSession.Instance.UpdateGoldAsync(newGold);
            Debug.Log("재화 획득 성공");
        }
        else
        {
            Debug.Log("골드 부족");
        }
    }
    
}
