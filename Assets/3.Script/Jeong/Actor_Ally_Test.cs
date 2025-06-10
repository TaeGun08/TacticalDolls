using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Ally_Test : Actor_Test
{
    public override void MyTurn(Actor_Test[] actor)
    {
        gridBehavior.Actor = transform;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile == null) return;
            if (tile.isWalkable == false) return;
            gridBehavior.IsMove = true;
            gridBehavior.PathFind(gridBehavior.RoundToTilePosition(gridBehavior.Actor.position), 
                new Vector3Int(tile.x, 0, tile.y));
        }
    }
}
