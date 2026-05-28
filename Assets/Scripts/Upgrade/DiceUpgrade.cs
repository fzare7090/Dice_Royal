using UnityEngine;

public class DiceUpgrade : MonoBehaviour
{
    [SerializeField] private DiceUpgradeData upgradeData;
    [SerializeField] private Dice dice;
    [SerializeField] private Elexir elexir;
    [SerializeField] private int currentLevel = 1;

    public event System.Action<int> OnUpgradeSuccess;
    public event System.Action<string> OnUpgradeFailed;

    private void Start()
    {
        if (dice != null && upgradeData != null)
        {
            ApplyUpgrade();
        }
    }

    public bool CanUpgrade()
    {
        return upgradeData != null && currentLevel < upgradeData.maxLevel;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
        {
            OnUpgradeFailed?.Invoke(GetFailReason());
            return false;
        }

        int cost = upgradeData.upgradeCost[currentLevel - 1];

        if (elexir == null)
        {
            OnUpgradeFailed?.Invoke("Elexir system not found!");
            return false;
        }

        if (elexir.GetElexir() < cost)
        {
            OnUpgradeFailed?.Invoke($"Not enough elexir! Need {cost}");
            return false;
        }

        elexir.UsingElexir(cost);
        currentLevel++;
        ApplyUpgrade();
        OnUpgradeSuccess?.Invoke(currentLevel);
        return true;
    }

    private string GetFailReason()
    {
        if (upgradeData == null) return "No upgrade data!";
        if (currentLevel >= upgradeData.maxLevel) return "Maximum level reached!";
        if (elexir == null) return "Elexir system not found!";
        return "Unknown reason";
    }

    private void ApplyUpgrade()
    {
        if (dice == null || upgradeData == null) return;

        int newMultiplier = upgradeData.elexirMultiplier[currentLevel - 1];

        dice.SetElexirMultiplier(newMultiplier);

  
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetMaxLevel()
    {
        return upgradeData != null ? upgradeData.maxLevel : 1;
    }

    public int GetCurrentMultiplier()
    {
        return upgradeData != null ? upgradeData.elexirMultiplier[currentLevel - 1] : 0;
    }

    public int GetNextMultiplier()
    {
        if (!CanUpgrade()) return 0;
        return upgradeData.elexirMultiplier[currentLevel];
    }

    public int GetNextUpgradeCost()
    {
        if (!CanUpgrade()) return 0;
        return upgradeData.upgradeCost[currentLevel - 1];
    }

    public float GetUpgradeProgress()
    {
        return upgradeData != null ? (float)currentLevel / upgradeData.maxLevel : 0f;
    }

    public DiceUpgradeInfo GetUpgradeInfo()
    {
        if (upgradeData == null)
            return new DiceUpgradeInfo();

        bool hasNext = currentLevel < upgradeData.maxLevel;

        return new DiceUpgradeInfo
        {
            canUpgrade = hasNext,
            currentLevel = currentLevel,
            maxLevel = upgradeData.maxLevel,
            currentMultiplier = upgradeData.elexirMultiplier[currentLevel - 1],
            nextMultiplier = hasNext ? upgradeData.elexirMultiplier[currentLevel] : 0,
            nextLevelCost = hasNext ? upgradeData.upgradeCost[currentLevel - 1] : 0
        };
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