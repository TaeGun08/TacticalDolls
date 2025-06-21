using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRoom : MonoBehaviour
{
    [SerializeField] private CharacterRoom characterRoom;

    [SerializeField] private Transform weaponIconSpawnPoint;
    [SerializeField] private GameObject weaponIconPrefab;
    
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text weaponUser;
    [SerializeField] private TMP_Text weaponLevel;
    [SerializeField] private TMP_Text weaponAttack;
    
    private List<WeaponData> allWeapons =  new List<WeaponData>();
    private List<WeaponData> equipableWeapons =  new List<WeaponData>();
    
    public WeaponData SelectedWeapon { get; private set; }
    
    [SerializeField] private Button levelUpButton;
    
    private void Awake()
    {
        levelUpButton.onClick.AddListener(()=>
        {
            RequestUpdateWeaponLevelUp(SelectedWeapon.ID);
        });
    }

    private void OnEnable()
    {
        SetUIWeapon();
        SetInfoWeapon(SelectedWeapon);
    }

    private void SetUIWeapon()
    {
        foreach (Transform child in weaponIconSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        allWeapons = PlayerManager.Instance.usingWeaponData;
        
        for (int i = 0; i < allWeapons.Count; i++)
        {
            if (characterRoom.SelectedCharacter.Stat.Weapon.WeaponType == allWeapons[i].WeaponType)
            {
                equipableWeapons.Add(allWeapons[i]);
                
                var icon = Instantiate(weaponIconPrefab, weaponIconSpawnPoint);
                icon.GetComponent<Image>().sprite = allWeapons[i].WeaponIcon;
                
                var button = icon.AddComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    SelectedWeapon =  allWeapons[i];
                });
            }
        }
        
        SelectedWeapon =  equipableWeapons[0];
    }
    
    private void SetInfoWeapon(WeaponData weaponData)
    {
        weaponName.text = weaponData.WeaponName;
        weaponUser.text = GetWeaponUserOrNull() + " Using";
        weaponLevel.text = "Lv. " + weaponData.Level + "/ 20";
        weaponAttack.text = "Up seo yo...";
    }

    private string GetWeaponUserOrNull()
    {
        var characters = PlayerManager.Instance.usingCharacterData;
        
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].Stat.Weapon.ID == SelectedWeapon.ID)
            {
                return characters[i].PrefabName;
            }
        }
        
        return null;
    }
    
    private async void RequestUpdateWeaponLevelUp(int weaponId)
    {
        // 무기 레벨업 구현하면 됩니다래끼
        Debug.Log($"RequestUpdateWeaponLevelUp :: {weaponId}");
        
        var result = await SelectedWeapon.UpdateWeaponLevel(weaponId, 1);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
            SetUIWeapon();
            SetInfoWeapon(SelectedWeapon);
        }
        else
        {
            Debug.LogWarning("무기 레벨업 실패");
        }
    }
}
