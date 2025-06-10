using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine;
using Image = UnityEngine.UIElements.Image;

using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterScroll : MonoBehaviour
{
    public Transform HasCharacterContent; 
    public GameObject[] characterUIPrefab;

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
                    Instantiate(characterUIPrefab[characterCode], HasCharacterContent.transform);
                }
            }
        }
    }
}