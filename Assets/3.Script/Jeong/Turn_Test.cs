using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Turn_Test : MonoBehaviour
{
    public static Turn_Test Instance;

    private GridBehavior_Test gridBehavior;

    public TurnManager turnManager;
    private ActorParent actorParent = ActorParent.None;

    private SamplePlayer selectedCharacter;
    private int currentTurn;
    private int onSelectSkill;
    private int aiSelectSkill;
    private bool isBlockedPlayerControl;

    public TaskCompletionSource<bool> moveTcs;
    
    public List<Actor_Test> Ally;
    public List<Actor_Test> Enemy;

    public List<Actor_Test> TurnActor;

    public bool IsAuto;

    public Button startButton; 

    private void Awake()
    {
        Instance = this;
        
        // startButton.onClick.AddListener(() =>
        // {
        //     turnManager.gameObject.SetActive(true);
        // });
    }

    private void Start()
    {
        gridBehavior = GridBehavior_Test.Instance;
        turnManager = TurnManager.Instance;

        turnManager.ActorChanged += OnTurnChangedWrapper;
        turnManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (turnManager == null) return;

        turnManager.ActorChanged -= OnTurnChangedWrapper;
        turnManager.GameStateChanged -= OnGameStateChanged;
    }

    #region TurnLogic

    private void OnTurnChangedWrapper(object sender, ActorParent actor)
    {
        _ = OnTurnChanged(sender, actor);
    }

    private async Task OnTurnChanged(object sender, ActorParent actor)
    {
        currentTurn = turnManager.TurnCount + 1;
        actorParent = actor;
        Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");
        moveTcs = new TaskCompletionSource<bool>();
        
        if (actor.Equals(ActorParent.Player))
        {
            AllyTest();
            await moveTcs.Task;
        }
        else if (actor == ActorParent.Enemy)
        {
            EnemyTest();
            await moveTcs.Task;
        }
        
        turnManager.TurnEndedSource.TrySetResult(true);
    }

    private async Task AllyTest()
    {
        await Task.Delay(1000);
        moveTcs.TrySetResult(true);
    }

    private async Task EnemyTest()
    {
        // foreach (var enemy in Enemy)
        // {
        //     if (gridBehavior.Actor) continue;
        //     enemy.MyTurn();
        // }
        
        await Task.Delay(1000);
        moveTcs.TrySetResult(true);
    }

    #endregion

    private void OnGameStateChanged(object sender, GameStateEventArgs e)
    {
        if (e.State == GameState.Ended)
        {
            Debug.Log("게임이 종료되었습니다.");
        }
    }
}