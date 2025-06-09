using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;


public enum FirebaseCollections
{
    Players,
    Rooms,
    MatchHistorys,
    Ranks,
    Invitations,
}



// [FirestoreData]
// public class PlayerData  //key : GUID
// {
//     [FirestoreProperty] public string Email { get; set; }
//     [FirestoreProperty] public string NickName { get; set; }
//     [FirestoreProperty] public string Role { get; set; }
//     [FirestoreProperty] public Timestamp CreatedAt { get; set; }
//     [FirestoreProperty] public bool IsTutorialCompleted { get; set; }
//     [FirestoreProperty] public int RankPoint { get; set; }
//     [FirestoreProperty] public List<string> Friends { get; set; } =  new List<string>();
//     [FirestoreProperty] public List<string> MatchHistorys { get; set; } =  new List<string>();
// }

[FirestoreData]
public class WeaponDataSample
{
    [FirestoreProperty] public int weaponCode { get; set; }
    [FirestoreProperty] public int level { get; set; }
}

[FirestoreData]
public class SkillDataSample
{
    [FirestoreProperty] public int skillCode { get; set; }
    [FirestoreProperty] public int level { get; set; }
}

[FirestoreData]
public class CharacterDataSample
{
    [FirestoreProperty] public int characterCode { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public WeaponDataSample weapon { get; set; }
    [FirestoreProperty] public SkillDataSample[] skills { get; set; }
}

[FirestoreData]
public class PlayerDataSample  //key : GUID
{
    [FirestoreProperty] public List<CharacterDataSample> HasCharacter { get; set; } =  new List<CharacterDataSample>();
}

// [FirestoreData]
// public class RoomData  //key : RoomCode
// {
//     [FirestoreProperty] public string RoomName { get; set; }
//     [FirestoreProperty] public string RoomInfo { get; set; }
//     [FirestoreProperty] public string RoomCode { get; set; }
//     [FirestoreProperty] public int MembersCount { get; set; }
//     [FirestoreProperty] public int MaxPlayers { get; set; }
//     [FirestoreProperty] public Timestamp CreatedAt { get; set; }
//     [FirestoreProperty] public bool IsGameStarted { get; set; }
//     [FirestoreProperty] public bool IsGameOver { get; set; }
// }

