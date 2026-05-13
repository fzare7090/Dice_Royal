using UnityEngine;

[CreateAssetMenu(menuName = "Castles/Castle Data")]
public class CastleData : ScriptableObject
{
    public float hp;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float attackSpeed;
    public GameObject projectilePrefab;
}
