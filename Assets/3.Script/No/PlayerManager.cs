using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager: MonoBehaviour
{
    public List<CharacterData> CharacterPrefab;
    
    [System.Serializable]
    public class PlayerData
    {
        // player 정보
        public string Email;
        public string NickName;
        public string Role;
        public bool IsTutorialCompleted;
        public int RankPoint;
        public List<string> Friends = new();
        public List<string> MatchHistorys = new();
        public List<int> ChracterId;
        
        // character 정보
        public List<CharacterData> Characters = new();
    }

    private void Start()
    {
        PlayerData  playerData = new PlayerData();
        
        playerData.Characters.Add(CharacterPrefab[0]);

        Instantiate(CharacterPrefab[0].gameObject, new Vector3(0, 1, 0), Quaternion.identity);
        
        Debug.Log($"playerData.Characters[0].CharacterID::{playerData.Characters[0].CharacterID}");

        for (int i = 0; i < playerData.Characters.Count; i++)
        {
            foreach (SkillSO skill in playerData.Characters[i].Skills)
            {
                Debug.Log("Skill info");
                Debug.Log($"Name::{skill.Name}");
                Debug.Log($"Name::{skill.SkillID}");
                Debug.Log($"Name::{skill.Type}");
                Debug.Log($"Name::{skill.Range}");
            }
        }

    }
}
