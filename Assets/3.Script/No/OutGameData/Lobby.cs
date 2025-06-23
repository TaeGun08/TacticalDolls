using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField]
    private Transform rawImageSpawnPoint;

    [SerializeField] private List<GameObject> characterPrefabs;
    
    private void Start()
    {
        // 상점 무기 리스트 추가
        //UploadAllCharactersToStore();
    }

    private void OnEnable()
    {
        foreach (Transform child in rawImageSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        
        Instantiate(PlayerManager.Instance.usingCharacterData[0].GameObject, rawImageSpawnPoint.position, rawImageSpawnPoint.rotation, rawImageSpawnPoint);
    }
    
    // 캐릭터 상점 리스트 추가
    public async Task UploadAllCharactersToStore()
    {
        List<CharacterDataSample> storeCharacters = new List<CharacterDataSample>();

        foreach (var character in characterPrefabs)
        {
            var characterData = character.GetComponent<CharacterData>();
            storeCharacters.Add(new CharacterDataSample
            {
                characterCode = characterData.CharacterID,
                level = 1,
                weapon = new WeaponDataSample
                {
                    weaponCode = characterData.Stat.Weapon.ID,
                    level = 1,
                    currentCharacter = characterData.CharacterID
                },
                skills = new SkillDataSample[]
                {
                    new SkillDataSample { skillCode = 0, level = 1 },
                    new SkillDataSample { skillCode = 1, level = 1 }
                }
            });            
        }

        CharacterStoreDataSample characterStoreData = new CharacterStoreDataSample
        {
            HasCharacter = storeCharacters
        };

        
        await FirestoreManager.Instance.WriteDataAsync<CharacterStoreDataSample>(
            FirebaseCollections.Stores,
            "StoreCharacters",
            characterStoreData
        );

        Debug.Log("게임 내 판매 캐릭터 정보 저장 완료.");
    }
}
