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
               saveData.GetLevel() < upgradeData.maxLevel;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
        {
            OnUpgradeFailed?.Invoke(GetFailReason());
            return false;
        }

        int cost = upgradeData.upgradeCost[saveData.GetLevel() - 1];

        if (playerResource == null)
        {
            OnUpgradeFailed?.Invoke("Player resource missing!");
            return false;
        }

        if (playerResource.GetCoin() < cost)
        {
            OnUpgradeFailed?.Invoke($"Need {cost} coins!");
            return false;
        }

        playerResource.SpendCoin(cost);
        saveData.LevelUp(upgradeData.maxLevel);
        saveData.SetLevel(upgradeData.maxLevel);


        AutoSaveEvents.CoinChanged();
        AutoSaveEvents.CastleChanged();

        OnUpgradeSuccess?.Invoke(saveData.GetLevel());
        return true;
    }

    private string GetFailReason()
    {
        if (saveData == null) return "SaveData missing!";
        if (upgradeData == null) return "UpgradeData missing!";
        if (saveData.GetLevel() >= upgradeData.maxLevel) return "Max level reached!";
        return "Unknown reason";
    }

    public UpgradeInfo GetUpgradeInfo()
    {
        if (saveData == null || upgradeData == null || castleData == null)
            return new UpgradeInfo();

        int level = saveData.GetLevel();
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
            : (float)saveData.GetLevel() / upgradeData.maxLevel;
    }

    public int GetCurrentLevel() => saveData.GetLevel();
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
