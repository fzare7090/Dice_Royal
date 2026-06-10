using UnityEngine;

public class CastleUpgrade : MonoBehaviour
{
    [SerializeField] private CastleSaveData saveData;
    [SerializeField] private CastleUpgradeData upgradeData;
    [SerializeField] private CastleData castleData;
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
            OnUpgradeFailed?.Invoke($"Need {cost} coins!");
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
        if (saveData.level >= upgradeData.maxLevel) return "Max level reached!";
        return "Unknown reason";
    }

    public UpgradeInfo GetUpgradeInfo()
    {
        if (saveData == null || upgradeData == null || castleData == null)
            return new UpgradeInfo();

        int level = saveData.level;
        bool hasNext = level < upgradeData.maxLevel;

        return new UpgradeInfo
        {
            canUpgrade = hasNext,
            currentLevel = level,
            maxLevel = upgradeData.maxLevel,

            nextLevelCost = hasNext ? upgradeData.upgradeCost[level - 1] : 0,

            currentHP = castleData.hp * upgradeData.hpMultiplier[level - 1],
            currentDamage = castleData.attackDamage * upgradeData.damageMultiplier[level - 1],

            nextHP = hasNext ? castleData.hp * upgradeData.hpMultiplier[level] : 0,
            nextDamage = hasNext ? castleData.attackDamage * upgradeData.damageMultiplier[level] : 0
        };
    }

    public float GetUpgradeProgress()
    {
        return (saveData == null || upgradeData == null)
            ? 0f
            : (float)saveData.level / upgradeData.maxLevel;
    }

    public int GetCurrentLevel() => saveData.level;
}

public struct UpgradeInfo
{
    public bool canUpgrade;
    public int currentLevel;
    public int maxLevel;
    public int nextLevelCost;

    public float currentHP;
    public float currentDamage;
    public float nextHP;
    public float nextDamage;
}