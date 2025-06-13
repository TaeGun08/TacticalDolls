using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeSystem : MonoBehaviour
{
    public static SkillRangeSystem Instance;
    
    private IDamageAble target;

    private Tile[,] tiles;
    private Tile currentTile;

    private int selectedSkillIndex;
    private bool isRangeVisible = false;

    private List<Tile> usableTiles = new List<Tile>();
    
    public List<IDamageAble> damageAbles = new List<IDamageAble>();
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetAllTiles()
    {
        tiles = TileManager.Instance.tiles;
    }

    private Tile GetCurrentTile(IDamageAble unit)
    {
        target = unit;

        if (target == null)
            return null;

        return TileManager.Instance.GetClosestTile(target.GameObject.transform.position);
    }
    
    public void ShowSkillRange(IDamageAble unit, int index)
    {
        currentTile = GetCurrentTile(unit);
        selectedSkillIndex = index;

        if (currentTile == null) return;

        SkillSO skill = null;
        skill = unit.Stat.Skills[index];

        if (skill == null) return;

        if (isRangeVisible)
        {
            ResetAllHighlights();
            isRangeVisible = false;
        }
        else
        {
            HighlightAllTilesInRange(currentTile, skill.RangeType, skill.Range);
            isRangeVisible = true;
        }
    }


    private void HighlightAllTilesInRange(Tile centerTile, RangeType rangeType, int range)
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

                if (inRange)
                {
                    tile.Highlight(Color.cyan);
                    usableTiles.Add(tile);
                }
                else
                    tile.ResetHighlight();
            }
        }
        
        SetDamageAbles();
    }

    public void ResetAllHighlights()
    {
        foreach (Tile tile in tiles)
        {
            tile.ResetHighlight();
        }
        
        isRangeVisible = false;
    }

    public void SetDamageAbles()
    {
        for (int i = 0; i < usableTiles.Count; i++)
        {
            if (usableTiles[i].GetOccupant() != null)
            {
                damageAbles.Add(usableTiles[i].GetOccupant());
            }
        }
        
        Debug.Log($"damageAbles: {damageAbles.Count}");
    }

    public void ClearUsableTiles()
    {
        usableTiles.Clear();
    }
    
    public void ClearDamageAbles()
    {
        damageAbles.Clear();
    }
}
