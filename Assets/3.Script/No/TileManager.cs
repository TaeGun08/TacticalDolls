using System.Collections.Generic;
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
    public TextAsset jsonFile;
    public float tileSize = 1.0f;
    public GameObject tilePrefab;

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
    }

    [System.Serializable]
    public class MapData
    {
        public Dictionary<string, List<TileData>> Stages;
    }

    private void LoadMap()
    {
        MapData mapTiles = JsonConvert.DeserializeObject<MapData>(jsonFile.text);

        string stageKey = selectedStage.ToString(); 

        foreach (TileData tile in mapTiles.Stages[stageKey])
        {
            Vector3 position = new Vector3(tile.x * tileSize, 0, tile.y * tileSize);
            var res = Instantiate(tilePrefab, position, Quaternion.identity, transform);

            var tileComp = res.AddComponent<Tile>();
            tileComp.Initialize(tile);
        }
    }

    private void Start()
    {
        LoadMap();
    }
}

