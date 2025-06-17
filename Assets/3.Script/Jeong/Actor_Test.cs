using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Actor_Test : MonoBehaviour, IDamageAble
{
    protected GridBehavior gridBehavior;
    protected Turn_Test turn;
    protected CombatSystem combatSystem;
    
    [SerializeField] protected int moveRange;
    [SerializeField] protected int attackRange;

    public TaskCompletionSource<bool> SkillTcs;
    
    public IStat Stat { get; }
    
    public Collider MainCollider { get; }
    public GameObject GameObject => gameObject;
    public int Team { get; }
    
    protected virtual void Start()
    {
        gridBehavior = GridBehavior.Instance;
        turn = Turn_Test.Instance;
        combatSystem = CombatSystem.Instance;
    }
    
    public List<Vector2Int> GetReachableTiles()
    {
        List<Vector2Int> reachable = new List<Vector2Int>();
        Vector3Int origin = PathFindingManager.Instance.RoundToTilePosition(transform.position);
        
        for (int dx = -moveRange; dx <= moveRange; dx++)
        {
            for (int dy = -moveRange; dy <= moveRange; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist <= moveRange)
                {
                    int x = origin.x + dx;
                    int y = origin.z + dy;
                    if (x >= 0 && y >= 0)
                        reachable.Add(new Vector2Int(x, y));
                }
            }
        }
        
        return reachable;
    }

    public List<Vector2Int> GetAttackableTilesFromReachable()
    {
        Vector3Int origin = PathFindingManager.Instance.RoundToTilePosition(transform.position);
        HashSet<Vector2Int> attackable = new HashSet<Vector2Int>();

        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= attackRange)
                {
                    int x = origin.x + dx;
                    int y = origin.z + dy;
                    if (x >= 0 && y >= 0)
                        attackable.Add(new Vector2Int(x, y));
                }
            }
        }

        return attackable.ToList();
    }
    
    public List<Vector2Int> GetReachableTiles(Vector2Int origin, int range)
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

    public List<Vector2Int> GetAttackableTilesFromReachable(Vector2Int origin, int moveRange, int attackRange)
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
    
    public async Task Excute(int selectedSkill)
    {
        // //스킬 애니메이션
        // //애니메이션에 맞춰 스킬 효과 적용 (데미지, 힐,)
        // SamplePlayer samplePlayer = new SamplePlayer(); //쓰레기값
        // SkillSample sampleSkill = new SkillSample(); //쓰레기값
        // sampleSkill.MakeSkillSequence(samplePlayer, samplePlayer);
        SkillTcs = new TaskCompletionSource<bool>();
        Debug.Log("뿡");
        combatSystem.ExecuteSkill(this, selectedSkill, SkillTcs);
        await SkillTcs.Task;
        Debug.Log("잉~긔뭐링~");
    }
    
    public void TakeDamage(CombatEvent combatEvent)
    {
    }

    public void TakeHeal(HealEvent combatEvent)
    {
    }

    public void TakeBuff(BuffEvent combatEvent)
    {
    }
}
