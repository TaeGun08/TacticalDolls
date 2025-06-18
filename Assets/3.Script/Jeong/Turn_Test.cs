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
    
    // 행동 확인
    public TaskCompletionSource<bool> MoveTcs;
    
    public TaskCompletionSource<bool> SkillTcs;

    // public List<Actor_Test> Ally;
    // public List<Actor_Test> Enemy;
    
    public List<CharacterData> TurnActor;

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
     
        //MoveTcs.TrySetResult(true);
    }

    private async Task OnTurnChanged(object sender, ActorParent actor)
    {
        currentTurn = turnManager.TurnCount + 1;
        actorParent = actor;
        Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");
        
        
        if (actor.Equals(ActorParent.Player))
        {
            GameManager.Instance.InitCharacterTurnSetting(GameManager.Instance.PlayerUnits[0]);
            
            TurnActor = GameManager.Instance.PlayerUnits;
            
            // 모든 캐릭터 행동 종료 체크
            //await OnCheckEndCharacterActor();
        }
        else if (actor.Equals(ActorParent.Enemy))
        {
            await EnemyTest();
        }

        turnManager.TurnEndedSource.TrySetResult(true);
    }

    private async Task OnCheckEndCharacterActor()
    {
        if (IsAuto)
        {
            // gridBehavior.IsAutoMove = true;
            //
            // gridBehavior.Actors = Enemy;
            //
            // foreach (var ally in Ally)
            // {
            //     if (AllyChecker(ally) || IsAuto == false) continue;
            //     MoveTcs = new TaskCompletionSource<bool>();
            //     SkillTcs = new TaskCompletionSource<bool>();
            //     gridBehavior.Actor = ally;
            //     await MoveTcs.Task;
            //     await SkillTcs.Task;
            // }
            //
            // Debug.Log("Ally turn 종료");
            //
            // gridBehavior.IsAutoMove = false;
        }
        else
        {
            // MoveTcs = new TaskCompletionSource<bool>();
            //
            // while (TurnActor.Count > 0)
            // {
            //     await Task.Delay(10);
            // }
            //
            // MoveTcs.TrySetResult(true);
            // await Task.Delay(1000);
            // await MoveTcs.Task;
            //
            // Debug.Log("플레이어 행동 종료");
        }
    }

    private async Task EnemyTest()
    {
        MoveTcs = new TaskCompletionSource<bool>();
        await MoveTcs.Task;
        
        foreach (var player in GameManager.Instance.PlayerUnits)
        {
            gridBehavior.Actors.Add(player);
        }
        
        gridBehavior.IsAutoMove = true;
        
        foreach (var enemy in GameManager.Instance.EnemyUnits)
        {
            MoveTcs = new TaskCompletionSource<bool>();
            gridBehavior.Actor = enemy;
            await MoveTcs.Task;
        }
        
        gridBehavior.IsAutoMove = false;
        
        // MoveTcs = new TaskCompletionSource<bool>();
        // await MoveTcs.Task;
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