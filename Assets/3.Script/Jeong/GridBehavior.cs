using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridBehavior : MonoBehaviour
{
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
    
    private Vector3Int startPos;
    private Vector3Int endPos;
    
    [SerializeField] private Transform currentPlayer;
    [SerializeField] private LayerMask characterLayer;

    private Animator playerAnimater;

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

    private bool isMove;

    private float turnCalmVelocity;
    
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
        if (Input.GetMouseButtonDown(0) && isMove == false)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitCharacter, 100f, characterLayer))
            {
                currentPlayer = hitCharacter.transform;
            }
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                if (currentPlayer == null) return;
                
                Tile tile =  hit.collider.GetComponent<Tile>();
                
                if (tile == null) return;
                if (tile.isWalkable == false) return;
                
                if (!MoveRangeSystem.Instance.IsTileInMoveRange(tile))
                {
                    Debug.Log("이동 불가능한 범위입니다.");
                    MoveRangeSystem.Instance.ResetAllHighlights();
                    MoveRangeSystem.Instance.ResetMovableTiles();
                    return;
                }
                
                isMove = true;
                MoveRangeSystem.Instance.ResetAllHighlights();

                playerAnimater = currentPlayer.GetComponent<Animator>();

                if (playerAnimater != null)
                {
                    playerAnimater.SetBool("isRunning", true);
                }
                
                PathFind(RoundToTilePosition(currentPlayer.position), new Vector3Int(tile.x, 0, tile.y));
            }
        }
    }
    
    private Vector3Int RoundToTilePosition(Vector3 position)
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
        
        if(current == startNode)
            path.Add(startNode);

        path.Reverse();

        StartCoroutine(MovePlayerAlongPath(path));
    }
    
    private IEnumerator MovePlayerAlongPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Vector3 targetPos = new Vector3(
                node.Position.x * tileManager.tileSize,
                0.5f,
                node.Position.z * tileManager.tileSize
            );

            while (Vector3.Distance(currentPlayer.position, targetPos) > 0.05f)
            {
                currentPlayer.position = Vector3.MoveTowards(currentPlayer.position, targetPos, 10f * Time.deltaTime);
                
                var temp = new Vector2(targetPos.x - currentPlayer.position.x, targetPos.z - currentPlayer.position.z);
                UpdateRotation(currentPlayer, temp, 0.1f);
                yield return null;
            }
        }
        
        if (playerAnimater != null)
        {
            playerAnimater.SetBool("isRunning", false);
        }
        
        isMove = false;
        MoveRangeSystem.Instance.ResetMovableTiles();
    }
    
    private void UpdateRotation(Transform player, Vector2 inputAxis, float smoothTime)
    {
        float targetAngle = Mathf.Atan2(inputAxis.x, inputAxis.y) * Mathf.Rad2Deg;
        float angle =
            Mathf.SmoothDampAngle(player.eulerAngles.y, targetAngle, ref turnCalmVelocity, smoothTime);
        player.rotation = Quaternion.Euler(0f, angle, 0f);
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