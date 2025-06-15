using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridBehavior_Test : MonoBehaviour
{
    public static GridBehavior_Test Instance;

    private TileManager tileManager;
    private PathFindingManager pathFindingManager;
    private Turn_Test turn;

    private Camera mainCam;

    private Vector3Int startPos;
    private Vector3Int endPos;

    public Actor_Test Actor;
    [SerializeField] private LayerMask characterLayer;

    public bool IsMove { get; set; }
    public bool IsAutoMove { get; set; }
    public List<Actor_Test> Actors;
    private float turnCalmVelocity;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tileManager = TileManager.Instance;
        pathFindingManager = PathFindingManager.Instance;
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
        if (Input.GetMouseButtonDown(0)
            && IsMove == false)
        {
            Debug.Log("클릭");
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitCharacter,
                    100f, characterLayer))
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

                if (tile == null) return;
                if (tile.isWalkable == false) return;

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
                
                List<Node> path = PathFindingManager.Instance.PathFind(Actor.transform.position, new Vector3Int(tile.x, 0, tile.y));
                StartCoroutine(MovePlayerAlongPath(path));
            }
        }
    }

    private void AutoMove()
    {
        if (IsMove || IsAutoMove == false) return;
        bool firstActor = false;
        Vector3Int targetPos = Vector3Int.zero;
        Vector3 prevPos = Vector3.zero;

        foreach (var targetActor in Actors)
        {
            if (firstActor == false)
            {
                targetPos = new Vector3Int((int)targetActor.transform.position.x, 0,
                    (int)targetActor.transform.position.z);
                prevPos = targetActor.transform.position;
                firstActor = true;
                continue;
            }

            float distance = Vector3.Distance(Actor.transform.position, targetActor.transform.position);
            float prevDistance = Vector3.Distance(Actor.transform.position, prevPos);

            if (distance >= prevDistance) continue;
            targetPos = new Vector3Int((int)targetActor.transform.position.x, 0, (int)targetActor.transform.position.z);
            prevPos = targetActor.transform.position;
        }

        List<Node> path = PathFindingManager.Instance.PathFind(Actor.transform.position, targetPos);
        StartCoroutine(MovePlayerAlongPath(path));
        IsMove = true;
    }

    private IEnumerator MovePlayerAlongPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Debug.Log("움직입니다");
            Vector3 targetPos = new Vector3(
                node.Position.x * tileManager.tileSize,
                1.5f,
                node.Position.z * tileManager.tileSize
            );

            while (Vector3.Distance(Actor.transform.position, targetPos) > 0.05f)
            {
                Actor.transform.position = 
                    Vector3.MoveTowards(Actor.transform.position, targetPos, 10f * Time.deltaTime);
                
                var temp = new Vector2(targetPos.x - Actor.transform.position.x, targetPos.z - Actor.transform.position.z);
                UpdateRotation(Actor.transform, temp, 0.1f);
                yield return null;
            }
        }

        yield return null;
        turn.MoveTcs.TrySetResult(true);
        IsMove = false;
        MoveRangeSystem.Instance.ResetMovableTiles();
    }
    
    private void UpdateRotation(Transform player, Vector2 inputAxis, float smoothTime)
    {
        float targetAngle = Mathf.Atan2(inputAxis.x, inputAxis.y) * Mathf.Rad2Deg;
        float angle =
            Mathf.SmoothDampAngle(player.eulerAngles.y, targetAngle, ref turnCalmVelocity, smoothTime);
        player.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}