using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class InGameUIManager : MonoBehaviour
{
    private TurnManager turnManager;                    //싱글톤 캐싱
    private ActorParent actorParent = ActorParent.None; //턴 가진 주체 캐싱
    
    private SamplePlayer selectedCharacter;             //선택한 개체
    private int currentTurn;                            //현재 턴
    private int onSelectSkill;                          //스킬을 선택하는 로직 필요
    private int aiSelectSkill;                          //Ai가 자동으로 선택한 스킬
    private bool isBlockedPlayerControl;                //스킬 사용 중 플레이어 입력 막음
    
    private void Start()
    {
        turnManager = TurnManager.Instance;
        if (turnManager == null)
        {
            Debug.LogError("턴 매니저 인스턴스가 없습니다.");
            return;
        }
        
        turnManager.ActorChanged += OnTurnChangedWrapper;
        turnManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (turnManager == null) return;

        turnManager.ActorChanged -= OnTurnChangedWrapper;
        turnManager.GameStateChanged -= OnGameStateChanged;
    }
    
    private void Update()
    {
        if(isBlockedPlayerControl) return; //입력 불가 상태일 경구 차단
        
        switch (actorParent)
        {
            case ActorParent.None: //아무 일도 일어나지 않음
                return;
            
            case ActorParent.Player:
                //클릭으로 캐릭터 선택하기 selectedPlayer 변경
                ClickObject();
                break;
            
            case ActorParent.Enemy:
                //Pause 버튼과 Pause UI 제외 인게임 조작 완전히 막힘
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region UI Control
    
    private void OnGUI()
    {
        
    }

    private void OfffGUI()
    {
        
    }
    
    private void ClickObject()
    {
        if(isBlockedPlayerControl) return;
        // 클릭 시 Ray발사, GUI 변경
        
        // 캐릭터인 경우
        //FocusCharacter(player)
        
        // 적인 경우
        
        // 맵 오브젝트인 경우
        
        // 스킬인 경우 <-버튼으로 처리
    }
    #endregion

    #region SelectCharacter
    
    private async Task FocusCharacter(SamplePlayer player)
    {
        selectedCharacter = player;
        //await please fix - 카메라 무빙 추가
    }
    
    // 기본 플레이어 자동 포커스 (무작위 또는 편성시 제일 앞의 캐릭터 (배열 상 가장 앞))
    private async Task AutoFocusCharacter(ActorParent actor)
    {
        SamplePlayer[] character;

        switch (actor)
        {
            case ActorParent.None:
                return;
            case ActorParent.Player:
                character = turnManager.PlayerUnits;
                break;
            case ActorParent.Enemy:
            default:
                character = turnManager.MonsterUnits;
                break;
        }
        
        //자동 선택
        foreach (var c in character)
        {
            if (c.isCompleteAction == false && //행동 완료한 캐릭터 제외, 사망 캐릭터 제외
                c.isDead == false)
            {
                await FocusCharacter(c); //카메라 무브 대기
                break;
            }
        }
    }
    

    #endregion
    
    #region TurnLogic
    
    private bool isAi;
    
    private void Inintialize()
    {
        //this 초기화
        selectedCharacter = null;
        currentTurn = 0;
        onSelectSkill = 0;
        isBlockedPlayerControl = false;
        
        //캐릭터 행동상태 초기화
        if (turnManager.PlayerUnits.Length == 0 || turnManager.MonsterUnits.Length == 0) return;
        
        foreach (var player in turnManager.PlayerUnits)
        {
            player.isCompleteAction = false;
        }
        
        foreach (var player in turnManager.MonsterUnits)
        {
            player.isCompleteAction = false;
        }
    }
    
    private void OnTurnChangedWrapper(object sender, ActorParent actor)
    {
        _= OnTurnChanged(sender, actor);
    }
    
    private async Task OnTurnChanged(object sender, ActorParent actor) //메서드 반복은 입력이 제한적, 코루티으로 리팩토링?
    {
        Inintialize(); //초기화
        
        currentTurn = turnManager.TurnCount + 1;
        actorParent = actor;
        Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");
        
        if (actor.Equals(ActorParent.Player)) //플레이어 조작
        {
            await AutoFocusCharacter(actor); //자동으로 행동가능한 캐릭터를 포커스 & 선택
            
            if (isAi)
            {
                _= ExcuteSkill(true); //Ai라면 바로 스킬을 실행
            }
            else
            {
                OnGUI(); //플레이어 GUI 활성화 후 클릭으로 스킬 실행
            }
        }
        else if (actor == ActorParent.Enemy) //적 조작 //적은 반드시 Ai
        {
            OfffGUI(); //플레이어 GUI 비활성화
            
            //적 캐릭터 랜덤 포커스 (Select)
            await AutoFocusCharacter(actor);
            _= ExcuteSkill(true); //Ai라면 바로 스킬을 실행
        }
    }
    
    #endregion
    
    #region Buttons

    public void OnClickedSkillDetailButton() //스킬 버튼 클릭 시 정보, 범위 표시
    {
        
    }

    public void OnClickedConfirmSelectSkillButtonWrapper1() //1스킬 확인(확정) 버튼 클릭 시 스킬 사용 - 버튼 할당
    {
        onSelectSkill = 1;
        OnClickedConfirmSelectSkillButton();
    }
    
    public void OnClickedConfirmSelectSkillButtonWrapper2() //2스킬 확인(확정) 버튼 클릭 시 스킬 사용 - 버튼 할당
    {
        onSelectSkill = 2;
        OnClickedConfirmSelectSkillButton();
    }
    
    public void OnClickedConfirmSelectSkillButtonWrapper3() //3스킬 확인(확정) 버튼 클릭 시 스킬 사용 - 버튼 할당
    {
        onSelectSkill = 3;
        OnClickedConfirmSelectSkillButton();
    }
    
    private void OnClickedConfirmSelectSkillButton() //실제 스킬이 사용되는 메서드 //플레이어 전용
    {
        if (isBlockedPlayerControl) return; 
        isBlockedPlayerControl = true; //플레이어 입력 차단

        if (selectedCharacter == null || onSelectSkill == 0)
        {
            Debug.Log($"onSelectSkill : {onSelectSkill}");
            Debug.LogError("캐릭터 또는 스킬이 선택되지 않았습니다. ");
            turnManager.TurnEndedSource?.TrySetResult(true); //턴 종료
            return;
        }
        
        OfffGUI(); //GUI 비활성화
        _= ExcuteSkill(); //스킬 실행
    }
    #endregion
    
    private async Task ExcuteSkill(bool isAiSkill = false)
    {
        if(selectedCharacter.isCompleteAction) return;
        
        if (isAiSkill)
        {
            int aiRandomSkill = Random.Range(1, 4); //please fix - 캐릭터마다 자동으로 스킬을 선택하는 로직을 두기?
            await selectedCharacter.Excute(aiRandomSkill); //스킬 실행 중 대기 ai전용
        }
        else
        {
            await selectedCharacter.Excute(onSelectSkill); //스킬 실행 중 대기
        }
        
        //스킬 실행 완료
        _= FocusCharacter(selectedCharacter); //행동한 캐릭터 자신을 포커스
        
        await Task.Delay(1000); //잠깐 대기
            
        foreach (var player in turnManager.PlayerUnits) //스킬 사용 후 행동 가능 캐릭터 검사
        {
            if (player.isCompleteAction) continue; //행동완료된 캐릭터는 지나침
            
            OnGUI();                               //GUI 활성화
            _= FocusCharacter(player);                //행동할 수 있는 캐릭터 포커스
            isBlockedPlayerControl = false;        //플레이어 입력 차단 비활성화
            return;
        }
        
        //여기까지 왔다면 모두 행동을 완료했습니다.
        turnManager.TurnEndedSource?.TrySetResult(true); //턴 종료
    }
    
    private void OnGameStateChanged(object sender, GameStateEventArgs e)
    {
        if (e.State == GameState.Ended)
        {
            Debug.Log("게임이 종료되었습니다.");
        }
    }
}
