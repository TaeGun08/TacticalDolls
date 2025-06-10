using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRangeSystem : MonoBehaviour
{
    public static MoveRangeSystem Instance;
    
    private CharacterData currentCharacterData;
    
    private Tile[,] tiles;
    private Tile currentTile;

    private bool isRangeVisible = false;
    
    private HashSet<Tile> movableTiles = new HashSet<Tile>();

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
    
    public void ShowMoveRange(CharacterData characterData)
    {
        currentTile = GetCurrentTile(characterData);
        
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
                currentCharacterData.Stat.MoveRange);
            isRangeVisible = true;
        }
    }

    private void HighlightAllTilesInRange(Tile centerTile, int range)
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

                if (inRange)
                {
                    tile.Highlight(Color.white);
                    movableTiles.Add(tile);
                }
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
    
    public bool IsTileInMoveRange(Tile tile)
    {
        return movableTiles.Contains(tile);
    }

    public void ResetMovableTiles()
    {
        movableTiles.Clear();
    }
}
