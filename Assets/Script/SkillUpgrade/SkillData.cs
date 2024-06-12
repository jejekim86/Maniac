using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string description;
    public int price;
    public int upgradeLevel;
    public GameObject skillPrefab;
}