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
    private float matchStartTime;  

    void Start()
    {
        matchStartTime = Time.time;
        gameTime.StartTimer();
        StartCoroutine(CheckGameFlow());

    }
    private void OnGameEnd(
    int winnerIndex,
    int player1Towers,
    int player2Towers)
    {
        float matchDuration = Time.time - matchStartTime;

        int playerLevel = players[0].GetLevel();
        int opponentLevel = players[1].GetLevel();

        if (DataGameManager.Instance != null)
        {
            DataGameManager.Instance.SaveMatchFromGameEnd(
                winnerIndex: winnerIndex,
                player1Score: player1Towers,
                player2Score: player2Towers,
                duration: matchDuration,
                playerLevel: playerLevel,
                opponentLevel: opponentLevel,
                trophiesChange: winnerIndex == 0 ? 20 : -15
            );
        }
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
                // Debug.Log("elexir added in game manager");
            }

            bool someoneLost = false;
            for (int i = 0; i < 2; i++) //or players count
            {
                var kingCastle = players[i].GetKingCastleConfig();
                if (kingCastle == null || !kingCastle.exist)
                {
                    int winner = (i + 1) % 2; //or players count

                    int p1CastleCount = players[0].GetBoard().GetCastleCount();
                    int p2CastleCount = players[1].GetBoard().GetCastleCount();

                    OnGameEnd(winner, p1CastleCount, p2CastleCount);
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

                        OnGameEnd(winnerindex, princeCastleCount1, princeCastleCount2);

                        gameOverPanel.GivingChest(winnerindex);

                        gameOverPanel.SetGameOverText(winnerindex);
                    }
                    else
                    {
                        int p1TotalHp = players[0].GetKingCastleConfig().GetCastleHP().GetHPCastle();
                        int p2TotalHp = players[1].GetKingCastleConfig().GetCastleHP().GetHPCastle();

                        int winnerIndex = p1TotalHp > p2TotalHp ? 0 : 1;

                        OnGameEnd(winnerIndex, princeCastleCount1, princeCastleCount2);

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

    public int GetPlayerTowerCount(int playerIndex)
    {
        return players[playerIndex].GetBoard().GetCastleCount();
    }


}