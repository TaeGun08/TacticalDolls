using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    [Header("Tile Info")]
    public int x;
    public int y;
    public bool isWalkable;
    public int tileType;
    public int obstacleDir;
    public bool isUsingTile;

    private MeshRenderer mr;
    private Renderer tileRenderer;
    
    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
    }
    
    public void Initialize(TileManager.TileData data)
    {
        x = data.x;
        y = data.y;
        isWalkable = data.isWalkable;
        tileType = data.tileType;
        obstacleDir = data.obstacleDir;
        isUsingTile = data.isUsingTile;

        mr = GetComponent<MeshRenderer>();
    }
    
    private void OnMouseDown()
    {
        Debug.Log($"Clicked tile at ({x}, {y})");
        Debug.Log($"Clicked tile at ({tileType})");

        
        if (tileType == 1)
        {
            TileManager.Instance.SetSelectedTile(this);
            //TileManager.Instance.selectedTile = this;
        }

        // 중앙 관리자로 클릭 알림 보내기
        // SkillRangeTester.Instance.OnTileClicked(this);
    }
    
    private IDamageAble occupant;
    
    public void SetOutline(bool enable)
    {
        if (tileRenderer != null)
        {
            tileRenderer.material = enable ? TileManager.Instance.outlineMaterial : TileManager.Instance.defaultMaterial;
        }
    }

    public void Highlight(Color color)
    {
        mr.material.color = color;
    }

    public void ResetHighlight()
    {
        mr.material.color = Color.gray;
    }

    // public GameObject ReturnTopOfTileOrNull()
    // {
    //     if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 100f))
    //     {
    //         return hit.transform.gameObject;
    //     }
    //     
    //     return null;
    // }
    
    // combat settings
    public void SetOccupant(IDamageAble obj)
    {
        occupant = obj;
    }

    public IDamageAble GetOccupant()
    {
        return occupant;
    }

    public void ClearOccupant()
    {
        occupant = null;
    }
}