using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class OrderRoom : MonoBehaviour
{
    // 캐릭터, 무기 모든 종류 중 랜덤으로 3개 띄우기 (일정 시간마다 품목이 바뀌어야 함)
    // 만약 있는 상품이라면 버튼 상호작용 X
    
    public PrefabsTable characterTable;
    public PrefabsTable weaponTable;

    public Transform itemSlotParent;
    
    public float refreshInterval = 3600f;     // 갱신 주기(초) 예: 1시간 = 3600
    private float timer;
    
    private List<ScriptableObject> currentShopItems = new List<ScriptableObject>();
    
    private void Start()
    {
        
    }
    
    // 캐릭터 구매
    private async void RequestBuyCharacter(CharacterData character)
    {
        var result = await PlayerManager.Instance.UpdateCharacterList(character);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
        }
        else
        {
            Debug.LogWarning("캐릭 구매 실패");
        }
    }
    
    // 무기 구매
    private async void RequestBuyWeapon(int weaponCode)
    {
        var result = await PlayerManager.Instance.UpdateWeaponList(weaponCode);
        
        if (result)
        {
            await FirebaseMainSession.Instance.FirestoreLoader();
            PlayerManager.Instance.UpdateCharacterData();
        }
        else
        {
            Debug.LogWarning("무기 구매 실패");
        }
    }
}
