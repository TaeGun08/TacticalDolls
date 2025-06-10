using System;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Info")]
    public int x;
    public int y;
    public bool isWalkable;
    public int tileType;
    public int obstacleDir;

    private MeshRenderer mr;
    
    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    public void Initialize(TileManager.TileData data)
    {
        x = data.x;
        y = data.y;
        isWalkable = data.isWalkable;
        tileType = data.tileType;
        obstacleDir = data.obstacleDir;
    }
    
    private void OnMouseDown()
    {
        Debug.Log($"Clicked tile at ({x}, {y})");

        // 중앙 관리자로 클릭 알림 보내기
        // SkillRangeTester.Instance.OnTileClicked(this);
    }

    public void Highlight(Color color)
    {
        mr.material.color = color;
    }

    public void ResetHighlight()
    {
        mr.material.color = Color.gray;
    }
}