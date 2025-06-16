using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Actor_Test : MonoBehaviour
{
    protected GridBehavior_Test gridBehavior;
    protected Turn_Test turn;
    public int MoveRange;
    public int AttackRange;

    protected virtual void Start()
    {
        gridBehavior = GridBehavior_Test.Instance;
        turn = Turn_Test.Instance;
    }

    public abstract void OnMoveStart();

    protected abstract void OnMoveEnd();
    
    protected List<Vector2Int> GetReachableTiles(Vector2Int origin, int range)
    {
        List<Vector2Int> reachable = new List<Vector2Int>();
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist <= range)
                {
                    int x = origin.x + dx;
                    int y = origin.y + dy;
                    if (x >= 0 && y >= 0)
                        reachable.Add(new Vector2Int(x, y));
                }
            }
        }

        return reachable;
    }

    protected List<Vector2Int> GetAttackableTilesFromReachable(Vector2Int origin, int moveRange, int attackRange)
    {
        var reachable = GetReachableTiles(origin, moveRange);
        HashSet<Vector2Int> attackable = new HashSet<Vector2Int>();

        foreach (var moveTile in reachable)
        {
            for (int dx = -attackRange; dx <= attackRange; dx++)
            {
                for (int dy = -attackRange; dy <= attackRange; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= attackRange)
                    {
                        int x = moveTile.x + dx;
                        int y = moveTile.y + dy;
                        if (x >= 0 && y >= 0)
                            attackable.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return attackable.ToList();
    }
}
