using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameTimeManager gameTime;
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private GameOverPanel gameOverPanel;

 
    void Start()
    {
        gameTime.StartTimer();
        StartCoroutine(CheckGameFlow());


    }

    private IEnumerator CheckGameFlow()
    {

        float normalPlayTime = 10f;
        float extraTime = 5f;
        float passedTime = 0f;

        while (passedTime < normalPlayTime)
        {
            passedTime += 1;

            foreach (var player in players)
            {
                int addingValue = player.GetElexir().GetAddingElexir();
                player.GetElexir().GettingElexir(addingValue);
            }

            for (int i = 0; i < players.Count; i++)
            {

                if (players[i].GetKingCastleConfig() != null && !players[i].GetKingCastleConfig().GetCastleHP().exist)

                {

                    gameTime.StopTimer();

                    int winner = (i + 1) % players.Count; /// or %2 
                    gameOverPanel.SetGameOverText( winner );

                    CheckForGameOver();

                    yield break;
                }

            }
            
            yield return new WaitForSeconds(1);
        }

        if (players.Count == 2 && players[0].GetBoard() != null && players[1].GetBoard() != null)
        {
            int princeCastleCount1 = players[0].GetBoard().GetPrinceCastleCount();
            int princeCastleCount2 = players[1].GetBoard().GetPrinceCastleCount();

            if (princeCastleCount1 != princeCastleCount2)
            {
                int winner = princeCastleCount1 > princeCastleCount2 ? 0 : 1;

                gameOverPanel.SetGameOverText(winner);
                gameTime.StopTimer();
                CheckForGameOver();
            }
         
        }


        else
        {

            while (passedTime < extraTime + normalPlayTime)
            {

                passedTime += 1;

                foreach (var player in players)
                {
                    int addingValue = player.GetElexir().GetExtraTimeElexirAdding();
                    player.GetElexir().GettingElexir(addingValue);
                }


                for (int i = 0; i < players.Count; i++) // or2
                {

                    if (players[i].GetKingCastleConfig() != null && !players[i].GetKingCastleConfig().GetCastleHP().exist)

                    {

                        gameTime.StopTimer();

                        int winner = (i + 1) % players.Count; /// or %2 
                        gameOverPanel.SetGameOverText(winner);

                        CheckForGameOver();

                        yield break;
                    }

                }
                yield return new WaitForSeconds(1);

            }


            //check for hiegher hp
            int p1TotalHp = players[0].GetKingCastleConfig().GetCastleHP().GetHPCastle();
            int p2TotalHp = players[1].GetKingCastleConfig().GetCastleHP().GetHPCastle();

            if (p1TotalHp > p2TotalHp)
            {
                gameOverPanel.SetGameOverText(0);
            }
            else
            {
                gameOverPanel.SetGameOverText(1);
            }

            gameTime.StopTimer();
            CheckForGameOver();


        }

    }

    private void CheckForGameOver()
    {
        if (gameTime.GetRunningTimeBool() == false)
        {
            gameOverPanel.gameObject.SetActive(true);
           
        }
    }


}
