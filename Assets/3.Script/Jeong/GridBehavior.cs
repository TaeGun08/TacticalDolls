using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridBehavior : MonoBehaviour
{
    // 노드에서 노드 이동 처리
    public static GridBehavior Instance;

    private TileManager tileManager;
    private Turn_Test turn;

    private Camera mainCam;

    public IDamageAble Actor;
    [SerializeField] private LayerMask characterLayer;

    public bool IsMove { get; set; }
    public bool IsAutoMove { get; set; }
    public List<IDamageAble> Actors = new List<IDamageAble>();
    private float turnCalmVelocity;

    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 1), new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 1), new Vector3Int(-1, 0, -1),
    };

    private HashSet<Vector2Int> reservedTiles = new HashSet<Vector2Int>();
    
    private Tile moveChoiceTile;
    private Tile skillChoiceTile;

    [SerializeField] private Button enemyTurnEnd; 

    private void Awake()
    {
        Instance = this;
        
        enemyTurnEnd.onClick.AddListener(() =>
        {
            Turn_Test.Instance.MoveTcs.TrySetResult(true);
        });
    }

    private void Start()
    {
        tileManager = TileManager.Instance;
        turn = Turn_Test.Instance;
        mainCam = Camera.main;
    }

    private void Update()
    {
        //AllyInputMove();
        
        // 이동 처리
        // if (Turn_Test.Instance.IsAuto)
        // {
        //     // 자동
        //     AutoMove();
        // }
        
        AutoMove();
    }

    private void AutoMove()
    {
        if (IsMove || IsAutoMove == false || Actor == null) return;
        reservedTiles.Clear();
        Test();
        
        Vector3Int targetPos = FindNearestTargetPos();
        Vector3Int finalTargetPos = FindAvailableAdjacentTile(targetPos);
        
        if (finalTargetPos == Vector3Int.zero)
        {
            Debug.Log("이동 가능한 위치가 없음");
            return;
        }
        
        if (new Vector2Int(finalTargetPos.x, finalTargetPos.z) ==
            new Vector2Int(Mathf.RoundToInt(Actor.GameObject.transform.position.x), Mathf.RoundToInt(Actor.GameObject.transform.position.z)))
        {
            IsMove = false;
            return;
        }
        
        Debug.Log(Actor.GameObject.transform.position);
        
        Debug.Log(finalTargetPos);
        List<Node> path = PathFindingManager.Instance.PathFind(Actor.GameObject.transform.position, finalTargetPos);
        reservedTiles.Add(new Vector2Int(finalTargetPos.x, finalTargetPos.z));
        _= MovePlayerAlongPath(path, finalTargetPos);
    }
    
    public async Task MovePlayerAlongPath(List<Node> path, Vector3 target)
    {
        IsMove = true;
        Tile currentTile = TileManager.Instance.GetClosestTile(Actor.GameObject.transform.position);
        if (currentTile != null)
        {
            currentTile.isUsingTile = false;
            currentTile.SetOccupant(null);
        }
        
        Vector2Int dollPos = new Vector2Int((int)Actor.GameObject.transform.position.x, 
            (int)Actor.GameObject.transform.position.z);
        List<Vector2Int> actorPos = TileManager.Instance.GetReachableTiles(dollPos, Actor.Stat.MoveRange);
        
        foreach (Node node in path)
        {
            Debug.Log("노드 선택");
            Vector3 targetPos = new Vector3(
                node.Position.x * tileManager.tileSize,
                0.5f,
                node.Position.z * tileManager.tileSize
            );
        
            if (IsAutoMove)
            {
                // if (AttackRangeChecker(Actor.GetAttackableTilesFromReachable(),
                //         PathFindingManager.Instance.RoundToTilePosition(target))) break;
                if (MoveRangeChecker(actorPos, targetPos) == false) break;
            }
        
            while (Vector3.Distance(Actor.GameObject.transform.position, targetPos) > 0.05f)
            {
                Debug.Log("이동 시작");
                Actor.GameObject.transform.position =
                    Vector3.MoveTowards(Actor.GameObject.transform.position, targetPos, 10f * Time.deltaTime);
        
                var temp = new Vector2(targetPos.x - Actor.GameObject.transform.position.x,
                    targetPos.z - Actor.GameObject.transform.position.z);
                UpdateRotation(Actor.GameObject.transform, temp, 0.1f);
                await Task.Delay(10);
            }
        }
        
        Tile newTile = TileManager.Instance.GetClosestTile(Actor.GameObject.transform.position);
        if (newTile != null)
        {
            newTile.isUsingTile = true;
            newTile.SetOccupant(Actor);
        }
        
        Actor = null;
        turn.MoveTcs.TrySetResult(true);
        IsMove = false;
    }

    private void Test()
    {
        foreach (IDamageAble actor in GameManager.Instance.PlayerUnits)
        {
            if (actor != Actor)
            {
                var tilePos = new Vector2Int(
                    Mathf.RoundToInt(actor.GameObject.transform.position.x),
                    Mathf.RoundToInt(actor.GameObject.transform.position.z)
                );
                reservedTiles.Add(tilePos);
            }
        }
        
        // foreach (IDamageAble actor in GameManager.Instance.EnemyUnits)
        // {
        //     if (actor != Actor)
        //     {
        //         var tilePos = new Vector2Int(
        //             Mathf.RoundToInt(actor.GameObject.transform.position.x),
        //             Mathf.RoundToInt(actor.GameObject.transform.position.z)
        //         );
        //         reservedTiles.Add(tilePos);
        //     }
        // }
    }

    private bool MoveRangeChecker(List<Vector2Int> actorPos, Vector3 targetPos)
    {
        return actorPos.Contains(new Vector2Int((int)targetPos.x, (int)targetPos.z));
    }

    private bool AttackRangeChecker(List<Vector2Int> attackRange, Vector3 targetPos)
    {
        return attackRange.Contains(new Vector2Int((int)targetPos.x, (int)targetPos.z));
    }

    private Vector3Int FindAvailableAdjacentTile(Vector3Int targetPos)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();

        foreach (var dir in directions.OrderBy(_ => Random.value))
        {
            Vector3Int candidate = targetPos + dir;

            if (candidate.x < 0 || candidate.z < 0 || candidate.x >= 51 || candidate.z >= 51)
                continue;

            Tile tile = tileManager.GetTileAt(candidate.x, candidate.z);
            if (tile == null || !tile.isWalkable) continue;

            Vector2Int tilePos = new Vector2Int(candidate.x, candidate.z);
            if (reservedTiles.Contains(tilePos) || tile.isUsingTile) continue;

            candidates.Add(candidate);
        }

        if (candidates.Count == 0)
            return Vector3Int.zero;

        return candidates.OrderBy(pos =>
            Vector3.Distance(Actor.GameObject.transform.position, new Vector3(pos.x, 0, pos.z))).First();
    }

    private Vector3Int FindNearestTargetPos()
    {
        Vector3Int nearest = Vector3Int.zero;
        float minDist = float.MaxValue;

        foreach (var target in Actors)
        {
            if (target == Actor) continue;
            float dist = Vector3.Distance(Actor.GameObject.transform.position, target.GameObject.transform.position);
        
            if (dist < minDist)
            {
                minDist = dist;
                nearest = new Vector3Int(
                    Mathf.RoundToInt(target.GameObject.transform.position.x),
                    0,
                    Mathf.RoundToInt(target.GameObject.transform.position.z)
                );
            }
        }

        return nearest;
    }

    private void UpdateRotation(Transform player, Vector2 inputAxis, float smoothTime)
    {
        float targetAngle = Mathf.Atan2(inputAxis.x, inputAxis.y) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(player.eulerAngles.y, targetAngle, ref turnCalmVelocity, smoothTime);
        player.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}