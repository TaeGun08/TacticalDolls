using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public TextAsset jsonFile;
    public float tileSize = 1.0f;
    public GameObject tilePrefab;
    
    [System.Serializable]
    public class TileData
    {
        public int x;
        public int y;
        public bool isWalkable;
        public int tileType;
        public int obstacleDir;
    }
    
    [System.Serializable]
    public class MapData
    {
        // stage에 맞는 key값을 변수명으로 지정해야합니다.
        public List<TileData> stage1;
    }
    
    private void LoadMap()
    {
        MapData mapTiles = JsonConvert.DeserializeObject<MapData>(jsonFile.text);
        
        foreach (TileData tile in mapTiles.stage1)
        {
            Vector3 position = new Vector3(tile.x * tileSize, 0, tile.y * tileSize);
            
            Instantiate(tilePrefab, position, Quaternion.identity, transform);
        }
    }
    
    private void Start()
    {
        LoadMap();
    }
}
