using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public partial class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // 턴 조작권 -> player/ enemy
    public event EventHandler<ActorParent> ActorChanged;
    
    // 턴 진행 상황
    
    public event EventHandler<GameStateEventArgs> GameStateChanged;

    // 초기 턴 상태 waitting 으로 시작
    public GameState State { get; private set; } = GameState.Waiting;
    
    // 초기 턴 시작 none
    public ActorParent CurrentTurn { get; private set; }
    
    // 턴 수량
    public int TurnCount { get; private set; } = 0;

    // 턴 종료 여부 -> callback
    public TaskCompletionSource<bool> TurnEndedSource;
    
    public Button startButton; 
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // startButton.onClick.AddListener(() =>
        // {
        //     //비동기로 게임 초기화를 기다립니다.
        //     InGameInitialize().ContinueWithOnMainThread(task =>
        //     {
        //         if (task.IsFaulted || task.IsCanceled) return;
        //         
        //         //--초기화 완료 시점--
        //         _= RunGameFlow();
        //     });
        // });
    }
    
    public async Task RunGameFlow()
    {
        await Task.Delay(1000);
    
        StartGame();
        
        while (State == GameState.Playing)
        {
            SetNextTurn();
            TurnEndedSource = new TaskCompletionSource<bool>();
            await TurnEndedSource.Task;
        
            if (CheckWinCondition())
            {
                // 게임 종료 조건 체크
                break;
            }
            
            SetNextTurn();
            TurnEndedSource = new TaskCompletionSource<bool>();
            await TurnEndedSource.Task;

            TurnCount++;

            if (CheckWinCondition())
            {
                break;
            }
        }
    }

    private void SetNextTurn() //턴 전환
    {
        //처음 실행되면 ActorParent.None 이므로 플레이어부터 시작되는 조건문
        CurrentTurn = (CurrentTurn == ActorParent.Player) ? ActorParent.Enemy : ActorParent.Player;
        Debug.Log($"턴 전환됨: {CurrentTurn}");
        TurnStart(CurrentTurn);
    }

    private void StartGame()
    {
        Debug.Log("게임 시작");
        State = GameState.Playing;
        GameStateChanged?.Invoke(this, new GameStateEventArgs(State));
    }
    
    private void TurnStart(ActorParent actor)
    {
        CurrentTurn = actor;
        ActorChanged?.Invoke(this, CurrentTurn);
    }
    
    private void EndGame(ActorParent winner)
    {
        Debug.Log($"게임 종료. 승자: {winner}");
        State = GameState.Ended;
        GameStateChanged?.Invoke(this, new GameStateEventArgs(State));
    }

    private bool CheckWinCondition() //승자가 나올 겨우 true, 아니라면 false 반환
    {
        return false; //please fix
        
        ActorParent winner;
        
        // 추가할 것 - 양쪽에 //&& 맵 승리조건이 있고, 그게 달성되었으면 && mapWinLogic?.Invoke ?
        
        if (playerUnits.All(unit => unit.isDead)) //playerAllDead
        {
            winner = ActorParent.Enemy;
            EndGame(winner);
            return true;
        }
        
        if (monsterUnits.All(unit => unit.isDead)) //enemyAllDead
        {
            winner = ActorParent.Player;
            EndGame(winner);
            return true;
        }
        
        if (TurnCount >= maxTurnCount) // 턴이 최대 턴 수를 지나 패배 처리
        {
            winner = ActorParent.Enemy;
            EndGame(winner);
            return true;
        }
        
        winner = ActorParent.None;
        return false;
    }
}

