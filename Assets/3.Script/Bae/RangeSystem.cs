using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSystem : MonoBehaviour
{
    public static RangeSystem Instance { get;  private set; }

    [Header("Tile Settings")]
    private Tile[,] tiles;
    private Tile currentTile;

    private List<Tile> movableTiles = new List<Tile>();
    private List<Tile> usableTiles = new List<Tile>();
    
    public List<Tile> attackableTiles;
    
    public List<IDamageAble> damageAbles = new List<IDamageAble>();
        

    private void Awake()
    {
        Instance = this;
    }
    
    public void SetAllTiles()
    {
        tiles = TileManager.Instance.tiles;
    }
    
    public void ShowMoveRange(Tile targetData, int range)
    {
        currentTile = targetData;

        if (currentTile == null) return;
        
        HighlightAllTilesInRange(currentTile, range);
        
        void HighlightAllTilesInRange(Tile centerTile, int range)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile tile = tiles[x, y];
                    if (tile == null) continue;

                    int dx = Mathf.Abs(tile.x - centerTile.x);
                    int dy = Mathf.Abs(tile.y - centerTile.y);

                    bool inRange = false;

                    inRange = (dx + dy) <= range;

                    if (inRange && tile.isWalkable)
                    {
                        tile.Highlight(Color.white);
                        movableTiles.Add(tile);
                    }
                    else
                        tile.ResetHighlight();
                }
            }

            if (GameManager.Instance.MoveChoiceTile != null)
            {
                GameManager.Instance.MoveChoiceTile.Highlight(Color.magenta);
            }
        }
    }
    
    public void ShowAttackRange(Tile targetData, int range)
    {
        currentTile = targetData;

        if (currentTile == null) return;
        
        HighlightAllTilesInRange(currentTile, range);
        
        void HighlightAllTilesInRange(Tile centerTile, int range)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile tile = tiles[x, y];
                    if (tile == null) continue;

                    int dx = Mathf.Abs(tile.x - centerTile.x);
                    int dy = Mathf.Abs(tile.y - centerTile.y);

                    bool inRange = false;

                    inRange = (dx + dy) <= range;

                    if (inRange && tile.isWalkable)
                    {
                        tile.Highlight(Color.yellow);
                        attackableTiles.Add(tile);
                    }
                    else
                        tile.ResetHighlight();
                }
            }
            
            centerTile.Highlight(Color.magenta);
        }
    }
    
    public void ShowSkillRange(IDamageAble attackAble, IDamageAble targetAble, int index)
    {
        Tile tempTile = GameManager.Instance.MoveChoiceTile == null
            ? TileManager.Instance.GetCurrentTileByIDamageAble(attackAble)
            : GameManager.Instance.MoveChoiceTile;
        
        ShowAttackRange(tempTile, attackAble.Stat.MoveRange);
        usableTiles.Clear();
        damageAbles.Clear();
        
        currentTile = TileManager.Instance.GetCurrentTileByIDamageAble(targetAble);

        if (currentTile == null) return;

        SkillEffectHandlerBase skill = null;
        skill = attackAble.Stat.Skills[index];

        if (skill == null) return;

        HighlightAllTilesInRange(currentTile, skill.RangeType, skill.Range);
        SetDamageAbles();
        
        void HighlightAllTilesInRange(Tile centerTile, RangeType rangeType, int range)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile tile = tiles[x, y];
                    if (tile == null) continue;

                    int dx = Mathf.Abs(tile.x - centerTile.x);
                    int dy = Mathf.Abs(tile.y - centerTile.y);

                    bool inRange = false;

                    switch (rangeType)
                    {
                        case RangeType.Straight:
                        case RangeType.Plus:
                            inRange = (dx == 0 && dy <= range) || (dy == 0 && dx <= range);
                            break;
                        case RangeType.Cross:
                            inRange = (dx == dy && dx <= range);
                            break;
                        case RangeType.Around:
                            inRange = (dx + dy) <= range;
                            break;
                    }

                    if (inRange && tile.isWalkable)
                    {
                        tile.Highlight(Color.cyan);
                        usableTiles.Add(tile);
                    }
                }
            }
            
            currentTile.Highlight(Color.black);
        }
        
        void SetDamageAbles()
        {
            for (int i = 0; i < usableTiles.Count; i++)
            {
                if (usableTiles[i].GetOccupant() != null)
                {
                    damageAbles.Add(usableTiles[i].GetOccupant());
                }
            }
        
            Debug.Log($"DamageAbles: {damageAbles.Count}");
        }
    }
    
    public void ResetAllTiles()
    {
        foreach (Tile tile in tiles)
        {
            tile.ResetHighlight();
        }
        
        movableTiles.Clear();   // 이동범위 타일 저장 초기화
        attackableTiles.Clear();    // 사거리범위 타일 저장 초기화
        usableTiles.Clear();    // 스킬범위 타일 저장 초기화 
        damageAbles.Clear();    // 스컬범위 내의 IDamageAble 저장 초기화
    }
    
    public bool IsTileInMoveRange(Tile tile)
    {
        return movableTiles.Contains(tile);
    }
}
