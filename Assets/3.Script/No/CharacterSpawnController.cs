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
    
    private readonly List<GameObject> spawnedCharacters = new List<GameObject>();
    private bool isInitFocus = false;

    public Dictionary<GameObject, Tile> characterTileMap = new Dictionary<GameObject, Tile>();
    
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
                var character = characterUI.GetComponent<CharacterData>();
        
                if (characterCode == character.CharacterID)
                {
                    var SpawnCharacter = Instantiate(characterUIPrefab[characterCode], HasCharacterContent.transform);
                    var _2DDragSystem = SpawnCharacter.GetComponent<Character2DDragSystem>();

                    _2DDragSystem.OnCharacterSpawned += (spawnedCharacter3D) =>
                    {
                        EventTest(spawnedCharacter3D);
                    };
                    
                    Button btn = SpawnCharacter.AddComponent<Button>();
                    btn.onClick.AddListener(() => OnCharacterSelected(SpawnCharacter));
                    
                    spawnedCharacters.Add(SpawnCharacter);
                    
                    // if (isInitFocus) continue;
                    // disposeCharacter = SpawnCharacter;
                    // SpawnCharacter.GetComponent<Outline>().enabled = true;
                    // isInitFocus = true;
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
        else
        {
            Debug.Log("캐릭터를 배치할 타일이 선택되지 않았습니다.");
        }
    }

    private void ApplyCharacter()
    {
        // 타일에 이미 배치된 오브젝트가 있는지 확인
        if(TileManager.Instance.selectedTile.isUsingTile) return;
        // 이미 배치된 캐릭터인지 체크
        if (PlayerManager.Instance.usingCharacter.Contains(disposeCharacter.GetComponent<CharacterData>().CharacterID)) return;
        
        var _2DDragSystem = disposeCharacter.GetComponent<Character2DDragSystem>();
        
        GameObject characterSpawn = Instantiate(_2DDragSystem.characterPrefab3D);
        characterSpawn.transform.position = TileManager.Instance.selectedTile.transform.position + Vector3.up;
        
        EventTest(characterSpawn);
        
        TileManager.Instance.selectedTile.isUsingTile = true;
        
        // 배치된 캐릭터 저장
        PlayerManager.Instance.usingCharacter.Add(characterSpawn.GetComponent<CharacterData>().CharacterID);
        
        // 배치된 타일과 캐릭터 저장
        characterTileMap[characterSpawn] = TileManager.Instance.selectedTile;
    }
    
    private GameObject RevertChracter;
    
    private void ActiveCancelBtn(GameObject character)
    {
        Debug.Log("캐릭터 배치 취소 가능");
        Apply.gameObject.SetActive(false);
        CancelApply.gameObject.SetActive(true);

        RevertChracter = character;
    }
    
    private void CancelApplyCharacter()
    {
        Debug.Log("캐릭터 배치 취소");

        if (RevertChracter == null) return;

        int charID = RevertChracter.GetComponent<CharacterData>().CharacterID;
    
        // 캐릭터 ID 제거
        PlayerManager.Instance.usingCharacter.Remove(charID);

        // 타일 상태 복구
        if (characterTileMap.TryGetValue(RevertChracter, out Tile tile))
        {
            tile.isUsingTile = false;
            characterTileMap.Remove(RevertChracter);
        }

        // 캐릭터 제거
        Destroy(RevertChracter);

        // 버튼 UI 리셋
        CancelApply.gameObject.SetActive(false);
        //Apply.gameObject.SetActive(true);

        RevertChracter = null;
    }

    private void EventTest(GameObject character)
    {
        var drag3D = character.GetComponent<Character3DDragSystem>();
        
        drag3D.OnCharacterClicked += () => ActiveCancelBtn(character);
    }
}