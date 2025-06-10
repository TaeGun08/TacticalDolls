using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCurrentTileSetter : MonoBehaviour
{
    public Tile currentTile;

    public void SetPosition(Tile tile)
    {
        currentTile = tile;
        transform.position = tile.transform.position + Vector3.up * 1f;
    }
}
