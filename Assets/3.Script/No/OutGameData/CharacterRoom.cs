using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterRoom : MonoBehaviour
{
    [SerializeField] private Transform iconSpawnPoint;
    [SerializeField] private Transform rawImageSpawnPoint;

    private List<CharacterData> playerCharacters;
    private List<CharacterData> characterIcons;
    
    [SerializeField] private TMP_Text characterPosition;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private TMP_Text characterLevel;
    [SerializeField] private TMP_Text characterAttack;
    [SerializeField] private TMP_Text characterHp;
    [SerializeField] private TMP_Text characterDefense;
    
    private CharacterData selectedCharacter;


    private void OnEnable()
    {
        SetUIPlayerCharacters();
        SetInfoPlayerCharacter(selectedCharacter);
    }

    private void SetUIPlayerCharacters()
    {
        foreach (Transform child in iconSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in rawImageSpawnPoint)
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
                    Instantiate(characterIcons[i].GameObject, iconSpawnPoint.position, iconSpawnPoint.rotation, iconSpawnPoint);
                }
            }
        }
        
        selectedCharacter = playerCharacters[0];
        Instantiate(selectedCharacter.GameObject, rawImageSpawnPoint.position, rawImageSpawnPoint.rotation, rawImageSpawnPoint);
    }

    private void SetInfoPlayerCharacter(CharacterData characterData)
    {
        characterPosition.text = "No Position";
        characterName.text = characterData.PrefabName;
        characterLevel.text = "Lv. " + characterData.Stat.Level + "/ 20";
        characterAttack.text = characterData.Stat.Attack.ToString();
        characterHp.text = characterData.Stat.HP.ToString();
        characterDefense.text = characterData.Stat.Defense.ToString();
    }
}
