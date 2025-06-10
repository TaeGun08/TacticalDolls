using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapJsonGenerator : MonoBehaviour
{
    [ContextMenu("GenerateMapJson")]
    void GenerateMapJson()
    {
        TileDataListWrapper wrapper = new TileDataListWrapper();
        wrapper.stage1 = new List<TileData>();

        for (int x = 0; x <= 50; x++)
        {
            for (int y = 0; y <= 50; y++)
            {
                TileData tile = new TileData
                {
                    x = x,
                    y = y,
                    isWalkable = true,
                    tileType = TileType.Normal,
                    obstacleDir = ObstacleDir.None
                };
                wrapper.stage1.Add(tile);
            }
        }

        string json = JsonUtility.ToJson(wrapper, true); // true = pretty print
        string path = Path.Combine(Application.dataPath, "Resources/TileData.json");

        File.WriteAllText(path, json);

        Debug.Log("Map JSON generated at: " + path);
    }

    private void Start()
    {
        GenerateMapJson();
    }
}

[System.Serializable]
public class TileData
{
    public int x;
    public int y;
    public bool isWalkable;
    public TileType tileType;
    public ObstacleDir obstacleDir;
}

public enum TileType
{
    Normal = 0,
    Obstacle = 1,
    StartPoint = 2,
    GoalPoint = 3
}

public enum ObstacleDir
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4
}

[System.Serializable]
public class TileDataListWrapper
{
    public List<TileData> stage1;
}