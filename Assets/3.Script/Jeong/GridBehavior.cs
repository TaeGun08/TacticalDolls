using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridBehavior : MonoBehaviour
{
    // 노드에서 노드 이동 처리
    public static GridBehavior Instance;
    
    private static readonly int IS_CROUCHING = Animator.StringToHash("isCrouching");
    private static readonly int IS_RUNNING = Animator.StringToHash("isRunning");

    private PathFindingManager pathFindingManager;
    private TileManager tileManager;

    private Camera mainCam;

    public IDamageAble Actor { get; set; } //현재 움직일 Actor
    [Header("CharacterLayer Setting")]
    [SerializeField] private LayerMask characterLayer;

    public bool IsMove { get; set; } //움직이는 중인지 체크
    public bool IsAutoMove { get; set; } //오토로 움직이는 중인지 체크
    public List<IDamageAble> Actors = new List<IDamageAble>(); //추적할 캐릭터들
    //private float turnCalmVelocity; 

    //이동을 위해 받아올 방향
    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 1), new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 1), new Vector3Int(-1, 0, -1),
    };

    //도착할 위치에 다른 캐릭터가 있을 경우 중복되지 않은 새로운 위치를 뽑기 위한 리스트
    private HashSet<Vector2Int> reservedTiles = new HashSet<Vector2Int>();
    
    [SerializeField] private Button autoButton;
    public bool IsAuto { get; private set; } //플레이어를 Auto로 설정해주는 변수

    private IDamageAble nearestTarget; //공격을 지정을 위해 타겟을 담아주는 변수

    private Node endNode = new Node(); //엄폐 체크를 위해 마지막 도착 노드를 확인 하기 위한 변수

    public class CallBack
    {
        public Action<Transform> startMove { get; set; }
        public Action<Transform> onCompleteMove { get; set; }
    }

    public CallBack callback;

    private void Awake()
    {
        Instance = this;

        autoButton.onClick.AddListener(() =>
        {
            IsAuto = IsAuto == false;
            if (IsAuto && IsMove == false)
            {
                _ = TurnController.Instance.OnCheckEndCharacterActor();
            }
        });
        callback = new CallBack();
    }

    private void Start()
    {
        pathFindingManager = PathFindingManager.Instance;
        tileManager = TileManager.Instance;
        mainCam = Camera.main;
    }

    /// <summary>
    /// 자동 이동을 위한 함수, 자신과 가까운 거리의 Actor를 찾아서 8방향 주위에 있는 경로를 탐색함
    /// </summary>
    public async Task AutoMove(IDamageAble actor)
    {
        Actor = actor;
        reservedTiles.Clear();
        TargetActors();

        Vector3Int finalTargetPos = FindAvailableAdjacentTileToNearestTargets();

        if (finalTargetPos == Vector3Int.zero)
        {
            Debug.Log("이동 가능한 위치가 없음");
            return;
        }

        if (new Vector2Int(finalTargetPos.x, finalTargetPos.z) ==
            new Vector2Int(Mathf.RoundToInt(Actor.GameObject.transform.position.x),
                Mathf.RoundToInt(Actor.GameObject.transform.position.z)))
        {
            IsMove = false;
            return;
        }

        List<Node> path = pathFindingManager.PathFind(Actor.GameObject.transform.position, finalTargetPos);
        reservedTiles.Add(new Vector2Int(finalTargetPos.x, finalTargetPos.z));

        nearestTarget = Actors
            .Where(target => target != Actor && actor.Stat.IsDead == false)
            .OrderBy(target =>
                Vector3.Distance(Actor.GameObject.transform.position, target.GameObject.transform.position))
            .FirstOrDefault();

        Vector3 targetPos = nearestTarget == null
            ? Vector3.zero
            : pathFindingManager.RoundToTilePosition(nearestTarget.GameObject.transform.position);

        await MovePlayerAlongPath(path, targetPos);
    }


    /// <summary>
    /// 경로를 넣어주면 그 경로에 맞는 위치로 이동하는 함수
    /// </summary>
    /// <param name="path"></param>
    /// <param name="target"></param>
    public async Task MovePlayerAlongPath(List<Node> path, Vector3 target)
    {
        IsMove = true; //이동 시작
        
        UsingTileChecker(false, null);; //현재 움직일 Actor의 타일을 받아 옴

        //harang 시작
        if (Actor is CharacterData character) //명시적 형변환 -> Actor가 CharacterData일 경우
        {
            callback.startMove?.Invoke(character.transform);
        }

        if (Actor?.Animator != null) //Actor에 Animator가 존재한다면 애니메이션 재생
        {
            Actor.Animator.SetBool(IS_CROUCHING, false);
            Actor.Animator.SetBool(IS_RUNNING, true);
        }

        //Actor의 X, Z 좌표를 담아주기 위한 변수
        Vector2Int actorPos = new Vector2Int((int)Actor.GameObject.transform.position.x,
            (int)Actor.GameObject.transform.position.z);
        List<Vector2Int> actorPosList = tileManager.GetReachableTiles(actorPos, Actor.Stat.MoveRange);

        foreach (Node node in path)
        {
            //타일 사이즈에 맞게 파인딩한 노드 위치로 이동하기 위한 좌표
            Vector3 targetPos = new Vector3(
                node.Position.x * tileManager.tileSize,
                0.5f,
                node.Position.z * tileManager.tileSize
            );

            //이동 시작
            await NodeMovement(targetPos);

            //끝난 노드
            endNode = node;

            if (IsAutoMove) //자동 이동 중일 때 적용
            {
                if (AttackRangeChecker(target)) break;
                if (MoveRangeChecker(actorPosList, targetPos) == false) break;
            }
        }

        if (Actor?.Animator != null)
        {
            Actor.Animator.SetBool(IS_RUNNING, false);
        }

        CrouchingRotate(endNode);

        UsingTileChecker(true, Actor);

        //harang 카메라 끝
        if (Actor is CharacterData _character) //명시적 형변환 -> Actor가 CharacterData일 경우
        {
            callback.onCompleteMove?.Invoke(_character.transform);
        }

        if (IsAutoMove && AttackRangeChecker(target)) //AI로 움직이는 중일 때, 공격 사거리에 든다면 공격
        {
            int skillNumber = Random.Range(0, Actor.HasSkills.Length);
            List<IDamageAble> targets = new List<IDamageAble> { nearestTarget };
            await Actor.Excute(skillNumber, targets, targets[0].GameObject.transform);
        }

        EndMovement();
    }

    /// <summary>
    /// 노드에 따른 순차적 이동
    /// </summary>
    /// <param name="targetPos"></param>
    private async Task NodeMovement(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - Actor.GameObject.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 eulerAngles = new Vector3(0f, lookRotation.eulerAngles.y, 0f);
            Actor.GameObject.transform.DORotate(eulerAngles, 0.1f).SetEase(Ease.Linear);
        }

        await Actor.GameObject.transform.DOMove(targetPos, 0.1f).SetEase(Ease.Linear).AsyncWaitForCompletion();
    }

    /// <summary>
    /// 이 타일 위에 캐릭터가 있다는 것을 체크해주는 함수
    /// </summary>
    /// <param name="usingTileCheck"></param>
    /// <param name="target"></param>
    private void UsingTileChecker(bool usingTileCheck, IDamageAble target)
    {
        Tile usingTile = tileManager.GetClosestTile(Actor.GameObject.transform.position);
        if (usingTile != null) //현재 Actor가 서있는 위치를 탐색하지 못하게 함
        {
            usingTile.isUsingTile = usingTileCheck;
            usingTile.SetOccupant(target);
        }
    }
    
    /// <summary>
    /// 이동 종료 시, 값 초기화 및 캐릭터 행동 가능여부 체크
    /// </summary>
    private void EndMovement()
    {
        Actor.Stat.IsCompleteAction = true;
        endNode = null;
        Actor = null;
        nearestTarget = null;
        IsMove = false;
    }

    /// <summary>
    /// 엄폐에 맞춰서 회전 방향을 결정
    /// </summary>
    /// <param name="endNode"></param>
    private void CrouchingRotate(Node endNode)
    {
        switch (endNode.Tile.obstacleDir)
        {
            case 1:
                Actor.GameObject.transform.DORotate(new Vector3(0f, 90f, 0f), 0.1f).SetEase(Ease.Linear);
                break;
            case 2:
                Actor.GameObject.transform.DORotate(new Vector3(0f, -90f, 0f), 0.1f).SetEase(Ease.Linear);
                break;
            case 3:
                Actor.GameObject.transform.DORotate(new Vector3(0f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
                break;
            case 4:
                Actor.GameObject.transform.DORotate(new Vector3(0f, 180f, 0f), 0.1f).SetEase(Ease.Linear);
                break;
        }

        if (endNode.Tile.obstacleDir == 0) return;
        Actor.Animator.SetBool("isCrouching", true);
    }

    /// <summary>
    /// Player나 Enemy 턴 때, 자신을 제외한 상대를 추적하기 위해 리스트를 담는 함수
    /// </summary>
    private void TargetActors()
    {
        switch (TurnManager.Instance.CurrentTurn)
        {
            case ActorParent.Player:
                foreach (IDamageAble actor in GameManager.Instance.EnemyUnits)
                {
                    if (actor != Actor && actor.Stat.IsDead == false)
                    {
                        var tilePos = new Vector2Int(
                            Mathf.RoundToInt(actor.GameObject.transform.position.x),
                            Mathf.RoundToInt(actor.GameObject.transform.position.z)
                        );
                        reservedTiles.Add(tilePos);
                    }
                }

                break;
            case ActorParent.Enemy:
                foreach (IDamageAble actor in GameManager.Instance.PlayerUnits)
                {
                    if (actor != Actor && actor.Stat.IsDead == false)
                    {
                        var tilePos = new Vector2Int(
                            Mathf.RoundToInt(actor.GameObject.transform.position.x),
                            Mathf.RoundToInt(actor.GameObject.transform.position.z)
                        );
                        reservedTiles.Add(tilePos);
                    }
                }

                break;
        }
    }

    /// <summary>
    /// 자신의 이동 범위 이상을 이동하지 못하게 체크하기 위한 함수
    /// </summary>
    /// <param name="actorPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private bool MoveRangeChecker(List<Vector2Int> actorPos, Vector3 targetPos)
    {
        return actorPos.Contains(new Vector2Int((int)targetPos.x, (int)targetPos.z));
    }

    /// <summary>
    /// 자신의 공격 사거리에 상대가 있는지 체크하기 위한 함수
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private bool AttackRangeChecker(Vector3 targetPos)
    {
        Vector2Int actorPos = new Vector2Int((int)Actor.GameObject.transform.position.x,
            (int)Actor.GameObject.transform.position.z);
        List<Vector2Int> actorPosList = tileManager.GetReachableTiles(actorPos, Actor.Stat.AttackRnage);

        return actorPosList.Contains(new Vector2Int((int)targetPos.x, (int)targetPos.z));
    }

    /// <summary>
    /// 대상을 탐색했을 때, 대상의 위치가 아닌 대상의 주위 랜덤한 위치로 지정하기 위한 함수
    /// </summary>
    /// <returns></returns>
    private Vector3Int FindAvailableAdjacentTileToNearestTargets()
    {
        // 대상들을 거리 기준으로 정렬
        var sortedTargets = Actors
            .Where(target => target != Actor)
            .OrderBy(target =>
                Vector3.Distance(Actor.GameObject.transform.position, target.GameObject.transform.position));

        foreach (var target in sortedTargets)
        {
            Vector3Int targetPos = new Vector3Int(
                Mathf.RoundToInt(target.GameObject.transform.position.x),
                0,
                Mathf.RoundToInt(target.GameObject.transform.position.z)
            );

            foreach (var dir in directions.OrderBy(_ => Random.value))
            {
                Vector3Int candidate = targetPos + dir;

                if (candidate.x < 0 || candidate.z < 0 ||
                    candidate.x >= tileManager.tiles.GetLength(0) ||
                    candidate.z >= tileManager.tiles.GetLength(1))
                    continue;

                Tile tile = tileManager.GetTileAt(candidate.x, candidate.z);
                if (tile == null || !tile.isWalkable) continue;

                Vector2Int tilePos = new Vector2Int(candidate.x, candidate.z);
                if (reservedTiles.Contains(tilePos) || tile.isUsingTile) continue;

                return candidate; // 가능한 위치 발견 시 즉시 반환
            }
        }

        return Vector3Int.zero; // 모든 대상 주위에 유효한 타일이 없을 경우
    }

    // private void UpdateRotation(Transform player, Vector2 inputAxis, float smoothTime)
    // {
    //     float targetAngle = Mathf.Atan2(inputAxis.x, inputAxis.y) * Mathf.Rad2Deg;
    //     float angle = Mathf.SmoothDampAngle(player.eulerAngles.y, targetAngle, ref turnCalmVelocity, smoothTime);
    //     player.rotation = Quaternion.Euler(0f, angle, 0f);
    // }
}