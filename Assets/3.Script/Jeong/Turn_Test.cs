using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Turn_Test : MonoBehaviour
{
    public static Turn_Test Instance;

    private GridBehavior_Test gridBehavior;

    public TurnManager turnManager; //싱글톤 캐싱
    private ActorParent actorParent = ActorParent.None; //턴 가진 주체 캐싱

    private SamplePlayer selectedCharacter; //선택한 개체
    private int currentTurn; //현재 턴
    private int onSelectSkill; //스킬을 선택하는 로직 필요
    private int aiSelectSkill; //Ai가 자동으로 선택한 스킬
    private bool isBlockedPlayerControl; //스킬 사용 중 플레이어 입력 막음

    public TaskCompletionSource<bool> moveTcs;
    
    public List<Actor_Test> Ally;
    public List<Actor_Test> Enemy;

    public List<Actor_Test> TurnActor;

    public bool IsAuto;

    public Button startButton; 

    private void Awake()
    {
        Instance = this;
        
        startButton.onClick.AddListener(() =>
        {
            turnManager.gameObject.SetActive(true);
        });
    }

    private void Start()
    {
        gridBehavior = GridBehavior_Test.Instance;

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

    private async Task OnTurnChanged(object sender, ActorParent actor) //메서드 반복은 입력이 제한적, 코루티으로 리팩토링?
    {
        currentTurn = turnManager.TurnCount + 1;
        actorParent = actor;
        Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");
        moveTcs = new TaskCompletionSource<bool>();

        if (actor.Equals(ActorParent.Player)) //플레이어 조작
        {
            AllyTest();
            await moveTcs.Task;
        }
        else if (actor == ActorParent.Enemy) //적 조작 //적은 반드시 Ai
        {
            EnemyTest();
            await moveTcs.Task;
        }

        turnManager.TurnEndedSource.TrySetResult(true); //턴 넘김  
    }


    private void AllyTest()
    {
        
        moveTcs.TrySetResult(true);
    }

    private void EnemyTest()
    {
        
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