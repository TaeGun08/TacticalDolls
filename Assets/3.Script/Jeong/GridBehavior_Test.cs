using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GridBehavior_Test : MonoBehaviour
{
    public static GridBehavior_Test Instance;

    public class Node
    {
        public Vector3Int Position { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;

        public Node ParentNode { get; set; }

        public Tile Tile { get; set; }

        public Node(Tile tile)
        {
            Position = new Vector3Int(tile.x, 0, tile.y);
            Tile = tile;
            G = int.MaxValue;
        }
    }

    [SerializeField] private TileManager tileManager;
    [SerializeField] private Turn_Test turn;

    private Vector3Int startPos;
    private Vector3Int endPos;

    public Actor_Test Actor;
    [SerializeField] private LayerMask characterLayer;

    //private Node[,,] nodeArray;
    private Node[,] nodeArray;

    //[SerializeField] private int depth = 5;

    // private readonly Vector3Int[] directions = new Vector3Int[]
    // {
    //     new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
    //     new Vector3Int(0,1,0), new Vector3Int(0,-1,0),
    //     new Vector3Int(0,0,1), new Vector3Int(0,0,-1),
    //
    //     new Vector3Int(1,1,0), new Vector3Int(1,-1,0), new Vector3Int(-1,1,0), new Vector3Int(-1,-1,0),
    //     new Vector3Int(1,0,1), new Vector3Int(1,0,-1), new Vector3Int(-1,0,1), new Vector3Int(-1,0,-1),
    //     new Vector3Int(0,1,1), new Vector3Int(0,1,-1), new Vector3Int(0,-1,1), new Vector3Int(0,-1,-1),
    //
    //     new Vector3Int(1,1,1), new Vector3Int(1,1,-1), new Vector3Int(1,-1,1), new Vector3Int(1,-1,-1),
    //     new Vector3Int(-1,1,1), new Vector3Int(-1,1,-1), new Vector3Int(-1,-1,1), new Vector3Int(-1,-1,-1),
    // };

    public bool IsMove { get; set; }
    public bool IsAutoMove { get; set; }
    public List<Actor_Test> Actors;

    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),

        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(-1, 0, -1),
    };

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        nodeArray = new Node[51, 51];

        for (int x = 0; x < 51; x++)
        {
            for (int z = 0; z < 51; z++)
            {
                nodeArray[x, z] = new Node(tileManager.tiles[x, z]);
            }
        }
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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitCharacter,
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
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
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
                PathFind(RoundToTilePosition(Actor.transform.position), new Vector3Int(tile.x, 0, tile.y));
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

        PathFind(RoundToTilePosition(Actor.transform.position), RoundToTilePosition(targetPos));
        IsMove = true;
    }

    public Vector3Int RoundToTilePosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileManager.tileSize);
        int z = Mathf.RoundToInt(position.z / tileManager.tileSize);
        return new Vector3Int(x, 0, z);
    }

    public void PathFind(Vector3Int start, Vector3Int end)
    {
        foreach (var node in nodeArray)
        {
            node.G = int.MaxValue;
            node.H = 0;
            node.ParentNode = null;
        }

        Node startNode = nodeArray[start.x, start.z];
        Node endNode = nodeArray[end.x, end.z];

        startNode.G = 0;
        startNode.H = CalculateDistanceCost(startNode, endNode);

        List<Node> openList = new List<Node> { startNode };
        HashSet<Node> closedList = new HashSet<Node>();

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                var node = openList[i];
                if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                    currentNode = node;
            }

            if (currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighbor in GetNeighbours(currentNode))
            {
                if (closedList.Contains(neighbor)) continue;

                int tentativeG = currentNode.G + CalculateDistanceCost(currentNode, neighbor);

                if (tentativeG < neighbor.G && neighbor.Tile.isWalkable)
                {
                    neighbor.ParentNode = currentNode;
                    neighbor.G = tentativeG;
                    neighbor.H = CalculateDistanceCost(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != null && current != startNode)
        {
            path.Add(current);
            current = current.ParentNode;
        }

        if (current == startNode)
            path.Add(startNode);

        path.Reverse();

        //movePlayerAlongPathTask(path);
        StartCoroutine(MovePlayerAlongPath(path));
    }

    // private async Task movePlayerAlongPathTask(List<Node> path)
    // {
    //     moveTcs = new TaskCompletionSource<bool>();
    //     
    //     foreach (Node node in path)
    //     {
    //         Vector3 targetPos = new Vector3(
    //             node.Position.x * tileManager.tileSize,
    //             1.5f,
    //             node.Position.z * tileManager.tileSize
    //         );
    //         
    //         Actor.transform.DOMove(targetPos, 0.3f)
    //             .SetEase(Ease.Linear)
    //             .OnComplete(() =>
    //         {
    //             moveTcs.TrySetResult(true);
    //         });
    //         
    //         await moveTcs.Task;
    //     }
    //     
    //
    //     turn.TurnActor.Remove(Actor);
    //     Actor = null;
    //     IsMove = false;
    //     MoveRangeSystem.Instance.ResetMovableTiles();
    // }

    private IEnumerator MovePlayerAlongPath(List<Node> path)
    {
        int range = 0;
        bool attackIn = false;
        
        foreach (var actor in Actors)
        {
            if (AttackChecker(actor))
            {
                attackIn = true;
                break;
            }
        }

        foreach (Node node in path)
        {
            if (IsAutoMove && (range > Actor.MoveRange) || attackIn)
            {
                break;
            }

            Vector3 targetPos = new Vector3(
                node.Position.x * tileManager.tileSize,
                1.5f,
                node.Position.z * tileManager.tileSize
            );

            while (Vector3.Distance(Actor.transform.position, targetPos) > 0.05f)
            {
                Actor.transform.position =
                    Vector3.MoveTowards(Actor.transform.position, targetPos, 10f * Time.deltaTime);
                yield return null;
            }

            range++;
        }

        yield return null;
        turn.MoveTcs.TrySetResult(true);
        IsMove = false;
        MoveRangeSystem.Instance.ResetMovableTiles();
    }

    private List<Vector2Int> GetReachableTiles(Vector2Int origin, int range)
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

    private List<Vector2Int> GetAttackableTilesFromReachable(Vector2Int origin, int moveRange, int attackRange)
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

    private bool AttackChecker(Actor_Test actor)
    {
        Vector2Int origin = new Vector2Int((int)Actor.transform.position.x, (int)Actor.transform.position.z);
        foreach (var attack in GetAttackableTilesFromReachable(origin, Actor.MoveRange, Actor.AttackRange))
        {
            if (attack.Equals(new Vector2Int((int)actor.transform.position.x,
                    (int)actor.transform.position.z))) return true;
        }

        return false;
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();

        foreach (var dir in directions)
        {
            int nx = node.Position.x + dir.x;
            int nz = node.Position.z + dir.z;

            if (nx < 0 || nz < 0 || nx >= 51 || nz >= 51)
                continue;

            if (Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.z) == 1)
            {
                Node nodeA = nodeArray[node.Position.x + dir.x, node.Position.z];
                Node nodeB = nodeArray[node.Position.x, node.Position.z + dir.z];

                if (!nodeA.Tile.isWalkable || !nodeB.Tile.isWalkable)
                    continue;
            }

            neighbors.Add(nodeArray[nx, nz]);
        }

        return neighbors;
    }

    private int CalculateDistanceCost(Node a, Node b)
    {
        int dx = Mathf.Abs(a.Position.x - b.Position.x);
        int dz = Mathf.Abs(a.Position.z - b.Position.z);

        int max = Mathf.Max(dx, dz);
        return 10 * max;
    }
}