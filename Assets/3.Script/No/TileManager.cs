using System;
using System.Collections.Generic;
using Exoa.Maths;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public enum StageType
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
}

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    
    public TextAsset jsonFile;
    public float tileSize = 1.0f;
    public GameObject tilePrefab;
    public List<EnemyData> EnmeyPrefab;
    
    public Tile[,] tiles;
    private int width;
    private int height;
    
    [Header("Stage Selection")]
    public StageType selectedStage;

    [System.Serializable]
    public class TileData
    {
        public int x;
        public int y;
        public bool isWalkable;
        public int tileType;
        public int obstacleDir;
        public bool isUsingTile;
    }

    [System.Serializable]
    public class MapData
    {
        public Dictionary<string, List<TileData>> Stages;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadMap();
        
        SkillRangeSystem.Instance.SetAllTiles();
        MoveRangeSystem.Instance.SetAllTiles();
    }
    
    private void LoadMap()
    {
        MapData mapTiles = JsonConvert.DeserializeObject<MapData>(jsonFile.text);
        string stageKey = selectedStage.ToString();
        List<TileData> stageTiles = mapTiles.Stages[stageKey];

        // 먼저 맵 크기 계산 (최댓값 기준)
        int maxX = 0;
        int maxY = 0;

        foreach (var tile in stageTiles)
        {
            if (tile.x > maxX) maxX = tile.x;
            if (tile.y > maxY) maxY = tile.y;
        }

        width = maxX + 1;
        height = maxY + 1;
        tiles = new Tile[width, height];

        // 타일 생성 및 배열에 저장
        foreach (TileData tile in stageTiles)
        {
            Vector3 position = new Vector3(tile.x * tileSize, 0, tile.y * tileSize);
            GameObject res = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            Tile tileComp = res.AddComponent<Tile>();
            tileComp.Initialize(tile);
            tiles[tile.x, tile.y] = tileComp;
            
            // 플레이어 스폰 위치 체크
            if (tileComp.tileType == 1)
            {
                Debug.Log("player spawn able");
                tileComp.Highlight(Color.green);
            }

            if (tile.tileType > 100)
            {
                foreach (var enemy in EnmeyPrefab)
                {
                    EnemyData enemyData = enemy.GetComponent<EnemyData>();

                    if (tile.tileType == enemyData.EnemyID)
                    {
                        Instantiate(enemy,tileComp.transform.position + Vector3.up, Quaternion.identity);
                    }
                }
            }
        }
    }
    
    public Tile GetTileAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return tiles[x, y];
        return null;
    }
    
    public Tile GetClosestTile(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.z);
        return GetTileAt(x, y);
    }
    
    public Tile GetTileAtWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.z);

        if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1))
            return null;

        return tiles[x, y];
    }
}

