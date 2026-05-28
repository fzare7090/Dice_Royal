using UnityEngine;
using System.Collections.Generic;

public class UpgradeDataBridge : MonoBehaviour
{
    public static UpgradeDataBridge Instance;


    public List<int> castlesLevel = new List<int>();

    public List<string> upgradedCardNames = new List<string>();
    public List<int> upgradedCardLevels = new List<int>();

    public int diceLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ذخیره دیتای قلعه‌ها
    public void SaveCastles(CastleUpgrade kingCastle, CastleUpgrade lPrinceCastle, CastleUpgrade rPrinceCastle)
    {
       
        if (kingCastle != null)
        {
            castlesLevel.Add(kingCastle.GetCurrentLevel());
            var info = kingCastle.GetUpgradeInfo();
 
        }

        if (lPrinceCastle != null)
        {
            castlesLevel.Add(lPrinceCastle.GetCurrentLevel());
            var info = lPrinceCastle.GetUpgradeInfo();

        }
        if (rPrinceCastle != null)
        {
            castlesLevel.Add(rPrinceCastle.GetCurrentLevel());
            var info = rPrinceCastle.GetUpgradeInfo();

        }
    }

    public void SaveDice(DiceUpgrade dice)
    {
        if (dice != null)
        {
            diceLevel = dice.GetCurrentLevel();
            var info = dice.GetUpgradeInfo();
        }
    }

    // ذخیره دیتای کارت آپگرید شده
    public void SaveUpgradedCard(string cardName, int newLevel)
    {
        if (upgradedCardNames.Contains(cardName))
        {
            int index = upgradedCardNames.IndexOf(cardName);
            upgradedCardLevels[index] = newLevel;
        }
        else
        {
            upgradedCardNames.Add(cardName);
            upgradedCardLevels.Add(newLevel);
        }
    }
}