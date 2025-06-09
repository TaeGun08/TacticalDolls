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

public enum InvitationStatus
{
    Pending,
    Accepted,
    Declined
}

[FirestoreData]
public class PlayerData  //key : GUID
{
    [FirestoreProperty] public string Email { get; set; }
    [FirestoreProperty] public string NickName { get; set; }
    [FirestoreProperty] public string Role { get; set; }
    [FirestoreProperty] public Timestamp CreatedAt { get; set; }
    [FirestoreProperty] public bool IsTutorialCompleted { get; set; }
    [FirestoreProperty] public int RankPoint { get; set; }
    [FirestoreProperty] public List<string> Friends { get; set; } =  new List<string>();
    [FirestoreProperty] public List<string> MatchHistorys { get; set; } =  new List<string>();
}

[FirestoreData]
public class RoomData  //key : RoomCode
{
    [FirestoreProperty] public string RoomName { get; set; }
    [FirestoreProperty] public string RoomInfo { get; set; }
    [FirestoreProperty] public string RoomCode { get; set; }
    [FirestoreProperty] public int MembersCount { get; set; }
    [FirestoreProperty] public int MaxPlayers { get; set; }
    [FirestoreProperty] public Timestamp CreatedAt { get; set; }
    [FirestoreProperty] public bool IsGameStarted { get; set; }
    [FirestoreProperty] public bool IsGameOver { get; set; }
}

[FirestoreData]
public class MatchHistoryData  //key : GUID
{
    [FirestoreProperty] public string PlayerKey { get; set; }
    [FirestoreProperty] public int Players { get; set; } //참여한 플레이어들
    [FirestoreProperty] public int Rank { get; set; } //등수
    [FirestoreProperty] public int KillCount { get; set; } //킬수
    [FirestoreProperty] public string PlayTime { get; set; } //플레이 시간
}

[FirestoreData]
public class RankData  //key : Player GUID
{
    [FirestoreProperty] public string PlayerName { get; set; }
    [FirestoreProperty] public int RankPoint { get; set; }
}


[FirestoreData]
public class InvitationData  // key: Firestore Document ID (자동 생성 또는 초대ID)
{
    [FirestoreProperty] public string From { get; set; }              // UID of sender
    [FirestoreProperty] public string To { get; set; }                // UID of receiver
    [FirestoreProperty] public string Type { get; set; }              // 예: "game_invite", "friend_invite"
    [FirestoreProperty] public string RoomId { get; set; }            // 게임방 ID 또는 친구 요청의 경우 null
    [FirestoreProperty] public string Message { get; set; }           // 친구 요청 메시지 등
    [FirestoreProperty] public Timestamp Timestamp { get; set; }      // 생성 시각
    [FirestoreProperty] public string Status { get; set; }            // 상태
}