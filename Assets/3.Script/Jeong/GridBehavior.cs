using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    public class Node
    {
        public Vector3Int Position { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;

        public Node ParentNode { get; set; }

        public Node(Vector3Int position)
        {
            Position = position;
            G = int.MaxValue;
        }
    }

    [SerializeField] private TileManager tileManager;

    [Header("Pathfinding")]
    [SerializeField] private Vector3Int startPos;
    [SerializeField] private Vector3Int endPos;

    [SerializeField] private Transform player;

    private Node[,,] nodeArray;

    [SerializeField] private int depth = 5;

    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
        new Vector3Int(0,1,0), new Vector3Int(0,-1,0),
        new Vector3Int(0,0,1), new Vector3Int(0,0,-1),

        new Vector3Int(1,1,0), new Vector3Int(1,-1,0), new Vector3Int(-1,1,0), new Vector3Int(-1,-1,0),
        new Vector3Int(1,0,1), new Vector3Int(1,0,-1), new Vector3Int(-1,0,1), new Vector3Int(-1,0,-1),
        new Vector3Int(0,1,1), new Vector3Int(0,1,-1), new Vector3Int(0,-1,1), new Vector3Int(0,-1,-1),

        new Vector3Int(1,1,1), new Vector3Int(1,1,-1), new Vector3Int(1,-1,1), new Vector3Int(1,-1,-1),
        new Vector3Int(-1,1,1), new Vector3Int(-1,1,-1), new Vector3Int(-1,-1,1), new Vector3Int(-1,-1,-1),
    };
    
    private void Awake()
    {
        nodeArray = new Node[51, depth, 51];

        for (int x = 0; x < 51; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                for (int z = 0; z < 51; z++)
                {
                    nodeArray[x, y, z] = new Node(new Vector3Int(x, y, z));
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            PathFind(new Vector3Int(0, 0, 0),new Vector3Int(4, 0, 4));
            // if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            // {
            //     Tile tile =  hit.collider.GetComponent<Tile>();
            //     if (tile == null) return;
            //     PathFind(new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, 
            //             (int)player.transform.position.z),
            //         new Vector3Int(tile.x, 0, tile.y));
            // }
        }
    }

    public void PathFind(Vector3Int start, Vector3Int end)
    {
        foreach (var node in nodeArray)
        {
            node.G = int.MaxValue;
            node.H = 0;
            node.ParentNode = null;
        }

        Node startNode = nodeArray[start.x, start.y, start.z];
        Node endNode = nodeArray[end.x, end.y, end.z];

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

                if (tentativeG < neighbor.G)
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
                node.Position.y * tileManager.tileSize,
                node.Position.z * tileManager.tileSize
            );

            while (Vector3.Distance(player.position, targetPos) > 0.05f)
            {
                player.position = Vector3.MoveTowards(player.position, targetPos, 10f * Time.deltaTime);
                yield return null;
            }
        }
    }
    
    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();

        foreach (var dir in directions)
        {
            int nx = node.Position.x + dir.x;
            int ny = node.Position.y + dir.y;
            int nz = node.Position.z + dir.z;

            if (nx >= 0 && ny >= 0 && nz >= 0 &&
                nx < 51 && ny < depth && nz < 51)
            {
                neighbors.Add(nodeArray[nx, ny, nz]);
            }
        }

        return neighbors;
    }

    private int CalculateDistanceCost(Node a, Node b)
    {
        int dx = Mathf.Abs(a.Position.x - b.Position.x);
        int dy = Mathf.Abs(a.Position.y - b.Position.y);
        int dz = Mathf.Abs(a.Position.z - b.Position.z);

        int max = Mathf.Max(dx, Mathf.Max(dy, dz));
        return 10 * max;
    }
}