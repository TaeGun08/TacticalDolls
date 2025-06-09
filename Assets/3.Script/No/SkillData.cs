public enum SkillType { Damage, Heal, Buff }
public enum RangeType { Straight, Plus, Cross, Around }

[System.Serializable]
public class SkillData
{
    public string Name;
    public string Icon;
    public SkillType Type;
    public RangeType RangeType;
    public int Range;
}