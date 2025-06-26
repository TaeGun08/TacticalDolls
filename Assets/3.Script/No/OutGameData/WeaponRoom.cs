using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRoom : MonoBehaviour
{
    [SerializeField] private NavigationHandler navigation;
    [SerializeField] private CharacterRoom characterRoom;

    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform weaponIconSpawnPoint;
    [SerializeField] private GameObject weaponIconPrefab;
    
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text weaponUser;
    [SerializeField] private TMP_Text weaponLevel;
    [SerializeField] private TMP_Text weaponAttack;
    
    private List<WeaponData> allWeapons =  new List<WeaponData>();
    
    public WeaponData SelectedWeapon { get; private set; }
    

    private void OnEnable()
    {
        allWeapons = PlayerManager.Instance.usingWeaponData
            .OrderBy(w => w.WeaponGrade)
            .ToList();
        
        SetUIWeapon();
        SetInfoWeapon(SelectedWeapon);
    }

    private void SetUIWeapon()
    {
        foreach (Transform child in weaponIconSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < allWeapons.Count; i++)
        {
            if (characterRoom.SelectedCharacter.Stat.Weapon.WeaponType == allWeapons[i].WeaponType)
            {
                var background = Instantiate(weaponIconPrefab, weaponIconSpawnPoint);
                
                if (characterRoom.SelectedCharacter.Stat.Weapon.ID == allWeapons[i].ID)
                {
                    background.GetComponent<Outline>().enabled = true;
                }

                background.GetComponent<Image>().color = allWeapons[i].SetWeaponBackgroundColor();
                
                var icon = Instantiate(weaponIconPrefab, background.transform);
                icon.GetComponent<Image>().sprite = allWeapons[i].WeaponIcon;
                
                var button = icon.AddComponent<Button>();
                var index = i;
                
                button.onClick.AddListener(() =>
                {
                    SelectedWeapon =  allWeapons[index];
                    RequestUpdateWeapon();
                });
            }
        }
        
        SelectedWeapon = characterRoom.SelectedCharacter.Stat.Weapon;
        weaponImage.sprite = SelectedWeapon.WeaponIcon;
    }
    
    private void SetInfoWeapon(WeaponData weaponData)
    {
        weaponName.text = weaponData.WeaponName;
        weaponUser.text = GetWeaponUserOrNull() + " Using";
        weaponLevel.text = "Lv. " + weaponData.Level + "/ 20";
        weaponAttack.text = weaponData.Damage.ToString();
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
    
    // 무기 레벨업
    public async void RequestUpdateWeaponLevelUp()
    {
        var complete = navigation.Complete.GetComponent<ModalWindowManager>();
        complete.description = $"{SelectedWeapon.WeaponName} 레벨업에 성공하였습니다.";
        
        var result = await SelectedWeapon.UpdateWeaponLevel(SelectedWeapon.ID, 1);
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
    
    // 무기 교체
    private async void RequestUpdateWeapon()
    {
        var result = await SelectedWeapon.UpdateCharacterCurrentWeapon(characterRoom.SelectedCharacter.CharacterID, SelectedWeapon.ID);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
            SetUIWeapon();
            SetInfoWeapon(SelectedWeapon);
        }
        else
        {
            Debug.LogWarning("무기 변경 실패");
        }
    }
}
