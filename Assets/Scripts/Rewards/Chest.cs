using TMPro;
using UnityEngine;
using RTLTMPro;

public class Chest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChestData chestData;
    [SerializeField] private ChestRewardGenerator rewardGenerator;
    [SerializeField] private PlayerResourceData playerResources;
    [SerializeField] private RTLTextMeshPro chestText;

    private RewardData currentReward;

    public RewardData OpenChest(MatchResult result)
    {
        if (rewardGenerator == null)
        {
            Debug.LogError("ChestRewardGenerator is missing.");
            return null;
        }

        if (playerResources == null)
        {
            Debug.LogError("PlayerResourceData is missing.");
            return null;
        }

   
        currentReward = rewardGenerator.GenerateReward(chestData, result);

        UpdateRewardText(currentReward);

        return currentReward;
    }

    private void UpdateRewardText(RewardData reward)
    {
        if (reward == null || chestText == null)
            return;

        string text =
            $"پاداش مسابقه\n\n" +
            $"سکه: {reward.coins}\n" +
            $"جم: {reward.gems}\n";

        if (reward.cards.Count > 0)
        {
            text += "\nکارت‌ها:\n";

            foreach (var card in reward.cards)
            {
                text += $"{card.cardId} × {card.amount}\n";
            }
        }

        chestText.text = text;
    }

    public RewardData GetCurrentReward()
    {
        return currentReward;
    }
}