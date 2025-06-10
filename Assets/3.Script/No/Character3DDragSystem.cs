using UnityEngine;
using UnityEngine.EventSystems;

public class Character3DDragSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging;
    
    private Vector3 initPosOffset;

    
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
                transform.position = new Vector3(targetTile.transform.position.x, 1f, targetTile.transform.position.z);
                Debug.Log("Valid tile. Character moved.");
            }
            else
            {
                Debug.Log("Invalid tile. Character not moved.");

                transform.position = initPosOffset;
            }
        }
    }
}