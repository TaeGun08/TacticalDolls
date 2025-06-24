using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OrderRoom : MonoBehaviour
{
    // 캐릭터, 무기 모든 종류 중 랜덤으로 3개 띄우기 (일정 시간마다 품목이 바뀌어야 함)
    // 만약 있는 상품이라면 버튼 상호작용 X
    
    public Image[] itemImageBackgrounds;
    public Image[] itemImages;
    public TMP_Text[] itemNames;
    public TMP_Text[] itemPrices;
    public Button[] itemButtons;

    public TMP_Text timerText;

    private List<CharacterDataSample> allCharacterSamples;
    private List<WeaponDataSample> allWeaponSamples;

    private CharacterData selectedCharacter;
    private WeaponData[] selectedWeapons = new WeaponData[2];

    public float resetTime = 10f;
    private float timer;
    
    private void Start()
    {
        // 캐릭터 조회
        Debug.Log($"FirebaseMainSession.Instance.FirebaseUser.characterStore.Count:: {FirebaseMainSession.Instance.FirebaseUser.characterStore.Count}");
    
        // 무기 조회
        Debug.Log($"FirebaseMainSession.Instance.FirebaseUser.WeaponStore.Count:: {FirebaseMainSession.Instance.FirebaseUser.weaponStore.Count}");

        itemButtons[0].onClick.AddListener(() =>
        {
            RequestBuyCharacter(selectedCharacter);
        });

        for (int i = 0; i < 2; i++)
        {
            var index = i;
            
            itemButtons[i+1].onClick.AddListener(() =>
            {
                RequestBuyWeapon(selectedWeapons[index].ID);
            });
        }
        
        allCharacterSamples = FirebaseMainSession.Instance.FirebaseUser.characterStore;
        allWeaponSamples = FirebaseMainSession.Instance.FirebaseUser.weaponStore;
        
        timer = resetTime;

        PickRandomItems();
        SetUI();
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = TimeSpan.FromSeconds(timer).ToString(@"hh\:mm\:ss");

        if (timer <= 0)
        {
            timer = resetTime;
            PickRandomItems();
            SetUI();
        }
    }
    
    private void PickRandomItems()
    {
        // 캐릭터 1개 랜덤
        var characterGameObject =
            PlayerManager.Instance.CharacterTable.GetPrefabByKey(
                allCharacterSamples[Random.Range(0, allCharacterSamples.Count)].characterCode);
        selectedCharacter = characterGameObject.GetComponent<CharacterData>();
    
        // 무기 2개 랜덤 (중복방지)
        var weaponList = allWeaponSamples.OrderBy(x => Random.value).ToList();
        
        var weaponGameObject0 =
            PlayerManager.Instance.WeaponTable.GetPrefabByKey(
                weaponList[0].weaponCode);
        var weaponGameObject1 =
            PlayerManager.Instance.WeaponTable.GetPrefabByKey(
                weaponList[1].weaponCode);
        
        selectedWeapons[0] = weaponGameObject0.GetComponent<WeaponData>();
        selectedWeapons[1] = weaponGameObject1.GetComponent<WeaponData>();
    }
    
    private void SetUI()
    {
        // 캐릭터
        itemImages[0].sprite = selectedCharacter.characterIcon;
        itemNames[0].text = selectedCharacter.PrefabName;
        // itemPrices[0].text = selectedCharacter.Price.ToString();

        // 소유 여부에 따라 버튼 비활성화
        bool hasChar = PlayerManager.Instance.usingCharacter.Contains(selectedCharacter.CharacterID);
        itemButtons[0].interactable = !hasChar;

        // 무기
        for(int i = 0; i < 2; i++)
        {
            itemImageBackgrounds[i + 1].color = selectedWeapons[i].SetWeaponBackgroundColor();
            itemImages[i+1].sprite = selectedWeapons[i].WeaponIcon;
            itemNames[i+1].text = selectedWeapons[i].WeaponName;
            // itemPrices[i+1].text = selectedWeapons[i].Price.ToString();

            bool hasWeapon = PlayerManager.Instance.usingWeaponData.Contains(selectedWeapons[i]);
            itemButtons[i+1].interactable = !hasWeapon;
        }
    }
    
    // 캐릭터 구매
    private async void RequestBuyCharacter(CharacterData character)
    {
        Debug.Log($"RequestBuyCharacter ::: {character.PrefabName}");
        
        // var result = await PlayerManager.Instance.UpdateCharacterList(character);
        //
        // if (result)
        // {
        //     await FirebaseMainSession.Instance.FirestoreLoader();
        //     PlayerManager.Instance.UpdateCharacterData();
        // }
        // else
        // {
        //     Debug.LogWarning("캐릭 구매 실패");
        // }
    }
    
    // 무기 구매
    private async void RequestBuyWeapon(int weaponCode)
    {
        Debug.Log($"RequestBuyWeapon ::: {weaponCode}");
        
        // var result = await PlayerManager.Instance.UpdateWeaponList(weaponCode);
        //
        // if (result)
        // {
        //     await FirebaseMainSession.Instance.FirestoreLoader();
        //     PlayerManager.Instance.UpdateCharacterData();
        // }
        // else
        // {
        //     Debug.LogWarning("무기 구매 실패");
        // }
    }
}
