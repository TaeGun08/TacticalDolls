using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using Michsky.UI.Dark;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OrderRoom : MonoBehaviour
{
    // 캐릭터, 무기 모든 종류 중 랜덤으로 3개 띄우기 (일정 시간마다 품목이 바뀌어야 함)
    // 만약 있는 상품이라면 버튼 상호작용 X
    [SerializeField] private NavigationHandler navigation;
    
    public Image[] itemImageBackgrounds;
    public Image[] itemImages;
    public TMP_Text[] itemNames;
    public TMP_Text[] itemPrices;
    public Button[] itemButtons;

    public TMP_Text timerText;

    private List<CharacterDataSample> allCharacterSamples;
    private List<WeaponDataSample> allWeaponSamples;
    
    private List<CharacterData> allCharacters = new List<CharacterData>();
    private List<WeaponData> allWeapons = new List<WeaponData>();

    private CharacterData selectedCharacter;
    private WeaponData[] selectedWeapons = new WeaponData[2];

    private const string KEY_ORDERROOM_RESET_TIME = "OrderRoom_NextResetTime";
    private DateTime nextResetTime;
    
    public float resetTime = 10f;
    private float timer;

    private WeaponData selectedWeapon;
    
    Dictionary<WeaponGrade, float> gradeWeights = new Dictionary<WeaponGrade, float>
    {
        { WeaponGrade.Rare, 0.6f },
        { WeaponGrade.Epic, 0.3f },
        { WeaponGrade.Unique, 0.1f }
    };
    
    private void Start()
    {
        // 캐릭터 조회
        Debug.Log($"FirebaseMainSession.Instance.FirebaseUser.characterStore.Count:: {FirebaseMainSession.Instance.FirebaseUser.characterStore.Count}");
    
        // 무기 조회
        Debug.Log($"FirebaseMainSession.Instance.FirebaseUser.WeaponStore.Count:: {FirebaseMainSession.Instance.FirebaseUser.weaponStore.Count}");
        
        for (int i = 0; i < 2; i++)
        {
            var index = i;
            itemButtons[i + 1].onClick.RemoveAllListeners();
            
            itemButtons[i+1].onClick.AddListener(() =>
            {
                selectedWeapon = selectedWeapons[index];
            });
        }
        
        allCharacterSamples = FirebaseMainSession.Instance.FirebaseUser.characterStore;
        allWeaponSamples = FirebaseMainSession.Instance.FirebaseUser.weaponStore;
        
        for (int i = 0; i < allCharacterSamples.Count; i++)
        {
            var ob = PlayerManager.Instance.CharacterTable.GetPrefabByKey(allCharacterSamples[i].characterCode);
            
            allCharacters.Add(ob.GetComponent<CharacterData>());
        }
        
        for (int i = 0; i < allWeaponSamples.Count; i++)
        {
            var ob = PlayerManager.Instance.WeaponTable.GetPrefabByKey(allWeaponSamples[i].weaponCode);
            
            allWeapons.Add(ob.GetComponent<WeaponData>());
        }
        
        LoadOrSetNextResetTime();

        PickRandomItems();
        SetUI();
    }
    
    private void Update()
    {
        var remain = (nextResetTime - DateTime.UtcNow).TotalSeconds;
        if (remain < 0) remain = 0;

        timerText.text = TimeSpan.FromSeconds(remain).ToString(@"hh\:mm\:ss");

        if (remain <= 0)
        {
            // 다음 리셋 시간 갱신: 지금부터 resetTime초 후
            nextResetTime = DateTime.UtcNow.AddSeconds(resetTime);
            PlayerPrefs.SetString(KEY_ORDERROOM_RESET_TIME, nextResetTime.ToString("o"));
            PlayerPrefs.Save();

            PickRandomItems();
            SetUI();
        }
    }
    
    private void PickRandomItems()
    {
        // 캐릭터 1개 랜덤
        selectedCharacter = allCharacters[Random.Range(0, allCharacterSamples.Count)];

        // 무기: Normal 등급 제외
        var filteredWeapons = allWeapons.Where(w => w.WeaponGrade != WeaponGrade.Normal).ToList();
    
        var firstWeapon = PickRandomWeaponByWeight(filteredWeapons, gradeWeights);
        filteredWeapons.Remove(firstWeapon);
        var secondWeapon = PickRandomWeaponByWeight(filteredWeapons, gradeWeights);
        
        selectedWeapons[0] = firstWeapon;
        selectedWeapons[1] = secondWeapon;
    }
    
    WeaponData PickRandomWeaponByWeight(List<WeaponData> weaponList, Dictionary<WeaponGrade, float> weights)
    {
        // weaponList는 Normal 등급 없는 상태여야 함!
        var gradeGroups = weaponList
            .GroupBy(w => w.WeaponGrade)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 전체 확률 합산
        float totalWeight = gradeGroups.Sum(g => weights.ContainsKey(g.Key) ? weights[g.Key] : 0f);

        float rnd = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var kv in gradeGroups)
        {
            float weight = weights.ContainsKey(kv.Key) ? weights[kv.Key] : 0f;
            cumulative += weight;
            if (rnd <= cumulative)
            {
                // 이 등급 그룹에서 랜덤 1개 뽑기
                var groupList = kv.Value;
                return groupList[Random.Range(0, groupList.Count)];
            }
        }
        // 예외처리(실패 시 첫 번째)
        return weaponList[0];
    }
    
    private void SetUI()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].interactable = true;
        }
        
        // 캐릭터
        itemImages[0].sprite = selectedCharacter.CharacterIcon;
        itemNames[0].text = selectedCharacter.PrefabName;
        itemPrices[0].text = "가격 미정";

        // 소유 여부에 따라 버튼 비활성화
        for (int i = 0; i < PlayerManager.Instance.usingCharacterData.Count; i++)
        {
            if (PlayerManager.Instance.usingCharacterData[i].CharacterID == selectedCharacter.CharacterID)
            {
                itemPrices[0].text = "<보유 중>";
                itemButtons[0].interactable = false;
                break;
            }
        }

        // 무기
        for(int i = 0; i < 2; i++)
        {
            itemImageBackgrounds[i + 1].color = selectedWeapons[i].SetWeaponBackgroundColor();
            itemImages[i+1].sprite = selectedWeapons[i].WeaponIcon;
            itemNames[i+1].text = selectedWeapons[i].WeaponName;
            itemPrices[i+1].text = "가격 미정";

            for (int j = 0; j < PlayerManager.Instance.usingWeaponData.Count; j++)
            {
                if (PlayerManager.Instance.usingWeaponData[j].ID == selectedWeapons[i].ID)
                {
                    itemPrices[i+1].text = "<보유 중>";
                    itemButtons[i+1].interactable = false;
                    break;
                }
            }
        }
    }
    
    private void LoadOrSetNextResetTime()
    {
        // 최초 실행시엔 저장된 값이 없음
        if (PlayerPrefs.HasKey(KEY_ORDERROOM_RESET_TIME))
        {
            var savedTimeStr = PlayerPrefs.GetString(KEY_ORDERROOM_RESET_TIME);
            nextResetTime = DateTime.Parse(savedTimeStr);
        }
        else
        {
            // 최초: 현재 시각 + resetTime초 후로
            nextResetTime = DateTime.UtcNow.AddSeconds(resetTime);
            PlayerPrefs.SetString(KEY_ORDERROOM_RESET_TIME, nextResetTime.ToString("o"));
            PlayerPrefs.Save();
        }
    }
    
    // 캐릭터 구매
    public async void RequestBuyCharacter()
    {
        var complete = navigation.Complete.GetComponent<ModalWindowManager>();
        complete.description = $"{selectedCharacter.PrefabName} 구매에 성공하였습니다.";
        
        var result = await PlayerManager.Instance.UpdateCharacterList(selectedCharacter);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
            SetUI();
        }
        else
        {
            Debug.LogWarning("캐릭 구매 실패");
        }
    }
    
    // 무기 구매
    public async void RequestBuyWeapon()
    {
        var complete = navigation.Complete.GetComponent<ModalWindowManager>();
        complete.description = $"{selectedWeapon.WeaponName} 구매에 성공하였습니다.";

        var result = await PlayerManager.Instance.UpdateWeaponList(selectedWeapon.ID);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
            SetUI();
        }
        else
        {
            Debug.LogWarning("무기 구매 실패");
        }
    }
}
