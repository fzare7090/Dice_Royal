using UnityEngine;

public class ChestRewardGenerator : MonoBehaviour
{
    [Header("Match Rewards")]
    [SerializeField] private int winCoins = 50;
    [SerializeField] private int loseCoins = 25;

    [SerializeField] private int winGems = 10;
    [SerializeField] private int loseGems = 5;

    public RewardData GenerateReward(
        ChestData chestData,
        MatchResult result)
    {
        RewardData reward = new RewardData();

        switch (result)
        {
            case MatchResult.Win:
                reward.coins = winCoins;
                reward.gems = winGems;
                break;

            case MatchResult.Lose:
                reward.coins = loseCoins;
                reward.gems = loseGems;
                break;

            case MatchResult.Draw:
                reward.coins = 35;
                reward.gems = 2;
                break;
        }

        GenerateRandomCards(reward, chestData);

        return reward;
    }

    private void GenerateRandomCards(
        RewardData reward,
        ChestData chestData)
    {
        if (chestData == null)
            return;

        if (chestData.possibleCards.Count == 0)
            return;

        string randomCard =
            chestData.possibleCards[
                Random.Range(
                    0,
                    chestData.possibleCards.Count)];

        reward.cards.Add(new CardReward
        {
            cardId = randomCard,
            amount = Random.Range(1, 4)
        });
    }

    /*
    public void ApplyReward(
     RewardData reward,
     PlayerResourceData player)
    {
        player.coin += reward.coins;
        player.gem += reward.gems;

        foreach (var cardReward in reward.cards)
        {
            CardData card = player.GetOrCreateCard(cardReward.cardId);

            card.ownedCopies += cardReward.amount;
        }
    }

    */
}