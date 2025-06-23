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

        MoveTcs = new TaskCompletionSource<bool>();

        if (actor.Equals(ActorParent.Player))
        {
            GameManager.Instance.InitCharacterTurnSetting(GameManager.Instance.PlayerUnits[0]);

            TurnActor = GameManager.Instance.PlayerUnits;

            // 모든 캐릭터 행동 종료 체크
            await OnCheckEndCharacterActor();
        }
        else if (actor.Equals(ActorParent.Enemy))
        {
            await OnCheckEndEnemyActor();
        }

        turnManager.TurnEndedSource.TrySetResult(true);
    }

    private async Task OnCheckEndCharacterActor()
    {
        while (GridBehavior.Instance.Actor != null)
        {
            await Task.Delay(100);
        }

        gridBehavior.Actors = new List<IDamageAble>();

        foreach (var enemy in GameManager.Instance.EnemyUnits)
        {
            gridBehavior.Actors.Add(enemy);
        }

        int checkCharacterAction = 0;

        gridBehavior.IsAutoMove = true;

        List<CharacterData> playerUnits = GameManager.Instance.PlayerUnits;

        while (checkCharacterAction < playerUnits.Count)
        {
            // Debug.Log("플레이어 턴");
            foreach (var character in playerUnits)
            {
                if (character.Stat.IsCompleteAction) continue;
                checkCharacterAction++;
            }

            if (gridBehavior.IsAuto)
            {
                foreach (var player in playerUnits)
                {
                    if (gridBehavior.IsAuto == false) break;
                    
                    MoveTcs = new TaskCompletionSource<bool>();
                    RangeSystem.Instance.ResetAllTiles();
                    GameManager.Instance.EndTurnBtn.gameObject.SetActive(false);
                    SkillSelectSystem.Instance.cancelButton.gameObject.SetActive(false);
                    SkillSelectSystem.Instance.selectButton.gameObject.SetActive(false);
                    gridBehavior.Actor = player;
                    await MoveTcs.Task;
                }
            }

            checkCharacterAction = 0;

            await Task.Delay(100);
        }

        gridBehavior.IsAutoMove = false;
    }

    private async Task OnCheckEndEnemyActor()
    {
        while (GridBehavior.Instance.Actor != null)
        {
            await Task.Delay(100);
        }

        gridBehavior.Actors = new List<IDamageAble>();

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