using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRoom : MonoBehaviour
{
    [SerializeField] private Button myInfoButton;
    [SerializeField] private Button optionButton;
    
    [SerializeField] private GameObject myInfoPanel;
    [SerializeField] private GameObject optionPanel;
    
    [SerializeField] private Button soundButton;
    
    [SerializeField] private GameObject soundPanel;
    
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerEmail;

    private void Start()
    {
        myInfoButton.onClick.AddListener(() =>
        {
            myInfoPanel.SetActive(true);
            optionPanel.SetActive(false);
        });
        
        optionButton.onClick.AddListener(() =>
        {
            myInfoPanel.SetActive(false);
            optionPanel.SetActive(true);
        });
        
        soundButton.onClick.AddListener(() =>
        {
            soundPanel.SetActive(true);
        });
    }

    private void OnEnable()
    {
        playerName.text = FirebaseMainSession.Instance.FirebaseUser.Username;
        playerEmail.text = FirebaseMainSession.Instance.FirebaseUser.UserData.Email;
        
        MyRoomData[] slots = FindObjectsOfType<MyRoomData>();

        var characterList = new List<int>();
        var weaponList = new List<int>();

        for (int i = 0; i < PlayerManager.Instance.usingCharacterData.Count; i++)
        {
            characterList.Add(PlayerManager.Instance.usingCharacterData[i].CharacterID);
        }
        
        for (int i = 0; i < PlayerManager.Instance.usingWeaponData.Count; i++)
        {
            weaponList.Add(PlayerManager.Instance.usingWeaponData[i].ID);
        }
        
        foreach (var slot in slots)
        {
            switch (slot.dataType)
            {
                case DataType.Character:
                    if (characterList.Contains(slot.characterID))
                        slot.SetActive();
                    break;
                case DataType.Weapon:
                    if (weaponList.Contains(slot.weaponID))
                        slot.SetActive();
                    break;
            }
        }
    }
}
