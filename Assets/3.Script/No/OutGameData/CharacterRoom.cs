using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Michsky.UI.Dark;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterRoom : MonoBehaviour
{
    [SerializeField] private Transform iconSpawnPoint;
    [SerializeField] private Transform playerRawImageSpawnPoint;
    
    [SerializeField] private TMP_Text characterPosition;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterLevel;
    [SerializeField] private TMP_Text characterAttack;
    [SerializeField] private TMP_Text characterHp;
    [SerializeField] private TMP_Text characterDefense;
    [SerializeField] private TMP_Text weaponLevel;
    
    [SerializeField] private Image[] skillImage;
    [SerializeField] private Button[] skillButtons;
    
    [SerializeField] private GameObject skillDetailPanel;
    [SerializeField] private Image skillDetailIcon;
    [SerializeField] private TMP_Text skillName;
    [SerializeField] private TMP_Text skillDescription;
    [SerializeField] private Button skillDetailCancel;
    
    [SerializeField] private Image weaponBackground;
    [SerializeField] private Image weaponImage;
    
    [SerializeField] private Transform characterIconBackground;

    private List<CharacterData> playerCharacters;

    public CharacterData SelectedCharacter { get; private set; }
    public int currentIndex = 0;
    private Transform currentIconBackground;
    
    public NavigationHandler navigation;

    private void Start()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            var i1 = i;
            skillButtons[i].onClick.AddListener(() =>
            {
                OpenSkillDetailPanel(i1);
            });
        }
        
        skillDetailCancel.onClick.AddListener(() =>
        {
            skillDetailPanel.SetActive(false);
        });
    }

    private void OnEnable()
    {
        playerCharacters = PlayerManager.Instance.usingCharacterData;
        
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            var spawnCharacterIconBackground = Instantiate(
                characterIconBackground, 
                iconSpawnPoint.position, 
                iconSpawnPoint.rotation, 
                iconSpawnPoint);
            var spawnCharacterUI = Instantiate(
                characterIconBackground, 
                spawnCharacterIconBackground.position, 
                spawnCharacterIconBackground.rotation, 
                spawnCharacterIconBackground);
            
            Button btn = spawnCharacterIconBackground.AddComponent<Button>();
            var image = spawnCharacterUI.GetComponent<Image>();
            var color = image.color;
            
            image.sprite = playerCharacters[i].CharacterIcon;
            color.a = 1;
            image.color = color;
                
            var i1 = i;
            btn.onClick.AddListener(() =>
            {
                currentIndex = i1;
                SelectedCharacter = playerCharacters[currentIndex];
                
                SetUIPlayerCharacters();
                SetInfoPlayerCharacter(SelectedCharacter);
            });
        }
        
        SelectedCharacter = playerCharacters[currentIndex];
        
        SetUIPlayerCharacters();
        SetInfoPlayerCharacter(SelectedCharacter);
    }

    private void OnDisable()
    {
        foreach (Transform child in iconSpawnPoint)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetUIPlayerCharacters()
    {
        foreach (Transform child in iconSpawnPoint)
        {
            var outline = child.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
        
        currentIconBackground = iconSpawnPoint.GetChild(currentIndex);
        
        var currentOutline = currentIconBackground.GetComponent<Outline>();
        if (currentOutline != null)
        {
            currentOutline.enabled = true;
        }
        
        foreach (Transform child in playerRawImageSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        Instantiate(SelectedCharacter.GameObject, 
            playerRawImageSpawnPoint.position, 
            playerRawImageSpawnPoint.rotation, 
            playerRawImageSpawnPoint);
    }

    private void SetInfoPlayerCharacter(CharacterData characterData)
    {
        characterPosition.text = characterData.position;
        characterName.text = characterData.PrefabName;
        characterLevel.text = "Lv. " + characterData.Stat.Level + "/ 20";
        characterAttack.text = characterData.Stat.Attack.ToString();
        characterHp.text = characterData.Stat.HP.ToString();
        characterDefense.text = characterData.Stat.Defense.ToString();
        weaponLevel.text = "Lv. " + characterData.Stat.Weapon.Level;

        for (int i = 0; i < skillImage.Length; i++)
        {
            skillImage[i].sprite = characterData.HasSkills[i].unitSkillComponents.skillIconSprite;
        }
        
        weaponBackground.color = characterData.Stat.Weapon.SetWeaponBackgroundColor();
        weaponImage.sprite = characterData.Stat.Weapon.WeaponIcon;
    }

    private void OpenSkillDetailPanel(int index)
    {
        skillDetailIcon.sprite = skillImage[index].sprite;
        skillName.text = SelectedCharacter.HasSkills[index].skillName;
        skillDescription.text = SelectedCharacter.HasSkills[index].skillInfoText;
        
        skillDetailPanel.SetActive(true);
    }
    
    public async void RequestUpdateCharacterLevelUp()
    {
        var complete = navigation.Complete.GetComponent<ModalWindowManager>();
        complete.description = $"{SelectedCharacter.PrefabName}의 레벨업에 성공하였습니다.";

        var result = await SelectedCharacter.UpdateCharacterLevel(SelectedCharacter.CharacterID, 1);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
            SetUIPlayerCharacters();
            SetInfoPlayerCharacter(SelectedCharacter);
        }
        else
        {
            Debug.LogWarning("캐릭터 레벨업 실패");
        }
    }
}
