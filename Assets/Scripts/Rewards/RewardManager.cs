using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public void GiveReward(
        PlayerResourceData resources,
        CardCollectionData collection,
        RewardData reward)
    {
        if (resources == null || reward == null)
            return;

        resources.AddCoin(reward.coins);
        resources.AddGem(reward.gems);
 

        foreach (var cardReward in reward.cards)
        {
            CardData card =
                collection.GetCard(cardReward.cardId);

            if (card == null)
            {
                collection.AddCard(cardReward.cardId);

                card = collection.GetCard(cardReward.cardId);
            }

            card.ownedCopies += cardReward.amount;
        }
    }
}