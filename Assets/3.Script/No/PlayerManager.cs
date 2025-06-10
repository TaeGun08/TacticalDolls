using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager: MonoBehaviour
{
    public List<CharacterData> CharacterPrefab;
    
    [System.Serializable]
    public class PlayerData
    {
        // player 정보
        // public string Email;
        // public string NickName;
        // public string Role;
        // public bool IsTutorialCompleted;
        // public int RankPoint;
        // public List<string> Friends = new();
        // public List<string> MatchHistorys = new();
        // public List<int> ChracterId;
        
        // character 정보
        //public List<PlayerDataSample> player = new();
    }

    public PlayerDataSample player;
    
    private void Start()
    {
        player = new PlayerDataSample();
        
        TestPlayerHasCharacter();
        
        //player[0].HasCharacter.
        // PlayerData  player = new PlayerData();
        //
        // playerData.Characters.Add(CharacterPrefab[0]);
        //
        // Instantiate(CharacterPrefab[0].gameObject, new Vector3(0, 1, 0), Quaternion.identity);
        //
        // Debug.Log($"playerData.Characters[0].CharacterID::{playerData.Characters[0].CharacterID}");
        //
        // for (int i = 0; i < playerData.Characters.Count; i++)
        // {
        //     foreach (SkillSO skill in playerData.Characters[i].Skills)
        //     {
        //         Debug.Log("Skill info");
        //         Debug.Log($"Name::{skill.Name}");
        //         Debug.Log($"Name::{skill.SkillID}");
        //         Debug.Log($"Name::{skill.Type}");
        //         Debug.Log($"Name::{skill.Range}");
        //     }
        // }

    }

    public void TestPlayerHasCharacter()
    {
        CharacterDataSample sampleCharacter = new CharacterDataSample
        {
            characterCode = 0,
            level = 1,
                
            weapon = new WeaponDataSample
            {
                weaponCode = 0,
                level = 1,
            },
                
            skills = new SkillDataSample[2]
            {
                new SkillDataSample
                {
                    skillCode = 1,
                    level = 1
                },
                new SkillDataSample
                {
                    skillCode = 2,
                    level = 3
                }
            }
        };
        
        CharacterDataSample sampleCharacter2 = new CharacterDataSample
        {
            characterCode = 2,
            level = 1,
                
            weapon = new WeaponDataSample
            {
                weaponCode = 0,
                level = 1,
            },
                
            skills = new SkillDataSample[2]
            {
                new SkillDataSample
                {
                    skillCode = 0,
                    level = 1
                },
                new SkillDataSample
                {
                    skillCode = 2,
                    level = 3
                }
            }
        };
        
        player.HasCharacter.Add(sampleCharacter); 
        player.HasCharacter.Add(sampleCharacter2); 
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"player.HasCharacter[0].characterCode:: {player.HasCharacter[0].characterCode}");

            int pos = 0; 
            
            foreach (var characterDataSample in player.HasCharacter)
            {
                Instantiate(CharacterPrefab[characterDataSample.characterCode].gameObject, new Vector3(pos, 1, 0),
                    Quaternion.identity);

                pos++;
            }
        }
    }
}
