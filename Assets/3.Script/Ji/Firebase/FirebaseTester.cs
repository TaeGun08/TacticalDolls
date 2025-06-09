using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTester : MonoBehaviour
{
    private List<CharacterDataSample> sampleList;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CharacterDataSample sampleCharacter = new CharacterDataSample
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
            
            PlayerDataSample samplePlayer = new PlayerDataSample
            {
                HasCharacter =  new List<CharacterDataSample> {sampleCharacter}
            };

            string guid = Guid.NewGuid().ToString();
            
            FirestoreManager.Instance.WriteDataAsync(FirebaseCollections.Players, guid, samplePlayer);
        }
    }
}
