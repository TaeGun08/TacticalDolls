using System.Collections.Generic;
using System.Threading.Tasks;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    //public PrefabsTable CharacterTable;
    public ButtonManager ExitButton;

    // 게임에 배치된 유닛    
    public List<CharacterData> PlayerUnits;
    public List<EnemyData> EnemyUnits;

    private bool isGameStart;

    // 캐릭터 클릭
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private SkillSelectSystem skillUI;
    [SerializeField] private ButtonManager endTurnBtn;

    // 캐릭터 스폰
    public CharacterSpawnController CharacterSpawnController;
    public GameObject SelectedCharacterPanel; //del
    // public Button StartBtn; //del

    private CharacterData currentCharacter;
    public Transform CurrentSkillTarget { get; set; }
    private Tile currentSkillTargetTile;

    public Tile MoveChoiceTile { get; set; }

    private bool isTargetInAttackRange;
    
    private Camera mainCamera;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ExitButton.onClick.AddListener(OnExitButtonClicked);
        // endTurnBtn.onClick.AddListener(OnCharacterEndTurn_Wrapper);
        //StartBtn.onClick.AddListener(StartGame);
    }

    private void Start()
    {
        SoundManager.Instance.PlayBgm("maou_bgm_cyber43");
        TurnManager.Instance.ActorChanged += UnitStateInitialize;
        mainCamera = Camera.main;
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

    public void OnCharacterEndTurn_Wrapper()
    {
        _ = OnCharacterEndTurn();
    }
    
    public async Task OnCharacterEndTurn()
    {
        RangeSystem.Instance.ResetAllTiles();
        endTurnBtn.gameObject.SetActive(false);
        skillUI.Close();
        
        if (MoveChoiceTile != null)
        {
            GridBehavior.Instance.Actor = currentCharacter;
            List<Node> path = PathFindingManager.Instance.PathFind(
                currentCharacter.transform.position, MoveChoiceTile.transform.position);
            await GridBehavior.Instance.MovePlayerAlongPath(path, Vector3.zero);
        }
        
        MoveChoiceTile = null;
        SkillSelectSystem.Instance.IsSelectingSkill = false;

        CurrentSkillTarget = null;

        currentCharacter.Stat.IsCompleteAction = true;

        _= CheckCharacterAction();
        NextCharacterSetting();
    }

    // 캐릭터 전체 행동 체크 후 턴 전환
    private async Task CheckCharacterAction()
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
        }

        await Task.Yield();
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
        // StartBtn.gameObject.SetActive(PlayerManager.Instance.usingCharacter.Count > 0);
        
        if (!isGameStart) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (SkillSelectSystem.Instance.IsSelectingSkill)
            {
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
                        out RaycastHit unitHit, 100f, unitLayer | tileLayer))
                {
                    InputHitTarget(unitHit);
                }
            }
            else
            {
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, 100f))
                {
                    int hitLayer = raycastHit.collider.gameObject.layer;
                    if ( ((1 << hitLayer) & unitLayer.value) != 0 )
                    {
                        InputSelectCharacter(raycastHit);
                    }
                    else
                    {
                        InputSelectTile(raycastHit);
                        InGameUIEventTerminal.UnitInfoEvents.DisableUnitInfoAction?.Invoke();
                    }
                }
            }
        }
    }

    private void InputHitTarget(RaycastHit hit)
    {
        int currentSkillIndex = SkillSelectSystem.Instance.GetCurrentSkillIndex();

        switch (currentCharacter.HasSkills[currentSkillIndex].targetType)
        {
            case TargetType.Tile:
                if (hit.collider.TryGetComponent(out Tile tile))
                {
                    currentSkillTargetTile = tile;
                }
                break;
            case TargetType.Ally:
                if (hit.collider.TryGetComponent(out CharacterData characterData))
                {
                    currentSkillTargetTile =
                        TileManager.Instance.GetCurrentTileByIDamageAble(characterData);
                }
                break;
            case TargetType.Enemy:
                if (hit.collider.TryGetComponent(out EnemyData enemyData))
                {
                    currentSkillTargetTile =
                        TileManager.Instance.GetCurrentTileByIDamageAble(enemyData);
                }
                break;
        }

        CurrentSkillTarget = hit.collider.gameObject.transform;
        
        TargetAttackRange();
    }

    private void TargetAttackRange()
    {
        isTargetInAttackRange =
            RangeSystem.Instance.attackableTiles.Contains(currentSkillTargetTile);
        
        if (isTargetInAttackRange)
        {
            SkillSelectSystem.Instance.CashedDamageAbles = 
                RangeSystem.Instance.ShowSkillRange(
                    currentCharacter, 
                    currentSkillTargetTile, 
                    skillUI.currentSkill);
            SkillSelectSystem.Instance.selectButton.interactable = true;
        }
    }

    private void InputSelectCharacter(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out CharacterData characterData))
        {
            currentCharacter = characterData;
            
            //유닛 패널 호출
            InGameUIEventTerminal.UnitInfoEvents.ShowUnitInfoEventHandler?.Invoke(this, new ShowUnitInfoEventArgs(characterData));
        }
        else if(hit.collider.TryGetComponent(out EnemyData enemyData))
        {
            //유닛 패널 호출
            InGameUIEventTerminal.UnitInfoEvents.ShowUnitInfoEventHandler?.Invoke(this, new ShowUnitInfoEventArgs(enemyData));
            return;
        }

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

    private void InputSelectTile(RaycastHit hit)
    {
        if (currentCharacter == null) return;

        Tile targetTile = hit.collider.GetComponent<Tile>();
        if (targetTile == null || !targetTile.isWalkable) return;

        if (!RangeSystem.Instance.IsTileInMoveRange(targetTile))
        {
            Debug.Log("이동 불가능한 범위입니다.");
            RangeSystem.Instance.ResetAllTiles();
            // endTurnBtn.gameObject.SetActive(false);
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
    
    // 게임 초기 캐릭터 설정
    public void InitCharacterTurnSetting(CharacterData target)
    {
        RangeSystem.Instance.ShowMoveRange(TileManager.Instance.GetCurrentTileByIDamageAble(target),
            target.Stat.MoveRange);
        endTurnBtn.gameObject.SetActive(true);
        skillUI.Open(target);
        currentCharacter = target;
    }
    
    // 게임 종료 초기화
    public void InitGameOverSetting()
    {
        PlayerUnits.Clear();
        EnemyUnits.Clear();
        PlayerManager.Instance.usingCharacter.Clear();
        
        // 타일 초기화
        foreach (var character in PlayerUnits)
        {
            var tile = TileManager.Instance.GetCurrentTileByIDamageAble(character);
            
            tile.isWalkable = true;
            tile.isUsingTile = false;
        }
        
        // 로비 -> 플레이어 데이터 초기화
        PlayerManager.Instance.InitializePlayerManager();
    }
}