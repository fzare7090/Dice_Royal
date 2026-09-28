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
        AutoSaveEvents.DiceChanged();

        OnUpgradeSuccess?.Invoke(saveData.GetLevel());
        return true;
    }

    private string GetFailReason()
    {
        if (saveData == null) return "SaveData missing!";
        if (upgradeData == null) return "UpgradeData missing!";
        if (saveData.GetLevel() >= upgradeData.maxLevel) return "Max level!";
        return "Unknown";
    }

    public DiceUpgradeInfo GetUpgradeInfo()
    {
        if (saveData == null || upgradeData == null)
            return new DiceUpgradeInfo();

        int level = saveData.GetLevel();
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
            : (float)saveData.GetLevel() / upgradeData.maxLevel;
    }

    public int GetCurrentMultiplier()
    {
        return upgradeData.elexirMultiplier[saveData.GetLevel() - 1];
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
