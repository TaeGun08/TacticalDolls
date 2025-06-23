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
    }

    private async Task OnTurnChanged(object sender, ActorParent actor)
    {
        currentTurn = turnManager.TurnCount + 1;
        actorParent = actor;
        Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");

        MoveTcs = new TaskCompletionSource<bool>();
        gridBehavior.Actors = new List<IDamageAble>();

        if (actor.Equals(ActorParent.Player))
        {
            GameManager.Instance.InitCharacterTurnSetting(GameManager.Instance.PlayerUnits[0]);

            TurnActor = GameManager.Instance.PlayerUnits;

            // 모든 캐릭터 행동 종료 체크
            await MoveTcs.Task;
        }
        else if (actor.Equals(ActorParent.Enemy))
        {
            await OnCheckEndEnemyActor();
        }
    }

    public async Task OnCheckEndCharacterActor()
    {
        foreach (var enemy in GameManager.Instance.EnemyUnits)
        {
            gridBehavior.Actors.Add(enemy);
        }

        gridBehavior.IsAutoMove = true;
        
        foreach (var player in GameManager.Instance.PlayerUnits)
        {
            if (gridBehavior.IsAuto) break;
            if (player.Stat.IsCompleteAction) continue;
            
            MoveTcs = new TaskCompletionSource<bool>();
            await gridBehavior.AutoMove(player);
        }

        bool playerIsCompleteCheck = true;
        
        foreach (var player in GameManager.Instance.PlayerUnits)
        {
            if (player.Stat.IsCompleteAction) continue;
            playerIsCompleteCheck = false;
        }

        if (playerIsCompleteCheck)
        {
            MoveTcs.TrySetResult(true);
        }
    }

    private void OffPlayerUI()
    {
        RangeSystem.Instance.ResetAllTiles();
        GameManager.Instance.EndTurnBtn.gameObject.SetActive(false);
        SkillSelectSystem.Instance.cancelButton.gameObject.SetActive(false);
        SkillSelectSystem.Instance.selectButton.gameObject.SetActive(false);
    }

    private async Task OnCheckEndEnemyActor()
    {
        foreach (var player in GameManager.Instance.PlayerUnits)
        {
            gridBehavior.Actors.Add(player);
        }

        gridBehavior.IsAutoMove = true;
        
        foreach (var enemy in GameManager.Instance.EnemyUnits)
        {
            MoveTcs = new TaskCompletionSource<bool>();
            await gridBehavior.AutoMove(enemy);
        }
        
        gridBehavior.IsAutoMove = false;
        
        turnManager.TurnEndedSource.TrySetResult(true);
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