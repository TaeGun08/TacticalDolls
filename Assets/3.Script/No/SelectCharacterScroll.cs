using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectCharacterScroll : MonoBehaviour
{
    public Transform HasCharacterContent; 
    public GameObject[] characterUIPrefab;
    
    public Button Apply;
    public Button CancelApply;

    private GameObject disposeCharacter;
    
    private readonly List<GameObject> spawnedCharacters = new List<GameObject>();
    private bool isInitFocus = false;

    private void Awake()
    {
        Apply.onClick.AddListener(ApplyCharacter);
        CancelApply.onClick.AddListener(CancelApplyCharacter);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log(PlayerManager.Instance.player.HasCharacter.Count);
        
        foreach (var characterDataSample in PlayerManager.Instance.player.HasCharacter)
        {
            int characterCode = characterDataSample.characterCode;
        
            foreach (var characterUI in characterUIPrefab)
            {
                var character = characterUI.GetComponent<CharacterData>();
        
                if (characterCode == character.CharacterID)
                {
                    var characterSprite = Instantiate(characterUIPrefab[characterCode], HasCharacterContent.transform);
                    
                    Button btn = characterSprite.AddComponent<Button>();
                    btn.onClick.AddListener(() => OnCharacterSelected(characterSprite));
                    
                    spawnedCharacters.Add(characterSprite);
                    
                    if (isInitFocus) continue;
                    disposeCharacter = characterSprite;
                    characterSprite.GetComponent<Outline>().enabled = true;
                    isInitFocus = true;
                }
            }
        }
    }
    
    private void OnCharacterSelected(GameObject character)
    {
        foreach (var characterPrefab in spawnedCharacters)
        {
            characterPrefab.GetComponent<Outline>().enabled = false;
        }
        
        character.GetComponent<Outline>().enabled = true;
        disposeCharacter = character;
    }

    private void ApplyCharacter()
    {
    }
    
    private void CancelApplyCharacter()
    {
    }
}