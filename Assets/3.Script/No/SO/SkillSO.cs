using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillSO : ScriptableObject
{
    public string SkillID;
    public string Name;
    public SkillType Type;
    public RangeType RangeType;
    public int Range;
}