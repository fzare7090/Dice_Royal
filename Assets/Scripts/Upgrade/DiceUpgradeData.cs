using UnityEngine;

[CreateAssetMenu(fileName = "DiceUpgradeData", menuName = "Game/Dice Upgrade Data")]
public class DiceUpgradeData : ScriptableObject
{
    public int maxLevel = 5;
    public int[] upgradeCost = new int[5] { 100, 250, 500, 1000, 2000 };
    public int[] elexirMultiplier = new int[5] { 10, 15, 20, 25, 30 };
}