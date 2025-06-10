using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Test : MonoBehaviour
{
    public static Turn_Test Instance;
    
    private GridBehavior_Test gridBehavior;

    private bool nextTurn;
    
    public enum NextTurnState
    {
        Ally,
        Enemy
    }
    
    [Header("State RunTimeCheck")]
    [SerializeField] private NextTurnState nextState;
    
    [SerializeField] private Actor_Test[] ally;
    [SerializeField] private Actor_Test[] enemy;
    
    public Queue<Actor_Test> ActorQueue = new Queue<Actor_Test>();
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        gridBehavior = GridBehavior_Test.Instance;
    }

    private void Update()
    {
        if (ActorQueue.Count <= 0)
        {
            switch (nextState)
            {
                case NextTurnState.Ally:
                    for (int i = 0; i < ally.Length; i++)
                    {
                        ActorQueue.Enqueue(ally[i]);
                    }
                    break;
                case NextTurnState.Enemy:
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        ActorQueue.Enqueue(enemy[i]);
                    }
                    break;
            }
            
            StartCoroutine(TurnCoroutine());
        }
    }

    private IEnumerator TurnCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while (ActorQueue.Count > 0)
        {
            if (gridBehavior.IsMove == false || gridBehavior.Actor) continue;
            
            ActorQueue.Dequeue().MyTurn(ally);
            
            yield return wait;
        }
        
        nextState = nextState == NextTurnState.Ally ? NextTurnState.Enemy : NextTurnState.Ally;
    }
}
