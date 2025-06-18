using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public PrefabsTable CharacterTable;
    public Button ExitButton;
    public GameObject EndGamePanel;
    
    // 게임에 배치된 유닛    
    public List<CharacterData> PlayerUnits;
    public List<EnemyData> EnemyUnits;

    private bool isGameStart;

    // 캐릭터 클릭
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private SkillSelectSystem skillUI;
    [SerializeField] private Button endTurnBtn;

    public Button EndTurnBtn => endTurnBtn;

    private CharacterData currentCharacter;
    public EnemyData CurrentEnemy { get; set; }

    private Tile currentEnemyTile;

    public Tile MoveChoiceTile { get; set; }

    private bool isEnemyInAttackRange;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ExitButton.onClick.AddListener(OnExitButtonClicked);

        endTurnBtn.onClick.AddListener(OnCharacterEndTurn);
    }

    private void Start()
    {
        TurnManager.Instance.ActorChanged += UnitStateInitialize;
    }

    // 첫 턴
    public void UnitInitializeStarSetting()
    {
        _ = TurnManager.Instance.RunGameFlow();
        currentCharacter = PlayerUnits[0];
        isGameStart = true;
    }

    // character 상태 초기화
    private void UnitStateInitialize(object sender, ActorParent actor)
    {
        if (actor == ActorParent.Player)
        {
            foreach (var character in PlayerUnits)
            {
                character.Stat.IsCompleteAction = false;
            }
        }
        else
        {
            foreach (var enemy in EnemyUnits)
            {
                enemy.Stat.IsCompleteAction = false;
            }
        }
    }

    // 전투 강제 종료
    private void OnExitButtonClicked()
    {
        PlayerManager.Instance.ResetCachedCharacterData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 애플리케이션에서는 종료
            Application.Quit();
#endif
    }

    public void OnCharacterEndTurn()
    {
        if (MoveChoiceTile != null)
        {
            GridBehavior.Instance.Actor = currentCharacter;
            List<Node> path = PathFindingManager.Instance.PathFind(
                currentCharacter.transform.position, MoveChoiceTile.transform.position);
            _ = GridBehavior.Instance.MovePlayerAlongPath(path, Vector3.zero);
        }

        MoveChoiceTile = null;
        SkillSelectSystem.Instance.IsSelectingSkill = false;

        RangeSystem.Instance.ResetAllTiles();
        endTurnBtn.gameObject.SetActive(false);
        skillUI.Close();

        CurrentEnemy = null;

        currentCharacter.Stat.IsCompleteAction = true;

        CheckCharacterAction();
        NextCharacterSetting();
    }

    // 캐릭터 전체 행동 체크 후 턴 전환
    private void CheckCharacterAction()
    {
        bool checkCharacterAction = true;

        foreach (var character in PlayerUnits)
        {
            if (character.Stat.IsCompleteAction) continue;
            checkCharacterAction = false;
        }

        if (checkCharacterAction)
        {
            TurnManager.Instance.TurnEndedSource.TrySetResult(true);
            StartCoroutine(TestTimerCoroutine());
        }
    }

    private IEnumerator TestTimerCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Turn_Test.Instance.MoveTcs.TrySetResult(true);
    }

    // 캐릭터 행동 종료 -> 다음 캐릭터 전환
    private void NextCharacterSetting()
    {
        foreach (var character in PlayerUnits)
        {
            if (character.Stat.IsCompleteAction)
            {
                continue;
            }

            InitCharacterTurnSetting(character);
            break;
        }
    }

    private void Update()
    {
        if (!isGameStart) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (SkillSelectSystem.Instance.IsSelectingSkill)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                        out RaycastHit unitHit, 100f, unitLayer))
                {
                    if (unitHit.collider.TryGetComponent(out CharacterData characterData))
                    {
                        // TODO 같은 팀한테 스킬 사용할 때 구현 해야함
                    }
                    
                    else if (unitHit.collider.TryGetComponent(out EnemyData enemyData))
                    {
                        currentEnemyTile =
                            TileManager.Instance.GetCurrentTileByIDamageAble(enemyData);
                        isEnemyInAttackRange =
                            RangeSystem.Instance.attackableTiles.Contains(currentEnemyTile);

                        if (isEnemyInAttackRange)
                        {
                            CurrentEnemy = enemyData;

                            RangeSystem.Instance.ShowSkillRange(currentCharacter, CurrentEnemy,
                                skillUI.currentSkill);
                            SkillSelectSystem.Instance.selectButton.interactable = true;
                        }
                    }
                }
            }
            else
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                        out RaycastHit characterHit, 100f, unitLayer))
                {
                    if (characterHit.collider.TryGetComponent(out CharacterData characterData))
                    {
                        currentCharacter = characterData;
                    }
                    else return;

                    // TODO 행동 완료 UI 추가

                    if (currentCharacter != null && !currentCharacter.Stat.IsCompleteAction)
                    {
                        RangeSystem.Instance.ResetAllTiles();
                        endTurnBtn.gameObject.SetActive(true);
                        skillUI.Open(currentCharacter);
                        MoveChoiceTile = null;
                        RangeSystem.Instance.ShowMoveRange(
                            TileManager.Instance.GetCurrentTileByIDamageAble(currentCharacter),
                            currentCharacter.Stat.MoveRange);
                    }
                }
                else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                             out var tileHit))
                {
                    if (currentCharacter == null) return;

                    Tile targetTile = tileHit.collider.GetComponent<Tile>();
                    if (targetTile == null || !targetTile.isWalkable) return;

                    if (!RangeSystem.Instance.IsTileInMoveRange(targetTile))
                    {
                        Debug.Log("이동 불가능한 범위입니다.");
                        RangeSystem.Instance.ResetAllTiles();
                        endTurnBtn.gameObject.SetActive(false);
                        skillUI.Close();

                        MoveChoiceTile = null;
                        return;
                    }

                    if (MoveChoiceTile != null)
                    {
                        MoveChoiceTile.Highlight(Color.white);
                    }

                    MoveChoiceTile = targetTile;
                    MoveChoiceTile.Highlight(Color.magenta);
                }
            }
        }
    }

    // 게임 초기 캐릭터 설정
    public void InitCharacterTurnSetting(CharacterData target)
    {
        RangeSystem.Instance.ShowMoveRange(TileManager.Instance.GetCurrentTileByIDamageAble(target),
            target.Stat.MoveRange);
        endTurnBtn.gameObject.SetActive(true);
        skillUI.Open(target);
        currentCharacter = target;
    }
}