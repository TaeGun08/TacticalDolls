using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoom : MonoBehaviour
{
    [SerializeField] private GameObject characterRoomPanel;

    private CharacterDataSample currentCharacter;
    
    private void OnEnable()
    {
        currentCharacter = FirebaseMainSession.Instance.FirebaseUser.playerData.HasCharacter[0];
        
    }
}
