using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHPBar : MonoBehaviour
{
    public UnitParent unitParent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CombatEvent e = new CombatEvent()
            {
                Damage = 30,
                Target = unitParent,
            };
            unitParent.TakeDamage(e);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HealEvent e = new HealEvent()
            {
                Heal = 30,
            };
            unitParent.TakeHeal(e);
        }
    }
}
