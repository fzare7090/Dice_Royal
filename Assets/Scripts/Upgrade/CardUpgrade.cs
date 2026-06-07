using UnityEngine;

public class CardUpgrade : MonoBehaviour
{
    [SerializeField] private CardSaveData saveData;
    [SerializeField] private CardUpgradeData upgradeData;
    [SerializeField] private UnitData unitData;
    [SerializeField] private PlayerResourceData playerResource;

    public event System.Action<int> OnUpgradeSuccess;
    public event System.Action<string> OnUpgradeFailed;

    public bool CanUpgrade()
    {
        return saveData != null &&
               upgradeData != null &&
               unitData != null &&
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
        int neededCards = upgradeData.cardCopiesNeeded[saveData.level - 1];

        if (playerResource.coin < cost)
        {
            OnUpgradeFailed?.Invoke($"Need {cost} coins");
            return false;
        }

        if (saveData.ownedCopies < neededCards)
        {
            OnUpgradeFailed?.Invoke($"Need {neededCards} cards");
            return false;
        }

        playerResource.coin -= cost;
        saveData.ownedCopies -= neededCards;
        saveData.level++;

        OnUpgradeSuccess?.Invoke(saveData.level);
        return true;
    }

    private string GetFailReason()
    {
        if (saveData == null) return "Save missing";
        if (upgradeData == null) return "Upgrade missing";
        if (unitData == null) return "Unit missing";
        return "Unknown";
    }

    public UpgradeCardInfo GetUpgradeInfo()
    {
        if (saveData == null || upgradeData == null || unitData == null)
            return new UpgradeCardInfo();

        int level = saveData.level;
        bool hasNext = level < upgradeData.maxLevel;

        return new UpgradeCardInfo
        {
            canUpgrade = hasNext,
            currentLevel = level,
            maxLevel = upgradeData.maxLevel,

            nextLevelCost = hasNext ? upgradeData.upgradeCost[level - 1] : 0,
            cardsNeeded = hasNext ? upgradeData.cardCopiesNeeded[level - 1] : 0,

            currentHP = unitData.hp * upgradeData.hpMultiplier[level - 1],
            currentDamage = unitData.attackDamage * upgradeData.damageMultiplier[level - 1],
            currentAttackSpeed = unitData.attackSpeed * upgradeData.attackSpeedMultiplier[level - 1],

            nextHP = hasNext ? unitData.hp * upgradeData.hpMultiplier[level] : 0,
            nextDamage = hasNext ? unitData.attackDamage * upgradeData.damageMultiplier[level] : 0,
            nextAttackSpeed = hasNext ? unitData.attackSpeed * upgradeData.attackSpeedMultiplier[level] : 0
        };
    }

    public float GetUpgradeProgress()
    {
        return (saveData == null || upgradeData == null)
            ? 0f
            : (float)saveData.level / upgradeData.maxLevel;
    }
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