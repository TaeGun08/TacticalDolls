using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character2DDragSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    public GameObject characterPrefab3D;

    public Action<GameObject> OnCharacterSpawned;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile.tileType != 1) return;
            if (tile.isUsingTile) return;

            // 드래그된 캐릭터의 characterCode를 가지고 있다고 가정
            int draggedCharacterCode = characterPrefab3D.GetComponent<CharacterData>().CharacterID;

            // 이미 배치된 캐릭터인지 체크
            if (PlayerManager.Instance.usingCharacter.Contains(draggedCharacterCode)) return;

            // 캐싱된 데이터에서 복제본을 만든다
            CharacterData cachedData = PlayerManager.Instance.usingCharacterData.Find(c => c.CharacterID == draggedCharacterCode);
            if (cachedData == null)
            {
                Debug.LogError("캐싱된 데이터 없음");
                return;
            }

            // 프리팹에서 복제하지 말고, 캐싱된 데이터를 Instantiate된 오브젝트에 복사한다.
            GameObject spawned = Instantiate(characterPrefab3D, hit.collider.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            CharacterData spawnedData = spawned.GetComponent<CharacterData>();

            // 캐싱된 데이터를 복사해서 스탯 초기화
            spawnedData.CalculateStatFromLevel(cachedData.Stat.Level);

            tile.isUsingTile = true;

            // 배치된 캐릭터 저장
            PlayerManager.Instance.usingCharacter.Add(spawnedData.CharacterID);

            // 타일에 적용된 오브젝트 저장
            Tile applyTileObj = TileManager.Instance.GetClosestTile(spawned.transform.position);
            applyTileObj.SetOccupant(spawnedData);

            
            // 이벤트 구독
            Debug.Log("Spawned character, invoking OnCharacterSpawned");
            OnCharacterSpawned?.Invoke(spawned);

            Debug.Log("캐릭터 배치 완료");
            
            // Tile tile = hit.collider.GetComponent<Tile>();
            //
            // if (tile.tileType != 1) return;
            // if(tile.isUsingTile) return;
            //
            // CharacterData characterData = characterPrefab3D.GetComponent<CharacterData>();
            //
            // // 이미 배치된 캐릭터인지 체크
            // if (PlayerManager.Instance.usingCharacter.Contains(characterData.CharacterID)) return;
            //
            // var spawned = Instantiate(characterPrefab3D, hit.collider.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            // tile.isUsingTile = true;
            //
            // // 배치된 캐릭터 저장
            // PlayerManager.Instance.usingCharacter.Add(characterData.CharacterID);
            //
            // // 타일에 적용된 오브젝트 저장
            // Tile applyTileObj = TileManager.Instance.GetClosestTile(spawned.transform.position);
            // applyTileObj.SetOccupant(characterData);
            //
            // // 이벤트 구독
            // Debug.Log("Spawned character, invoking OnCharacterSpawned");
            // OnCharacterSpawned?.Invoke(spawned); 
            //
            // Debug.Log("캐릭터 배치 완료");
        }
    }
}