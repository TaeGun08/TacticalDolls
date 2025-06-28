using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // 로비 Panel
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject lobbyRender;
    
    private void Start()
    {
        PlayerManager.Instance.InitializePlayerManager();
        
        lobbyPanel.SetActive(true);
        lobbyRender.SetActive(true);
    }
}
