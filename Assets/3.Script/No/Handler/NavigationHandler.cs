using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NavigationHandler : MonoBehaviour
{
    [Header("Contents Room Panel Settings")]
    public GameObject Lobby;
    public GameObject Stage;
    public GameObject CharacterRoom;
    public GameObject WeaponRoom;
    public GameObject Order;
    public GameObject My;
    
    [Header("Render Texture Settings")]
    public GameObject LobbyRender;
    public GameObject StageRender;
    public GameObject CharacterRender;
    public GameObject WeaponRender;
    public GameObject OrderRender;
    public GameObject MyRender;

    [Header("Public Settings")]
    public Button BackButton;

    [Header("Lobby Button Settings")]
    public Button StageRoomButton;
    public Button CharacterRoomButton;
    public Button WeaponRoomButton;
    public Button OrderRoomButton;
    public Button MyRoomButton;

    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    private Stack<GameObject> renderHistory = new Stack<GameObject>();

    private GameObject currentPanel;
    private GameObject currentRender;
    
    void Start()
    {
        BackButton.onClick.AddListener(BackButtonClicked);
        StageRoomButton.onClick.AddListener(OnMoveStageRoom);
        CharacterRoomButton.onClick.AddListener(OnMoveCharacterRoom);
        WeaponRoomButton.onClick.AddListener(OnMoveWeaponRoom);
        OrderRoomButton.onClick.AddListener(OnMoveOrderRoom);
        MyRoomButton.onClick.AddListener(OnMoveMyRoom);
        
        currentPanel = Lobby;
        currentRender = LobbyRender;
    }

    void ShowRoom(GameObject panel, GameObject render, bool pushToHistory = true)
    {
        if (pushToHistory && currentPanel != null && currentPanel != panel)
        {
            panelHistory.Push(currentPanel);
            renderHistory.Push(currentRender);
        }
        
        Lobby.SetActive(false);
        Stage.SetActive(false);
        CharacterRoom.SetActive(false);
        WeaponRoom.SetActive(false);
        Order.SetActive(false);
        My.SetActive(false);
        
        LobbyRender.SetActive(false);
        StageRender.SetActive(false);
        CharacterRender.SetActive(false);
        
        if (panel == CharacterRoom || panel == WeaponRoom)
        {
            WeaponRender.SetActive(true);
        }
        else
        {
            WeaponRender.SetActive(false);
        }
        
        OrderRender.SetActive(false);
        MyRender.SetActive(false);
        
        panel.SetActive(true);
        render.SetActive(true);

        currentPanel = panel;
        currentRender = render;
    }

    private void OnMoveLobby() => ShowRoom(Lobby, LobbyRender);
    private void OnMoveStageRoom() => ShowRoom(Stage, StageRender);
    private void OnMoveCharacterRoom() => ShowRoom(CharacterRoom, CharacterRender);
    private void OnMoveWeaponRoom() => ShowRoom(WeaponRoom, WeaponRender);
    private void OnMoveOrderRoom() => ShowRoom(Order, OrderRender);
    private void OnMoveMyRoom() => ShowRoom(My, MyRender);

    private void BackButtonClicked()
    {
        if (panelHistory.Count > 0 && renderHistory.Count > 0)
        {
            var prevPanel = panelHistory.Pop();
            var prevRender = renderHistory.Pop();
            ShowRoom(prevPanel, prevRender, false);
        }
        else
        {
            ShowRoom(Lobby, LobbyRender, false);
        }
    }

    // FireBase요청 필요하면 구조 변경할때 사용하면 됨
    // void OnMoveLobby()
    // {
    //     
    // }
    //
    // void OnMoveStageRoom()
    // {
    //     
    // }
    //
    // void OnMoveCharacterRoom()
    // {
    //     
    // }
    //
    // void OnMoveOrderRoom()
    // {
    //     
    // }
    //
    // void OnMoveMyRoom()
    // {
    //     
    // }
}
