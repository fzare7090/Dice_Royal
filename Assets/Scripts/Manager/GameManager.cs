using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameTimeManager gameTime;
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private List<UnitData> playerDeck = new List<UnitData>();

    private bool isGameFinished = false;

    void Start()
    {
        gameTime.StartTimer();
        StartCoroutine(CheckGameFlow());

    }
    private IEnumerator CheckGameFlow()
    {


        while (!isGameFinished)
        {
            yield return new WaitForSeconds(1f);


            float currentGameTime = gameTime.GetCurrentTime();
            foreach (var player in players)
            {
                int addingValue = currentGameTime < 60f ?
                    player.GetElexir().GetAddingElexir() :
                    player.GetElexir().GetExtraTimeElexirAdding();
                    player.GetElexir().GettingElexir(addingValue);
            }

            bool someoneLost = false;
            for (int i = 0; i < 2; i++) //or players count
            {
                var kingCastle = players[i].GetKingCastleConfig();
                if (kingCastle == null || !kingCastle.exist)
                {
                    int winner = (i + 1) % 2; //or players count

                    gameOverPanel.SetGameOverText(winner);
                    gameOverPanel.GivingChest(winner);
                    gameTime.StopTimer();
                    someoneLost = true;
                    isGameFinished = true;
                    break;
                }
            }

            if (someoneLost)
            {
                CheckForGameOver();
                yield break;
            }

            if (!gameTime.GetRunningTimeBool())
            {

                if (players.Count == 2 && players[0].GetBoard() != null && players[1].GetBoard() != null)
                {
                    int princeCastleCount1 = players[0].GetBoard().GetPrinceCastleCount();
                    int princeCastleCount2 = players[1].GetBoard().GetPrinceCastleCount();

                    if (princeCastleCount1 != princeCastleCount2)
                    {
                        int winnerindex = princeCastleCount1 > princeCastleCount2 ? 0 : 1;

                        gameOverPanel.GivingChest(winnerindex);

                        gameOverPanel.SetGameOverText(winnerindex);
                    }
                    else
                    {
                        int p1TotalHp = players[0].GetKingCastleConfig().GetCastleHP().GetHPCastle();
                        int p2TotalHp = players[1].GetKingCastleConfig().GetCastleHP().GetHPCastle();

                        int winnerIndex = p1TotalHp > p2TotalHp ? 0 : 1;

                        gameOverPanel.GivingChest(winnerIndex);

                        gameOverPanel.SetGameOverText(winnerIndex);
                    }
                }

                CheckForGameOver();
                isGameFinished = true;
                yield break;
            }
        }
    }

    private void CheckForGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }
}