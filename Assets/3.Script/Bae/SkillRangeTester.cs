using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeTester : MonoBehaviour
{
    public static SkillRangeTester Instance { get; private set; }

    public enum RangeType { Straight, Plus, Cross, Around }

    public RangeType currentRangeType = RangeType.Straight;
    public int currentRange = 5;

    private void Awake()
    {
        Instance = this;
    }

    public void OnTileClicked(Tile clickedTile)
    {
        Debug.Log($"TileManager handling click at ({clickedTile.x}, {clickedTile.y})");

        HighlightSkillRange(clickedTile, currentRangeType, currentRange);
    }

    public void HighlightSkillRange(Tile centerTile, RangeType rangeType, int range)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in allTiles)
        {
            int dx = Mathf.Abs(tile.x - centerTile.x);
            int dy = Mathf.Abs(tile.y - centerTile.y);

            bool inRange = false;

            switch (rangeType)
            {
                case RangeType.Straight:
                    inRange = (dx == 0 && dy <= range) || (dy == 0 && dx <= range);
                    break;

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
            }
            else
            {
                tile.ResetHighlight();
            }
        }
    }
}
