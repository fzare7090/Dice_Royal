using UnityEngine;

[CreateAssetMenu(menuName = "Castles/Castle Upgrade Data")]
public class CastleUpgradeData : ScriptableObject
{
    [Header("Level Settings")]
    public int maxLevel = 5;

    [Header("Upgrade Costs")]
    public int[] upgradeCost = new int[5] { 100, 250, 500, 1000, 2000 };

    [Header("Stat Multipliers")]
    public float[] hpMultiplier = new float[5] { 1f, 1.2f, 1.4f, 1.7f, 2f };
    public float[] damageMultiplier = new float[5] { 1f, 1.15f, 1.3f, 1.5f, 1.8f };
    public float[] attackSpeedMultiplier = new float[5] { 1f, 1.1f, 1.2f, 1.35f, 1.5f };
}