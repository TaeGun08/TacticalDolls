using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;

namespace _3.Script.Ji
{
    public class InGameUIManager : MonoBehaviour
    {
        private TurnManager turnManager;
        private SamplePlayer selectedPlayer;
        private int OnSelectSkill = 0;
        
        private TaskCompletionSource<bool> skillConfirmTcs;
        
        private void Start()
        {
            turnManager = TurnManager.Instance;
            if (turnManager == null) return;

            turnManager.ActorChanged += OnTurnChangedWrapper;
            turnManager.GameStateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            if (turnManager == null) return;

            turnManager.ActorChanged -= OnTurnChangedWrapper;
            turnManager.GameStateChanged -= OnGameStateChanged;
        }

        private void OnTurnChangedWrapper(object sender, ActorParent actor)
        {
            _= OnTurnChanged(sender, actor);
        }
        
        private async Task OnTurnChanged(object sender, ActorParent actor) //메서드 반복은 입력이 제한적, 코루티으로 리팩토링?
        {
            int currentTurn = turnManager.TurnCount + 1;

            Debug.Log($"{actor.ToString()}의 {currentTurn}턴이 시작되었습니다.");

            while (turnManager.State == GameState.Playing)
            {
                Debug.Log("e...");

                if (actor == ActorParent.Player)
                {
                    // 기본 플레이어 캐릭터 자동 포커스 (무작위 또는 편성시 제일 앞의 캐릭터 (배열 상 가장 앞))
                    // 또한 이미 행동한 캐릭터는 제외한다.
                    foreach (var t in turnManager.PlayerUnits)
                    {
                        if (t.isCompleteAction == false &&
                            t.isDead == false)
                        {
                            selectedPlayer = t;
                            //please fix + 카메라 무빙
                        }
                    }
                    
                    //클릭시 캐릭터 포커스 변경
                        //if(클릭 시 캐릭터 변경)
                        //selectedPlayer = target
                    
                    //선택된 캐릭터의 스킬 버튼 표시
                    //선택된 오브젝트의 정보 표시
                    
                    //스킬 확정 입력 시 실행?????
                    //아니. 플레이어가 입력 확정을 할 때까지 Await해야 한다. 따라서
                    StartCoroutine(WaitInputCoroutine());
                    
                    await skillConfirmTcs.Task; //입력 완료할때까지 대기
                    
                    await selectedPlayer.Excute(OnSelectSkill); //스킬 선택 확정 시 Excute 애니메이션, CombatSystem 대기
                    
                    if (turnManager.PlayerUnits.All(unit => unit.isCompleteAction)) //모두 행동 완료시 턴 종료
                    {
                        turnManager.TurnEndedSource?.TrySetResult(true);
                        break;
                    }
                    
                    // if (Input.GetKeyDown(KeyCode.Space)) // 테스트용 턴 넘기기
                    // {
                    //     turnManager.TurnEndedSource?.TrySetResult(true);
                    //     break;
                    // }
                }
                else if (actor == ActorParent.Enemy)
                {
                    //기본 적 캐릭터 포커스 (Select)
                    
                    foreach (var t in turnManager.MonsterUnits)
                    {
                        if (t.isCompleteAction == false &&
                            t.isDead == false)
                        {
                            selectedPlayer = t;
                            //please fix + 카메라 무빙
                        }
                    }
                    
                    //적 캐릭터
                    //자신의 로직에 따라 스킬 사용 Ai 사용
                
                    // if (turnManager.MonsterUnits.All(unit => unit.IsCompleteAction)) //모두 행동 완료
                    // {
                    //     turnManager.TurnEndedSource?.TrySetResult(true);
                    //     break;
                    // }
                    
                    // if (Input.GetKeyDown(KeyCode.Space)) // 테스트용 턴 넘기기
                    // {
                    //     turnManager.TurnEndedSource?.TrySetResult(true);
                    //     break;
                    // }
                }
            }
        }

        private IEnumerator WaitInputCoroutine()
        {
            while (true)
            {
                //입력을 기다리는 동안 UI 반응이 가능하도록 코루틴에서 설계
                
                //please fix - UI 활동
                
                //스킬 입력 확정 시
                if (selectedPlayer != null && OnSelectSkill != 0 && true) // please fix - if ai일 경우를 생각해야 합니다. && 스킬 확정하는 방식 추가하기
                {
                    skillConfirmTcs.TrySetResult(true); //대기 해제
                    yield break;
                }
                
                yield return null;
            }
        }
        
        private void OnGameStateChanged(object sender, GameStateEventArgs e)
        {
            if (e.State == GameState.Ended)
            {
                Debug.Log("게임이 종료되었습니다.");
            }
        }
    }
}
