using UnityEngine;

public class DiceUpgrade : MonoBehaviour
{
    [SerializeField] private DiceSaveData saveData;
    [SerializeField] private DiceUpgradeData upgradeData;
    [SerializeField] private PlayerResourceData playerResource;

    public event System.Action<int> OnUpgradeSuccess;
    public event System.Action<string> OnUpgradeFailed;

    public bool CanUpgrade()
    {
        return saveData != null &&
               upgradeData != null &&
               saveData.level < upgradeData.maxLevel;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
        {
            OnUpgradeFailed?.Invoke(GetFailReason());
            return false;
        }

        int cost = upgradeData.upgradeCost[saveData.level - 1];

        

        if (playerResource.elexir < cost)
        {
            OnUpgradeFailed?.Invoke($"Need {cost} elexir!");
            return false;
        }

        playerResource.elexir -= cost;

        saveData.level++;

        OnUpgradeSuccess?.Invoke(saveData.level);
        return true;
    }

    private string GetFailReason()
    {
        if (saveData == null) return "SaveData missing!";
        if (upgradeData == null) return "UpgradeData missing!";
        if (saveData.level >= upgradeData.maxLevel) return "Max level!";
        return "Unknown";
    }

    public DiceUpgradeInfo GetUpgradeInfo()
    {
        if (saveData == null || upgradeData == null)
            return new DiceUpgradeInfo();

        int level = saveData.level;
        bool hasNext = level < upgradeData.maxLevel;

        return new DiceUpgradeInfo
        {
            canUpgrade = hasNext,
            currentLevel = level,
            maxLevel = upgradeData.maxLevel,

            currentMultiplier = upgradeData.elexirMultiplier[level - 1],
            nextMultiplier = hasNext ? upgradeData.elexirMultiplier[level] : 0,

            nextLevelCost = hasNext ? upgradeData.upgradeCost[level - 1] : 0
        };
    }

    public float GetUpgradeProgress()
    {
        return (saveData == null || upgradeData == null)
            ? 0f
            : (float)saveData.level / upgradeData.maxLevel;
    }

    public int GetCurrentMultiplier()
    {
        return upgradeData.elexirMultiplier[saveData.level - 1];
    }
}

public struct DiceUpgradeInfo
{
    public bool canUpgrade;
    public int currentLevel;
    public int maxLevel;
    public int currentMultiplier;
    public int nextMultiplier;
    public int nextLevelCost;
}