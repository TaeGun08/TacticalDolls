using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class TurnManager_Test : MonoBehaviour
{
    public static TurnManager_Test Instance { get; private set; }

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
            _ = RunGameFlow();
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
        //처음 실행되면 ActorParent.None 이므로 플레이어부터 시작되는 조건문
        CurrentTurn = (CurrentTurn == ActorParent.Player) ? ActorParent.Enemy : ActorParent.Player;
        Debug.Log($"턴 전환됨: {CurrentTurn}");
        TurnStart(CurrentTurn);
    }

    private bool CheckWinCondition() //승자가 나올 겨우 true, 아니라면 false 반환
    {
        return false; //please fix
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
    
    private int maxTurnCount = 10;
    
    [ReadOnly] private SamplePlayer[] playerUnits;
    [ReadOnly] private SamplePlayer[] monsterUnits;
    
    public SamplePlayer[] PlayerUnits => playerUnits;
    public SamplePlayer[] MonsterUnits => monsterUnits;
    
    // [Header("PrefabsTable")]
    // [SerializeField] private PrefabsTable playerPrefabsTable;
    // [SerializeField] private PrefabsTable monsterPrefabsTable;
    // [SerializeField] private PrefabsTable playerSkillPrefabsTable;
    
    private Task<bool> InGameInitialize() //게임 시작 초기화
    {
        // maxTurnCount = 50; //맵의 최대 턴 수 정보로
        // playerUnits = new SamplePlayer[10];
        //적 monsterUnits
        
        playerUnits = new SamplePlayer[3];
        monsterUnits = new SamplePlayer[3];

        for (int i = 0; i < 3; i++)
        {
            playerUnits[i] = new SamplePlayer();
            monsterUnits[i] = new SamplePlayer();
        }
        
        return Task.FromResult(true);
    }
}