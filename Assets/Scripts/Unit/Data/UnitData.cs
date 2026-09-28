using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public new string name;
    public int spawnCount;
    public float hp;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float enemyFindingRange;
    public float attackCooldown;
    public float attackSpeed;
    public int elexir;
    public Sprite cardIcon;

    public enum UnitType
    {
        Ground,
        Air,
        Building
    }

    public enum TargetPreference
    {
        GroundOnly,
        AirOnly,
        BuildingsOnly,
        GroundAndAir
    }

    [Header("Targeting Settings")]
    public UnitType unitType;
    public TargetPreference targetPreference;

    public GameObject Unitprefab;
}