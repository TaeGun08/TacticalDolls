using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character3DDragSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging;
    
    private Vector3 initPosOffset;
    
    public Action OnCharacterClicked;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        
        initPosOffset = gameObject.transform.position;
        
        isDragging = true;

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            offset = transform.position - hit.point;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point + offset;

            float fixedY = initPosOffset.y; 

            transform.position = new Vector3(hitPoint.x, fixedY, hitPoint.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile targetTile = TileManager.Instance.GetTileAtWorldPosition(hit.point);
            if (targetTile == null)
            {
                transform.position = initPosOffset;
                return;
            }
            
            if (targetTile.tileType == 1 && !targetTile.isUsingTile)
            {
                // 캐릭터 ID 가져오기
                int characterId = GetComponent<CharacterData>().CharacterID;

                // 이미 배치된 캐릭터인지 체크
                if (PlayerManager.Instance.usingCharacter.Contains(characterId))
                {
                    // 이미 배치된 캐릭터라면 다시 원위치
                    transform.position = initPosOffset;
                    return;
                }

                // 캐싱된 데이터에서 원본 CharacterData 찾기
                CharacterData cachedData = PlayerManager.Instance.usingCharacterData.Find(c => c.CharacterID == characterId);
                if (cachedData == null)
                {
                    Debug.LogError("캐싱된 캐릭터 데이터가 없습니다.");
                    transform.position = initPosOffset;
                    return;
                }

                // 새 인스턴스 생성 (캐싱된 prefab에서)
                GameObject spawned = Instantiate(GameManager.Instance.CharacterTable.GetPrefabByIndex(characterId),
                    targetTile.transform.position + Vector3.up * 0.5f, Quaternion.identity);

                CharacterData spawnedData = spawned.GetComponent<CharacterData>();

                // 캐싱 데이터 복사 (스탯 등)
                spawnedData.CalculateStatFromLevel(cachedData.Stat.Level);

                // 타일 상태 업데이트
                targetTile.isUsingTile = true;
                targetTile.SetOccupant(spawnedData);

                // 배치된 캐릭터 리스트에 추가
                PlayerManager.Instance.usingCharacter.Add(characterId);

                Debug.Log("캐릭터가 타일에 성공적으로 배치되었습니다.");

                // 원래 드래그 오브젝트는 다시 초기 위치로 되돌림
                transform.position = initPosOffset;

                // 필요한 이벤트 호출
                // OnCharacterSpawned?.Invoke(spawned);  // 필요하면 이벤트 추가
            }
            else
            {
                // 유효하지 않은 타일이면 원위치
                transform.position = initPosOffset;
                Debug.Log("유효하지 않은 타일입니다.");
            }
        }
        else
        {
            // 레이가 아무것도 안맞으면 원위치
            transform.position = initPosOffset;
        }
    }

    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     isDragging = false;
    //
    //     Ray ray = mainCamera.ScreenPointToRay(eventData.position);
    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         Tile targetTile = TileManager.Instance.GetTileAtWorldPosition(hit.point);
    //         if (targetTile.tileType == 1) 
    //         {
    //             if(targetTile.isUsingTile) return;
    //             
    //             transform.position = targetTile.transform.position + Vector3.up * 0.5f;
    //             Debug.Log("Valid tile. Character moved.");
    //         }
    //         else
    //         {
    //             Debug.Log("Invalid tile. Character not moved.");
    //
    //             transform.position = initPosOffset;
    //         }
    //     }
    // }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCharacterClicked?.Invoke();
    }
}