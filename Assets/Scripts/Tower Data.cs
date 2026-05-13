using UnityEngine;

[CreateAssetMenu(menuName = "Towers/Tower Data")]
public class TowerData : ScriptableObject
{
    public float hp;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float attackSpeed;

    public GameObject projectilePrefab;
    
}
