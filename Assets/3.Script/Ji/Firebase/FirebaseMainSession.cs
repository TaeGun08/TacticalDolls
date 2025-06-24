using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;

public class FirebaseUser
{
    public Firebase.Auth.FirebaseUser UserData { get; set; }
    public string Username { get; set; }
    
    public PlayerDataSample playerData { get; set; }
    
    public List<CharacterDataSample> characterStore { get; set; }
    public List<WeaponDataSample> weaponStore { get; set; }

}

// 로그인 유저 정보 계속 유지
public class FirebaseMainSession : MonoBehaviour
{
    public static FirebaseMainSession Instance { get; private set; }

    public FirebaseUser FirebaseUser { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        FirebaseUser = new FirebaseUser();
    }

    public void SetUserData(Firebase.Auth.FirebaseUser user, string username)
    {
        FirebaseUser.UserData = user;
        FirebaseUser.Username =  username;

        if (user != null) //디버그용
        {
            Debug.Log($"MainSystem UserId ::: {FirebaseUser.UserData.UserId}");
            Debug.Log($"MainSystem userName ::: {username}");
        }
        
        FirestoreLoader();
    }

    public async Task FirestoreLoader()
    {
        string userId = FirebaseUser.UserData.UserId;
        
        PlayerDataSample playerData = await FirestoreManager.Instance.ReadDataAsync<PlayerDataSample>(
            FirebaseCollections.Players,
            userId
        );
        
        if (playerData != null)
        {
            Debug.Log("플레이어 데이터 로드 성공");
            FirebaseUser.playerData = playerData;
            
            await LoadCharacterStoreList();
            await LoadWeaponStoreList();
        }
        else
        {
            Debug.LogWarning("플레이어 데이터가 존재하지 않음.");
        }
    }
    
    private async Task LoadCharacterStoreList()
    {

        var doc = await FirebaseFirestore.DefaultInstance
            .Collection(FirebaseCollections.Stores.ToString())
            .Document("StoreCharacters")
            .GetSnapshotAsync();

        if (!doc.Exists)
        {
            Debug.LogWarning("CharacterStore 문서를 찾을 수 없습니다.");
            return;
        }
        
        CharacterStoreDataSample storeData = doc.ConvertTo<CharacterStoreDataSample>();
        
        Debug.Log(storeData.HasCharacter.Count);
        
        FirebaseUser.characterStore = storeData.HasCharacter;
    }
    
    private async Task LoadWeaponStoreList()
    {
        var doc = await FirebaseFirestore.DefaultInstance
            .Collection(FirebaseCollections.Stores.ToString())
            .Document("StoreWeapons")
            .GetSnapshotAsync();

        if (!doc.Exists)
        {
            Debug.LogWarning("WeaponStore 문서를 찾을 수 없습니다.");
            return;
        }
        
        WeaponStoreDataSample storeData = doc.ConvertTo<WeaponStoreDataSample>();
        
        Debug.Log(storeData.HasWeapon.Count);
        
        FirebaseUser.weaponStore = storeData.HasWeapon;
    }
}