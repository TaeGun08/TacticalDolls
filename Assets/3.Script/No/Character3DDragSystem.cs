using UnityEngine;
using UnityEngine.EventSystems;

public class Character3DDragSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        
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
            Vector3 targetPos = hit.point + offset;
            transform.position = new Vector3(targetPos.x, 1f, targetPos.z);
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
                transform.position = new Vector3(targetTile.transform.position.x, 1f, targetTile.transform.position.z);
                Debug.Log("Valid tile. Character moved.");
            }
            else
            {
                Debug.Log("Invalid tile. Character not moved.");
                // 원래 위치로 되돌릴 수도 있음
            }
        }
    }
}