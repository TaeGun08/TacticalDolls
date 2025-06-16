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
            if (targetTile.tileType == 1) 
            {
                if(targetTile.isUsingTile) return;
                
                transform.position = targetTile.transform.position + Vector3.up * 0.5f;
                
                // 이동 전 점유한 타일
                var characterData = GetComponent<CharacterData>();
                var charID = characterData.CharacterID;
                
                // 타일 초기화
                if (PlayerManager.Instance.CharacterSpawnController.characterTileMap.TryGetValue(charID, out Tile oldTile))
                {
                    oldTile.isUsingTile = false;
                    oldTile.ClearOccupant();
                }
                
                // 타일 재설정
                targetTile.isUsingTile = true;
                PlayerManager.Instance.CharacterSpawnController.characterTileMap[characterData.CharacterID] = targetTile;
                
                Debug.Log("Valid tile. Character moved.");
            }
            else
            {
                Debug.Log("Invalid tile. Character not moved.");
                transform.position = initPosOffset;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCharacterClicked?.Invoke();
    }
}