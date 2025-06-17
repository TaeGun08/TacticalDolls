using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Ally_Test : Actor_Test
{
    private CharacterData characterData;

    protected void Awake()
    {
        characterData = GetComponent<CharacterData>();
    }
    
    protected override void Start()
    {
        base.Start();
        turn.Ally.Add(this);
    }

    public override void Excute(int selectedSkill)
    {
        combatSystem.ExecuteSkill(characterData, selectedSkill);
    }
}
