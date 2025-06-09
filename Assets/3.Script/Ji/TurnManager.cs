// using System;
// using System.Threading.Tasks;
// using UnityEngine;
//
// namespace _3.Script.Ji
// {
//     public class TurnManager : MonoBehaviour
//     {
//         public int MAX_TURN_COUNT { private get; set; } = 5;
//     
//         public static TurnManager Instance { get; private set; }
//     
//         public event EventHandler<ActorParent> ActorChanged;
//         public event EventHandler<GameStateEventArgs> GameStateChanged;
//     
//         public GameState State { get; private set; } = GameState.Waiting;
//         public ActorParent CurrentTurn { get; private set; } = ActorParent.None;
//         public int TurnCount { get; private set; } = 0;
//
//         public TaskCompletionSource<bool> TurnEndedSource;
//
//         private void Awake()
//         {
//             if (Instance != null && Instance != this)
//             {
//                 Destroy(gameObject);
//                 return;
//             }
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//
//         private void Start()
//         {
//             //if 이니셜라이즈 오브젝트가 완료면
//             CurrentTurn = ActorParent.Player;
//             _ = RunGameFlow();
//         }
//
//         #region Server Logic
//
//         private async Task RunGameFlow()
//         {
//             await Task.Delay(1000);
//         
//             while (State == GameState.Playing)
//             {
//                 SetNextTurn();
//                 TurnEndedSource = new TaskCompletionSource<bool>();
//                 await TurnEndedSource.Task;
//             
//                 SetNextTurn();
//                 TurnEndedSource = new TaskCompletionSource<bool>();
//                 await TurnEndedSource.Task;
//
//                 TurnCount++;
//
//                 if (CheckWinCondition(out var winner))
//                 {
//                     EndGame(winner);
//                     break;
//                 }
//             }
//         }
//     
//         private void SetNextTurn() //턴 전환
//         {
//             CurrentTurn = (CurrentTurn == ActorParent.Player) ? ActorParent.Enemy : ActorParent.Player;
//             Debug.Log($"턴 전환됨: {CurrentTurn}");
//         }
//     
//         private bool CheckWinCondition(out ActorParent winner)
//         {
//             if (TurnCount >= MAX_TURN_COUNT)
//             {
//                 winner = DetermineWinnerByScore();
//                 return true;
//             }
//     
//             winner = ActorParent.None;
//             return false;
//         }
//
//         private ActorParent DetermineWinnerByScore()
//         {
//             return ActorParent.None;
//         }
//
//         private void EndGame(ActorParent winner)
//         {
//             Debug.Log($"게임 종료. 승자: {winner}");
//             State = GameState.Ended;
//             GameStateChanged?.Invoke(this, new GameStateEventArgs(State));
//         }
//     
//         private void RpcTurnStart_S2C(ActorParent actor)
//         {
//             CurrentTurn = actor;
//             ActorChanged?.Invoke(this, CurrentTurn);
//         }
//         #endregion
//     
//         // #region Server To Client | (Client Logic)
//         //
//         // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
//         // public void RpcSetScoreBoard_S2C()
//         // {
//         //     ScoreManager.Instance.HostBoard = new ScoreManager.ScoreBoard(YachtPlayer.HostPlayer.PlayerRef);
//         //     ScoreManager.Instance.ClientBoard = new ScoreManager.ScoreBoard(YachtPlayer.ClientPlayer.PlayerRef);
//         // }
//         //
//         // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
//         // public void RpcStartGame_S2C()
//         // {
//         //     Debug.Log("게임 시작");
//         //     State = GameState.Playing;
//         //     GameStateChanged?.Invoke(this, new GameStateEventArgs(State));
//         // }
//         //
//         // [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
//
//         //
//         // #endregion
//     
//         // #region Client To Server | (Server Logic)
//         //
//         // [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
//         // public void RpcTurnEnd_C2S(PlayerRef playerRef)
//         // {
//         //     turnEndedSource?.TrySetResult(true);
//         // }
//         //
//         // #endregion
//     }
// }
//
