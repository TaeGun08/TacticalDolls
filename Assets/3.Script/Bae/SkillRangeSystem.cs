using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeSystem : MonoBehaviour
{
    public static SkillRangeSystem Instance;
    
    private CharacterData currentCharacterData;
    
    private Tile[,] tiles;
    private Tile currentTile;

    private int selectedSkillIndex;
    private bool isRangeVisible = false;

    private void Awake()
    {
        Instance = this;
    }

    public void SetAllTiles()
    {
        tiles = TileManager.Instance.tiles;
    }

    private Tile GetCurrentTile(CharacterData characterData)
    {
        currentCharacterData = characterData;
        
        return TileManager.Instance.GetClosestTile(currentCharacterData.gameObject.transform.position);
    }
    
    public void ShowSkillRange(CharacterData characterData, int index)
    {
        currentTile = GetCurrentTile(characterData);
        selectedSkillIndex = index;
        
        if (currentTile == null) return;

        if (isRangeVisible)
        {
            ResetAllHighlights();
            isRangeVisible = false;
        }
        else
        {
            HighlightAllTilesInRange(
                currentTile, 
                currentCharacterData.Skills[selectedSkillIndex].RangeType, 
                currentCharacterData.Skills[selectedSkillIndex].Range);
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
        
        isRangeVisible = false;
    }
}
