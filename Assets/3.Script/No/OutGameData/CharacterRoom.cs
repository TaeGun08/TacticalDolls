using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    
    [SerializeField] private Image weaponImage;

    private List<CharacterData> playerCharacters;
    private List<CharacterData> characterIcons;

    public CharacterData SelectedCharacter { get; private set; }
    private CharacterData selectedCharacterIcon;

    [SerializeField] private Button levelUpButton;

    private void Awake()
    {
        levelUpButton.onClick.AddListener(RequestUpdateCharacterLevelUp);
    }
    
    private void OnEnable()
    {
        playerCharacters = PlayerManager.Instance.usingCharacterData;
        characterIcons = PlayerManager.Instance.characterIcons;
        
        foreach (Transform child in iconSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            for (int j = 0; j < characterIcons.Count; j++)
            {
                if (playerCharacters[i].CharacterID == characterIcons[j].CharacterID)
                {
                    var spawnCharacterUI = Instantiate(characterIcons[j].GameObject, iconSpawnPoint.position, iconSpawnPoint.rotation, iconSpawnPoint);
                    Button btn = spawnCharacterUI.AddComponent<Button>();
                    
                    int currentIndex_i = i;
                    int currentIndex_j = j;
                    
                    btn.onClick.AddListener(() =>
                    {
                        SelectedCharacter = playerCharacters[currentIndex_i];
                        selectedCharacterIcon = characterIcons[currentIndex_j];
                        
                        SetUIPlayerCharacters();
                        SetInfoPlayerCharacter(SelectedCharacter);
                    });

                    if (selectedCharacterIcon != null) continue;
                    selectedCharacterIcon = characterIcons[j];
                    Debug.Log($"selectedCharacterIcon ::: {selectedCharacterIcon}");
                }
            }
        }
        
        SelectedCharacter = playerCharacters[0];
        
        SetInfoPlayerCharacter(SelectedCharacter);
        SetUIPlayerCharacters();
    }

    private void SetUIPlayerCharacters()
    {
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
        characterPosition.text = "No Position";
        characterName.text = characterData.PrefabName;
        characterLevel.text = "Lv. " + characterData.Stat.Level + "/ 20";
        characterAttack.text = characterData.Stat.Attack.ToString();
        characterHp.text = characterData.Stat.HP.ToString();
        characterDefense.text = characterData.Stat.Defense.ToString();
        weaponLevel.text = "Lv. " + characterData.Stat.Weapon.Level;
        
        weaponImage.sprite = characterData.Stat.Weapon.WeaponIcon;
    }
    
    private async void RequestUpdateCharacterLevelUp()
    {
        Debug.Log($"RequestUpdateCharacterLevelUp :: {SelectedCharacter.CharacterID}");

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
