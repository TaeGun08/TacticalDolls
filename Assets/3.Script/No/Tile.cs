using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Info")]
    public int x;
    public int y;
    public bool isWalkable;
    public int tileType;
    public int obstacleDir;

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
    }
}