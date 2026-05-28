using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class SceneManagerDataSaver : MonoBehaviour
{
    [SerializeField] private CastleUpgrade kingCastle;
    [SerializeField] private CastleUpgrade rPrinceCastle;
    [SerializeField] private CastleUpgrade lPrinceCastle;

    [SerializeField] private List<CardUpgrade> cards = new List<CardUpgrade>();
    [SerializeField] private DiceUpgrade dice;


    public void SaveInfo()
    {
        UpgradeDataBridge.Instance.SaveCastles(kingCastle, rPrinceCastle, lPrinceCastle);
        UpgradeDataBridge.Instance.SaveDice(dice);
        foreach (CardUpgrade card in cards)
        {
            string cardName = card.GetUnitData().name;
            int cardLevel = card.GetCurrentLevel();

            UpgradeDataBridge.Instance.SaveUpgradedCard(cardName, cardLevel);
        }

    }

    public void RegisterCard(CardUpgrade card)
    {
        if (!cards.Contains(card))
        {
            cards.Add(card);
        }
    }

}

