using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    
    // 로비 Panel
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject lobbyRender;
    
    [SerializeField] private TMP_Text goldFigure;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerManager.Instance.InitializePlayerManager();
        UpdateGold();
        
        lobbyPanel.SetActive(true);
        lobbyRender.SetActive(true);
    }

    // 골드 UI 업데이트
    public void UpdateGold()
    {
        goldFigure.text = FirebaseMainSession.Instance.FirebaseUser.player.Gold.ToString();
        
        Debug.Log(FirebaseMainSession.Instance.FirebaseUser.player.Gold);
    }
}
