using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterSpawnController : MonoBehaviour
{
    public Transform HasCharacterContent; 
    public GameObject[] characterUIPrefab;
    
    public Button Apply;
    public Button CancelApply;

    private List<Tile> characterUseAbleTiles;
    private GameObject disposeCharacter;
    private GameObject revertCharacter;

    private readonly List<GameObject> spawnedCharacters = new List<GameObject>();
    private bool isInitFocus = false;

    public Dictionary<int, Tile> characterTileMap = new Dictionary<int, Tile>();
    
    private void Awake()
    {
        Apply.onClick.AddListener(ApplyCharacter);
        CancelApply.onClick.AddListener(CancelApplyCharacter);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        
        characterUseAbleTiles = TileManager.Instance.CharacterSpawnUseTiles;
        
        foreach (var characterDataSample in PlayerManager.Instance.player.HasCharacter)
        {
            int characterCode = characterDataSample.characterCode;
        
            foreach (var characterUI in characterUIPrefab)
            {
                var character = characterUI.GetComponent<Character2DDragSystem>().characterPrefab3D.GetComponent<CharacterData>();
        
                if (characterCode == character.CharacterID)
                {
                    var SpawnCharacter = Instantiate(characterUIPrefab[characterCode], HasCharacterContent.transform);
                    var _2DDragSystem = SpawnCharacter.GetComponent<Character2DDragSystem>();

                    _2DDragSystem.OnCharacterSpawned += (spawnedCharacter3D) =>
                    {
                        CancelApplyEvent(spawnedCharacter3D);
                    };
                    
                    Button btn = SpawnCharacter.AddComponent<Button>();
                    btn.onClick.AddListener(() => OnCharacterSelected(SpawnCharacter));
                    
                    spawnedCharacters.Add(SpawnCharacter);
                }
            }
        }
    }
    
    private void OnCharacterSelected(GameObject character)
    {
        foreach (var characterPrefab in spawnedCharacters)
        {
            characterPrefab.GetComponent<Outline>().enabled = false;
        }
        
        character.GetComponent<Outline>().enabled = true;
        disposeCharacter = character;
        
        if (TileManager.Instance.selectedTile != null && characterUseAbleTiles.Contains(TileManager.Instance.selectedTile))
        {
            CancelApply.gameObject.SetActive(false);
            Apply.gameObject.SetActive(true);
        }
    }

    private void ApplyCharacter()
    {
        var dragSystem = disposeCharacter.GetComponent<Character2DDragSystem>();
        var prefabData = dragSystem.characterPrefab3D.GetComponent<CharacterData>();
        
        CharacterData cachedData = PlayerManager.Instance.usingCharacterData
            .Find(c => c.CharacterID == prefabData.CharacterID);
        
        // 이미 배치된 플레이어인지 확인
        if (PlayerManager.Instance.usingCharacter.Contains(prefabData.CharacterID)) return;
        
        // 타일에 이미 배치된 오브젝트가 있는지 확인
        if (TileManager.Instance.selectedTile.isUsingTile) return;
        
        // Instantiate는 프리팹에서 하지만, 캐싱된 데이터를 복사해서 초기화
        GameObject spawned = Instantiate(dragSystem.characterPrefab3D, 
            TileManager.Instance.selectedTile.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        CharacterData spawnedData = spawned.GetComponent<CharacterData>();
        revertCharacter = spawned;
        CancelApplyEvent(revertCharacter);
        
        // 캐싱된 데이터를 기반으로 스탯 초기화 (복사)
        spawnedData.CalculateStatFromLevel(cachedData.Stat.Level);

        // 타일 상태 업데이트
        TileManager.Instance.selectedTile.isUsingTile = true;

        // 배치된 캐릭터 저장
        PlayerManager.Instance.usingCharacter.Add(spawnedData.CharacterID);
        
        // 배치된 캐릭터가 할당된 타일 저장
        characterTileMap[spawnedData.CharacterID] = TileManager.Instance.selectedTile;
        
        // 타일에 적용된 오브젝트 저장
        Tile applyTileObj = TileManager.Instance.GetClosestTile(spawned.transform.position);
        applyTileObj.SetOccupant(spawnedData);

        Debug.Log("캐릭터 배치 완료");
    }
    
    private void ActiveCancelBtn(GameObject character)
    {
        Apply.gameObject.SetActive(false);
        CancelApply.gameObject.SetActive(true);

        revertCharacter = character;
    }

    private void CancelApplyCharacter()
    {
        Debug.Log("캐릭터 배치 취소");

        if (revertCharacter == null) return;

        int charID = revertCharacter.GetComponent<CharacterData>().CharacterID;

        PlayerManager.Instance.usingCharacter.Remove(charID);
        
        if (characterTileMap.TryGetValue(charID, out Tile tile))
        {
            characterTileMap.Remove(charID);
            tile.ClearOccupant();
        }

        Destroy(revertCharacter);

        CancelApply.gameObject.SetActive(false);

        revertCharacter = null;
    }

    private void CancelApplyEvent(GameObject character)
    {
        var drag3D = character.GetComponent<Character3DDragSystem>();
        drag3D.OnCharacterClicked = null;
        drag3D.OnCharacterClicked += () =>
        {
            ActiveCancelBtn(character);
        };
    }
}