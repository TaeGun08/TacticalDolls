using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

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

public class PathFindingManager : MonoBehaviour
{
    public static PathFindingManager Instance { get; private set; }
    
    private TileManager tileManager;
    private Turn_Test turn;
    
    private Node[,] nodeArray;
    public Node[,]  NodeArray => nodeArray;
    
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
        tileManager = TileManager.Instance;
        turn = Turn_Test.Instance;
        
        nodeArray = new Node[tileManager.tiles.GetLength(0), tileManager.tiles.GetLength(0)];

        for (int x = 0; x < nodeArray.GetLength(0); x++)
        {
            for (int z = 0; z < nodeArray.GetLength(1); z++)
            {
                nodeArray[x, z] = new Node(tileManager.tiles[x, z]);
            }
        }
    }

    public List<Node> PathFind(Vector3 start, Vector3 end)
    {
        foreach (var node in nodeArray)
        {
            node.G = int.MaxValue;
            node.H = 0;
            node.ParentNode = null;
        }

        Vector3Int startPos = RoundToTilePosition(start);
        Vector3Int endPos = RoundToTilePosition(end);
        
        Node startNode = nodeArray[startPos.x, startPos.z];
        startNode.Tile.isUsingTile = false;
        Node endNode = nodeArray[endPos.x, endPos.z];

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
                return RetracePath(startNode, endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighbor in GetNeighbours(currentNode))
            {
                if (closedList.Contains(neighbor)) continue;

                int tentativeG = currentNode.G + CalculateDistanceCost(currentNode, neighbor);
                
                //Vector2Int neighborPos = new Vector2Int(neighbor.Position.x, neighbor.Position.z);

                // bool canPass = neighbor.Tile.isWalkable && 
                //                (neighbor.Tile.isUsingTile == false || (reser))
                
                if (tentativeG < neighbor.G 
                    && neighbor.Tile.isWalkable
                    && neighbor.Tile.isUsingTile == false)
                {
                    neighbor.ParentNode = currentNode;
                    neighbor.G = tentativeG;
                    neighbor.H = CalculateDistanceCost(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }
    
    public Vector3Int RoundToTilePosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileManager.tileSize);
        int z = Mathf.RoundToInt(position.z / tileManager.tileSize);
        return new Vector3Int(x, 0, z);
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
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

        return path;
    }
    
    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();
        
        foreach (var dir in directions)
        {
            int nx = node.Position.x + dir.x;
            int nz = node.Position.z + dir.z;

            if (nx < 0 || nz < 0 || nx >= 51 || nz >= 51)
            {
                Debug.Log("탐색 타일 벗어남");
                continue;
            }

            Node neighbor = nodeArray[nx, nz];
            if (neighbor == null || !neighbor.Tile.isWalkable)
            {
                Debug.Log("이웃 노드 없거나 또는 밟을 수 없는 타일임");
                continue;
            }

            bool isDiagonal = Mathf.Abs(dir.x) == 1 && Mathf.Abs(dir.z) == 1;

            if (isDiagonal)
            {
                Node nodeA = nodeArray[node.Position.x + dir.x, node.Position.z];
                Node nodeB = nodeArray[node.Position.x, node.Position.z + dir.z];

                if (nodeA == null || nodeB == null || !nodeA.Tile.isWalkable || !nodeB.Tile.isWalkable)
                {
                    Debug.Log("이웃 노드가 없거나 또는 밟을 수 없는 타일임 ( A노드와 B 노드 관련)");
                    continue;
                }
            }

            if (IsTargetAtPosition(neighbor.Position))
                continue;

            neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private bool IsTargetAtPosition(Vector3Int pos)
    {
        if (TurnManager.Instance == null || turn == null)
            return false;
        switch (TurnManager.Instance.CurrentTurn)
        {
            // case ActorParent.Player:
            //     foreach (EnemyData enemy in GameManager.Instance.EnemyUnits)
            //     {
            //         if (RoundToTilePosition(enemy.transform.position) == pos)
            //             return true;
            //     }
            //     break;

            case ActorParent.Enemy:
                foreach (CharacterData player in GameManager.Instance.PlayerUnits)
                {
                    if (RoundToTilePosition(player.transform.position) == pos)
                        return true;
                }
                break;
        }

        return false;
    }
    
    private int CalculateDistanceCost(Node a, Node b)
    {
        int dx = Mathf.Abs(a.Position.x - b.Position.x);
        int dz = Mathf.Abs(a.Position.z - b.Position.z);

        int max = Mathf.Max(dx, dz);
        return 10 * max;
    }
}