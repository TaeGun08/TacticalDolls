using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looby : MonoBehaviour
{
    private void Start()
    {
        // 플레이어 데이터 조회 테스트 디버그
        // Debug.Log(FirebaseMainSession.Instance.FirebaseUser.UserData.Email);
        // Debug.Log(FirebaseMainSession.Instance.FirebaseUser.UserData);
        //
        // foreach (var character in FirebaseMainSession.Instance.FirebaseUser.playerData.HasCharacter)
        // {
        //     Debug.Log($"캐릭터 코드: {character.characterCode}, 레벨: {character.level}");
        // }
        //
        // foreach (var weapon in FirebaseMainSession.Instance.FirebaseUser.playerData.HasWeapon)
        // {
        //     Debug.Log($"무기 코드: {weapon.weaponCode}, 레벨: {weapon.level}");
        // }
    }
}
