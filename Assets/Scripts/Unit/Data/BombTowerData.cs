using UnityEngine;

[CreateAssetMenu(fileName = "BombTowerData", menuName = "Game/Unit Data/BombTowerData")]
public class BombTowerData : UnitData
{
    [Header("Bomb Tower Settings")]
    public GameObject bombPrefab;
    public float explosionRadius;
    public float deathExplosionRadius;
    public float deathExplosionDamage;
}
