using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data", order = 1)]
public class SkillData : ScriptableObject
{
    [SerializeField] private string zombieName;
    public string ZombieName { get { return zombieName; } }
    [SerializeField] private int hp;
    public int Hp { get { return hp; } }
    [SerializeField] private int damage;
    public int Damage { get { return damage; } }
    [SerializeField] private float sightRange;
    public float SightRange { get { return sightRange; } }
    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
}