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

    [Header("Pathfinding")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3Int startPos;
    [SerializeField] private Vector3Int endPos;
    [SerializeField] public int rows = 5;
    [SerializeField] public int columns = 5;
    public Color lineColor = Color.white;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject tilePrefab;

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
        nodeArray = new Node[columns, depth, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < depth; y++)
            {
                for (int z = 0; z < rows; z++)
                {
                    Vector3 worldPos = new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, z * cellSize + cellSize * 0.5f);
                    Instantiate(tilePrefab, worldPos, Quaternion.identity);
                    nodeArray[x, y, z] = new Node(new Vector3Int(x, y, z));
                }
            }
        }
    }

    private void Start()
    {
        PathFind(startPos, endPos);
    }

    #region 임시 타일 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        for (int x = 0; x <= columns; x++)
        {
            Vector3 start = transform.position + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, rows * cellSize);
            Gizmos.DrawLine(start, end);
        }

        for (int z = 0; z <= rows; z++)
        {
            Vector3 start = transform.position + new Vector3(0, 0, z * cellSize);
            Vector3 end = start + new Vector3(columns * cellSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }
    #endregion

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
            // 최소 F값을 갖는 노드 찾기 - 개선 가능: 우선순위 큐 사용 추천
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
                node.Position.x * cellSize + cellSize * 0.5f,
                node.Position.y,
                node.Position.z * cellSize + cellSize * 0.5f
            );

            while (Vector3.Distance(player.position, targetPos) > 0.05f)
            {
                player.position = Vector3.MoveTowards(player.position, targetPos, 3f * Time.deltaTime);
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
                nx < columns && ny < depth && nz < rows)
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
        return 10 * max; // 단순화: 대각선 포함 최소 비용 계산
    }
}