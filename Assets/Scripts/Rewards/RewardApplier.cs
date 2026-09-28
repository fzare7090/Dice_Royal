
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using System;

public class RewardApplier
{
    public void ApplyReward(
        RewardData reward,
        PlayerResourceData player)
    {
        player.AddCoin(reward.coins);
        player.AddGem(reward.gems);

        foreach (var rewardCard in reward.cards)
        {
            CardData card =
                player.GetOrCreateCard(rewardCard.cardId);

            card.ownedCopies += rewardCard.amount;
        }

        Debug.Log("rewarded coins:" + player.GetCoin());
        Debug.Log("rewarded gem:" + player.GetGem());
    }
}