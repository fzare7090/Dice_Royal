using UnityEngine;

[CreateAssetMenu(menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public float hp;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float attackSpeed;

    public GameObject firePrefab;
}
