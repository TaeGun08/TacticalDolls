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
        wrapper.Stage1 = new List<TileData>();

        for (int x = 0; x <= 50; x++)
        {
            for (int y = 0; y <= 50; y++)
            {
                if (x <= 30 && x >= 20 && y == 5)
                {
                    // player spawn 
                    TileData tile = new TileData
                    {
                        x = x,
                        y = y,
                        isWalkable = true,
                        tileType = 1,
                        obstacleDir = ObstacleDir.None,
                        isUsingTile = false
                    };
                    
                    wrapper.Stage1.Add(tile);

                } else if (x <= 30 && x >= 20 && y == 30)
                {
                    // enemy spawn 
                    TileData tile = new TileData
                    {
                        x = x,
                        y = y,
                        isWalkable = true,
                        tileType = 101,
                        obstacleDir = ObstacleDir.None,
                        isUsingTile = true
                    };
                    
                    wrapper.Stage1.Add(tile);
                }
                else
                {
                    TileData tile = new TileData
                    {
                        x = x,
                        y = y,
                        isWalkable = true,
                        tileType = 0,
                        obstacleDir = ObstacleDir.None,
                        isUsingTile = false
                    };
                    
                    wrapper.Stage1.Add(tile);
                }
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
    public int tileType;
    public ObstacleDir obstacleDir;
    public bool isUsingTile;
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
    public List<TileData> Stage1;
}