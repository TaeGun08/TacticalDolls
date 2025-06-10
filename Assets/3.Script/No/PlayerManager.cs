using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager: MonoBehaviour
{
    public static PlayerManager Instance;
    
    public List<CharacterData> CharacterPrefab;
    public PlayerDataSample player;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        player = new PlayerDataSample();
        
        TestPlayerHasCharacter();
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
            characterCode = 1,
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
        
        CharacterDataSample sampleCharacter3 = new CharacterDataSample
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
        player.HasCharacter.Add(sampleCharacter3); 
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // int pos = 0; 
            //
            // foreach (var characterDataSample in player.HasCharacter)
            // {
            //     Instantiate(CharacterPrefab[characterDataSample.characterCode].gameObject, new Vector3(pos, 1, 0),
            //         Quaternion.identity);
            //
            //     pos++;
            // }
        }
    }
}
