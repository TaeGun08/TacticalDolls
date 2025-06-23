// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class AttackUpBuff : BuffParent
// {
//     private int attackIncrease;
//
//     public AttackUpBuff(int turns, int increase)
//     {
//         remainingTurns = turns;
//         attackIncrease = increase;
//     }
//
//     public override void OnApply(IDamageAble target)
//     {
//         target.Stat.Attack += attackIncrease;
//     }
//
//     public override void OnExpire(IDamageAble target)
//     {
//         target.Stat.Attack -= attackIncrease;
//     }
// }
//
