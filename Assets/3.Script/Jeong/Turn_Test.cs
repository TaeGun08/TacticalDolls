using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Turn_Test : MonoBehaviour
{
    public static Turn_Test Instance;

    private GridBehavior gridBehavior;

    public TurnManager turnManager;
    private ActorParent actorParent = ActorParent.None;

    private SamplePlayer selectedCharacter;
    private int currentTurn;
    private int onSelectSkill;
    private int aiSelectSkill;
    private bool isBlockedPlayerControl;

    public TaskCompletionSource<bool> MoveTcs;
    public TaskCompletionSource<bool> SkillTcs;

    public List<Actor_Test> Ally;
    public List<Actor_Test> Enemy;

    public List<Actor_Test> TurnActor;

    public bool IsAuto;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gridBehavior = GridBehavior.Instance;

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
        TurnActor = new List<Actor_Test>();

        if (actor.Equals(ActorParent.Player))
        {
            await AllyTest();
        }
        else if (actor.Equals(ActorParent.Enemy))
        {
            await EnemyTest();
        }

        turnManager.TurnEndedSource.TrySetResult(true);
    }

    private async Task AllyTest()
    {
        if (IsAuto)
        {
            gridBehavior.IsAutoMove = true;

            gridBehavior.Actors = Enemy;

            foreach (var ally in Ally)
            {
                if (AllyChecker(ally) || IsAuto == false) continue;
                
                MoveTcs = new TaskCompletionSource<bool>();
                SkillTcs = new TaskCompletionSource<bool>();
                gridBehavior.Actor = ally;
                await MoveTcs.Task;
                await SkillTcs.Task;
            }

            Debug.Log("Ally turn 종료");
            
            gridBehavior.IsAutoMove = false;
        }
        else
        {
            MoveTcs = new TaskCompletionSource<bool>();

            while (TurnActor.Count < Ally.Count || IsAuto)
            {
                await Task.Delay(10);
            }

            //await gridBehavior.Actor.Excute();
            MoveTcs.TrySetResult(true);
            await Task.Delay(1000);

            await MoveTcs.Task;
        }
    }

    private async Task EnemyTest()
    {
        gridBehavior.IsAutoMove = true;

        gridBehavior.Actors = Ally;

        foreach (var enemy in Enemy)
        {
            MoveTcs = new TaskCompletionSource<bool>();
            gridBehavior.Actor = enemy;
            await MoveTcs.Task;
        }

        gridBehavior.IsAutoMove = false;
    }

    private bool AllyChecker(Actor_Test actor)
    {
        foreach (var turnActor in TurnActor)
        {
            if (turnActor.Equals(actor)) return true;
        }

        return false;
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