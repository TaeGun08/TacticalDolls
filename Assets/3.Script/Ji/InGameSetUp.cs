using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _3.Script.Ji
{
    public partial class TurnManager : MonoBehaviour
    {
        private int maxTurnCount = 10;
        
        [ReadOnly] public SamplePlayer[] playerUnits;
        [ReadOnly] public SamplePlayer[] monsterUnits;
        
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
            
            return Task.FromResult(true);
        }
    }
}
