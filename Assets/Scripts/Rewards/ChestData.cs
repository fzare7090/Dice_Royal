using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ChestData",
    menuName = "Chest/Chest Data")]
public class ChestData : ScriptableObject
{
    [Header("Coins")]
    public int minCoins;
    public int maxCoins;

    [Header("Gems")]
    public int minGems;
    public int maxGems;

    [Header("Cards")]
    public List<string> possibleCards = new(); ///string??!!! or carddata
}