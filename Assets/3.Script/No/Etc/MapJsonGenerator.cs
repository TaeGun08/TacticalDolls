using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapJsonGenerator : MonoBehaviour
{
    private Vector2Int[] offsets = { 
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };
    
    private ObstacleDir[] dirs = { 
        ObstacleDir.Down,
        ObstacleDir.Up,
        ObstacleDir.Right,
        ObstacleDir.Left
    };

    private int maxX = 50;
    private int maxY = 50;
    
    [ContextMenu("GenerateMapJson")]
    private void GenerateMapJson()
    {
        TileDataListWrapper wrapper = new TileDataListWrapper();
        wrapper.Stage1 = new List<TileData>();
        wrapper.Stage2 = new List<TileData>();
        wrapper.Stage3 = new List<TileData>();
        wrapper.Stage4 = new List<TileData>();

        GenerateStage1(wrapper);
        GenerateStage2(wrapper);
        GenerateStage3(wrapper);
        GenerateStage4(wrapper);
        
        string json = JsonUtility.ToJson(wrapper, true); // true = pretty print
        string path = Path.Combine(Application.dataPath, "Resources/TileData.json");

        File.WriteAllText(path, json);

        Debug.Log("Map JSON generated at: " + path);
    }
    
    TileData GetTile(List<TileData> tileList, int x, int y)
    {
        return tileList.Find(t => t.x == x && t.y == y);
    }

    private void Start()
    {
        GenerateMapJson();
    }

    private void GenerateStage1(TileDataListWrapper wrapper)
    {
        List<Vector2Int> playerSpawnPoints = new List<Vector2Int> 
        {
            new Vector2Int(21, 13),
        };
    
        List<Vector2Int> enemySpawnPoints101 = new List<Vector2Int> 
        {
            new Vector2Int(25, 29),
            new Vector2Int(27, 37),
        };
        
        List<Vector2Int> enemySpawnPoints102 = new List<Vector2Int> 
        {
            new Vector2Int(19, 36),
        };
    
        List<Vector2Int> obstaclePoints202 = new List<Vector2Int> 
        {
            new Vector2Int(16, 11),
            new Vector2Int(16, 12),
            new Vector2Int(16, 13),
            new Vector2Int(16, 14),
            new Vector2Int(16, 15),
    
            #region X: 26~30, Y:27~31
    
            new Vector2Int(26, 27),
            new Vector2Int(27, 27),
            new Vector2Int(28, 27),
            new Vector2Int(29, 27),
            new Vector2Int(30, 27),
            new Vector2Int(26, 28),
            new Vector2Int(27, 28),
            new Vector2Int(28, 28),
            new Vector2Int(29, 28),
            new Vector2Int(30, 28),
            new Vector2Int(26, 29),
            new Vector2Int(27, 29),
            new Vector2Int(28, 29),
            new Vector2Int(29, 29),
            new Vector2Int(30, 29),
            new Vector2Int(26, 30),
            new Vector2Int(27, 30),
            new Vector2Int(28, 30),
            new Vector2Int(29, 30),
            new Vector2Int(30, 30),
            new Vector2Int(26, 31),
            new Vector2Int(27, 31),
            new Vector2Int(28, 31),
            new Vector2Int(29, 31),
            new Vector2Int(30, 31),
    
            #endregion
            
        };
    
        List<Vector2Int> obstaclePoints203 = new List<Vector2Int> 
        {
            new Vector2Int(19, 15),
            new Vector2Int(20, 15),
            new Vector2Int(21, 15),
            new Vector2Int(22, 15),
            new Vector2Int(23, 15),
    
            #region X: 27~34, Y: 17~23
    
            new Vector2Int(27, 17),
            new Vector2Int(28, 17),
            new Vector2Int(29, 17),
            new Vector2Int(30, 17),
            new Vector2Int(31, 17),
            new Vector2Int(32, 17),
            new Vector2Int(33, 17),
            new Vector2Int(34, 17),
            new Vector2Int(27, 18),
            new Vector2Int(28, 18),
            new Vector2Int(29, 18),
            new Vector2Int(30, 18),
            new Vector2Int(31, 18),
            new Vector2Int(32, 18),
            new Vector2Int(33, 18),
            new Vector2Int(34, 18),
            new Vector2Int(27, 19),
            new Vector2Int(28, 19),
            new Vector2Int(29, 19),
            new Vector2Int(30, 19),
            new Vector2Int(31, 19),
            new Vector2Int(32, 19),
            new Vector2Int(33, 19),
            new Vector2Int(34, 19),
            new Vector2Int(27, 20),
            new Vector2Int(28, 20),
            new Vector2Int(29, 20),
            new Vector2Int(30, 20),
            new Vector2Int(31, 20),
            new Vector2Int(32, 20),
            new Vector2Int(33, 20),
            new Vector2Int(34, 20),
            new Vector2Int(27, 21),
            new Vector2Int(28, 21),
            new Vector2Int(29, 21),
            new Vector2Int(30, 21),
            new Vector2Int(31, 21),
            new Vector2Int(32, 21),
            new Vector2Int(33, 21),
            new Vector2Int(34, 21),
            new Vector2Int(27, 22),
            new Vector2Int(28, 22),
            new Vector2Int(29, 22),
            new Vector2Int(30, 22),
            new Vector2Int(31, 22),
            new Vector2Int(32, 22),
            new Vector2Int(33, 22),
            new Vector2Int(34, 22),
            new Vector2Int(27, 23),
            new Vector2Int(28, 23),
            new Vector2Int(29, 23),
            new Vector2Int(30, 23),
            new Vector2Int(31, 23),
            new Vector2Int(32, 23),
            new Vector2Int(33, 23),
            new Vector2Int(34, 23),
    
            #endregion
    
            #region X: 16~20, Y: 24~30
    
            new Vector2Int(16, 24),
            new Vector2Int(17, 24),
            new Vector2Int(18, 24),
            new Vector2Int(19, 24),
            new Vector2Int(20, 24),
            new Vector2Int(16, 25),
            new Vector2Int(17, 25),
            new Vector2Int(18, 25),
            new Vector2Int(19, 25),
            new Vector2Int(20, 25),
            new Vector2Int(16, 26),
            new Vector2Int(17, 26),
            new Vector2Int(18, 26),
            new Vector2Int(19, 26),
            new Vector2Int(20, 26),
            new Vector2Int(16, 27),
            new Vector2Int(17, 27),
            new Vector2Int(18, 27),
            new Vector2Int(19, 27),
            new Vector2Int(20, 27),
            new Vector2Int(16, 28),
            new Vector2Int(17, 28),
            new Vector2Int(18, 28),
            new Vector2Int(19, 28),
            new Vector2Int(20, 28),
            new Vector2Int(16, 29),
            new Vector2Int(17, 29),
            new Vector2Int(18, 29),
            new Vector2Int(19, 29),
            new Vector2Int(20, 29),
            new Vector2Int(16, 30),
            new Vector2Int(17, 30),
            new Vector2Int(18, 30),
            new Vector2Int(19, 30),
            new Vector2Int(20, 30),
    
            #endregion
            
            new Vector2Int(23, 35),
            new Vector2Int(24, 35),
            new Vector2Int(25, 35),
            new Vector2Int(26, 35),
    
        };
    
        List<Vector2Int> obstaclePoints204 = new List<Vector2Int> 
        {
            new Vector2Int(17, 20),
            new Vector2Int(18, 20),
            new Vector2Int(19, 20),
            new Vector2Int(20, 20),
            
            new Vector2Int(22, 20),
            new Vector2Int(23, 20),
            new Vector2Int(24, 20),
            new Vector2Int(25, 20),
            
            new Vector2Int(23, 27),
            new Vector2Int(24, 27),
            new Vector2Int(25, 27),
            new Vector2Int(26, 27),
            
            new Vector2Int(18, 34),
            new Vector2Int(19, 34),
            new Vector2Int(20, 34),
            new Vector2Int(21, 34),
            
            new Vector2Int(28, 34),
            new Vector2Int(29, 34),
            new Vector2Int(30, 34),
            new Vector2Int(31, 34),
        };
        
        for (int x = 0; x <= maxX; x++)
        {
            for (int y = 0; y <= maxY; y++)
            {
                TileData tile = new TileData
                {
                    x = x,
                    y = y,
                    isWalkable = false,
                    tileType = 0,
                    obstacleDir = ObstacleDir.None,
                    isUsingTile = false
                };

                // 벽 내부 이동 가능 처리
                if (x > 15 && x < 35 && y > 10 && y < 40)
                {
                    tile.isWalkable = true;
                }
                
                // 벽 스폰 위치 (201)
                if ((x == 15 || x == 35) && y >= 10 && y <= 40)
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                if (x > 15 && x < 35 && (y == 10 || y == 40))
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                // 플레이어 스폰 위치
                if (playerSpawnPoints.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 1;
                    tile.isUsingTile = false;
                }
                
                // 적 스폰 위치
                if (enemySpawnPoints101.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 101;
                    tile.isUsingTile = true;
                }
                
                if (enemySpawnPoints102.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 102;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (202)
                if (obstaclePoints202.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 202;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (203)
                if (obstaclePoints203.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 203;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (204)
                if (obstaclePoints204.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 204;
                    tile.isUsingTile = true;
                }
                
                wrapper.Stage1.Add(tile);
            }
        }
        
        foreach (var pos in obstaclePoints202)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage1, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints203)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage1, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints204)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage1, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
    }
    
    private void GenerateStage2(TileDataListWrapper wrapper)
    {
        List<Vector2Int> playerSpawnPoints = new List<Vector2Int> 
        {
            new Vector2Int(19, 12),
            new Vector2Int(31, 13),
        };
    
        List<Vector2Int> enemySpawnPoints101 = new List<Vector2Int> 
        {
            new Vector2Int(19, 36),
            new Vector2Int(29, 35),
            new Vector2Int(31, 35),
        };
        
        List<Vector2Int> enemySpawnPoints102 = new List<Vector2Int> 
        {
            new Vector2Int(24, 37),
            new Vector2Int(30, 35),
        };
    
        List<Vector2Int> obstaclePoints202 = new List<Vector2Int> 
        {
            new Vector2Int(18, 19),
            new Vector2Int(19, 19),
            new Vector2Int(20, 19),
            new Vector2Int(21, 19),
            
            new Vector2Int(16, 29),
            new Vector2Int(17, 29),
            new Vector2Int(16, 30),
            new Vector2Int(17, 30),
            new Vector2Int(16, 31),
            new Vector2Int(17, 31),
            new Vector2Int(16, 32),
            new Vector2Int(17, 32),
            
            new Vector2Int(26, 30),
            new Vector2Int(27, 30),
            new Vector2Int(28, 30),
            new Vector2Int(29, 30),
        };
    
        List<Vector2Int> obstaclePoints203 = new List<Vector2Int> 
        {
            new Vector2Int(24, 22),
            new Vector2Int(25, 22),
            new Vector2Int(26, 22),
            new Vector2Int(27, 22),
            new Vector2Int(24, 23),
            new Vector2Int(25, 23),
            new Vector2Int(26, 23),
            new Vector2Int(27, 23),
            new Vector2Int(24, 24),
            new Vector2Int(25, 24),
            new Vector2Int(26, 24),
            new Vector2Int(27, 24),
            new Vector2Int(24, 25),
            new Vector2Int(25, 25),
            new Vector2Int(26, 25),
            new Vector2Int(27, 25),
            
            new Vector2Int(20, 28),
            new Vector2Int(21, 28),
            new Vector2Int(22, 28),
            new Vector2Int(20, 29),
            new Vector2Int(21, 29),
            new Vector2Int(22, 29),
            new Vector2Int(20, 30),
            new Vector2Int(21, 30),
            new Vector2Int(22, 30),
        };
    
        List<Vector2Int> obstaclePoints204 = new List<Vector2Int> 
        {
            new Vector2Int(18, 13),
            new Vector2Int(19, 13),
            new Vector2Int(20, 13),
            new Vector2Int(21, 13),
            
            new Vector2Int(24, 15),
            new Vector2Int(25, 15),
            new Vector2Int(26, 15),
            new Vector2Int(27, 15),
            
            new Vector2Int(30, 14),
            new Vector2Int(31, 14),
            new Vector2Int(32, 14),
            
            new Vector2Int(30, 22),
            new Vector2Int(31, 22),
            new Vector2Int(32, 22),
            
            new Vector2Int(32, 30),
            new Vector2Int(33, 30),
            new Vector2Int(34, 30),
            
            new Vector2Int(18, 35),
            new Vector2Int(19, 35),
            new Vector2Int(20, 35),
            
            new Vector2Int(23, 36),
            new Vector2Int(24, 36),
            new Vector2Int(25, 36),
            
            new Vector2Int(28, 34),
            new Vector2Int(29, 34),
            new Vector2Int(30, 34),
            new Vector2Int(31, 34),
            new Vector2Int(32, 34),
        };
        
        for (int x = 0; x <= maxX; x++)
        {
            for (int y = 0; y <= maxY; y++)
            {
                TileData tile = new TileData
                {
                    x = x,
                    y = y,
                    isWalkable = false,
                    tileType = 0,
                    obstacleDir = ObstacleDir.None,
                    isUsingTile = false
                };

                // 벽 내부 이동 가능 처리
                if (x > 15 && x < 35 && y > 10 && y < 40)
                {
                    tile.isWalkable = true;
                }
                
                // 벽 스폰 위치 (201)
                if ((x == 15 || x == 35) && y >= 10 && y <= 40)
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                if (x > 15 && x < 35 && (y == 10 || y == 40))
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                // 플레이어 스폰 위치
                if (playerSpawnPoints.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 1;
                    tile.isUsingTile = false;
                }
                
                // 적 스폰 위치
                if (enemySpawnPoints101.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 101;
                    tile.isUsingTile = true;
                }
                
                if (enemySpawnPoints102.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 102;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (202)
                if (obstaclePoints202.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 202;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (203)
                if (obstaclePoints203.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 203;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (204)
                if (obstaclePoints204.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 204;
                    tile.isUsingTile = true;
                }
                
                wrapper.Stage2.Add(tile);
            }
        }
        
        foreach (var pos in obstaclePoints202)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage2, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints203)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage2, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints204)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage2, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
    }
    
    private void GenerateStage3(TileDataListWrapper wrapper)
    {
        List<Vector2Int> playerSpawnPoints = new List<Vector2Int> 
        {
            new Vector2Int(20, 14),
            new Vector2Int(26, 15),
            new Vector2Int(31, 13),
        };
    
        List<Vector2Int> enemySpawnPoints101 = new List<Vector2Int> 
        {
            new Vector2Int(25, 31),
            new Vector2Int(22, 33),
            new Vector2Int(24, 36),
            new Vector2Int(26, 36),
        };
        
        List<Vector2Int> enemySpawnPoints102 = new List<Vector2Int> 
        {
            new Vector2Int(28, 33),
            new Vector2Int(19, 35),
            new Vector2Int(31, 35),
        };
    
        List<Vector2Int> obstaclePoints202 = new List<Vector2Int> 
        {
            new Vector2Int(18, 13),
            new Vector2Int(18, 14),
            new Vector2Int(18, 15),
            new Vector2Int(19, 15),
            new Vector2Int(20, 15),
            new Vector2Int(21, 15),
            new Vector2Int(22, 15),
            new Vector2Int(22, 14),
            new Vector2Int(22, 13),
            
            new Vector2Int(18, 20),
            new Vector2Int(19, 20),
            new Vector2Int(20, 20),
            
            new Vector2Int(18, 23),
            new Vector2Int(19, 23),
            new Vector2Int(20, 23),
            
            new Vector2Int(24, 30),
            new Vector2Int(25, 30),
            new Vector2Int(26, 30),
            
            new Vector2Int(21, 32),
            new Vector2Int(22, 32),
            new Vector2Int(23, 32),
            
            new Vector2Int(27, 32),
            new Vector2Int(28, 32),
            new Vector2Int(29, 32),
            
            new Vector2Int(18, 34),
            new Vector2Int(19, 34),
            new Vector2Int(20, 34),
            
            new Vector2Int(30, 34),
            new Vector2Int(31, 34),
            new Vector2Int(32, 34),
        };
    
        List<Vector2Int> obstaclePoints203 = new List<Vector2Int> 
        {
            new Vector2Int(25, 16),
            new Vector2Int(26, 16),
            new Vector2Int(27, 16),
            new Vector2Int(25, 17),
            new Vector2Int(26, 17),
            new Vector2Int(27, 17),
            
            new Vector2Int(23, 21),
            new Vector2Int(24, 21),
            new Vector2Int(23, 22),
            new Vector2Int(24, 22),
            
            new Vector2Int(23, 24),
            new Vector2Int(24, 24),
            new Vector2Int(23, 25),
            new Vector2Int(24, 25),
            
            new Vector2Int(23, 37),
            new Vector2Int(23, 36),
            new Vector2Int(23, 35),
            new Vector2Int(24, 35),
            new Vector2Int(25, 35),
            new Vector2Int(26, 35),
            new Vector2Int(27, 35),
            new Vector2Int(27, 36),
            new Vector2Int(27, 37),
        };
    
        List<Vector2Int> obstaclePoints204 = new List<Vector2Int> 
        {
            new Vector2Int(30, 14),
            new Vector2Int(31, 14),
            new Vector2Int(32, 14),
            
            new Vector2Int(28, 21),
            new Vector2Int(29, 21),
            new Vector2Int(30, 21),
            new Vector2Int(31, 21),
            new Vector2Int(32, 21),
            
            new Vector2Int(28, 24),
            new Vector2Int(29, 24),
            new Vector2Int(30, 24),
            new Vector2Int(31, 24),
            new Vector2Int(32, 24),
        };
        
        for (int x = 0; x <= maxX; x++)
        {
            for (int y = 0; y <= maxY; y++)
            {
                TileData tile = new TileData
                {
                    x = x,
                    y = y,
                    isWalkable = false,
                    tileType = 0,
                    obstacleDir = ObstacleDir.None,
                    isUsingTile = false
                };

                // 벽 내부 이동 가능 처리
                if (x > 15 && x < 35 && y > 10 && y < 40)
                {
                    tile.isWalkable = true;
                }
                
                // 벽 스폰 위치 (201)
                if ((x == 15 || x == 35) && y >= 10 && y <= 40)
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                if (x > 15 && x < 35 && (y == 10 || y == 40))
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                // 플레이어 스폰 위치
                if (playerSpawnPoints.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 1;
                    tile.isUsingTile = false;
                }
                
                // 적 스폰 위치
                if (enemySpawnPoints101.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 101;
                    tile.isUsingTile = true;
                }
                
                if (enemySpawnPoints102.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 102;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (202)
                if (obstaclePoints202.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 202;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (203)
                if (obstaclePoints203.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 203;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (204)
                if (obstaclePoints204.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 204;
                    tile.isUsingTile = true;
                }
                
                wrapper.Stage3.Add(tile);
            }
        }
        
        foreach (var pos in obstaclePoints202)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage3, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints203)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage3, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints204)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage3, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
    }
    
    private void GenerateStage4(TileDataListWrapper wrapper)
    {
        List<Vector2Int> playerSpawnPoints = new List<Vector2Int> 
        {
            new Vector2Int(23, 15),
            new Vector2Int(25, 15),
            new Vector2Int(27, 15),
        };
    
        List<Vector2Int> enemySpawnPoints101 = new List<Vector2Int> 
        {
            new Vector2Int(25, 31),
            new Vector2Int(20, 32),
            new Vector2Int(30, 32),
        };
        
        List<Vector2Int> enemySpawnPoints103 = new List<Vector2Int> 
        {
            new Vector2Int(25, 35),
        };
    
        List<Vector2Int> obstaclePoints202 = new List<Vector2Int> 
        {
            new Vector2Int(24, 22),
            new Vector2Int(25, 22),
            new Vector2Int(26, 22),
            
            new Vector2Int(18, 26),
            new Vector2Int(20, 26),
            new Vector2Int(22, 26),
            new Vector2Int(28, 26),
            new Vector2Int(30, 26),
            new Vector2Int(32, 26),
        };
    
        List<Vector2Int> obstaclePoints203 = new List<Vector2Int> 
        {
            new Vector2Int(21, 13),
            new Vector2Int(21, 14),
            new Vector2Int(21, 15),
            new Vector2Int(21, 16),
            new Vector2Int(22, 16),
            new Vector2Int(23, 16),
            new Vector2Int(24, 16),
            new Vector2Int(25, 16),
            new Vector2Int(26, 16),
            new Vector2Int(27, 16),
            new Vector2Int(28, 16),
            new Vector2Int(29, 16),
            new Vector2Int(29, 15),
            new Vector2Int(29, 14),
            new Vector2Int(29, 13),
            
            new Vector2Int(19, 30),
            new Vector2Int(20, 30),
            new Vector2Int(21, 30),
            new Vector2Int(19, 31),
            new Vector2Int(20, 31),
            new Vector2Int(21, 31),
            
            new Vector2Int(29, 30),
            new Vector2Int(30, 30),
            new Vector2Int(31, 30),
            new Vector2Int(29, 31),
            new Vector2Int(30, 31),
            new Vector2Int(31, 31),
        };
    
        List<Vector2Int> obstaclePoints204 = new List<Vector2Int> 
        {
            new Vector2Int(18, 19),
            new Vector2Int(19, 19),
            new Vector2Int(20, 19),
            new Vector2Int(21, 19),
            
            new Vector2Int(29, 19),
            new Vector2Int(30, 19),
            new Vector2Int(31, 19),
            new Vector2Int(32, 19),
            
            new Vector2Int(24, 30),
            new Vector2Int(25, 30),
            new Vector2Int(26, 30),
            
            new Vector2Int(23, 34),
            new Vector2Int(24, 34),
            new Vector2Int(25, 34),
            new Vector2Int(26, 34),
            new Vector2Int(27, 34),
        };
        
        for (int x = 0; x <= maxX; x++)
        {
            for (int y = 0; y <= maxY; y++)
            {
                TileData tile = new TileData
                {
                    x = x,
                    y = y,
                    isWalkable = false,
                    tileType = 0,
                    obstacleDir = ObstacleDir.None,
                    isUsingTile = false
                };

                // 벽 내부 이동 가능 처리
                if (x > 15 && x < 35 && y > 10 && y < 40)
                {
                    tile.isWalkable = true;
                }
                
                // 벽 스폰 위치 (201)
                if ((x == 15 || x == 35) && y >= 10 && y <= 40)
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                if (x > 15 && x < 35 && (y == 10 || y == 40))
                {
                    tile.isWalkable = false;
                    tile.tileType = 201;
                    tile.isUsingTile = true;
                }
                
                // 플레이어 스폰 위치
                if (playerSpawnPoints.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 1;
                    tile.isUsingTile = false;
                }
                
                // 적 스폰 위치
                if (enemySpawnPoints101.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 101;
                    tile.isUsingTile = true;
                }
                
                if (enemySpawnPoints103.Contains(new Vector2Int(x, y)))
                {
                    tile.tileType = 103;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (202)
                if (obstaclePoints202.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 202;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (203)
                if (obstaclePoints203.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 203;
                    tile.isUsingTile = true;
                }
                
                // 구조물 위치 (204)
                if (obstaclePoints204.Contains(new Vector2Int(x, y)))
                {
                    tile.isWalkable = false;
                    tile.tileType = 204;
                    tile.isUsingTile = true;
                }
                
                wrapper.Stage4.Add(tile);
            }
        }
        
        foreach (var pos in obstaclePoints202)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage4, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints203)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage4, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
        
        foreach (var pos in obstaclePoints204)
        {
            for (int i = 0; i < 4; i++)
            {
                int nx = pos.x + offsets[i].x;
                int ny = pos.y + offsets[i].y;

                if (nx < 0 || nx > maxX || ny < 0 || ny > maxY)
                    continue;

                TileData neighborTile = GetTile(wrapper.Stage4, nx, ny);
                if (neighborTile == null) continue;

                neighborTile.obstacleDir = dirs[i];
            }
        }
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
    public List<TileData> Stage2;
    public List<TileData> Stage3;
    public List<TileData> Stage4;
}