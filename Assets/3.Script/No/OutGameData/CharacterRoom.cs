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
    [SerializeField] private Transform weaponRawImageSpawnPoint;

    private List<CharacterData> playerCharacters;
    private List<CharacterData> characterIcons;
    
    [SerializeField] private TMP_Text characterPosition;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterLevel;
    [SerializeField] private TMP_Text characterAttack;
    [SerializeField] private TMP_Text characterHp;
    [SerializeField] private TMP_Text characterDefense;
    [SerializeField] private TMP_Text weaponLevel;
    
    private CharacterData selectedCharacter;

    public Button LevelUpButton;

    private void OnEnable()
    {
        SetUIPlayerCharacters();
        SetInfoPlayerCharacter(selectedCharacter);
    }

    private void Awake()
    {
        LevelUpButton.onClick.AddListener(RequestUpdateCharacterLevelUp);
    }

    private void SetUIPlayerCharacters()
    {
        foreach (Transform child in iconSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in playerRawImageSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in weaponRawImageSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        playerCharacters = PlayerManager.Instance.usingCharacterData;
        characterIcons = PlayerManager.Instance.characterIcons;
        
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            for (int j = 0; j < characterIcons.Count; j++)
            {
                if (playerCharacters[i].CharacterID == characterIcons[j].CharacterID)
                {
                    var spawnCharacterUI = Instantiate(characterIcons[i].GameObject, iconSpawnPoint.position, iconSpawnPoint.rotation, iconSpawnPoint);
                    Button btn = spawnCharacterUI.AddComponent<Button>();
                    
                    btn.onClick.AddListener(() =>
                    {
                        selectedCharacter = playerCharacters[i];
                    });
                }
            }
        }
        
        selectedCharacter = playerCharacters[0];
        Instantiate(selectedCharacter.GameObject, 
            playerRawImageSpawnPoint.position, 
            playerRawImageSpawnPoint.rotation, 
            playerRawImageSpawnPoint);
        Instantiate(selectedCharacter.Stat.Weapon.gameObject, 
            Vector3.zero, 
            Quaternion.identity,
            weaponRawImageSpawnPoint);
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
    }
    
    private async void RequestUpdateCharacterLevelUp()
    {
        Debug.Log($"RequestUpdateCharacterLevelUp :: {selectedCharacter.CharacterID}");

        var result = await selectedCharacter.UpdateCharacterLevel(selectedCharacter.CharacterID, 1);

        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
            SetUIPlayerCharacters();
            SetInfoPlayerCharacter(selectedCharacter);
        }
        else
        {
            Debug.LogWarning("캐릭터 레벨업 실패");
        }
    }
}
