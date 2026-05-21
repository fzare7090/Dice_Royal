using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputControlScheme;

public class Chest : MonoBehaviour
{
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private TMP_Text chestText;

    private int winCoinReward = 50;
    private int loseCoinReward = 25;
    private int drawCoinReward = 35;

    public void SetPlayers(List<Player> newPlayers)
    {
        if (newPlayers == null)
        {
            players = new List<Player>();
            return;
        }

        players = new List<Player>(newPlayers);
    }


    public Player GetPlayer(int playerIndex)
    {
        return players[playerIndex];
    }

    public void GiveRewards(Player player, MatchResult result, int winnerIndex) // index -1?
    {
        if (players == null || players.Count == 0) return;

        for (int i = 0; i < players.Count; i++)
        {
            GiveReward(player, i, result, winnerIndex);
        }
    }

    private void GiveReward(Player player, int playerIndex, MatchResult result, int winnerIndex)
    {
        if (player == null) return;

        Coin coin = player.GetCoin();
        if (coin == null) return;

        int rewardAmount = GetRewardAmount(playerIndex, result, winnerIndex);
        coin.AddCoin(rewardAmount);
    }

    private int GetRewardAmount(int playerIndex, MatchResult result, int winnerIndex)
    {
        switch (result)
        {
            case MatchResult.Win:
                return playerIndex == winnerIndex ? winCoinReward : loseCoinReward;

            case MatchResult.Draw:
                return drawCoinReward;

            case MatchResult.Lose:
            default:
                return loseCoinReward;
        }
    }

    public void GiveMatchRewards(int winnerIndex)
    {
        if (players == null || players.Count < 2) return;

        int loserIndex = (winnerIndex + 1) % 2;

        Player winner = players[winnerIndex];
        Player loser = players[loserIndex];

        if (winner != null && winner.GetCoin() != null)
        {
            winner.GetCoin().AddCoin(winCoinReward);
            Debug.Log($"Player {winnerIndex + 1} won {winCoinReward} coins! Total: {winner.GetCoin().GetCoin()}");
        }

        if (loser != null && loser.GetCoin() != null)
        {
            loser.GetCoin().AddCoin(loseCoinReward);
            Debug.Log($"Player {loserIndex + 1} got {loseCoinReward} coins! Total: {loser.GetCoin().GetCoin()}");
        }
    }

    public void SetRewardsText(int winnerIndex)
    {
        int loserIndex = (winnerIndex + 1) % 2;


        chestText.SetText($"Player {winnerIndex + 1} won {winCoinReward}  Coins!\n Player {loserIndex + 1} got {loseCoinReward}Coins!");
    }


}

