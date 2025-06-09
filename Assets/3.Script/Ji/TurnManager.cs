using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public event EventHandler<ActorParent> ActorChanged;
    public event EventHandler<GameStateEventArgs> GameStateChanged;

    public GameState State { get; private set; } = GameState.Waiting;
    public ActorParent CurrentTurn { get; private set; } = ActorParent.None;
    public int TurnCount { get; private set; } = 0;

    public TaskCompletionSource<bool> TurnEndedSource;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //비동기로 게임 초기화를 기다립니다.
        InGameInitialize().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled) return;
            //--초기화 완료 시점--
            _= RunGameFlow();
        });
    }
    
    private async Task RunGameFlow()
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
        //처음 실행되면 ActorParent.None 이므로 플레이어부터 시작
        CurrentTurn = (CurrentTurn == ActorParent.Player) ? ActorParent.Enemy : ActorParent.Player;
        Debug.Log($"턴 전환됨: {CurrentTurn}");
        TurnStart(CurrentTurn);
    }

    private bool CheckWinCondition() //승자가 나올 겨우 true, 아니라면 false
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
    
    // #region Server To Client | (Client Logic)
    //
    // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    // public void RpcSetScoreBoard_S2C()
    // {
    //     ScoreManager.Instance.HostBoard = new ScoreManager.ScoreBoard(YachtPlayer.HostPlayer.PlayerRef);
    //     ScoreManager.Instance.ClientBoard = new ScoreManager.ScoreBoard(YachtPlayer.ClientPlayer.PlayerRef);
    // }
    //
    // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]

    //
    // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]

    //
    // #endregion

    // #region Client To Server | (Server Logic)
    //
    // [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    // public void RpcTurnEnd_C2S(PlayerRef playerRef)
    // {
    //     turnEndedSource?.TrySetResult(true);
    // }
    //
    // #endregion
}

