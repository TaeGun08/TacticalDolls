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
    
    [Header("Chapter Settings")]
    private GameObject SelectedChapter;
    public Button Chapter1;
    public Button Chapter2;
    public Button Check;

    [Header("Stage Settings")]
    public GameObject Stage1;
    public GameObject Stage2;
    
    void Start()
    {
        BackButton.onClick.AddListener(OnMoveLobby);
        StageRoomButton.onClick.AddListener(OnMoveStageRoom);
        CharacterRoomButton.onClick.AddListener(OnMoveCharacterRoom);
        WeaponRoomButton.onClick.AddListener(OnMoveWeaponRoom);
        OrderRoomButton.onClick.AddListener(OnMoveOrderRoom);
        MyRoomButton.onClick.AddListener(OnMoveMyRoom);
        
        Chapter1.onClick.AddListener(()=> OnSelectChapter(Stage1));
        Chapter2.onClick.AddListener(()=> OnSelectChapter(Stage2));
        
        Check.onClick.AddListener(OnMoveChapter);
    }

    void ShowRoom(GameObject panel, GameObject render)
    {
        Lobby.SetActive(false);
        Stage.SetActive(false);
        CharacterRoom.SetActive(false);
        WeaponRoom.SetActive(false);
        Order.SetActive(false);
        My.SetActive(false);
        Stage1.SetActive(false);
        Stage2.SetActive(false);
        
        LobbyRender.SetActive(false);
        StageRender.SetActive(false);
        CharacterRender.SetActive(false);
        WeaponRender.SetActive(false);
        OrderRender.SetActive(false);
        MyRender.SetActive(false);
        
        panel.SetActive(true);
        render.SetActive(true);
    }

    private void OnMoveLobby() => ShowRoom(Lobby, LobbyRender);
    private void OnMoveStageRoom() => ShowRoom(Stage, StageRender);
    private void OnMoveCharacterRoom() => ShowRoom(CharacterRoom, CharacterRender);
    private void OnMoveWeaponRoom() => ShowRoom(WeaponRoom, WeaponRender);
    private void OnMoveOrderRoom() => ShowRoom(Order, OrderRender);
    private void OnMoveMyRoom() => ShowRoom(My, MyRender);
    private void OnMoveChapter() => OnMoverStagePage();
    
    private void OnSelectChapter(GameObject stage)
    {
        SelectedChapter = stage;
    }
    
    private void OnMoverStagePage()
    {
        Stage1.SetActive(false);
        Stage2.SetActive(false);
        SelectedChapter.SetActive(true);  
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
