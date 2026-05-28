using UnityEngine;

public class CastleUpgrade : MonoBehaviour
{
    [SerializeField] private Castle castle;
    [SerializeField] private CastleData castleData;
    [SerializeField] private CastleUpgradeData upgradeData;
    [SerializeField] private Coin coin; 
    [SerializeField] private int currentLevel = 1;

    private float originalHP;
    private float originalDamage;
    private float originalAttackSpeed;

    public event System.Action<int> OnUpgradeSuccess;
    public event System.Action<string> OnUpgradeFailed;

    private void Start()
    {

        if (castleData == null && castle != null)
            castleData = castle.GetData();


        if (castleData != null)
        {
            originalHP = castleData.hp;
            originalDamage = castleData.attackDamage;
            originalAttackSpeed = castleData.attackSpeed;
            ApplyUpgrade();
        }
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
        {
            OnUpgradeFailed?.Invoke(GetFailReason());
            return false;
        }

        int cost = upgradeData.upgradeCost[currentLevel - 1];

 
        if (coin != null && coin.SpendCoin(cost))
        {
            currentLevel++;
            ApplyUpgrade();
            OnUpgradeSuccess?.Invoke(currentLevel);
            return true;
        }

        OnUpgradeFailed?.Invoke("Not enough coins!");
        return false;
    }

    private string GetFailReason()
    {
        if (upgradeData == null) return "No upgrade data!";
        if (currentLevel >= upgradeData.maxLevel) return "Maximum level reached!";

        int cost = upgradeData.upgradeCost[currentLevel - 1];
        if (coin == null) return "Coin system not found!";
        if (coin.GetCoin() < cost) return $"Need {cost} coins!";

        return "Unknown reason";
    }
    public bool CanUpgrade()
    {
        return upgradeData != null && currentLevel < upgradeData.maxLevel;
    }



    private void ApplyUpgrade()
    {
        if (castle == null || castleData == null || upgradeData == null) return;

        float newHP = originalHP * upgradeData.hpMultiplier[currentLevel - 1];
        float newDamage = originalDamage * upgradeData.damageMultiplier[currentLevel - 1];
        float newAttackSpeed = originalAttackSpeed * upgradeData.attackSpeedMultiplier[currentLevel - 1];

        castle.ApplyUpgrade(newHP, newDamage, newAttackSpeed);
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
        if (upgradeData == null) return 0;
        if (currentLevel >= upgradeData.maxLevel) return 0;
        return upgradeData.upgradeCost[currentLevel - 1];
    }

    public float GetUpgradeProgress()
    {
        if (upgradeData != null)  
        {
            return (float)currentLevel / upgradeData.maxLevel; 
        }
        else 
        {
            return 0f; 
        }
    }

    public UpgradeInfo GetUpgradeInfo()
    {
        if (upgradeData == null || castleData == null)
            return new UpgradeInfo();

        bool hasNext = currentLevel < upgradeData.maxLevel;

        return new UpgradeInfo
        {
            canUpgrade = hasNext,
            currentLevel = currentLevel,
            maxLevel = upgradeData.maxLevel,
            nextLevelCost = hasNext ? upgradeData.upgradeCost[currentLevel - 1] : 0,
            currentHP = originalHP * upgradeData.hpMultiplier[currentLevel - 1],
            currentDamage = originalDamage * upgradeData.damageMultiplier[currentLevel - 1],
            nextHP = hasNext ? originalHP * upgradeData.hpMultiplier[currentLevel] : 0,
            nextDamage = hasNext ? originalDamage * upgradeData.damageMultiplier[currentLevel] : 0
        };
    }
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