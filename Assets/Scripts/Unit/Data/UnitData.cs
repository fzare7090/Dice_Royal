using UnityEngine;


// [CreateAssetMenu(fileName = "UnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public string name;
    public int spawnCount;
    public float hp;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float enemyFindingRange;
    public float attackCooldown;
    public float attackSpeed;
    public int elexir;
    public GameObject Unitprefab;

}
