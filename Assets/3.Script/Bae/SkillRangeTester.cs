using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeTester : MonoBehaviour
{
    public static SkillRangeTester Instance;

    public enum RangeType { Straight, Plus, Cross, Around }
    public RangeType currentRangeType = RangeType.Around;
    public int currentRange = 3;

    private bool isRangeVisible = false;
    public CharacterCurrentTileSetter playerUnit; // 현재 캐릭터 (혹은 선택된 캐릭터)

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleSkillRange();
        }
    }

    private void ToggleSkillRange()
    {
        if (playerUnit == null || playerUnit.currentTile == null) return;

        if (isRangeVisible)
        {
            ResetAllHighlights();
            isRangeVisible = false;
        }
        else
        {
            HighlightSkillRange(playerUnit.currentTile, currentRangeType, currentRange);
            isRangeVisible = true;
        }
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

    public void ResetAllHighlights()
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.ResetHighlight();
        }
    }
}
