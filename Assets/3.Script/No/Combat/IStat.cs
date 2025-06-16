using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStat
{
    int Level { get; set; }
    int HP { get; set; }
    int Attack { get; set; }
    int Defense { get; set; }
    int MoveRange { get; set; }
    List<SkillEffectHandlerBase> Skills { get; set; }
}
