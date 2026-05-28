using UnityEngine;

[CreateAssetMenu(fileName = "CardUpgradeData", menuName = "Game/Card Upgrade Data")]
public class CardUpgradeData : ScriptableObject
{
    public int maxLevel = 10;
    public int[] upgradeCost = new int[10] { 50, 100, 200, 400, 800, 1500, 2500, 4000, 6000, 8000 };
    public int[] cardCopiesNeeded = new int[10] { 2, 4, 10, 20, 50, 100, 200, 400, 800, 1200 };
    public float[] hpMultiplier = new float[10] { 1f, 1.2f, 1.4f, 1.7f, 2f, 2.3f, 2.7f, 3.1f, 3.6f, 4.2f };
    public float[] damageMultiplier = new float[10] { 1f, 1.15f, 1.3f, 1.5f, 1.7f, 1.9f, 2.2f, 2.5f, 2.9f, 3.4f };
    public float[] attackSpeedMultiplier = new float[10] { 1f, 1.1f, 1.2f, 1.35f, 1.5f, 1.65f, 1.8f, 2f, 2.2f, 2.5f };
}