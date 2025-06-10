using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    private TurnManager turnManager;                    //싱글톤 캐싱
    private ActorParent actorParent = ActorParent.None; //턴 가진 주체 캐싱
    
    private SamplePlayer selectedCharacter;             //선택한 개체
    private int currentTurn;                            //현재 턴
    private int onSelectSkill;                          //스킬을 선택하는 로직 필요
    private bool isBlockedPlayerControl = false;        //스킬 사용 중 플레이어 입력 막음
    
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

    private void Inintialize()
    {
        //this 초기화
        selectedCharacter = null;
        currentTurn = 0;
        onSelectSkill = 0;
        isBlockedPlayerControl = false;
        
        //캐릭터 행동상태 초기화
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

    private void Update()
    {
        if(isBlockedPlayerControl) return;
        
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

    private async Task FocusCharacter(SamplePlayer player)
    {
        selectedCharacter = player;
        //await please fix + 카메라 무빙
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
    
    private async Task OnTurnChanged(object sender, ActorParent actor) //메서드 반복은 입력이 제한적, 코루티으로 리팩토링?
    {
        Inintialize(); //초기화
        
        currentTurn = turnManager.TurnCount + 1;
        actorParent = actor;
        Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");
        
        if (actor.Equals(ActorParent.Player))
        {
            //if(Ai) OffGUI(); //please fix - ai활성화 상태라면 GUI없이
            //else
            OnGUI(); //플레이어 GUI 활성화
            
            await AutoFocusCharacter(ActorParent.Player);
            
            //if (selectedCharacter.TryGetComponent(Ai)) //Ai 달려있다면
            // {
            //     자동 스킬 실행
            // }
        }
        else if (actor == ActorParent.Enemy)
        {
            //플레이어 GUI 활성화
            OfffGUI();
            
            //적 캐릭터 랜덤 포커스 (Select)
            await AutoFocusCharacter(ActorParent.Enemy);
            
            //적 캐릭터
            //자신의 로직에 따라 스킬 사용 Ai 사용
            if (turnManager.MonsterUnits.All(unit => unit.isCompleteAction)) //모두 행동 완료
            {
                turnManager.TurnEndedSource?.TrySetResult(true);
            }
        }
        
    }
    
    #region Buttons

    public void OnClickedSkillDetailButton() //스킬 버튼 클릭 시 정보, 범위 표시
    {
        
    }

    public void OnClickedConfirmSelectSkillButtonWrapper() //스킬 확인(확정) 버튼 클릭 시 스킬 사용 - 버튼 할당
    {
        OnClickedConfirmSelectSkillButton();
    }
    
    private async Task OnClickedConfirmSelectSkillButton() //스킬 확인(확정) 버튼 클릭 시 스킬 사용 //플레이어 전용
    {
        if (isBlockedPlayerControl) return; 
        isBlockedPlayerControl = true; //플레이어 입력 차단
        
        if (selectedCharacter != null && onSelectSkill != 0) // please fix - if ai일 경우를 생각해야 합니다 (버튼 말고 다른곳에서 하기)
        {
            OfffGUI(); //GUI 비활성화
            
            await selectedCharacter.Excute(onSelectSkill); //스킬 실행 중 대기
            
            FocusCharacter(selectedCharacter); //행동한 캐릭터 자신을 포커스
            
            await Task.Delay(1000); //잠깐 대기
            
            foreach (var player in turnManager.PlayerUnits) //스킬 사용 후 
            {
                if (player.isCompleteAction) continue; //행동완료된 캐릭터는 지나침
                
                FocusCharacter(player);                //행동할 수 있는 캐릭터 포커스
                OnGUI();                               //GUI 활성화
                isBlockedPlayerControl = false;        //플레이어 입력 차단 비활성화
                return;
            }
            
            // //여기까지 왔다면
            // //모두 행동 완료시 턴 종료
            turnManager.TurnEndedSource?.TrySetResult(true);
            
            // if (turnManager.PlayerUnits.All(unit => unit.isCompleteAction)) //모두 행동 완료시 턴 종료
            // {
            //     turnManager.TurnEndedSource?.TrySetResult(true);
            //     return;
            // }
            // else
            // {
            //     foreach (var player in turnManager.PlayerUnits)
            //     {
            //         if (player.isCompleteAction) continue; //행동완료된 캐릭터는 지나침
            //         FocusCharacter(player); //행동할 수 있는 캐릭터 포커스
            //         OnGUI();                //GUI 활성화
            //         return;
            //     }
            // }
        }
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
