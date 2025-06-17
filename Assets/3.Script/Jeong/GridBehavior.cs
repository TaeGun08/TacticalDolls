using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GridBehavior : MonoBehaviour
{
    public static GridBehavior Instance;

    private TileManager tileManager;
    private Turn_Test turn;

    private Camera mainCam;

    public Actor_Test Actor;
    [SerializeField] private LayerMask characterLayer;

    public bool IsMove { get; set; }
    public bool IsAutoMove { get; set; }
    public List<Actor_Test> Actors;
    private float turnCalmVelocity;

    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 1), new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 1), new Vector3Int(-1, 0, -1),
    };

    private HashSet<Vector2Int> reservedTiles = new HashSet<Vector2Int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tileManager = TileManager.Instance;
        turn = Turn_Test.Instance;
        mainCam = Camera.main;
    }

    private void Update()
    {
        AllyInputMove();
        AutoMove();
    }

    private void AllyInputMove()
    {
        if (Input.GetMouseButtonDown(0) && !IsMove)
        {
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitCharacter, 100f,
                    characterLayer))
            {
                Actor = hitCharacter.transform.GetComponent<Actor_Test>();
                foreach (var actor in turn.TurnActor)
                {
                    if (actor.Equals(Actor))
                    {
                        Actor = null;
                        return;
                    }
                }
            }
            else if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                if (Actor == null) return;

                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile == null || !tile.isWalkable) return;

                if (!MoveRangeSystem.Instance.IsTileInMoveRange(tile))
                {
                    Debug.Log("이동 불가능한 범위입니다.");
                    MoveRangeSystem.Instance.ResetAllHighlights();
                    MoveRangeSystem.Instance.ResetMovableTiles();
                    return;
                }

                IsMove = true;
                MoveRangeSystem.Instance.ResetAllHighlights();
                turn.TurnActor.Add(Actor);

                List<Node> path =
                    PathFindingManager.Instance.PathFind(Actor.transform.position, new Vector3Int(tile.x, 0, tile.y));
                StartCoroutine(MovePlayerAlongPath(path, Vector3.zero));
            }
        }
    }

    private void AutoMove()
    {
        if (IsMove || !IsAutoMove || Actor == null) return;

        reservedTiles.Clear();
        foreach (var actor in turn.Ally.Concat(turn.Enemy))
        {
            if (actor != Actor)
            {
                var tilePos = new Vector2Int(
                    Mathf.RoundToInt(actor.transform.position.x),
                    Mathf.RoundToInt(actor.transform.position.z)
                );
                reservedTiles.Add(tilePos);
            }
        }

        Vector3Int targetPos = FindNearestTargetPos();
        Vector3Int finalTargetPos = FindAvailableAdjacentTile(targetPos);

        if (finalTargetPos == Vector3Int.zero)
        {
            Debug.Log("이동 가능한 위치가 없음");
            return;
        }

        // 자기 위치와 같으면 이동 생략
        if (new Vector2Int(finalTargetPos.x, finalTargetPos.z) ==
            new Vector2Int(Mathf.RoundToInt(Actor.transform.position.x), Mathf.RoundToInt(Actor.transform.position.z)))
        {
            IsMove = false;
            return;
        }

        List<Node> path = PathFindingManager.Instance.PathFind(Actor.transform.position, finalTargetPos);
        reservedTiles.Add(new Vector2Int(finalTargetPos.x, finalTargetPos.z));
        IsMove = true;
        if (path == null) return;
        StartCoroutine(MovePlayerAlongPath(path, finalTargetPos));
    }

    private IEnumerator MovePlayerAlongPath(List<Node> path, Vector3 target)
    {
        // 현재 타일 비우기
        Tile currentTile = TileManager.Instance.GetClosestTile(Actor.transform.position);
        if (currentTile != null)
        {
            currentTile.isUsingTile = false;
            currentTile.SetOccupant(null);
        }

        List<Vector2Int> actorPos = Actor.GetReachableTiles();

        foreach (Node node in path)
        {
            Vector3 targetPos = new Vector3(
                node.Position.x * tileManager.tileSize,
                0.5f,
                node.Position.z * tileManager.tileSize
            );

            if (IsAutoMove)
            {
                if (AttackRangeChecker(Actor.GetAttackableTilesFromReachable(),
                        PathFindingManager.Instance.RoundToTilePosition(target))) break;
                if (!MoveRangeChecker(actorPos, targetPos)) break;
            }

            while (Vector3.Distance(Actor.transform.position, targetPos) > 0.05f)
            {
                Actor.transform.position =
                    Vector3.MoveTowards(Actor.transform.position, targetPos, 10f * Time.deltaTime);

                var temp = new Vector2(targetPos.x - Actor.transform.position.x,
                    targetPos.z - Actor.transform.position.z);
                UpdateRotation(Actor.transform, temp, 0.1f);
                yield return null;
            }
        }

        yield return null;
        Tile newTile = TileManager.Instance.GetClosestTile(Actor.transform.position);
        if (newTile != null)
        {
            newTile.isUsingTile = true;
            newTile.SetOccupant(Actor.GetComponent<IDamageAble>());
        }
        
        Actor = null;
        turn.MoveTcs.TrySetResult(true);
        IsMove = false;
        MoveRangeSystem.Instance.ResetMovableTiles();
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
            Vector3.Distance(Actor.transform.position, new Vector3(pos.x, 0, pos.z))).First();
    }

    private Vector3Int FindNearestTargetPos()
    {
        Vector3Int nearest = Vector3Int.zero;
        float minDist = float.MaxValue;

        foreach (var target in Actors)
        {
            if (target == Actor) continue;
            float dist = Vector3.Distance(Actor.transform.position, target.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = new Vector3Int(
                    Mathf.RoundToInt(target.transform.position.x),
                    0,
                    Mathf.RoundToInt(target.transform.position.z)
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