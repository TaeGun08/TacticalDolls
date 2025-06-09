using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum GameState
{
    Waiting,
    Playing,
    Ended
}

public enum TurnState
{
    None,
    Player,
    Enemy
}

public class PlayerTurnEventArgs : EventArgs
{
    public TurnState Turn { get; }
    public PlayerTurnEventArgs(TurnState turn) => Turn = turn;
}

public class GameStateEventArgs : EventArgs
{
    public GameState State { get; }
    public GameStateEventArgs(GameState state) => State = state;
}

public class TurnManager : MonoBehaviour
{
    private const int MAX_TURN_COUNT = 12;
    
    public static TurnManager Instance { get; private set; }
    
    public event EventHandler<TurnState> TurnChanged;
    public event EventHandler<GameStateEventArgs> GameStateChanged;
    
    public GameState State { get; private set; } = GameState.Waiting;
    public TurnState CurrentTurn { get; private set; } = TurnState.None;
    public int TurnCount { get; private set; } = 0;
    
    private TaskCompletionSource<bool> turnEndedSource;

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
        //if 이니셜라이즈 오브젝트가 완료면
        CurrentTurn = TurnState.Player;
        _ = RunGameFlow();
    }

    #region Server Logic

    private async Task RunGameFlow()
    {
        await Task.Delay(1000);
        
        while (State == GameState.Playing)
        {
            SetNextTurn();
            turnEndedSource = new TaskCompletionSource<bool>();
            await turnEndedSource.Task;
            
            SetNextTurn();
            turnEndedSource = new TaskCompletionSource<bool>();
            await turnEndedSource.Task;

            TurnCount++;

            // if (CheckWinCondition(out var winner))
            // {
            //     EndGame(winner);
            //     break;
            // }
        }
    }
    
    private void SetNextTurn() //턴 전환
    {
        CurrentTurn = (CurrentTurn == TurnState.Player) ? TurnState.Enemy : TurnState.Player;
        Debug.Log($"턴 전환됨: {CurrentTurn}");
    }
    
    // private bool CheckWinCondition(out YachtPlayer winner)
    // {
    //     if (TurnCount >= MAX_TURN_COUNT)
    //     {
    //         winner = DetermineWinnerByScore();
    //         return true;
    //     }
    //
    //     winner = null;
    //     return false;
    // }

    // private YachtPlayer DetermineWinnerByScore()
    // {
    //     return null;
    // }

    // private void EndGame(YachtPlayer winner)
    // {
    //     Debug.Log($"게임 종료. 승자: {winner}");
    //     State = GameState.Ended;
    //     GameStateChanged?.Invoke(this, new GameStateEventArgs(State));
    // }
    
    #endregion
    
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
    // public void RpcStartGame_S2C()
    // {
    //     Debug.Log("게임 시작");
    //     State = GameState.Playing;
    //     GameStateChanged?.Invoke(this, new GameStateEventArgs(State));
    // }
    //
    // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    // private void RpcTurnStart_S2C(PlayerTurn turn)
    // {
    //     CurrentTurn = turn;
    //     TurnChanged?.Invoke(this, CurrentTurn);
    // }
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

