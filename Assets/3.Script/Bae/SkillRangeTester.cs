using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeTester : MonoBehaviour
{
    public static SkillRangeTester Instance;

    public enum RangeType { Straight, Plus, Cross, Around }
    public RangeType currentRangeType = RangeType.Around;
    public int currentRange = 3;
    
    public Tile[,] tiles;
    public Tile currentTile;

    private bool isRangeVisible = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tiles = TileManager.Instance.tiles;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleSkillRange();
        }
    }

    public Tile GetCurrentTile()
    {
        return TileManager.Instance.GetClosestTile(transform.position);
    }
    
    private void ToggleSkillRange()
    {
        currentTile = GetCurrentTile();
        
        if (currentTile == null) return;

        if (isRangeVisible)
        {
            ResetAllHighlights();
            isRangeVisible = false;
        }
        else
        {
            HighlightAllTilesInRange(currentTile, currentRangeType, currentRange);
            isRangeVisible = true;
        }
    }

    public void HighlightAllTilesInRange(Tile centerTile, RangeType rangeType, int range)
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
                    tile.Highlight(Color.cyan);
                else
                    tile.ResetHighlight();
            }
        }
    }

    public void ResetAllHighlights()
    {
        foreach (Tile tile in tiles)
        {
            tile.ResetHighlight();
        }
    }
}
