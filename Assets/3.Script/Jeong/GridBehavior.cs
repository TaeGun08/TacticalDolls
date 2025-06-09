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
        }
    }
    
    private static readonly Vector3Int[] DIRECTIONS = {
        new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1),
        new Vector3Int(-1, 0, -1), new Vector3Int(-1, 0, 1),
        new Vector3Int(1, 0, -1), new Vector3Int(1, 0, 1)
    };

    [Header("Pathfinding")] 
    [SerializeField] private float cellSize = 1f;

    [SerializeField] private Vector3Int startPos;
    [SerializeField] private Vector3Int endPos;

    [SerializeField] public int rows = 5;
    [SerializeField] public int columns = 5;
    public Color lineColor = Color.white;

    [SerializeField] private Transform player;

    [SerializeField] private GameObject tilePrefab;
    
    private Node[,] nodeArray;
    
    private void Awake()
    {
        nodeArray = new Node[columns, rows];
        
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject obj = Instantiate(tilePrefab,  
                    new Vector3(i * cellSize + cellSize * 0.5f, 0, j * cellSize + cellSize * 0.5f), Quaternion.identity);
                nodeArray[i, j] = new Node(new Vector3Int((int)obj.transform.position.x, 
                    (int)obj.transform.position.y, (int)obj.transform.position.z));
            }
        }
    }

    private void Start()
    {
        PathFind(startPos, endPos);
    }

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
    
    public void PathFind(Vector3Int start, Vector3Int end)
    {
        Node startNode = new Node(start);
        Node endNode = new Node(end);
        
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            foreach (Node open in openList)
            {
                if (open.F < currentNode.F || 
                    open.F == currentNode.F && 
                    open.H < currentNode.H)
                {
                    currentNode = open;
                }
            }
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            //player.transform.position = closedList.Position;
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> pathNode = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            pathNode.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }
        
        pathNode.Reverse();
    }
}