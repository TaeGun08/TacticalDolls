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
            if(tile.isUsingTile) return;

            CharacterData characterData = characterPrefab3D.GetComponent<CharacterData>();
            
            // 이미 배치된 캐릭터인지 체크
            if (PlayerManager.Instance.usingCharacter.Contains(characterData.CharacterID)) return;
            
            var spawned = Instantiate(characterPrefab3D, hit.collider.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            tile.isUsingTile = true;
            
            // 배치된 캐릭터 저장
            PlayerManager.Instance.usingCharacter.Add(characterData.CharacterID);
            
            // 타일에 적용된 오브젝트 저장
            Tile applyTileObj = TileManager.Instance.GetClosestTile(spawned.transform.position);
            applyTileObj.SetOccupant(characterData);
            
            // 이벤트 구독
            Debug.Log("Spawned character, invoking OnCharacterSpawned");
            OnCharacterSpawned?.Invoke(spawned); 
            
            Debug.Log("캐릭터 배치 완료");
        }
    }
}