using UnityEngine;

public class CardUpgrade : MonoBehaviour
{
    private readonly CardUpgradeData upgradeData;
    private readonly PlayerResourceData playerResource;

    public event System.Action<int> OnUpgradeSuccess;
    public event System.Action<string> OnUpgradeFailed;

  


    public bool TryUpgrade(CardData card)
    {
        if (card == null)
            return false;

        if (card.level >= upgradeData.maxLevel)
            return false;

        int cost = upgradeData.upgradeCost[card.level - 1];
        int copies = upgradeData.cardCopiesNeeded[card.level - 1];

        if (playerResource.GetCoin() < cost)
            return false;

        if (card.ownedCopies < copies)
            return false;

        playerResource.SpendCoin(cost);

        card.ownedCopies -= copies;
        card.level++;

        SaveManager.Instance.SaveGame();

        AutoSaveEvents.CardChanged();
        AutoSaveEvents.CoinChanged();

        return true;
    }


    public UpgradeCardInfo GetUpgradeInfo(CardData card, UnitData unit)
    {
        if (card == null || unit == null || upgradeData == null)
            return new UpgradeCardInfo();

        int level = card.level;
        bool hasNext = level < upgradeData.maxLevel;

        return new UpgradeCardInfo
        {
            canUpgrade = hasNext,
            currentLevel = level,
            maxLevel = upgradeData.maxLevel,

            nextLevelCost = hasNext ? upgradeData.upgradeCost[level - 1] : 0,
            cardsNeeded = hasNext ? upgradeData.cardCopiesNeeded[level - 1] : 0,

            currentHP = unit.hp * upgradeData.hpMultiplier[level - 1],
            currentDamage = unit.attackDamage * upgradeData.damageMultiplier[level - 1],
            currentAttackSpeed = unit.attackSpeed * upgradeData.attackSpeedMultiplier[level - 1],

            nextHP = hasNext ? unit.hp * upgradeData.hpMultiplier[level] : 0,
            nextDamage = hasNext ? unit.attackDamage * upgradeData.damageMultiplier[level] : 0,
            nextAttackSpeed = hasNext ? unit.attackSpeed * upgradeData.attackSpeedMultiplier[level] : 0
        };
    }

    public float GetUpgradeProgress(CardData card)
    {
        if (card == null || upgradeData == null)
            return 0f;

        return (float)card.level / upgradeData.maxLevel;
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