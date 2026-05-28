using UnityEngine;

public class CardUpgrade : MonoBehaviour
{
    [SerializeField] private CardUpgradeData upgradeData;
    [SerializeField] private UnitData unitData;
    [SerializeField] private int currentLevel = 1;

    private float originalHP;
    private float originalDamage;
    private float originalAttackSpeed;

    public event System.Action<int> OnUpgradeSuccess;
    public event System.Action<string> OnUpgradeFailed;

    private void Start()
    {
        if (unitData != null)
        {
            originalHP = unitData.hp;
            originalDamage = unitData.attackDamage;
            originalAttackSpeed = unitData.attackSpeed;
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
        int neededCards = upgradeData.cardCopiesNeeded[currentLevel - 1];

        // اینجا با سیستم سکه و کارت خودت چک کن
        bool hasEnoughCoin = true;  // جایگزین کن با Coin.SpendCoin(cost)
        bool hasEnoughCards = true; // جایگزین کن با کارت‌های جمع‌آوری شده

        if (!hasEnoughCoin)
        {
            OnUpgradeFailed?.Invoke($"Not enough coins! Need {cost}");
            return false;
        }

        if (!hasEnoughCards)
        {
            OnUpgradeFailed?.Invoke($"Not enough card copies! Need {neededCards}");
            return false;
        }

        currentLevel++;
        ApplyUpgrade();
        OnUpgradeSuccess?.Invoke(currentLevel);
        return true;
    }

    private string GetFailReason()
    {
        if (upgradeData == null) return "No upgrade data!";
        if (currentLevel >= upgradeData.maxLevel) return "Maximum level reached!";
        return "Unknown reason";
    }

    private void ApplyUpgrade()
    {
        if (unitData == null || upgradeData == null) return;

        float newHP = originalHP * upgradeData.hpMultiplier[currentLevel - 1];
        float newDamage = originalDamage * upgradeData.damageMultiplier[currentLevel - 1];
        float newAttackSpeed = originalAttackSpeed * upgradeData.attackSpeedMultiplier[currentLevel - 1];

        unitData.hp = newHP;
        unitData.attackDamage = newDamage;
        unitData.attackSpeed = newAttackSpeed;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetMaxLevel()
    {
        return upgradeData != null ? upgradeData.maxLevel : 1;
    }

    public int GetNextUpgradeCost()
    {
        if (upgradeData == null || currentLevel >= upgradeData.maxLevel) return 0;
        return upgradeData.upgradeCost[currentLevel - 1];
    }

    public int GetCardsNeeded()
    {
        if (upgradeData == null || currentLevel >= upgradeData.maxLevel) return 0;
        return upgradeData.cardCopiesNeeded[currentLevel - 1];
    }

    public float GetUpgradeProgress()
    {
        return upgradeData != null ? (float)currentLevel / upgradeData.maxLevel : 0f;
    }

    public UpgradeCardInfo GetUpgradeInfo()
    {
        if (upgradeData == null || unitData == null)
            return new UpgradeCardInfo();

        bool hasNext = currentLevel < upgradeData.maxLevel;

        return new UpgradeCardInfo
        {
            canUpgrade = hasNext,
            currentLevel = currentLevel,
            maxLevel = upgradeData.maxLevel,
            nextLevelCost = hasNext ? upgradeData.upgradeCost[currentLevel - 1] : 0,
            cardsNeeded = hasNext ? upgradeData.cardCopiesNeeded[currentLevel - 1] : 0,
            currentHP = originalHP * upgradeData.hpMultiplier[currentLevel - 1],
            currentDamage = originalDamage * upgradeData.damageMultiplier[currentLevel - 1],
            currentAttackSpeed = originalAttackSpeed * upgradeData.attackSpeedMultiplier[currentLevel - 1],
            nextHP = hasNext ? originalHP * upgradeData.hpMultiplier[currentLevel] : 0,
            nextDamage = hasNext ? originalDamage * upgradeData.damageMultiplier[currentLevel] : 0,
            nextAttackSpeed = hasNext ? originalAttackSpeed * upgradeData.attackSpeedMultiplier[currentLevel] : 0
        };
    }

    public UnitData GetUnitData() { return unitData; }
}

public struct UpgradeCardInfo
{
    public bool canUpgrade;
    public int currentLevel;
    public int maxLevel;
    public int nextLevelCost;
    public int cardsNeeded;
    public float currentHP;
    public float currentDamage;
    public float currentAttackSpeed;
    public float nextHP;
    public float nextDamage;
    public float nextAttackSpeed;
}