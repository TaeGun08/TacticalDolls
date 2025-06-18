using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

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
    public GameObject combatScript;
    
    [Header("Tile Settings")]
    public TextAsset jsonFile;
    public float tileSize = 1.0f;
    public GameObject tileTransform;
    public GameObject structureTransform;
    public GameObject wallTransform;
    public GameObject tilePrefab;
    public GameObject structurePrefab201Left;
    public GameObject structurePrefab201Up;
    public GameObject structurePrefab201Right;
    public GameObject structurePrefab201Down;
    public GameObject structurePrefab202;
    public GameObject structurePrefab203;
    public GameObject structurePrefab204;
    public List<EnemyData> EnmeyPrefab;
    public Tile[,] tiles;
    private int width;
    private int height;
    
    [Header("Select Tile Settings")]
    public List<Tile> CharacterSpawnUseTiles;
    public Tile selectedTile;
    private Tile previousSelectedTile;
    public Material defaultMaterial;
    public Material outlineMaterial;
    
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
        
        RangeSystem.Instance.SetAllTiles();
        
        combatScript.SetActive(false);
    }
    
    private void LoadMap()
    {
        MapData mapTiles = JsonConvert.DeserializeObject<MapData>(jsonFile.text);
        string stageKey = selectedStage.ToString();
        List<TileData> stageTiles = mapTiles.Stages[stageKey];

        // 먼저 맵 크기 계산 (최댓값 기준)
        int maxX = 0;
        int maxY = 0;

        int wallMinX = 15;
        int wallMaxX = 35;
        int wallMinY = 10;
        int wallMaxY = 40;

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
            GameObject res = Instantiate(tilePrefab, position, Quaternion.identity, tileTransform.transform);
            Tile tileComp = res.AddComponent<Tile>();
            tileComp.Initialize(tile);
            tiles[tile.x, tile.y] = tileComp;
            
            // 플레이어 스폰 위치 체크
            if (tileComp.tileType == 1)
            {
                Debug.Log("player spawn able");
                tileComp.Highlight(Color.green);
                CharacterSpawnUseTiles.Add(tileComp);
            }

            if (tile.tileType > 100)
            {
                foreach (var enemy in EnmeyPrefab)
                {
                    EnemyData enemyData = enemy.GameObject.GetComponent<EnemyData>();
                    
                    if (tile.tileType == enemyData.EnemyID)
                    {
                        var spawnEnemey = Instantiate(enemyData.GameObject,tileComp.transform.position + Vector3.up, Quaternion.identity);
                        Tile applyTileObj = GetClosestTile(spawnEnemey.transform.position);
                        applyTileObj.SetOccupant(spawnEnemey.GetComponent<IDamageAble>());
                         
                        // add enemy unit 
                        GameManager.Instance.EnemyUnits.Add(spawnEnemey.GetComponent<EnemyData>());
                    }
                }
            }

            if (tile.tileType == 201)
            {
                var structurePosition = new Vector3(tile.x, 0.5f, tile.y);

                if (tile.x == wallMinX && tile.y % 5 == 0 && tile.y >= wallMinY && tile.y < wallMaxY)
                {
                    GameObject structure201 = new GameObject("Structure201");
                    structure201.transform.SetParent(wallTransform.transform);
                    structure201.transform.position = structurePosition;
                    
                    Instantiate(structurePrefab201Left, structure201.transform);
                }
                
                if (tile.y == wallMaxY && tile.x % 5 == 0 && tile.x >= wallMinX && tile.x < wallMaxX)
                {
                    GameObject structure201 = new GameObject("Structure201");
                    structure201.transform.SetParent(wallTransform.transform);
                    structure201.transform.position = structurePosition;
                    
                    Instantiate(structurePrefab201Up, structure201.transform);
                }
                
                if (tile.x == wallMaxX && tile.y % 5 == 0 && tile.y >= wallMinY && tile.y < wallMaxY)
                {
                    GameObject structure201 = new GameObject("Structure201");
                    structure201.transform.SetParent(wallTransform.transform);
                    structure201.transform.position = structurePosition;
                    
                    Instantiate(structurePrefab201Right, structure201.transform);
                }
                
                if (tile.y == wallMinY && tile.x % 5 == 0 && tile.x >= wallMinX && tile.x < wallMaxX)
                {
                    GameObject structure201 = new GameObject("Structure201");
                    structure201.transform.SetParent(wallTransform.transform);
                    structure201.transform.position = structurePosition;
                    
                    Instantiate(structurePrefab201Down, structure201.transform);
                }
            }
            
            if (tile.tileType == 202)
            {
                var structurePosition = new Vector3(tile.x, 0.5f, tile.y);
                
                GameObject structure202 = new GameObject("Structure202");
                structure202.transform.SetParent(structureTransform.transform);
                structure202.transform.position = structurePosition;
                
                Instantiate(structurePrefab202, structure202.transform);
            }
            
            if (tile.tileType == 203)
            {
                var structurePosition = new Vector3(tile.x, 0.5f, tile.y);
                
                GameObject structure203 = new GameObject("Structure203");
                structure203.transform.SetParent(structureTransform.transform);
                structure203.transform.position = structurePosition;
                
                Instantiate(structurePrefab203, structure203.transform);
            }
            
            if (tile.tileType == 204)
            {
                var structurePosition = new Vector3(tile.x, 0.5f, tile.y);
                
                GameObject structure204 = new GameObject("Structure204");
                structure204.transform.SetParent(structureTransform.transform);
                structure204.transform.position = structurePosition;
                
                Instantiate(structurePrefab204, structure204.transform);
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
    
    public Tile GetCurrentTileByIDamageAble(IDamageAble damageAble)
    {
        return GetClosestTile(damageAble.GameObject.transform.position);
    }
    
    public void SetSelectedTile(Tile tile)
    {
        defaultMaterial = tile.GetComponent<Renderer>().material;
        
        if (previousSelectedTile != null)
            previousSelectedTile.SetOutline(false);

        selectedTile = tile;
        selectedTile.SetOutline(true);

        previousSelectedTile = selectedTile;
    }
    
    public List<Vector2Int> GetReachableTiles(Vector2Int origin, int range)
    {
        List<Vector2Int> reachable = new List<Vector2Int>();
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist <= range)
                {
                    int x = origin.x + dx;
                    int y = origin.y + dy;
                    if (x >= 0 && y >= 0)
                        reachable.Add(new Vector2Int(x, y));
                }
            }
        }

        return reachable;
    }
}

